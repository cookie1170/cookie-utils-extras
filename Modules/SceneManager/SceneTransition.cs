using System.Threading.Tasks;
using UnityEngine;

namespace CookieUtils.Extras.SceneManager
{
    public abstract class SceneTransition : MonoBehaviour
    {
        public abstract Task PlayForwards();
        public abstract Task PlayBackwards();
    }
}
