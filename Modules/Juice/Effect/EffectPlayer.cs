using System.Threading.Tasks;
using JetBrains.Annotations;
using PrimeTween;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace CookieUtils.Extras.Juice
{
    [PublicAPI]
    public class EffectPlayer : MonoBehaviour
    {
        private static readonly int ProgressID = Shader.PropertyToID("_Progress");
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        [Tooltip("The effect object to play")] public Effect effect;

        [Tooltip("Whether to override the renderers")]
        public bool overrideRenderers = false;

        [Tooltip("The renderer overrides")] public Renderer[] rendererOverrides;

        private Transform _cam;
        private Renderer[] _renderers;

        protected virtual void Awake() {
            Initialize();
        }

        protected virtual void Initialize() {
            Camera mainCam = Camera.main;
            _cam = mainCam?.transform;

            if (!effect) {
                Debug.LogError(
                    $"{(transform.parent ? transform.parent.name : name)}'s EffectPlayer has no data object!"
                );
                Destroy(this);

                return;
            }

            if (overrideRenderers && rendererOverrides.Length > 0)
                _renderers = rendererOverrides;
            else
                _renderers = GetComponentsInChildren<Renderer>();
        }

        public virtual async Task Play() {
            if (!_cam) _cam = Camera.main?.transform;

            if (_cam) {
                Vector3 difference = transform.position - _cam.position;
                if (effect.Is2D) difference.z = 0;


                Vector3 direction = difference.normalized;

                if (direction.sqrMagnitude < 0.05f * 0.05f) direction = Vector3.right;

                await Play(direction);
            } else {
                await Play(Vector3.right);
            }
        }

        public async Task Play(Vector3 direction) {
            await Play(direction, transform.position);
        }

        public async Task Play(Vector3 direction, Vector3 contactPoint) {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;

            await effect.Play(_renderers, gameObject, direction, contactPoint);

            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }
    }
}