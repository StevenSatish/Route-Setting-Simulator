#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class HoldPrefabCreator : EditorWindow
{
    private string m_SourceFolder = "Assets/Models/Holds";
    private string m_TargetFolder = "Assets/Resources/ClimbingHolds";
    private string m_PreviewFolder = "Assets/Resources/HoldPreviews";
    private int m_PreviewSize = 128;
    private Color m_HoldColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Light gray, fully opaque

    [MenuItem("Tools/Convert Holds to Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<HoldPrefabCreator>("Hold Converter");
    }

    private void OnGUI()
    {
        m_SourceFolder = EditorGUILayout.TextField("Source Folder:", m_SourceFolder);
        m_TargetFolder = EditorGUILayout.TextField("Target Folder:", m_TargetFolder);
        m_PreviewFolder = EditorGUILayout.TextField("Preview Folder:", m_PreviewFolder);
        m_PreviewSize = EditorGUILayout.IntField("Preview Size:", m_PreviewSize);
        m_HoldColor = EditorGUILayout.ColorField("Hold Color:", m_HoldColor);

        if (GUILayout.Button("Convert All Holds"))
        {
            ConvertHolds();
        }
    }

    private void ConvertHolds()
    {
        // Create necessary directories
        if (!Directory.Exists(m_TargetFolder))
            Directory.CreateDirectory(m_TargetFolder);
        if (!Directory.Exists(m_PreviewFolder))
            Directory.CreateDirectory(m_PreviewFolder);

        string[] fbxFiles = Directory.GetFiles(m_SourceFolder, "*.fbx", SearchOption.AllDirectories);

        // Create preview camera
        GameObject cameraObj = new GameObject("PreviewCamera");
        Camera previewCamera = cameraObj.AddComponent<Camera>();
        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = Color.clear;
        previewCamera.orthographic = false;
        previewCamera.transform.rotation = Quaternion.Euler(0, 180, 0);

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
            if (!instance.GetComponent<SelectableHold>())
                instance.AddComponent<SelectableHold>();

            // Setup material
            SetupMaterial(instance);

            // Generate preview
            string previewPath = Path.Combine(m_PreviewFolder, fbxModel.name + "_preview.png");
            GeneratePreview(instance, previewCamera, previewPath);
        
            // Set transform values
            instance.transform.localScale = new Vector3(0.007f, 0.007f, 0.007f);
            instance.transform.rotation = Quaternion.Euler(0, 180, 0);

            // Create prefab
            string prefabPath = Path.Combine(m_TargetFolder, fbxModel.name + ".prefab");
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            
            // Cleanup
            DestroyImmediate(instance);
        }

        // Cleanup camera
        DestroyImmediate(cameraObj);
        
        AssetDatabase.Refresh();
    }

    private void SetupMaterial(GameObject _instance)
    {
        var renderers = _instance.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            // Create new material using URP/Lit shader
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            
            // Set material properties for complete opacity
            material.SetFloat("_Surface", 0); // Opaque
            material.SetFloat("_Metallic", 0);
            material.SetFloat("_Smoothness", 0.5f);
            material.SetFloat("_Cull", 2); // Back face culling
            material.SetFloat("_AlphaClip", 0); // Disable alpha clipping
            material.SetFloat("_Blend", 0); // Normal blend mode
            
            // Ensure opacity
            material.SetFloat("_ZWrite", 1);
            material.SetFloat("_DstBlend", 0);
            material.SetFloat("_SrcBlend", 1);
            
            // Disable all transparency-related keywords
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            
            // Enable opaque-related keywords
            material.EnableKeyword("_OPAQUE_ON");
            
            // Set color with full alpha
            Color opaqueColor = m_HoldColor;
            opaqueColor.a = 1f;
            material.color = opaqueColor;
            
            // Set rendering properties
            material.SetOverrideTag("RenderType", "Opaque");
            material.renderQueue = 2000; // Use opaque queue instead of transparent
            
            // Assign material
            renderer.material = material;
            
            // Save material asset
            string materialPath = Path.Combine(m_TargetFolder, $"{_instance.name}_material.mat");
            AssetDatabase.CreateAsset(material, materialPath);
        }
        
        // Set layer (unchanged)
        foreach (Transform child in _instance.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer("ClimbingHolds");
        }
    }

    private void GeneratePreview(GameObject _target, Camera _camera, string _savePath)
    {
        // Position object and camera
        _target.transform.position = Vector3.zero;
        
        // Set up orthographic camera
        _camera.orthographic = true;
        _camera.transform.rotation = Quaternion.Euler(90, 0, 0);
        
        // Frame the object
        Renderer renderer = _target.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            float maxDim = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            
            // Adjust camera to frame object more tightly
            _camera.orthographicSize = maxDim * 0.6f;
            _camera.transform.position = Vector3.down * 10f;
        }

        // Create square render texture
        RenderTexture rt = new RenderTexture(m_PreviewSize, m_PreviewSize, 24)
        {
            antiAliasing = 8
        };
        _camera.targetTexture = rt;
        
        // Render to texture
        _camera.Render();
        
        // Read pixels
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(m_PreviewSize, m_PreviewSize, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, m_PreviewSize, m_PreviewSize), 0, 0);
        tex.Apply();
        
        // Save to file
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(_savePath, bytes);
        
        // Cleanup
        RenderTexture.active = null;
        _camera.targetTexture = null;
        DestroyImmediate(rt);
        DestroyImmediate(tex);

        // Import asset with specific settings
        AssetDatabase.ImportAsset(_savePath);
        TextureImporter importer = AssetImporter.GetAtPath(_savePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            
            TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings
            {
                maxTextureSize = m_PreviewSize,
                format = TextureImporterFormat.RGBA32,
                textureCompression = TextureImporterCompression.Uncompressed,
                crunchedCompression = false
            };
            importer.SetPlatformTextureSettings(platformSettings);

            TextureImporterSettings texSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(texSettings);
            texSettings.spriteMeshType = SpriteMeshType.FullRect;
            texSettings.spriteExtrude = 1;
            texSettings.spritePixelsPerUnit = 100;
            texSettings.spriteAlignment = (int)SpriteAlignment.Center;
            importer.SetTextureSettings(texSettings);

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
    }
}
#endif 