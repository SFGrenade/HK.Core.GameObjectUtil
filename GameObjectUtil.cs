using JetBrains.Annotations;
using UnityEngine;
using UScene = UnityEngine.SceneManagement.Scene;

namespace Core.GameObjectUtil
{
    /// <summary>
    ///     Utils specifically for interacting with GameObjects and Scenes.
    /// </summary>
    public static class GameObjectUtil
    {
        /// <summary>
        ///     Finds a GameObject in a given scene at the root level.
        /// </summary>
        /// <param name="scene">The scene to search in</param>
        /// <param name="name">The name of the GameObject</param>
        /// <returns>The found GameObject, null if none is found.</returns>
        [PublicAPI]
        public static GameObject FindRoot(this UScene scene, string name)
        {
            if (scene.IsValid())
            {
                GameObject[] rootGos = scene.GetRootGameObjects();
                int rootGosCount = rootGos.Length;
                int i;
                for (i = 0; i < rootGosCount; i++)
                {
                    if (rootGos[i].name == name)
                    {
                        return rootGos[i];
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Finds a GameObject in a given scene.
        /// </summary>
        /// <param name="scene">The scene to search in</param>
        /// <param name="name">The name of the GameObject</param>
        /// <returns>The found GameObject, null if none is found.</returns>
        [PublicAPI]
        public static GameObject Find(this UScene scene, string name)
        {
            if (!scene.IsValid()) return null;
            GameObject[] rootGos = scene.GetRootGameObjects();
            int rootGosCount = rootGos.Length;
            int i;
            GameObject retGo;
            for (i = 0; i < rootGosCount; i++)
            {
                if (rootGos[i].name == name) return rootGos[i];
                retGo = GameObject.Find(name);
                if (retGo != null) return retGo;
            }
            return null;
        }

        /// <summary>
        ///     Finds a child GameObject of a given GameObject.
        /// </summary>
        /// <param name="o">The GameObject to start the search from</param>
        /// <param name="name">The name of the GameObject</param>
        /// <returns>The found GameObject, null if none is found.</returns>
        [PublicAPI]
        public static GameObject FindGameObjectInChildren(GameObject o, string name) => o.Find(name);
        /// <inheritdoc cref="FindGameObjectInChildren"/>
        [PublicAPI]
        public static GameObject Find(this GameObject o, string name)
        {
            if (o == null)
            {
                return null;
            }
            Transform ot = o.transform;
            int count = ot.childCount;
            GameObject topLevelChild;
            GameObject deeperChild;
            for (int i = 0; i < count; i++)
            {
                topLevelChild = ot.GetChild(i).gameObject;
                if (name == topLevelChild.name) return topLevelChild;
                deeperChild = topLevelChild.Find(name);
                if (deeperChild != null) return deeperChild;
            }
            return null;
        }

        /// <summary>
        ///     Logs a scene with all GameObjects and Components on them.
        /// </summary>
        /// <param name="scene">The scene to log</param>
        [PublicAPI]
        public static void Log(this UScene scene)
        {
            Debug.Log($"[SceneLog] - Scene \"{scene.name}\"");
            foreach (var go in scene.GetRootGameObjects())
                go.transform.Log();
        }

        /// <summary>
        ///     Logs a transform and all children with their Components.
        /// </summary>
        /// <param name="go">The Transform to log</param>
        /// <param name="n">The indentation to use</param>
        [PublicAPI]
        public static void Log(this Transform go, string n = "\t")
        {
            Debug.Log($"[SceneLog] - {n}\"{go.name}\"");
            foreach (var comp in go.GetComponents<Component>())
                Debug.Log($"[SceneLog] - {n} => \"{comp.GetType()}\": {comp}");
            for (var i = 0; i < go.childCount; i++)
                go.GetChild(i).Log($"{n}\t");
        }
    }
}