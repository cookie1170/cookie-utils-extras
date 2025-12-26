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
        private TweenSettings<float> inSettings = new()
        {
            startFromCurrent = false,
            endValue = 0f,
            settings = new TweenSettings
            {
                useUnscaledTime = true,
                duration = 0.5f,
                endDelay = 0.25f,
                ease = Ease.OutQuad,
            },
        };

        [SerializeField]
        private TweenSettings<float> outSettings = new()
        {
            startFromCurrent = true,
            startValue = 0f,
            settings = new TweenSettings
            {
                useUnscaledTime = true,
                duration = 0.5f,
                endDelay = 0.25f,
                ease = Ease.InQuad,
            },
        };

        protected override void Awake()
        {
            base.Awake();
            Vector3 position = screen.rectTransform.localPosition;
            switch (axis)
            {
                case SlideAxis.X:
                    inSettings.startValue = screen.canvas.pixelRect.width * 2;
                    outSettings.endValue = screen.canvas.pixelRect.width * 2;
                    position.x = outSettings.endValue;

                    break;
                case SlideAxis.Y:
                    inSettings.startValue = screen.canvas.pixelRect.height * 2;
                    outSettings.endValue = screen.canvas.pixelRect.height * 2;
                    position.y = outSettings.endValue;

                    break;
            }

            screen.rectTransform.localPosition = position;
            screen.enabled = false;
        }

        public override async Task PlayForwards()
        {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            screen.enabled = true;
            switch (axis)
            {
                case SlideAxis.X:
                {
                    await Tween.LocalPositionX(screen.rectTransform, inSettings);

                    break;
                }
                case SlideAxis.Y:
                {
                    await Tween.LocalPositionY(screen.rectTransform, inSettings);

                    break;
                }
            }

            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        public override async Task PlayBackwards()
        {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            switch (axis)
            {
                case SlideAxis.X:
                {
                    await Tween.LocalPositionX(screen.rectTransform, outSettings);

                    break;
                }
                case SlideAxis.Y:
                {
                    await Tween.LocalPositionY(screen.rectTransform, outSettings);

                    break;
                }
            }

            screen.enabled = false;
            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        private enum SlideAxis
        {
            X,
            Y,
        }
    }
}
