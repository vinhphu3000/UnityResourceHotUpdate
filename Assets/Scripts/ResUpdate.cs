/*
文件名（File Name）:   ScriptTitleChange.cs

作者（Author）:    老而不死是为妖

创建时间（CreateTime）:  2016-6-3 11:30:57
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
//按需修改
public class ResUpdate : MonoBehaviour
{
    public static readonly string VersionFile = "version.txt";
    public static readonly string LocalResURL = "file://" + Application.dataPath + "/Cilent/";//本地资源，WWW加载用
    public static readonly string ServerResURL = "file:///E:/Server/";//服务器资源，WWW加载用
    public static readonly string LocalResPath = Application.dataPath + "/Cilent/";//FileStream读写用

    private Dictionary<string, string> LocalResVersion = new Dictionary<string, string>();
    private Dictionary<string, string> ServerResVersion = new Dictionary<string, string>();
    private List<string> NeedDownFiles = new List<string>();
    private bool NeedUpdateLocalVersionFile = false;

    public delegate void HandleFinishDownload(WWW www);
    void Awake()
    {
        StartCoroutine(DownLoad(LocalResURL + VersionFile, delegate(WWW localVersion)
            {
                //读取客户端版本文件
                ParseVersiobFile(localVersion.text, LocalResVersion);
                StartCoroutine(DownLoad(ServerResURL + VersionFile, delegate(WWW serverVersion)
                    {
                        //读取服务器版本文件
                        ParseVersiobFile(serverVersion.text, ServerResVersion);
                        //计算需要下载的文件
                        CompareVersion();
                        //开始下载所需资源
                        DownLoadRes();
                    }));
            }));
    }
    void Start()
    {
    }
    void Update()
    {
    }
    private IEnumerator DownLoad(string url, HandleFinishDownload finishFun)
    {
        WWW www = new WWW(url);
        yield return www;
        if (finishFun != null)
        {
            finishFun(www);
        }
        www.Dispose();
    }
    private void ParseVersiobFile(string content, Dictionary<string, string> dictionary)
    {
        if (content == null || content.Length == 0)
        {
            return;
        }
        string[] items = content.Split('\n');
        foreach (string item in items)
        {
            string[] info = item.Split(',');
            if (info != null && info.Length == 2)
            {
                dictionary.Add(info[0], info[1]);
            }
        }
    }
    private void CompareVersion()
    {
        foreach (var item in ServerResVersion)
        {
            string fileName = item.Key;
            string serverMD5 = item.Value;
            if (!LocalResVersion.ContainsKey(fileName))
            {
                NeedDownFiles.Add(fileName);
            }
            else
            {
                string localMD5;
                LocalResVersion.TryGetValue(fileName, out localMD5);
                if (!serverMD5.Equals(localMD5))
                {
                    NeedDownFiles.Add(fileName);
                }
            }
        }
        NeedUpdateLocalVersionFile = NeedDownFiles.Count > 0;
    }
    private void DownLoadRes()
    {
        print(NeedDownFiles.Count + "----" + System.DateTime.Now.ToString("hh:mm:ss:fff"));
        //如果需要下载的资源加载完成，更新本地版本文件（用服务器版本文件替换）并开始游戏
        if (NeedDownFiles.Count == 0)
        {
            UpdateLocalVersionFile();
            return;
        }
        string file = NeedDownFiles[0];
        NeedDownFiles.RemoveAt(0);

        StartCoroutine(DownLoad(ServerResURL + file, delegate(WWW www)
            {
                //下载资源或替换本地资源
                ReplaceLocalRes(file, www.bytes);

                //开始下一个资源下载
                DownLoadRes();
            }));
    }

    private void UpdateLocalVersionFile()
    {
        if (NeedUpdateLocalVersionFile)
        {
            StringBuilder versions = new StringBuilder();
            foreach (var item in ServerResVersion)
            {
                versions.Append(item.Key).Append(",").Append(item.Value).Append("\n");
            }
            FileStream stream = new FileStream(LocalResPath + VersionFile, FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(versions.ToString());
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
        }
        StartCoroutine(StartLoad());
    }
    private IEnumerator StartLoad()
    {
        WWW www = new WWW(LocalResURL + "version.txt");
        yield return www;
        print(www.text);
    }
    private void ReplaceLocalRes(string fileName, byte[] data)
    {
        string filePath = LocalResPath + fileName;
        FileStream stream = new FileStream(filePath, FileMode.Create);
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();
    }

}