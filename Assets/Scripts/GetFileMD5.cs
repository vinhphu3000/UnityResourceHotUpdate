/*
文件名（File Name）:   ScriptTitleChange.cs

作者（Author）:    老而不死是为妖

创建时间（CreateTime）:  2016-6-2 17:30:21
*/
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using UnityEditor;
public class GetFileMD5 : MonoBehaviour
{
    void Awake()
    {
        //生成服务器版本文件和客户端版本文件
        //CreatVersionTxt(Application.dataPath + "/Server/");
        //CreatVersionTxt(Application.dataPath + "/Cilent/");
    }
    void Start()
    {
    }
    void Update()
    {
    }
    IEnumerator LoadCube()
    {
        WWW www = new WWW("file://" + Application.dataPath + "/StreamingAssets/cube");
        yield return www;
        GameObject obj = www.assetBundle.LoadAsset<UnityEngine.Object>("cube.prefab") as GameObject;
        print(www.assetBundle.GetHashCode().ToString());
        print(www.assetBundle.GetInstanceID().ToString());
        Instantiate(obj);
        www.assetBundle.Unload(false);
        print(obj.transform.position);
    }
    public void CreatVersionTxt(string resPath)
    {
        string[] files = Directory.GetFiles(resPath, "*");
        print(files.Length);
        StringBuilder versions = new StringBuilder();
        for (int i = 0, len = files.Length; i < len; i++)
        {
            string filePath = files[i];
            string extension = filePath.Substring(files[i].LastIndexOf("/") + 1);
            //5.3版本会自动生成文件夹同名assetbundle，暂时未找到不生成的方法只好手动过滤
            if (!extension.Contains(".") && extension != "Server" && extension != "Cilent")
            {
                //string relativePath = filePath.Replace(resPath, "").Replace("\\", "/");
                string md5 = MD5File(filePath);
                print(md5);
                versions.Append(extension).Append(",").Append(md5).Append("\n");
            }
        }
        // 生成配置文件  
        FileStream stream = new FileStream(resPath + "version.txt", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(versions.ToString());
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();
        AssetDatabase.Refresh();
        Debug.Log("版本文件生成完毕！");
    }
    public static string MD5File(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }  
}