using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CookieUtils.Extras.SceneManager.Editor
{
    [CustomPropertyDrawer(typeof(SceneGroupReference))]
    public class SceneGroupReferencePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();

            SerializedProperty name = property.FindPropertyRelative("name");
            ScenesSettings data = ScenesSettings.Get();

            VisualElement layout = new() { style = { flexDirection = FlexDirection.Row } };

            DropdownField dropdown = new()
            {
                value = name.stringValue,
                label = property.displayName,
                style = { flexGrow = 1f },
            };
            dropdown.AddToClassList("unity-base-field__aligned");

            UpdateChoices();

            dropdown.RegisterCallback<FocusEvent>(_ => UpdateChoices());

            dropdown.RegisterValueChangedCallback(e =>
            {
                name.stringValue = e.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });

            Button edit = new(ScenesSettings.OpenWindow) { text = "Edit" };

            layout.Add(dropdown);
            layout.Add(edit);
            root.Add(layout);

            return root;

            void UpdateChoices()
            {
                dropdown.choices = data.groups.ConvertAll(g => g.name);
            }
        }
    }
}
