using CookieUtils.Extras.SceneManager;
using UnityEngine;
using UnityEngine.UI;

namespace CookieUtils.Extras.Juice
{
    public abstract class FullscreenTransition : SceneTransition
    {
        [SerializeField] protected Graphic screen;

        protected virtual void Awake() {
            if (!screen) screen = GetComponentInChildren<Graphic>();
        }
    }
}