using System.Reflection;
using JetBrains.Annotations;
using Modding;
using UnityEngine;
using UScene = UnityEngine.SceneManagement.Scene;
using UObject = UnityEngine.Object;

namespace Core.GameObjectUtil;

/// <summary>
///     Utils specifically for interacting with GameObjects and Scenes.
/// </summary>
public static class GameObjectUtil
{
    #region Find GameObject

    /// <summary>
    ///     Finds a GameObject in a given scene at the root level.
    /// </summary>
    /// <param name="scene">The scene to search in</param>
    /// <param name="name">The name of the GameObject</param>
    /// <returns>The found GameObject, null if none is found.</returns>
    [PublicAPI]
    public static GameObject FindRoot(this UScene scene, string name)
    {
        if (!scene.IsValid()) return null;
        
        GameObject[] rootGos = scene.GetRootGameObjects();
        int rootGosCount = rootGos.Length;
        int i;
        for (i = 0; i < rootGosCount; i++)
            if (rootGos[i].name == name)
                return rootGos[i];
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
            if (rootGos[i].name == name)
                return rootGos[i];
            retGo = GameObject.Find(name);
            if (retGo != null)
                return retGo;
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
        if (o == null) return null;
        
        Transform ot = o.transform;
        int count = ot.childCount;
        GameObject topLevelChild;
        GameObject deeperChild;
        for (int i = 0; i < count; i++)
        {
            topLevelChild = ot.GetChild(i).gameObject;
            if (name == topLevelChild.name)
                return topLevelChild;
            deeperChild = topLevelChild.Find(name);
            if (deeperChild != null)
                return deeperChild;
        }
        return null;
    }

    #endregion

    #region Get DontDestroyOnLoad Scene

    /// <summary>
    ///     Gets the permanent scene where object go that are marked DontDestroyOnLoad.
    /// </summary>
    /// <returns>The DontDestroyOnLoad scene</returns>
    [PublicAPI]
    public static UScene GetDontDestroyOnLoadScene()
    {
        GameObject tmp = new GameObject();
        UObject.DontDestroyOnLoad(tmp);
        UScene ret = tmp.scene;
        UObject.Destroy(tmp);
        return ret;
    }

    #endregion

    #region Copy Component

    /// <summary>
    ///     Adds a Component to a GameObject with the same values as a preexisting Component.
    /// </summary>
    /// <param name="comp">The Component to take the values from</param>
    /// <param name="go">The GameObject to add the new component to</param>
    /// <param name="includePrivateFields">Whether or not to include private fields in the copying</param>
    /// <param name="includeStaticFields">Whether or not to include static fields in the copying</param>
    /// <typeparam name="T">The type of the component to copy</typeparam>
    /// <returns>The resulting copied component</returns>
    [PublicAPI]
    public static T CopyOnto<T>(this T comp, GameObject go, bool includePrivateFields = false, bool includeStaticFields = false) where T : Component
    {
        BindingFlags bFlags = BindingFlags.Instance | BindingFlags.Public;
        if (includePrivateFields)
            bFlags |= BindingFlags.NonPublic;
        if (includeStaticFields)
            bFlags |= BindingFlags.Static;
        T newComp = go.AddComponent<T>();
        foreach (var field in typeof(T).GetFields(bFlags))
        {
            ReflectionHelper.SetFieldSafe(newComp, field.Name, ReflectionHelper.GetField<T, object>(comp, field.Name));
        }
        return newComp;
    }

    #endregion

    #region Log

    /// <summary>
    ///     Logs a scene with all GameObjects and Components on them.
    /// </summary>
    /// <param name="scene">The scene to log</param>
    [PublicAPI]
    public static void LogStructure(this UScene scene)
    {
        InternalLogger.Log($"[SceneLog] - Scene \"{scene.name}\"");
        foreach (var go in scene.GetRootGameObjects())
            go.transform.LogStructure();
    }

    /// <summary>
    ///     Logs a transform and all children with their Components.
    /// </summary>
    /// <param name="go">The Transform to log</param>
    /// <param name="n">The indentation to use</param>
    [PublicAPI]
    public static void LogStructure(this Transform go, string n = "\t")
    {
        InternalLogger.Log($"[SceneLog] - {n}\"{go.name}\"");
        foreach (var comp in go.GetComponents<Component>())
            InternalLogger.Log($"[SceneLog] - {n} => \"{comp.GetType()}\": {comp}");
        for (var i = 0; i < go.childCount; i++)
            go.GetChild(i).LogStructure($"{n}\t");
    }

    #endregion
}