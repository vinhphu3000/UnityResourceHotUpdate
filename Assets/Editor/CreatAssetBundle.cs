/*
文件名（File Name）:   ScriptTitleChange.cs

作者（Author）:    老而不死是为妖

创建时间（CreateTime）:  2016-6-2 16:58:16
*/
using UnityEngine;
using System.Collections;
using UnityEditor;
public class CreatAssetBundle : MonoBehaviour
{
    [MenuItem("CreatAssetBundle/ToCilent")]
    public static void CreatAssetBundleToCilent()
    {
        Caching.CleanCache();
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        foreach (Object obj in selectedAssets)
        {
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = obj.name;
            //string[] assetsName = new string[1];
            string[] assetsName = {"Assets/Prefabs/" + obj.name.ToString() + ".prefab"};
            buildMap[0].assetNames = assetsName;
            if (BuildPipeline.BuildAssetBundles("Assets/Cilent", buildMap, BuildAssetBundleOptions.DeterministicAssetBundle))
            {
                Debug.Log("打包成功！");
            }
            else
            {
                Debug.Log("打包失败！");
            }
        }
        AssetDatabase.Refresh();
    }
    [MenuItem("CreatAssetBundle/ToServer")]
    public static void CreatAssetBundleToServer()
    {
        Caching.CleanCache();
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        foreach (Object obj in selectedAssets)
        {
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = obj.name;
            //string[] assetsName = new string[1];
            string[] assetsName = { "Assets/Prefabs/" + obj.name.ToString() + ".prefab" };
            buildMap[0].assetNames = assetsName;
            if (BuildPipeline.BuildAssetBundles("Assets/Server", buildMap, BuildAssetBundleOptions.DeterministicAssetBundle))
            {
                Debug.Log("打包成功！");
            }
            else
            {
                Debug.Log("打包失败！");
            }
        }
        AssetDatabase.Refresh();
    }
}