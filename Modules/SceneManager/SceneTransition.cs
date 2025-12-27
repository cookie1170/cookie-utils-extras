using System;
using System.Threading.Tasks;
using UnityEngine;

namespace CookieUtils.Extras.SceneManager
{
    public abstract class SceneTransition : MonoBehaviour
    {
        public abstract IProgress<float> Progress { get; }
        public abstract void ShowImmediately();
        public abstract void HideImmediately();
        public abstract Task Show();
        public abstract Task Hide();
    }
}
