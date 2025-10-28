using UnityEngine;

namespace Samples.Scenes
{
    public class KeepIfExistsTest : MonoBehaviour
    {
        private float _value;

        private void OnGUI() {
            GUILayout.Label($"This will stay between loads! {_value:####}");
            _value = GUILayout.HorizontalSlider(_value, 0, 100);
        }
    }
}