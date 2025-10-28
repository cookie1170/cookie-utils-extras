using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CookieUtils.Extras.Juice.Editor
{
    [CustomPropertyDrawer(typeof(TransformTweenInstruction))]
    public class TransformTweenInstructionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            VisualElement root = new();

            Foldout foldout = new() {
                text = property.displayName,
            };
            PropertyField parallelField = new(property.FindPropertyRelative("parallel"));
            PropertyField typeField = new(property.FindPropertyRelative("type"));
            PropertyField normalSettings = new(property.FindPropertyRelative("settings"), "Settings");
            PropertyField shakeSettings = new(property.FindPropertyRelative("shakeSettings"), "Settings");

            CheckType();
            typeField.RegisterValueChangeCallback(_ => CheckType());

            foldout.Add(parallelField);
            foldout.Add(typeField);
            foldout.Add(normalSettings);
            foldout.Add(shakeSettings);
            root.Add(foldout);

            return root;

            void CheckType() {
                var type = (TweenType)property.FindPropertyRelative("type").enumValueIndex;
                if (type is TweenType.Punch or TweenType.Shake) {
                    shakeSettings.style.display = DisplayStyle.Flex;
                    normalSettings.style.display = DisplayStyle.None;
                } else {
                    shakeSettings.style.display = DisplayStyle.None;
                    normalSettings.style.display = DisplayStyle.Flex;
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(ScaleTweenInstruction))]
    public class ScaleTweenInstructionPropertyDrawer : TransformTweenInstructionDrawer { }

    [CustomPropertyDrawer(typeof(RotationTweenInstruction))]
    public class RotationTweenInstructionPropertyDrawer : TransformTweenInstructionDrawer { }
}