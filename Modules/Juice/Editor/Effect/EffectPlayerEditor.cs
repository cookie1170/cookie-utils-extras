using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CookieUtils.Extras.Juice.Editor
{
    [CustomEditor(typeof(EffectPlayer))]
    public class EffectPlayerEditor : UnityEditor.Editor
    {
        [SerializeField]
        private VisualTreeAsset inspector;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            inspector.CloneTree(root);

            var player = (EffectPlayer)target;

            var dataObjectInspectorPanel = root.Q<VisualElement>("DataObjectInspectorPanel");
            var dataTitle = root.Q<Foldout>("DataTitle");
            var dataObject = root.Q<PropertyField>("DataObject");
            var overrideRenderer = root.Q<PropertyField>("OverrideRenderer");
            var rendererOverride = root.Q<PropertyField>("RendererOverride");
            var previewButton = root.Q<Button>("Preview");

            CheckDataObject();

            previewButton.SetEnabled(EditorApplication.isPlaying);

            previewButton.RegisterCallback<ClickEvent>(ev => _ = player.Play());

            dataObject.RegisterValueChangeCallback(_ => CheckDataObject());

            overrideRenderer.RegisterValueChangeCallback(_ =>
                rendererOverride.style.display = player.overrideRenderers
                    ? DisplayStyle.Flex
                    : DisplayStyle.None
            );

            return root;

            void CheckDataObject()
            {
                var dataInspectorCurrent = dataObjectInspectorPanel.Q<VisualElement>(
                    "DataInspector"
                );
                if (dataInspectorCurrent != null)
                    dataTitle.Remove(dataInspectorCurrent);

                if (player.effect)
                {
                    InspectorElement dataInspector = new(player.effect) { name = "DataInspector" };

                    dataObjectInspectorPanel.style.display = DisplayStyle.Flex;
                    dataTitle.Add(dataInspector);
                    dataTitle.text = player.effect.name;
                }
                else
                {
                    dataObjectInspectorPanel.style.display = DisplayStyle.None;
                }
            }
        }
    }
}
