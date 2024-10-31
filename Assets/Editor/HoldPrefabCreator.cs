#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class HoldPrefabCreator : EditorWindow
{
    private string m_SourceFolder = "Assets/Models/Holds";
    private string m_TargetFolder = "Assets/Resources/ClimbingHolds";

    [MenuItem("Tools/Convert Holds to Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<HoldPrefabCreator>("Hold Converter");
    }

    private void OnGUI()
    {
        m_SourceFolder = EditorGUILayout.TextField("Source Folder:", m_SourceFolder);
        m_TargetFolder = EditorGUILayout.TextField("Target Folder:", m_TargetFolder);

        if (GUILayout.Button("Convert All Holds"))
        {
            ConvertHolds();
        }
    }

    private void ConvertHolds()
    {
        if (!Directory.Exists(m_TargetFolder))
        {
            Directory.CreateDirectory(m_TargetFolder);
        }

        string[] fbxFiles = Directory.GetFiles(m_SourceFolder, "*.fbx", SearchOption.AllDirectories);

        foreach (string fbxPath in fbxFiles)
        {
            GameObject fbxModel = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
            if (fbxModel == null) continue;

            // Create instance in scene
            GameObject instance = PrefabUtility.InstantiatePrefab(fbxModel) as GameObject;
            
            // Add required components
            if (!instance.GetComponent<Collider>())
                instance.AddComponent<MeshCollider>();
            if (!instance.GetComponent<ClimbingHold>())
                instance.AddComponent<ClimbingHold>();

            // Create prefab
            string prefabPath = Path.Combine(m_TargetFolder, fbxModel.name + ".prefab");
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            
            // Cleanup
            DestroyImmediate(instance);
        }
        
        AssetDatabase.Refresh();
    }
}
#endif 