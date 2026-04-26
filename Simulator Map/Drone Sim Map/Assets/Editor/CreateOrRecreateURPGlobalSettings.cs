using System;
using UnityEditor;
using UnityEngine;

public static class CreateOrRecreateURPGlobalSettings
{
    [MenuItem("Tools/URP/Create or Backup + Create Global Settings")]
    public static void CreateOrBackupGlobalSettings()
    {
        const string targetPath = "Assets/Settings/UniversalRenderPipelineGlobalSettings.asset";
        Type urpType = GetUrpGlobalSettingsType();

        if (urpType == null)
        {
            Debug.LogError("URP Global Settings type not found. Install a compatible Universal RP package via Package Manager and recompile.");
            return;
        }

        // Find existing global settings assets and back them up
        string[] guids = AssetDatabase.FindAssets("t:UniversalRenderPipelineGlobalSettings");
        if (guids != null && guids.Length > 0)
        {
            foreach (var g in guids)
            {
                string existingPath = AssetDatabase.GUIDToAssetPath(g);
                string backupPath = existingPath + ".bak_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                if (AssetDatabase.CopyAsset(existingPath, backupPath))
                    Debug.Log($"Backup created: {backupPath}");
                else
                    Debug.LogWarning($"Failed to create backup for: {existingPath}");
            }
        }

        // Ensure folder exists (create Assets/Settings if needed)
        const string folderPath = "Assets/Settings";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            // create folder under Assets
            AssetDatabase.CreateFolder("Assets", "Settings");
            AssetDatabase.Refresh();
        }

        // Create new instance via reflection and save asset
        var instance = ScriptableObject.CreateInstance(urpType) as ScriptableObject;
        if (instance == null)
        {
            Debug.LogError("Failed to create URP Global Settings instance.");
            return;
        }

        AssetDatabase.CreateAsset(instance, targetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Created new URP Global Settings at: {targetPath}");
        Debug.Log("Next steps: open the new asset in the Inspector and click Upgrade (if shown). Then assign it to your pipeline assets or reassign the pipeline in Project Settings -> Graphics / Quality as needed.");
    }

    private static Type GetUrpGlobalSettingsType()
    {
        // Try likely assembly-qualified names for different URP versions
        string[] candidates = new[]
        {
            "UnityEngine.Rendering.Universal.UniversalRenderPipelineGlobalSettings, Unity.RenderPipelines.Universal.Runtime",
            "UnityEngine.Rendering.Universal.UniversalRenderPipelineGlobalSettings, Unity.RenderPipelines.Core.Runtime",
            "UnityEngine.Rendering.Universal.UniversalRenderPipelineGlobalSettings, Unity.RenderPipelines.Universal.Editor"
        };

        foreach (var name in candidates)
        {
            Type t = Type.GetType(name);
            if (t != null)
                return t;
        }

        return null;
    }
}