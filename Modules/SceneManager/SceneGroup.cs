using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using Eflatun.SceneReference;
using JetBrains.Annotations;
using UnityEngine;

namespace CookieUtils.Extras.SceneManager
{
    [Serializable]
    #if ALCHEMY
    [DisableAlchemyEditor]
    #endif
    public class SceneGroup
    {
        public string name;
        public List<SceneData> scenes;
    }

    [Serializable]
    #if ALCHEMY
    [DisableAlchemyEditor]
    #endif
    public class SceneGroupReference
    {
        public string name;
        public SceneGroup Group => ScenesSettings.Get().FindSceneGroupFromName(name);
    }

    [Serializable]
    #if ALCHEMY
    [DisableAlchemyEditor]
    #endif
    public class SceneData
    {
        public SceneReference scene;
        public SceneType type;

        [Tooltip("Whether to reload the scene if it's already loaded")]
        public bool reloadIfExists = false;

        public string Name => scene.Name;
    }

    [PublicAPI]
    public enum SceneType
    {
        Active,
        Setup,
        Environment,
        UI,
        Other,
    }
}