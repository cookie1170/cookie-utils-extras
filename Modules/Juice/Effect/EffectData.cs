using PrimeTween;
using UnityEngine;

namespace CookieUtils.Extras.Juice
{
    [CreateAssetMenu(fileName = "EffectData", menuName = "Cookie Utils/Effect data")]
    public class EffectData : ScriptableObject
    {
        [Tooltip("Whether to shake the camera using CinemachineImpulseSource")]
        public bool shakeCamera;

        [Range(0, 2)] [Tooltip("The force to shake the camera with")]
        public float shakeForce = 0.25f;


        [Tooltip("Whether to use 2D coordinates for direction and particle rotation")]
        public bool is2D;


        [Tooltip("Whether to spawn particles")]
        public bool spawnParticles;

        [Tooltip("Whether the particles should be directional. Expected original direction is to the right")]
        public bool directionalParticles;

        [Tooltip("The particles to spawn")] public GameObject particlePrefab;


        [Tooltip("Whether to play audio")] public bool playAudio = false;

        [Tooltip("The spatial blend of the sound")] [Range(0, 1)]
        public float spatialBlend = 0f;

        [Tooltip("The volume of the sound")] [Range(0, 1)]
        public float audioVolume = 1f;

        [Tooltip("The delay with which to pay the audio clip")]
        public float audioDelay = 0;

        [Tooltip("The audio clips to play (random pick)")]
        public AudioClip[] audioClips;

        [Tooltip("Whether to punch the scale")]
        public bool animateScale = true;

        [Tooltip("The animation for the scale")]
        public ScaleTweenInstruction[] scaleAnimation = {
            new() {
                type = TweenType.Punch,
                shakeSettings = new ShakeSettings {
                    duration = 0.4f,
                    strength = Vector3.one * 0.25f,
                    enableFalloff = true,
                    frequency = 2f,
                    asymmetry = 0.5f,
                },
            },
        };

        [Tooltip("Whether to shake the rotation")]
        public bool animateRotation = true;

        [Tooltip("The animation for the rotation")]
        public RotationTweenInstruction[] rotationAnimation = {
            new() {
                type = TweenType.Punch,
                shakeSettings = new ShakeSettings {
                    duration = 0.4f,
                    enableFalloff = true,
                    strength = Vector3.forward * 30f,
                    frequency = 10f,
                    asymmetry = 0.5f,
                },
            },
        };

        [Tooltip("Whether to flash the sprite, requires a Renderer")]
        public bool animateFlash = true;

        [Tooltip("The animation for the flash (0 is original colour, 1 is flash colour)")]
        public FloatTweenInstruction[] flashAnimation = {
            new() {
                settings = new TweenSettings<float> {
                    endValue = 1,
                    settings = new TweenSettings {
                        ease = Ease.OutCirc,
                        duration = 0.15f,
                        endDelay = 0.2f,
                    },
                },
            },
            new() {
                settings = new TweenSettings<float> {
                    startValue = 1,
                    endValue = 0,
                    settings = new TweenSettings {
                        ease = Ease.InCirc,
                        duration = 0.3f,
                    },
                },
            },
        };

        [Tooltip("The color to flash to")] public Color flashColour = Color.crimson;

        [Tooltip("The type of material to use for the flash")]
        public Effect.MaterialType materialType;
    }
}