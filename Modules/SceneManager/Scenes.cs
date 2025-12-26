using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnityScenes = UnityEngine.SceneManagement.SceneManager;
#if DEBUG_CONSOLE
using IngameDebugConsole;
#endif

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace CookieUtils.Extras.SceneManager
{
    [PublicAPI]
    public static class Scenes
    {
        private static ScenesSettings _settings;
        private static SceneTransition _transition;

        private static ScenesSettings Settings
        {
            get
            {
                if (_settings)
                    return _settings;

                _settings = ScenesSettings.Get();
                return _settings;
            }
        }

        public static SceneGroup ActiveGroup { get; private set; }
        public static event Action<SceneGroup, SceneGroup> GroupLoaded = delegate { };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static async void Initialize()
        {
            ActiveGroup = null;
            _transition = null;

            if (!Settings.useSceneManager)
                return;

            SceneGroup startingGroup = null;
#if UNITY_EDITOR
            startingGroup = GetEditorStartingGroup();

            startingGroup ??= Settings.startingGroup.Group;
            LogTrace($"Determined starting group: '{startingGroup.name}'");
#else
            startingGroup = Settings.startingGroup.Group;
#endif

            if (
                Settings.bootstrapScene != null
                && Settings.bootstrapScene.UnsafeReason != SceneReferenceUnsafeReason.Empty
            )
            {
                LogTrace(
                    $"Bootstrap scene found: '{Settings.bootstrapScene.Name}', trying to load"
                );

                if (!IsSceneLoaded(Settings.bootstrapScene))
                    await LoadBootstrapScene();
                else
                    LogInfo("Bootstrap scene is already loaded, skipping");

                FindSceneTransition();
            }
            else
                LogTrace("Bootstrap scene is empty, not loading");

            if (startingGroup != null)
                await LoadGroupAsync(startingGroup, false);

#if UNITY_EDITOR
            InitializeEditorCleanup();
#endif
        }

        public static async Task LoadGroupAsync(SceneGroup targetGroup, bool useTransition = true)
        {
            if (!Settings.useSceneManager)
            {
                LogError(
                    "Scene manager disabled! Can't load scene group, enable it in the project settings"
                );

                return;
            }

            SceneGroup oldGroup = ActiveGroup;

            if (_transition && useTransition)
            {
                LogTrace("Transition exists and used, playing");
                await PlayTransitionForwards();
            }
            if (ActiveGroup != null)
            {
                LogTrace($"Unloading current active group: '{ActiveGroup.name}'");
                await UnloadSceneGroup(ActiveGroup, targetGroup);
            }

            ActiveGroup = targetGroup;

            List<Task> loadTasks = new();

            foreach (SceneData scene in targetGroup.scenes)
            {
                if (LoadScene(scene, out Task task))
                    loadTasks.Add(task);
            }

            await Task.WhenAll(loadTasks);
            LogInfo($"Loaded group '{targetGroup.name}'");
            GroupLoaded(targetGroup, oldGroup);

            if (_transition && useTransition)
                _ = PlayTransitionBackwards();
        }

        private static bool LoadScene(SceneData scene, out Task task)
        {
            task = null;

            if (scene == null)
                return false;

            string name = scene.scene.Name;

            if (UnityScenes.GetSceneByName(name).isLoaded)
            {
                LogTrace($"Scene '{name}' is already loaded, skipping");
                return false;
            }

            AsyncOperation operation = UnityScenes.LoadSceneAsync(name, LoadSceneMode.Additive);

            if (operation == null)
            {
                LogWarning($"Failed to load scene '{name}'");
                return false;
            }

            LogTrace($"Loading scene '{name}'");

            if (Settings.logLevel <= LogLevel.Trace)
            {
                operation.completed += _ =>
                {
                    LogTrace($"Loaded scene '{name}'");
                };
            }

            if (scene.type == SceneType.Active)
                operation.completed += _ =>
                {
                    LogTrace($"Scene '{name}' is the Active type, setting as active");
                    UnityScenes.SetActiveScene(UnityScenes.GetSceneByName(name));
                };

            task = Task.FromResult(operation);
            return true;
        }

        public static async Task UnloadSceneGroup(SceneGroup group, SceneGroup newGroup = null)
        {
            if (!Settings.useSceneManager)
            {
                LogError(
                    "Scene manager disabled! Can't unload scene group, enable it in the project settings"
                );

                return;
            }

            if (group == null || group.scenes.Count == 0)
            {
                LogTrace("Group is null or empty, cancelling unload");
                return;
            }

            int count = group.scenes.Count;
            LogTrace($"Group '{group.name}' contains {count} scenes");

            List<Task> tasks = new();

            for (int i = 0; i < count; i++)
            {
                SceneData scene = group.scenes[i];
                LogTrace($"Trying to unload scene '{scene.Name}' with index {i}");

                if (UnloadScene(newGroup, scene, out Task task))
                    tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            LogInfo($"Unloaded group '{group.name}'");
        }

        private static bool UnloadScene(SceneGroup newGroup, SceneData scene, out Task task)
        {
            task = null;

            if (!scene.reloadIfExists && newGroup != null)
            {
                SceneData matchingScene = newGroup.scenes.Find(s =>
                    scene.scene.Name == s.scene.Name
                );

                if (matchingScene != null)
                {
                    LogTrace($"Scene '{scene.Name}' found in group '{newGroup.name}', keeping");
                    return false;
                }
            }

            LogTrace($"Unloading scene '{scene.Name}'");
            AsyncOperation operation = UnityScenes.UnloadSceneAsync(scene.scene.Name);
            task = Task.FromResult(operation);

            return true;
        }

        public static Task PlayTransitionForwards()
        {
            LogTrace("Playing transition forwards");
            return _transition.PlayForwards();
        }

        public static Task PlayTransitionBackwards()
        {
            LogTrace("Playing transition backwards");
            return _transition.PlayBackwards();
        }

        public static async Task UnloadAllScenes()
        {
            if (!Settings.useSceneManager)
            {
                LogError(
                    "Scene manager disabled! Can't unload all scenes, enable it in the project settings"
                );

                return;
            }

            int sceneCount = UnityScenes.sceneCount;
            int unloadedScenes = 0;

            List<Task> tasks = new(unloadedScenes);

            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = UnityScenes.GetSceneAt(i);

                if (scene.name == Settings.bootstrapScene.Name)
                    continue;

                LogTrace($"Unloading scene '{scene.name}' at index {i}");

                AsyncOperation operation = UnityScenes.UnloadSceneAsync(scene);
                unloadedScenes++;
                tasks.Add(Task.FromResult(operation));
            }

            await Task.WhenAll(tasks);

            if (unloadedScenes > 0)
                LogInfo($"Unloaded {unloadedScenes} scenes");
        }

        private static bool IsSceneLoaded(SceneReference scene)
        {
            int loadedCount = UnityScenes.sceneCount;

            for (int i = 0; i < loadedCount; i++)
            {
                if (UnityScenes.GetSceneAt(i).name == scene.Name)
                    return true;
            }

            return false;
        }

#if DEBUG_CONSOLE
        [ConsoleMethod("load", "Loads the specified scene group")]
#endif
        public static void LoadGroup(string groupName, bool useTransition = true)
        {
            _ = LoadGroupAsync(groupName, useTransition);
        }

        public static void LoadGroup(SceneGroupReference group, bool useTransition = true)
        {
            _ = LoadGroupAsync(group.Group, useTransition);
        }

        public static void LoadGroup(SceneGroup targetGroup, bool useTransition = true)
        {
            _ = LoadGroupAsync(targetGroup, useTransition);
        }

        public static async Task LoadGroupAsync(string groupName)
        {
            await LoadGroupAsync(groupName, true);
        }

        public static async Task LoadGroupAsync(string groupName, bool useTransition)
        {
            SceneGroup targetGroup = Settings.FindSceneGroupFromName(groupName);

            await LoadGroupAsync(targetGroup, useTransition);
        }

        public static async Task LoadGroupAsync(
            SceneGroupReference group,
            bool useTransition = true
        )
        {
            await LoadGroupAsync(group.Group, useTransition);
        }

        private static async Task LoadBootstrapScene()
        {
            await UnityScenes.LoadSceneAsync(Settings.bootstrapScene.Name);
            LogInfo("Loaded the bootstrap scene");
        }

        private static void FindSceneTransition()
        {
            // ugly but only called once so should be fine
            var transition = Object.FindAnyObjectByType<SceneTransition>(
                FindObjectsInactive.Include
            );

            if (!transition)
                return;

            LogTrace("Found a scene transition in the bootstrap scene");
            _transition = transition;
        }

#if UNITY_EDITOR
        private static SceneGroup GetEditorStartingGroup()
        {
            SceneGroupToolbarElement instance = SceneGroupToolbarElement.instance;

            if (!instance)
                return null;

            return instance.selectedState switch
            {
                SceneGroupSelection.Group => instance.selectedGroup,
                SceneGroupSelection.Auto => GetAutoGroup(),
                _ => null,
            };
        }

        private static SceneGroup GetAutoGroup()
        {
            if (UnityScenes.sceneCount <= 0)
            {
                LogTrace("No loaded scenes, falling back to the default scene group");
                return null;
            }

            Scene activeScene = UnityScenes.GetActiveScene();

            if (activeScene == null)
            {
                LogTrace("Active scene is null, trying the first group");
                activeScene = UnityScenes.GetSceneAt(0);
            }

            SceneGroup groupWithActive = Settings.groups.Find(g =>
                g.FindActiveGroup().Name == activeScene.name
            );

            if (groupWithActive != null)
            {
                LogTrace(
                    $"Found a group with '{activeScene.name}' as the active scene: '{groupWithActive.name}'"
                );
                return groupWithActive;
            }

            LogTrace($"Couldn't find a group with '{activeScene.name}' as the active scene");

            SceneGroup groupWithAny = Settings.groups.Find(g =>
                g.scenes.Any(s => s.Name == activeScene.name)
            );

            if (groupWithAny == null)
            {
                LogTrace($"Couldn't find a group with '{activeScene.name}'");
            }
            else
                LogTrace($"Found a group with '{activeScene.name}': '{groupWithAny.name}'");

            return groupWithAny;
        }

        private static void InitializeEditorCleanup()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            static void OnPlayModeStateChanged(PlayModeStateChange change)
            {
                if (change == PlayModeStateChange.ExitingPlayMode)
                {
                    ActiveGroup = null;
                    _transition = null;
                    _settings = null;
                    GroupLoaded = delegate { };
                }
            }
        }
#endif

        private static string GetLogMessage(string message, LogLevel level)
        {
            return $"[CookieUtils.Extras.SceneManager - {level}] {message}";
        }

        private static void LogError(string message)
        {
            if (Settings.logLevel > LogLevel.Error)
                return;

            Debug.LogError(GetLogMessage(message, LogLevel.Error));
        }

        private static void LogWarning(string message)
        {
            if (Settings.logLevel > LogLevel.Warning)
                return;

            Debug.LogWarning(GetLogMessage(message, LogLevel.Warning));
        }

        private static void LogInfo(string message) => Log(message, LogLevel.Info);

        private static void LogTrace(string message) => Log(message, LogLevel.Trace);

        private static void Log(string message, LogLevel level)
        {
            if (Settings.logLevel > level)
                return;

            Debug.Log(GetLogMessage(message, level));
        }
    }
}
