using System.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace CookieUtils.Extras.Juice
{
    public class FadeTransition : FullscreenTransition
    {
        [SerializeField] private TweenSettings<float> inSettings = new() {
            endValue = 1,
            startFromCurrent = true,
            settings = new TweenSettings {
                duration = 0.5f,
                endDelay = 0.25f,
                ease = Ease.InQuad,
                useUnscaledTime = true,
            },
        };

        [SerializeField] private TweenSettings<float> outSettings = new() {
            endValue = 0,
            startFromCurrent = true,
            settings = new TweenSettings {
                duration = 0.5f,
                ease = Ease.OutQuad,
                useUnscaledTime = true,
            },
        };

        protected override void Awake() {
            base.Awake();
            Color color = screen.color;
            color.a = 0;
            screen.color = color;
            screen.enabled = false;
        }

        public override async Task PlayForwards() {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            screen.enabled = true;
            await Tween.Alpha(screen, inSettings);
            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }

        public override async Task PlayBackwards() {
            PrimeTweenConfig.warnEndValueEqualsCurrent = false;
            await Tween.Alpha(screen, outSettings);
            screen.enabled = false;
            PrimeTweenConfig.warnEndValueEqualsCurrent = true;
        }
    }
}