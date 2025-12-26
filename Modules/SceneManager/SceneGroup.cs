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

        public SceneData FindActiveGroup()
        {
            return scenes.Find(s => s.type == SceneType.Active);
        }
    }

    [Serializable]
#if ALCHEMY
    [DisableAlchemyEditor]
#endif
    public class SceneGroupReference
    {
        public string name;
        public SceneGroup Group => ScenesSettings.Get().FindSceneGroupFromName(name);

        public static implicit operator SceneGroup(SceneGroupReference groupRef)
        {
            return groupRef.Group;
        }
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
