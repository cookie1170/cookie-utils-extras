namespace CookieUtils.Extras.Juice.Bindings
{
    public class EffectOnDeath : EffectOnHurt
    {
        protected override void OnEnable() {
            Health.onDeath.AddListener(OnTrigger);
        }

        protected override void OnDisable() {
            Health.onDeath.RemoveListener(OnTrigger);
        }
    }
}