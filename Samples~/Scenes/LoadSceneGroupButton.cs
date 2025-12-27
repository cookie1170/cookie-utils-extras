using CookieUtils.Extras.SceneManager;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Scenes
{
    [RequireComponent(typeof(Button))]
    public class LoadSceneGroupButton : MonoBehaviour
    {
        [SerializeField]
        private SceneGroupReference group;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(LoadGroup);
        }

        private void LoadGroup()
        {
            CookieUtils.Extras.SceneManager.Scenes.LoadGroup(group);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
