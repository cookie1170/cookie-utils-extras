using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace CookieUtils.Extras.Juice
{
    public class SlideTransition : FullscreenTransition
    {
        [SerializeField]
        private SlideAxis axis;

        [SerializeField]
        private float inMult = 1f;

        [SerializeField]
        private float outMult = 1f;

        [SerializeField]
        private TweenSettings showSettings = new()
        {
            useUnscaledTime = true,
            duration = 0.5f,
            endDelay = 0.1f,
            ease = Ease.OutQuad,
        };

        [SerializeField]
        private TweenSettings hideSettings = new()
        {
            useUnscaledTime = true,
            duration = 0.5f,
            ease = Ease.InQuad,
        };

        private TweenSettings<float> _showSettings;
        private TweenSettings<float> _hideSettings;

        protected override void Awake()
        {
            base.Awake();
            Vector3 position = Screen.rectTransform.localPosition;

            _showSettings.settings = showSettings;
            _hideSettings.settings = hideSettings;

            switch (axis)
            {
                case SlideAxis.X:
                    _showSettings.startValue = Screen.canvas.pixelRect.width * 2;
                    _hideSettings.endValue = Screen.canvas.pixelRect.width * 2;
                    break;

                case SlideAxis.Y:
                    _showSettings.startValue = Screen.canvas.pixelRect.width * 2;
                    _hideSettings.endValue = Screen.canvas.pixelRect.width * 2;
                    break;
            }

            _showSettings.startValue *= inMult;
            _hideSettings.endValue *= outMult;

            HideImmediately();

            Screen.rectTransform.localPosition = position;
            Screen.enabled = false;
        }

        protected override async Task OnShow()
        {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            Screen.enabled = true;

            switch (axis)
            {
                case SlideAxis.X:
                {
                    await Sequence
                        .Create(useUnscaledTime: true)
                        .Chain(Tween.LocalPositionX(Screen.rectTransform, _showSettings))
                        .Insert(_showSettings.settings.duration * 0.5f, ShowProgressBar());

                    break;
                }
                case SlideAxis.Y:
                {
                    await Sequence
                        .Create(useUnscaledTime: true)
                        .Chain(Tween.LocalPositionY(Screen.rectTransform, _showSettings))
                        .Insert(_showSettings.settings.duration * 0.5f, ShowProgressBar());

                    break;
                }
            }

            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        protected override async Task OnHide()
        {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;

            switch (axis)
            {
                case SlideAxis.X:
                {
                    await Sequence
                        .Create(useUnscaledTime: true)
                        .Chain(HideProgressBar())
                        .Chain(Tween.LocalPositionX(Screen.rectTransform, _hideSettings));

                    break;
                }
                case SlideAxis.Y:
                {
                    await Sequence
                        .Create(useUnscaledTime: true)
                        .Chain(HideProgressBar())
                        .Chain(Tween.LocalPositionY(Screen.rectTransform, _hideSettings));

                    break;
                }
            }

            Screen.enabled = false;
            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        public override void ShowImmediately()
        {
            Screen.enabled = true;
            ShowProgressBarImmediately();

            switch (axis)
            {
                case SlideAxis.X:
                {
                    Screen.rectTransform.localPosition = Screen.rectTransform.localPosition.With(
                        x: _showSettings.endValue
                    );

                    break;
                }
                case SlideAxis.Y:
                {
                    Screen.rectTransform.localPosition = Screen.rectTransform.localPosition.With(
                        y: _showSettings.endValue
                    );

                    break;
                }
            }
        }

        public override void HideImmediately()
        {
            switch (axis)
            {
                case SlideAxis.X:
                {
                    Screen.rectTransform.localPosition = Screen.rectTransform.localPosition.With(
                        x: _hideSettings.endValue
                    );

                    break;
                }
                case SlideAxis.Y:
                {
                    Screen.rectTransform.localPosition = Screen.rectTransform.localPosition.With(
                        y: _hideSettings.endValue
                    );

                    break;
                }
            }

            Screen.enabled = false;
            HideProgressBarImmediately();
        }

        private enum SlideAxis
        {
            X,
            Y,
        }
    }
}
