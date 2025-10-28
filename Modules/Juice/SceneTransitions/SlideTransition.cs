using System;
using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace CookieUtils.Extras.Juice
{
    public class SlideTransition : FullscreenTransition
    {
        [SerializeField] private SlideAxis axis;

        [SerializeField] private TweenSettings<float> inSettings = new() {
            startFromCurrent = false,
            startValue = -2200f,
            endValue = 0f,
            settings = new TweenSettings {
                useUnscaledTime = true,
                duration = 0.5f,
                endDelay = 0.25f,
                ease = Ease.OutQuad,
            },
        };

        [SerializeField] private TweenSettings<float> outSettings = new() {
            startFromCurrent = true,
            startValue = 0f,
            endValue = -2200f,
            settings = new TweenSettings {
                useUnscaledTime = true,
                duration = 0.5f,
                endDelay = 0.25f,
                ease = Ease.InQuad,
            },
        };

        protected override void Awake() {
            base.Awake();
            Vector3 position = screen.rectTransform.localPosition;
            switch (axis) {
                case SlideAxis.X:
                    position.x = outSettings.endValue;

                    break;
                case SlideAxis.Y:
                    position.y = outSettings.endValue;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            screen.rectTransform.localPosition = position;
            screen.enabled = false;
        }

        public override async Task PlayForwards() {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            screen.enabled = true;
            switch (axis) {
                case SlideAxis.X: {
                    await Tween.LocalPositionX(screen.rectTransform, inSettings);

                    break;
                }
                case SlideAxis.Y: {
                    await Tween.LocalPositionY(screen.rectTransform, inSettings);

                    break;
                }
                default: {
                    throw new ArgumentOutOfRangeException();
                }
            }

            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        public override async Task PlayBackwards() {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            switch (axis) {
                case SlideAxis.X: {
                    await Tween.LocalPositionX(screen.rectTransform, outSettings);

                    break;
                }
                case SlideAxis.Y: {
                    await Tween.LocalPositionY(screen.rectTransform, outSettings);

                    break;
                }
                default: {
                    throw new ArgumentOutOfRangeException();
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