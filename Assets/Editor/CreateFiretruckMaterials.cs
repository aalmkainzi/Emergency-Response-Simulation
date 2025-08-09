// Save as e.g. Assets/Editor/CreateFiretruckMaterials.cs
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CreateFiretruckMaterials
{
    [MenuItem("Tools/Materials/Create Materials From Textures (Firetruck)")]
    public static void CreateMaterials()
    {
        string texturesFolder = "Assets/Models/Vehicles/firetruck/source/model/textures";
        string modelFolder = "Assets/Models/Vehicles/firetruck/source/model";
        string materialsFolder = Path.Combine(modelFolder, "Materials").Replace("\\", "/");

        // Ensure textures folder exists
        if (!AssetDatabase.IsValidFolder(texturesFolder))
        {
            Debug.LogError($"Textures folder not found: {texturesFolder}");
            return;
        }

        // Ensure Materials folder exists under model
        if (!AssetDatabase.IsValidFolder(materialsFolder))
        {
            AssetDatabase.CreateFolder(modelFolder, "Materials");
            Debug.Log($"Created folder: {materialsFolder}");
        }

        // Find all textures in the texturesFolder
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { texturesFolder });

        var albedoMap = new Dictionary<string, string>();
        var normalMap = new Dictionary<string, string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = Path.GetFileNameWithoutExtension(path);
            string lower = name.ToLowerInvariant();

            if (lower.EndsWith("_albedo"))
            {
                string baseName = name.Substring(0, name.Length - "_albedo".Length);
                albedoMap[baseName] = path;
            }
            else if (lower.EndsWith("_normal"))
            {
                string baseName = name.Substring(0, name.Length - "_normal".Length);
                normalMap[baseName] = path;
            }
        }

        int created = 0;
        int updated = 0;

        foreach (var kv in albedoMap)
        {
            string baseName = kv.Key;
            if (!normalMap.ContainsKey(baseName)) continue; // need both

            string albedoPath = albedoMap[baseName];
            string normalPath = normalMap[baseName];

            // Load textures
            Texture2D albedoTex = AssetDatabase.LoadAssetAtPath<Texture2D>(albedoPath);
            Texture2D normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);

            if (albedoTex == null || normalTex == null)
            {
                Debug.LogWarning($"Could not load textures for {baseName}: albedo null? {albedoTex == null}, normal null? {normalTex == null}");
                continue;
            }

            // Make sure normal texture is set as NormalMap
            var importer = AssetImporter.GetAtPath(normalPath) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.NormalMap)
            {
                importer.textureType = TextureImporterType.NormalMap;
                importer.SaveAndReimport();
            }

            // Material path
            string matName = $"{baseName}_mat";
            string matPath = Path.Combine(materialsFolder, matName + ".mat").Replace("\\", "/");

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                // create new material
                mat = new Material(Shader.Find("Standard"));
                mat.name = matName;
                mat.SetTexture("_MainTex", albedoTex);
                mat.SetTexture("_BumpMap", normalTex);
                AssetDatabase.CreateAsset(mat, matPath);
                created++;
            }
            else
            {
                // update existing material
                mat.SetTexture("_MainTex", albedoTex);
                mat.SetTexture("_BumpMap", normalTex);
                EditorUtility.SetDirty(mat);
                updated++;
            }

            Debug.Log($"Material processed: {matPath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Materials creation complete. Created: {created}, Updated: {updated}");
    }
}
