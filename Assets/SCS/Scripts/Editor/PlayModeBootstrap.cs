using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeBootstrap
{
    private const string startScenePath = "Assets/SCS/Scenes/BootstrapScene.unity";

    static PlayModeBootstrap()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            SceneAsset startSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(startScenePath);

            if (startSceneAsset == null)
            {
                Debug.LogWarning($"Start scene not found at: {startScenePath}");
                return;
            }

            EditorSceneManager.playModeStartScene = startSceneAsset;
            Debug.Log($"Set play mode start scene: {startSceneAsset.name}");
        }

        if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorSceneManager.playModeStartScene = null;
        }
    }
}