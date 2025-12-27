using System;
using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace CookieUtils.Extras.Juice
{
    public class FadeTransition : FullscreenTransition
    {
        [SerializeField]
        private TweenSettings<float> showSettings = new()
        {
            endValue = 1,
            startFromCurrent = true,
            settings = new TweenSettings
            {
                duration = 0.5f,
                endDelay = 0.1f,
                ease = Ease.InQuad,
                useUnscaledTime = true,
            },
        };

        [SerializeField]
        private TweenSettings<float> hideSettings = new()
        {
            endValue = 0,
            startFromCurrent = true,
            settings = new TweenSettings
            {
                duration = 0.5f,
                ease = Ease.OutQuad,
                useUnscaledTime = true,
            },
        };

        public override IProgress<float> Progress => null;

        protected override void Awake()
        {
            base.Awake();
            Color color = Screen.color;
            color.a = 0;
            Screen.color = color;
            Screen.enabled = false;
        }

        protected override async Task OnShow()
        {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            Screen.enabled = true;

            await Sequence
                .Create(useUnscaledTime: true)
                .Chain(Tween.Alpha(Screen, showSettings))
                .Insert(showSettings.settings.duration * 0.5f, ShowProgressBar());

            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        protected override async Task OnHide()
        {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;

            await Sequence
                .Create(useUnscaledTime: true)
                .Chain(HideProgressBar())
                .Chain(Tween.Alpha(Screen, hideSettings));

            Screen.enabled = false;
            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        public override void ShowImmediately()
        {
            ShowProgressBarImmediately();

            Color color = Screen.color;
            color.a = showSettings.endValue;
            Screen.color = color;
            Screen.enabled = true;
        }

        public override void HideImmediately()
        {
            HideProgressBarImmediately();

            Color color = Screen.color;
            color.a = hideSettings.endValue;
            Screen.color = color;
            Screen.enabled = false;
        }
    }
}
