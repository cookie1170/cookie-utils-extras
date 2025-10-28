using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CookieUtils.Extras.SceneManager.Editor
{
    [CustomPropertyDrawer(typeof(SceneGroupReference))]
    public class SceneGroupReferencePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            VisualElement root = new();

            SerializedProperty name = property.FindPropertyRelative("name");
            ScenesSettings data = ScenesSettings.Get();

            VisualElement layout = new() {
                style = {
                    flexDirection = FlexDirection.Row,
                    paddingLeft = new StyleLength(Length.Pixels(4)),
                },
            };

            DropdownField dropdown = new() {
                value = name.stringValue,
                style = {
                    width = new StyleLength(Length.Percent(50)),
                },
            };

            UpdateChoices();

            dropdown.RegisterCallback<FocusEvent>(_ => UpdateChoices());

            dropdown.RegisterValueChangedCallback(e => {
                    name.stringValue = e.newValue;
                    property.serializedObject.ApplyModifiedProperties();
                }
            );

            Button edit = new(ScenesSettings.OpenWindow) {
                text = "Edit",
            };

            layout.Add(
                new Label(property.displayName) {
                    style = {
                        unityTextAlign = TextAnchor.MiddleLeft,
                    },
                }
            );
            layout.Add(
                new VisualElement {
                    style = {
                        flexGrow = 1f,
                    },
                }
            );
            layout.Add(dropdown);
            layout.Add(edit);
            root.Add(layout);

            return root;

            void UpdateChoices() {
                dropdown.choices = data.groups.ConvertAll(g => g.name);
            }
        }
    }
}