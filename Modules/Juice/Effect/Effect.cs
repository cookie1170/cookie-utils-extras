using System;
using CookieUtils.Audio;
using CookieUtils.Runtime.ObjectPooling;
using JetBrains.Annotations;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace CookieUtils.Extras.Juice
{
    [PublicAPI]
    public class Effect : MonoBehaviour
    {
        public enum MaterialType
        {
            Lit,
            Unlit,
        }

        private static readonly int ProgressID = Shader.PropertyToID("_Progress");
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        [Tooltip("The data object used for this effect")]
        public EffectData data;

        [Tooltip("Whether to override the renderers")]
        public bool overrideRenderers = false;

        [Tooltip("The renderer overrides")] public Renderer[] rendererOverrides;

        [Tooltip("Whether to override the material, must have a _Color and a _Progress properties")]
        public bool overrideMaterial = false;

        [Tooltip("The material override, must have a _Color and a _Progress properties")]
        public Material materialOverride;

        [SerializeField] [HideInInspector] private Material hitMaterialSpriteLit;
        [SerializeField] [HideInInspector] private Material hitMaterialSpriteUnlit;
        [SerializeField] [HideInInspector] private Material hitMaterialMeshLit;
        [SerializeField] [HideInInspector] private Material hitMaterialMeshUnlit;
        private Transform _cam;
        private Renderer[] _renderers;

        private CinemachineImpulseSource _source;

        protected virtual void Awake() {
            Initialize();
        }

        protected virtual void Initialize() {
            Camera mainCam = Camera.main;
            Debug.Assert(mainCam != null, "Camera.main != null");
            _cam = mainCam.transform;

            if (!data) {
                Debug.LogError($"{(transform.parent ? transform.parent.name : name)}'s Effect has no data object!");
                Destroy(this);

                return;
            }

            if (data.shakeCamera)
                if (!TryGetComponent(out _source))
                    _source = gameObject.AddComponent<CinemachineImpulseSource>();

            if (overrideRenderers && rendererOverrides.Length > 0)
                _renderers = rendererOverrides;
            else
                _renderers = GetComponentsInChildren<Renderer>();

            if (!(_renderers.Length > 0 && data.animateFlash)) return;

            foreach (Renderer rendererIteration in _renderers) SetMaterial(rendererIteration);
        }

        public virtual void Play() {
            if (!_cam) _cam = Camera.main?.transform;

            if (_cam) {
                Vector3 difference = transform.position - _cam.position;
                if (data.is2D) difference.z = 0;


                Vector3 direction = difference.normalized;

                if (direction.sqrMagnitude < 0.05f * 0.05f) direction = Vector3.right;

                Play(direction);
            } else {
                Play(Vector3.right);
            }
        }

        public virtual void Play(Vector3 direction) {
            Play(direction, transform.position);
        }

        public virtual void Play(Vector3 direction, Vector3 contactPoint) {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;

            if (data.shakeCamera)
                ShakeCamera(direction);

            if (data.spawnParticles)
                SpawnParticles(direction, contactPoint);

            if (data.playAudio)
                PlayAudio();

            if (data.animateScale)
                AnimateScale();

            if (data.animateRotation)
                AnimateRotation();

            if (data.animateFlash)
                AnimateFlash();

            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        private void ShakeCamera(Vector3 direction) {
            if (!_source) _source = GetComponent<CinemachineImpulseSource>();

            _source.GenerateImpulse(direction * data.shakeForce);
        }

        private void SpawnParticles(Vector3 direction, Vector3 contactPoint) {
            Quaternion angle = Quaternion.identity;

            if (data.directionalParticles)
                angle = data.is2D
                    ? Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction))
                    : Quaternion.AngleAxis(0, direction);

            data.particlePrefab.Get(contactPoint, angle);
        }

        private async void PlayAudio() {
            await Awaitable.WaitForSecondsAsync(data.audioDelay, destroyCancellationToken);

            Vector3 pos = transform.position;

            data.audioClips.Play(pos, data.audioVolume, data.spatialBlend);
        }

        private void AnimateScale() {
            var sequence = Sequence.Create();
            foreach (ScaleTweenInstruction instruction in data.scaleAnimation) {
                Tween tween = instruction.Process(transform);
                if (instruction.parallel) sequence.Group(tween);
                else sequence.Chain(tween);
            }
        }

        private void AnimateRotation() {
            var sequence = Sequence.Create();
            foreach (RotationTweenInstruction instruction in data.rotationAnimation) {
                Tween tween = instruction.Process(transform);
                if (instruction.parallel) sequence.Group(tween);
                else sequence.Chain(tween);
            }
        }

        private async void AnimateFlash() {
            if (!didStart)
                await Awaitable.EndOfFrameAsync(
                    destroyCancellationToken
                ); // weird hack but doesn't work if called in the first OnEnable without it

            foreach (Renderer rendererIteration in _renderers) {
                var sequence = Sequence.Create();
                if (!rendererIteration.material.HasFloat(ProgressID)) SetMaterial(rendererIteration);

                rendererIteration.material.SetColor(ColorID, data.flashColour);

                foreach (FloatTweenInstruction instruction in data.flashAnimation) {
                    Tween tween = Tween.MaterialProperty(rendererIteration.material, ProgressID, instruction.settings);

                    if (instruction.parallel) _ = sequence.Group(tween);
                    else _ = sequence.Chain(tween);
                }
            }
        }

        private void SetMaterial(Renderer render) {
            render.material = overrideMaterial && materialOverride
                ? materialOverride
                : data.materialType switch {
                    MaterialType.Lit => data.is2D ? hitMaterialSpriteLit : hitMaterialMeshLit,
                    MaterialType.Unlit => data.is2D ? hitMaterialSpriteUnlit : hitMaterialMeshUnlit,
                    _ => throw new ArgumentOutOfRangeException(nameof(data.materialType)),
                };
        }
    }
}