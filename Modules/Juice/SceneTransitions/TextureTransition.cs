using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace CookieUtils.Extras.Juice
{
    public class TextureTransition : FullscreenTransition
    {
        private static readonly int TransitionProgress = Shader.PropertyToID("_TransitionProgress");
        private static readonly int TransitionTexture = Shader.PropertyToID("_TransitionTexture");

        [SerializeField]
        private Texture2D showTexture;

        [SerializeField]
        private Texture2D hideTexture;

        [SerializeField]
        private Vector2 tiling = Vector2.one;

        [SerializeField]
        private Vector2 offset;

        [SerializeField]
        private TweenSettings<float> showSettings = new()
        {
            startFromCurrent = true,
            endValue = 1f,
            settings = new TweenSettings
            {
                useUnscaledTime = true,
                duration = 0.5f,
                ease = Ease.OutQuad,
            },
        };

        [SerializeField]
        private TweenSettings<float> hideSettings = new()
        {
            startFromCurrent = true,
            endValue = 0f,
            settings = new TweenSettings
            {
                useUnscaledTime = true,
                duration = 0.5f,
                ease = Ease.InQuad,
            },
        };

        [SerializeField]
        [Tooltip(
            "The material to use, must have a _TransitionProgress and a _TransitionTexture texture 2D property"
        )]
        private Material material;

        protected override void Awake()
        {
            base.Awake();
            Screen.material = material;
            Screen.material.SetFloat(TransitionProgress, 0);
            Screen.material.SetTextureOffset(TransitionTexture, offset);
            Screen.material.SetTextureScale(TransitionTexture, tiling);
            Screen.enabled = false;
        }

        protected override async Task OnShow()
        {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            Screen.enabled = true;
            Screen.material.SetTexture(TransitionTexture, showTexture);

            await Sequence
                .Create(useUnscaledTime: true)
                .Chain(Tween.MaterialProperty(Screen.material, TransitionProgress, showSettings))
                .Insert(showSettings.settings.duration * 0.5f, ShowProgressBar());

            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        protected override async Task OnHide()
        {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            Screen.material.SetTexture(TransitionTexture, hideTexture);

            await Sequence
                .Create(useUnscaledTime: true)
                .Chain(HideProgressBar())
                .Chain(Tween.MaterialProperty(Screen.material, TransitionProgress, hideSettings));

            Screen.enabled = false;
            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        public override void ShowImmediately()
        {
            ShowProgressBarImmediately();

            Screen.enabled = true;
            Screen.material.SetFloat(TransitionProgress, showSettings.endValue);
        }

        public override void HideImmediately()
        {
            HideProgressBarImmediately();

            Screen.material.SetFloat(TransitionProgress, hideSettings.endValue);
            Screen.enabled = false;
        }
    }
}
