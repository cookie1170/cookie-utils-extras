using CookieUtils.HealthSystem;

namespace CookieUtils.Extras.Juice.Bindings
{
    public class EffectOnHurt : Effect
    {
        protected Health Health;

        protected override void Awake() {
            base.Awake();
            Health = GetComponent<Health>();
        }

        protected virtual void OnEnable() {
            Health.onHit.AddListener(OnTrigger);
        }

        protected virtual void OnDisable() {
            Health.onHit.RemoveListener(OnTrigger);
        }

        protected void OnTrigger(Health.AttackInfo info) {
            Play(info.HitboxInfo.Direction, info.ContactPoint);
        }
    }
}