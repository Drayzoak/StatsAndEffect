#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Common
{
    public static class AssetFinder
    {
        public static List<TObject> FindAssetsByInterface<TObject, TInterface>(params string[] paths) 
            where TObject : UnityEngine.Object 
            where TInterface : class
        {
            List<TObject> assets = new List<TObject>();

            // Find all assets of type T in the specified paths
            string[] assetGUIDs = AssetDatabase.FindAssets($"t:{typeof(TObject).Name}", paths);

            foreach (string guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                TObject asset = AssetDatabase.LoadAssetAtPath<TObject>(assetPath);

                // Check if the asset implements the interface T1
                if (asset is TInterface)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }
        
        
        public static List<T> FindAssets<T>(string paths) where T : UnityEngine.Object
        {
    
            string[] assetGUIDs = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { paths });
    
            List<T> assets = new List<T>();
            foreach (string guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }
        
        public static bool FileExistsInAssets(string fileName)
        {
            
            string filePath = Application.dataPath.Replace("Assets", "")  +fileName;
            Debug.Log(filePath);
            return File.Exists(filePath);
        }
    }
    
    
}
#endif