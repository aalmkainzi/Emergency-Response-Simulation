using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;

public class CreatFireFighterMaterials
{
    [MenuItem("Tools/Materials/Create Materials From Textures (FireFighter)")]
    public static void CreateMaterials()
    {
        string texture_dir = "Assets/Models/Characters/FireFighter/textures";
        string mats_dir = "Assets/Models/Characters/FireFighter/Materials";

        if (!AssetDatabase.IsValidFolder(mats_dir))
        {
            AssetDatabase.CreateFolder("Assets/Models/Characters/FireFighter", "Materials");
        }

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { texture_dir });

        Dictionary<string, (string, string)> material_textures = new(); 

        foreach(string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string lower = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();

            char last_letter = '\0';
            int i;
            for(i = lower.Length - 1; i >= 0; i--)
            {
                if (lower[i] == '_')
                    break;
                last_letter = lower[i];
            }

            StringBuilder mat_name_builder = new StringBuilder();
            for (int j = i - 1; j >= 0; j--)
            {
                if (lower[j] == '_')
                    break;
                mat_name_builder.Append(lower[j]);
            }
            string mat_name = new string(mat_name_builder.ToString().Reverse().ToArray());;
            
            if (last_letter == 'n')
            {
                if(!material_textures.ContainsKey(mat_name))
                {
                    material_textures[mat_name] = (null, null);
                }
                (string,string) texs = material_textures[mat_name];
                texs.Item2 = path;
                material_textures[mat_name] = texs;
            }
            else if(last_letter == 'd')
            {
                if (!material_textures.ContainsKey(mat_name))
                {
                    material_textures[mat_name] = (null, null);
                }
                (string, string) texs = material_textures[mat_name];
                texs.Item1 = path;
                material_textures[mat_name] = texs;
            }
            else
            {
                continue;
            }
        }

        foreach(var m in material_textures)
        {
            Texture2D albedo_tex = AssetDatabase.LoadAssetAtPath<Texture2D>(m.Value.Item1);
            Texture2D normal_tex = AssetDatabase.LoadAssetAtPath<Texture2D>(m.Value.Item2);

            string mat_name = m.Key + "_mat";
            string mat_path = Path.Combine(mats_dir, mat_name + ".mat");

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(mat_path);
            mat = new Material(Shader.Find("Standard"));
            mat.name = mat_name;
            mat.SetTexture("_MainTex", albedo_tex);
            mat.SetTexture("_BumpMap", normal_tex);
            AssetDatabase.CreateAsset(mat, mat_path);
        }
    }
}
