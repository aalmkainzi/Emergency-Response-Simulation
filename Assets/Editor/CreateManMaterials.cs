using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CreateManMaterials
{
    [MenuItem("Tools/Materials/Create Materials From Textures (Man)")]
    public static void CreateMaterials()
    {
        string texture_dir = "Assets/Models/Characters/Man/textures";
        string mats_dir = "Assets/Models/Characters/Man/Materials";

        if (!AssetDatabase.IsValidFolder(mats_dir))
        {
            AssetDatabase.CreateFolder("Assets/Models/Characters/Man", "Materials");
        }

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { texture_dir });

        Dictionary<string, string> material_textures = new();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string lower = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();

            char last_letter = '\0';
            int i;
            for (i = lower.Length - 1; i >= 0; i--)
            {
                if (lower[i] == '_')
                    break;
                last_letter = lower[i];
            }

            StringBuilder mat_name_builder = new StringBuilder();
            for (int j = i - 1; j >= 0; j--)
            {
                mat_name_builder.Append(lower[j]);
            }
            string mat_name = new string(mat_name_builder.ToString().Reverse().ToArray()); ;

            if (last_letter == 'd')
            {
                material_textures[mat_name] = path;
            }
            else
            {
                continue;
            }
        }

        foreach (var m in material_textures)
        {
            Texture2D albedo_tex = AssetDatabase.LoadAssetAtPath<Texture2D>(m.Value);

            string mat_name = m.Key + "_mat";
            string mat_path = Path.Combine(mats_dir, mat_name + ".mat");

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(mat_path);
            mat = new Material(Shader.Find("Standard"));
            mat.name = mat_name;
            mat.SetTexture("_MainTex", albedo_tex);
            AssetDatabase.CreateAsset(mat, mat_path);
        }
    }
}
