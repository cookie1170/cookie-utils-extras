using System;
using PrimeTween;
using UnityEngine;

namespace CookieUtils.Extras.Juice
{
    [Serializable]
    public abstract class TweenInstruction<T> where T : struct
    {
        public bool parallel;
        public TweenSettings<T> settings;
    }

    [Serializable]
    public abstract class TransformTweenInstruction : TweenInstruction<Vector3>
    {
        public TweenType type;
        public ShakeSettings shakeSettings;

        public Tween Process(Transform target) {
            return type switch {
                TweenType.Normal => ProcessNormal(target),
                TweenType.Punch => ProcessPunch(target),
                TweenType.Shake => ProcessShake(target),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        protected abstract Tween ProcessNormal(Transform target);
        protected abstract Tween ProcessPunch(Transform target);
        protected abstract Tween ProcessShake(Transform target);
    }

    [Serializable]
    public class ScaleTweenInstruction : TransformTweenInstruction
    {
        protected override Tween ProcessNormal(Transform target) => Tween.Scale(target, settings);
        protected override Tween ProcessPunch(Transform target) => Tween.PunchScale(target, shakeSettings);
        protected override Tween ProcessShake(Transform target) => Tween.ShakeScale(target, shakeSettings);
    }

    [Serializable]
    public class RotationTweenInstruction : TransformTweenInstruction
    {
        protected override Tween ProcessNormal(Transform target) => Tween.LocalRotation(target, settings);
        protected override Tween ProcessPunch(Transform target) => Tween.PunchLocalRotation(target, shakeSettings);
        protected override Tween ProcessShake(Transform target) => Tween.ShakeLocalRotation(target, shakeSettings);
    }

    [Serializable]
    public class FloatTweenInstruction : TweenInstruction<float> { }

    public enum TweenType
    {
        Normal,
        Punch,
        Shake,
    }
}