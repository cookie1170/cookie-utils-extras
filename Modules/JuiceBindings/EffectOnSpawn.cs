namespace CookieUtils.Extras.Juice.Bindings
{
    public class EffectOnSpawn : EffectPlayer
    {
        protected override void Awake() { }

        private void OnEnable()
        {
            if (!didStart)
                Initialize();

            Play();
        }
    }
}
