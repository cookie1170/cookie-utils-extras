using System;
using System.Collections.Generic;
using System.Text;
using Alchemy.Inspector;
using Eflatun.SceneReference;
using IngameDebugConsole;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace CookieUtils.Extras.SceneManager
{
    #if ALCHEMY
    [DisableAlchemyEditor]
    #endif
    [PublicAPI]
    [SettingsObject(
        "ScenesSettings",
        "Scenes settings",
        "Cookie Utils/Scenes",
        "Scenes", "Cookie Utils", "Scene", "Groups", "Group"
    )]
    public class ScenesSettings : SettingsObject<ScenesSettings>
    {
        public bool useSceneManager = true;
        public SceneGroupReference startingGroup;
        public SceneReference bootstrapScene;
        public List<SceneGroup> groups;

        public SceneGroup FindSceneGroupFromName(string groupName) {
            if (string.IsNullOrWhiteSpace(groupName)) {
                Debug.LogError("[CookieUtils.Extras.SceneManager] Group name is null or whitespace!");

                return null;
            }

            SceneGroup targetGroup = groups.Find(g => g.name == groupName);

            if (targetGroup != null) return targetGroup;
            {
                targetGroup = groups.Find(g =>
                    string.Equals(g.name, groupName, StringComparison.CurrentCultureIgnoreCase)
                );

                if (targetGroup != null) return targetGroup;

                Debug.LogError($"[CookieUtils.Extras.SceneManager] Group \"{groupName}\" not found!");

                return null;
            }
        }

        #if DEBUG_CONSOLE
        [ConsoleMethod("groups", "Prints all scene groups")]
        #endif
        public static void PrintAllGroups() {
            string groups = Get().GetAllGroups();
            Debug.Log(groups);
        }

        private string GetAllGroups() {
            StringBuilder builder = new();
            builder.AppendLine("[CookieUtils.Extras.SceneManager]");
            builder.AppendLine("Scene groups:");
            foreach (SceneGroup group in groups) {
                builder.AppendLine($"  {group.name}:");
                foreach (SceneData scene in group.scenes) builder.AppendLine($"    {scene.Name}");
            }

            return builder.ToString();
        }

        #if UNITY_EDITOR
        [SettingsProvider]
        private static SettingsProvider ProvideSettings() => GetSettings();
        #endif
    }
}