namespace CookieUtils.Extras.Juice.Bindings
{
    public class EffectOnSpawn : Effect
    {
        protected override void Awake() { }

        private void OnEnable() {
            if (!didStart) Initialize();

            Play();
        }
    }
}