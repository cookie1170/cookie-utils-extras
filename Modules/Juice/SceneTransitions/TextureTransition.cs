using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace CookieUtils.Extras.Juice
{
    public class TextureTransition : FullscreenTransition
    {
        private static readonly int TransitionProgress = Shader.PropertyToID("_TransitionProgress");
        private static readonly int TransitionTexture = Shader.PropertyToID("_TransitionTexture");

        [SerializeField] private Texture2D inTexture;
        [SerializeField] private Texture2D outTexture;

        [SerializeField] private Vector2 tiling = Vector2.one;
        [SerializeField] private Vector2 offset;

        [SerializeField] private TweenSettings<float> inSettings = new() {
            startFromCurrent = true,
            endValue = 1f,
            settings = new TweenSettings {
                useUnscaledTime = true,
                duration = 0.5f,
                endDelay = 0.25f,
                ease = Ease.OutQuad,
            },
        };

        [SerializeField] private TweenSettings<float> outSettings = new() {
            startFromCurrent = true,
            endValue = 0f,
            settings = new TweenSettings {
                useUnscaledTime = true,
                duration = 0.5f,
                ease = Ease.InQuad,
            },
        };

        [SerializeField]
        [Tooltip("The material to use, must have a _TransitionProgress and a _TransitionTexture texture 2D property")]
        private Material material;

        protected override void Awake() {
            base.Awake();
            screen.material = material;
            screen.material.SetFloat(TransitionProgress, 0);
            screen.material.SetTextureOffset(TransitionTexture, offset);
            screen.material.SetTextureScale(TransitionTexture, tiling);
            screen.enabled = false;
        }

        public override async Task PlayForwards() {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            screen.enabled = true;
            screen.material.SetTexture(TransitionTexture, inTexture);
            await Tween.MaterialProperty(screen.material, TransitionProgress, inSettings);
            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        public override async Task PlayBackwards() {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            screen.material.SetTexture(TransitionTexture, outTexture);
            await Tween.MaterialProperty(screen.material, TransitionProgress, outSettings);
            screen.enabled = false;
            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }
    }
}