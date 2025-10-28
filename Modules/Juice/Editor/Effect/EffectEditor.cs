using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CookieUtils.Extras.Juice.Editor
{
    [CustomEditor(typeof(Effect))]
    public class EffectEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset inspector;

        public override VisualElement CreateInspectorGUI() {
            VisualElement root = new();

            inspector.CloneTree(root);

            var effect = (Effect)target;

            var dataObjectInspectorPanel = root.Q<VisualElement>("DataObjectInspectorPanel");
            var createDataObject = root.Q<Button>("CreateDataObject");
            var dataTitle = root.Q<Foldout>("DataTitle");
            var dataObject = root.Q<PropertyField>("DataObject");
            var overrideRenderer = root.Q<PropertyField>("OverrideRenderer");
            var rendererOverride = root.Q<PropertyField>("RendererOverride");
            var overrideMaterial = root.Q<PropertyField>("OverrideMaterial");
            var materialOverride = root.Q<PropertyField>("MaterialOverride");
            var previewButton = root.Q<Button>("Preview");

            CheckDataObject();

            previewButton.SetEnabled(EditorApplication.isPlaying);

            previewButton.RegisterCallback<ClickEvent>(_ => effect.Play());

            dataObject.RegisterValueChangeCallback(_ => CheckDataObject());

            createDataObject.RegisterCallback<ClickEvent>(CreateDataObject);

            overrideRenderer.RegisterValueChangeCallback(_ =>
                rendererOverride.style.display = effect.overrideRenderers ? DisplayStyle.Flex : DisplayStyle.None
            );

            overrideMaterial.RegisterValueChangeCallback(_ =>
                materialOverride.style.display = effect.overrideMaterial ? DisplayStyle.Flex : DisplayStyle.None
            );

            return root;

            void CheckDataObject() {
                var dataInspectorCurrent = dataObjectInspectorPanel.Q<VisualElement>("DataInspector");
                if (dataInspectorCurrent != null) dataTitle.Remove(dataInspectorCurrent);

                if (effect.data) {
                    createDataObject.style.display = DisplayStyle.None;
                    InspectorElement dataInspector = new(effect.data) {
                        name = "DataInspector",
                    };

                    dataObjectInspectorPanel.style.display = DisplayStyle.Flex;
                    dataTitle.Add(dataInspector);
                    dataTitle.text = effect.data.name;
                } else {
                    createDataObject.style.display = DisplayStyle.Flex;
                    dataObjectInspectorPanel.style.display = DisplayStyle.None;
                }
            }

            void CreateDataObject(ClickEvent evt) {
                string path = EditorUtility.SaveFilePanelInProject(
                    "Create hurt effect  data", $"{effect.name}_EffectData", "asset",
                    "Choose a path for the data object"
                );

                var data = CreateInstance<EffectData>();

                AssetDatabase.CreateAsset(data, path);
                Undo.RegisterCreatedObjectUndo(data, "Created effect data");
                Undo.RecordObject(effect, "Assigned effect data");
                effect.data = data;

                CheckDataObject();
            }
        }
    }
}