using System.Threading.Tasks;
using UnityEngine;

namespace CookieUtils.Extras.Juice
{
    public abstract class Effect : ScriptableObject
    {
        public abstract bool Is2D { get; }
        public abstract Task Play(
            Renderer[] renderers,
            GameObject host,
            Vector3 direction,
            Vector3 contactPoint
        );
    }
}
