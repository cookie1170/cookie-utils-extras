using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using IngameDebugConsole;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CookieUtils.Extras.SceneManager
{
    [PublicAPI]
    public static class Scenes
    {
        private static ScenesSettings _settings;
        private static SceneTransition _transition;

        public static SceneGroup ActiveGroup { get; private set; }
        public static event Action<SceneGroup> OnGroupLoaded = delegate { };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static async void Initialize() {
            _transition = null;

            _settings = ScenesSettings.Get();

            if (!_settings.useSceneManager) return;

            if (_settings.bootstrapScene.UnsafeReason != SceneReferenceUnsafeReason.Empty) {
                if (!IsSceneLoaded(_settings.bootstrapScene))
                    await LoadBootstrapScene();
                else Debug.Log("[CookieUtils.Extras.SceneManager] Bootstrap scene already loaded, skipping");

                FindSceneTransition();
            }

            if (_settings.startingGroup.Group != null)
                await LoadGroup(_settings.startingGroup, false);
        }

        private static async Task LoadBootstrapScene() {
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_settings.bootstrapScene.BuildIndex);
            Debug.Log("[CookieUtils.Extras.SceneManager] Loaded bootstrap scene");
        }

        private static void FindSceneTransition() {
            var
                transition =
                    Object.FindFirstObjectByType<SceneTransition>(
                        FindObjectsInactive.Include
                    ); // ugly but only called once so should be fine

            if (!transition) return;

            Debug.Log("[CookieUtils.Extras.SceneManager] Found scene transition in bootstrap scene");
            _transition = transition;
        }

        #if DEBUG_CONSOLE
        [ConsoleMethod("load", "Loads the specified scene group")]
        #endif
        public static async Task LoadGroup(string groupName) {
            await LoadGroup(groupName, true);
        }

        public static async Task LoadGroup(string groupName, bool useTransition) {
            SceneGroup targetGroup = _settings.FindSceneGroupFromName(groupName);

            await LoadGroup(targetGroup, useTransition);
        }

        public static async Task LoadGroup(SceneGroupReference group, bool useTransition = true) {
            await LoadGroup(group.Group, useTransition);
        }

        public static async Task LoadGroup(SceneGroup targetGroup, bool useTransition = true) {
            if (!_settings.useSceneManager) {
                Debug.LogWarning(
                    "[CookieUtils.Extras.SceneManager] Scene manager disabled! Can't load scene group, enable it in the project settings"
                );

                return;
            }

            if (_transition && useTransition) await _transition.PlayForwards();
            if (ActiveGroup != null) await UnloadSceneGroup(ActiveGroup, targetGroup);

            ActiveGroup = targetGroup;

            List<Task> loadTasks = new();

            foreach (SceneData scene in targetGroup.scenes) {
                int buildIndex = scene.scene.BuildIndex;

                if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(buildIndex).isLoaded) continue;

                AsyncOperation operation =
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);

                if (operation == null) continue;

                if (scene.type == SceneType.Active)
                    operation.completed += _ =>
                        UnityEngine.SceneManagement.SceneManager.SetActiveScene(
                            UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(buildIndex)
                        );

                loadTasks.Add(Task.FromResult(operation));
            }

            await Task.WhenAll(loadTasks);
            Debug.Log($"[CookieUtils.Extras.SceneManager] Loaded group {targetGroup.name}");
            OnGroupLoaded(targetGroup);

            if (_transition && useTransition) _ = _transition.PlayBackwards();
        }

        public static async Task UnloadSceneGroup(SceneGroup group, SceneGroup newGroup = null) {
            if (!_settings.useSceneManager) {
                Debug.LogWarning(
                    "[CookieUtils.Extras.SceneManager] Scene manager disabled! Can't unload scene group, enable it in the project settings"
                );

                return;
            }

            if (group == null || group.scenes.Count == 0) return;

            int count = group.scenes.Count;

            List<Task> tasks = new();

            for (int i = 0; i < count; i++) {
                SceneData scene = group.scenes[i];

                if (!scene.reloadIfExists && newGroup != null)
                    if (newGroup.scenes.Find(s => scene.scene.BuildIndex == s.scene.BuildIndex) != null)
                        continue;

                AsyncOperation operation =
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene.scene.BuildIndex);
                tasks.Add(Task.FromResult(operation));
            }

            await Task.WhenAll(tasks);
            Debug.Log($"[CookieUtils.Extras.SceneManager] Unloaded group {group.name}");
        }

        public static async Task UnloadAllScenes() {
            if (!_settings.useSceneManager) {
                Debug.LogWarning(
                    "[CookieUtils.Extras.SceneManager] Scene manager disabled! Can't unload all scenes, enable it in the project settings"
                );

                return;
            }

            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            int unloadedScenes = 0;

            List<Task> tasks = new(unloadedScenes);

            for (int i = 0; i < sceneCount; i++) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                if (scene.buildIndex == _settings.bootstrapScene.BuildIndex) continue;

                AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                unloadedScenes++;
                tasks.Add(Task.FromResult(operation));
            }

            await Task.WhenAll(tasks);

            if (unloadedScenes > 0) Debug.Log($"[CookieUtils.Extras.SceneManager] Unloaded {unloadedScenes} scenes");
        }

        private static bool IsSceneLoaded(SceneReference scene) {
            int loadedCount = UnityEngine.SceneManagement.SceneManager.sceneCount;

            for (int i = 0; i < loadedCount; i++) {
                if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).buildIndex == scene.BuildIndex)
                    return true;
            }

            return false;
        }
    }
}