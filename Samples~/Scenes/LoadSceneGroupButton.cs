using CookieUtils.Extras.SceneManager;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Scenes
{
    public class LoadSceneGroupButton : MonoBehaviour
    {
        [SerializeField] private SceneGroupReference group;

        private void Awake() {
            GetComponent<Button>().onClick
                .AddListener(() => _ = CookieUtils.Extras.SceneManager.Scenes.LoadGroup(group));
        }
    }
}