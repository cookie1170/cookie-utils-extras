using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CookieUtils.Extras.Juice.Editor
{
    [CustomEditor(typeof(EffectData))]
    public class EffectDataEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset inspector;

        public override VisualElement CreateInspectorGUI() {
            VisualElement root = new();

            inspector.CloneTree(root);

            var data = (EffectData)target;

            var shakeCamera = root.Q<PropertyField>("ShakeCamera");
            var shakeForce = root.Q<PropertyField>("ShakeForce");
            var spawnParticles = root.Q<PropertyField>("SpawnParticles");
            var hideIfNoParticles = root.Q<VisualElement>("HideIfNoParticles");
            var playAudio = root.Q<PropertyField>("PlayAudio");
            var hideIfNoAudio = root.Q<VisualElement>("HideIfNoAudio");
            var animateScale = root.Q<PropertyField>("AnimateScale");
            var scaleSettings = root.Q<PropertyField>("ScaleSettings");
            var animateRotation = root.Q<PropertyField>("AnimateRotation");
            var rotationSettings = root.Q<PropertyField>("RotationSettings");
            var animateFlash = root.Q<PropertyField>("AnimateFlash");
            var hideIfNoFlash = root.Q<VisualElement>("HideIfNoFlash");

            shakeCamera.RegisterValueChangeCallback(_ =>
                shakeForce.style.display = data.shakeCamera ? DisplayStyle.Flex : DisplayStyle.None
            );

            spawnParticles.RegisterValueChangeCallback(_ =>
                hideIfNoParticles.style.display = data.spawnParticles ? DisplayStyle.Flex : DisplayStyle.None
            );

            playAudio.RegisterValueChangeCallback(_ =>
                hideIfNoAudio.style.display = data.playAudio ? DisplayStyle.Flex : DisplayStyle.None
            );

            animateScale.RegisterValueChangeCallback(_ =>
                scaleSettings.style.display = data.animateScale ? DisplayStyle.Flex : DisplayStyle.None
            );

            animateRotation.RegisterValueChangeCallback(_ =>
                rotationSettings.style.display = data.animateRotation ? DisplayStyle.Flex : DisplayStyle.None
            );

            animateFlash.RegisterValueChangeCallback(_ =>
                hideIfNoFlash.style.display = data.animateFlash ? DisplayStyle.Flex : DisplayStyle.None
            );

            return root;
        }
    }
}