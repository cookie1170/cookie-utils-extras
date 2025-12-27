// Extending the main toolbar was only added in 6000.3
// we also need to put it in this assembly because you can't have circular references between assemblies

#if UNITY_EDITOR && UNITY_6000_3_OR_NEWER
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace CookieUtils.Extras.SceneManager
{
    internal class SceneGroupToolbarElement : ScriptableSingleton<SceneGroupToolbarElement>
    {
        public SceneGroup selectedGroup = null;
        public SceneGroupSelection selectedState = SceneGroupSelection.Auto;

        private const string path = "Cookie Utils/Scene Group";
        private const string IconName = "UnityLogo";

        private void OnEnable()
        {
            Scenes.GroupLoaded -= OnGroupLoaded;
            Scenes.GroupLoaded += OnGroupLoaded;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            Scenes.GroupLoaded -= OnGroupLoaded;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        [MainToolbarElement(path, defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement CreateDropdown()
        {
            MainToolbarDropdown dropdown = new(instance.GetContent(), ShowDropdownMenu);

            return dropdown;
        }

        private static void ShowDropdownMenu(Rect rect)
        {
            GenericMenu menu = new();

            if (!Application.isPlaying)
            {
                menu.AddItem(
                    new GUIContent("Auto"),
                    false,
                    static () =>
                    {
                        instance.selectedGroup = null;
                        instance.selectedState = SceneGroupSelection.Auto;
                        MainToolbar.Refresh(path);
                    }
                );
                menu.AddItem(
                    new GUIContent("Default"),
                    false,
                    static () =>
                    {
                        instance.selectedGroup = null;
                        instance.selectedState = SceneGroupSelection.Default;
                        MainToolbar.Refresh(path);
                    }
                );
            }

            foreach (SceneGroup group in ScenesSettings.Get().groups)
            {
                menu.AddItem(
                    new GUIContent(group.name),
                    false,
                    async () =>
                    {
                        if (Application.isPlaying)
                        {
                            // the first half of the transition doesn't play without the next frame wait
                            await Awaitable.NextFrameAsync();
                            await Scenes.LoadGroupAsync(group);
                            return;
                        }

                        instance.selectedGroup = group;
                        instance.selectedState = SceneGroupSelection.Group;

                        MainToolbar.Refresh(path);
                    }
                );
            }

            menu.DropDown(rect);
        }

        private MainToolbarContent GetContent()
        {
            string label;
            if (!Application.isPlaying)
            {
                label = selectedState switch
                {
                    SceneGroupSelection.Group => selectedGroup?.name,
                    SceneGroupSelection.Auto => "Auto",
                    SceneGroupSelection.Default => "Default",
                    _ => "None",
                };
            }
            else
            {
                label = Scenes.ActiveGroup?.name;
            }

            label ??= "None";

            Texture2D icon = EditorGUIUtility.IconContent(IconName).image as Texture2D;
            MainToolbarContent content = new(label, icon, "Select active scene group");

            return content;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            MainToolbar.Refresh(path);
        }

        private static void OnGroupLoaded(SceneGroup group, SceneGroup old)
        {
            MainToolbar.Refresh(path);
        }
    }

    public enum SceneGroupSelection
    {
        None,
        Group,
        Auto,
        Default,
    }
}
#endif
