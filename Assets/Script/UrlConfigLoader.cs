/*
 * ==========================================
 * FileName：#FileName#
 * Author：#Name#
 * CreatTime：#CreateTime#
 * NowPath：#path#
 * Description:#desc#
 * ==========================================
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UrlConfigLoader
{
    //配置文件加全局ip端口读取，若配置文件中的该属性为空，则使用全局ip+port
    //ip和port合为一个
    private static string configPath = string.Empty;
    private static Dictionary<string, Dictionary<string, UrlItem>> dictUrls = new Dictionary<string, Dictionary<string, UrlItem>>();
    private static bool isGetdatas = false;
    private static GIpPortConfig urlConfig;

    public static string GetUrl(string sceneName, string interfaceName)
    {
        if (string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(interfaceName))
            return null;
        if (DictUrls.ContainsKey(sceneName))
        {
            if (DictUrls[sceneName].ContainsKey(interfaceName))
            {
                if (!string.IsNullOrEmpty(DictUrls[sceneName][interfaceName].socket))
                    return DictUrls[sceneName][interfaceName].Url();    //如果socket不为空则使用自身的socket
                else
                    return urlConfig.globalSocket+ DictUrls[sceneName][interfaceName].Url();  //否则使用全局的
            }
            else
            {
                Debug.LogError($"传入的参数[{sceneName}]/[{interfaceName}]未正确在UrlConfig.json文件中配置，请检查！");
                return null;
            }
        }
        else
        {
            Debug.LogError($"传入的参数[{sceneName}]/[{interfaceName}]未正确在UrlConfig.json文件中配置，请检查！");
            return null;
        }

    }

    private static Dictionary<string, Dictionary<string, UrlItem>> DictUrls
    {
        get
        {
            if (!isGetdatas)
            {
                isGetdatas = true;
                configPath = Path.Combine(Application.streamingAssetsPath, "UrlConfig.json");
                if (!File.Exists(configPath))
                    JsonGenerator();
                return JsonReader();
            }
            else
                return dictUrls;
        }
    }


    private static void JsonGenerator()
    {
        GIpPortConfig gIpPortConfig = new GIpPortConfig();
        gIpPortConfig.globalSocket = "http://127.0.0.1:80";
        gIpPortConfig.lstUrls.Add(new UrlItem()
        {
            sceneName = "接口所属场景名称",
            interfaceName = "接口名称写在这里",
            socket = "接口IP+PORT",
            api = "api地址写在这里",
        });
        string jsonStr = JsonUtility.ToJson(gIpPortConfig);
        File.WriteAllText(configPath, jsonStr);
    }

    private static Dictionary<string, Dictionary<string, UrlItem>> JsonReader()
    {
        string jsonString = File.ReadAllText(configPath);
        urlConfig = JsonUtility.FromJson<GIpPortConfig>(jsonString);
        if (urlConfig.lstUrls.Count > 0)
        {
            for (int i = 0; i < urlConfig.lstUrls.Count; i++)
            {
                if (!DictUrls.ContainsKey(urlConfig.lstUrls[i].sceneName))
                {
                    Dictionary<string, UrlItem> dict = new Dictionary<string, UrlItem>();
                    dict.Add(urlConfig.lstUrls[i].interfaceName, urlConfig.lstUrls[i]);
                    DictUrls.Add(urlConfig.lstUrls[i].sceneName, dict);
                }
                else
                {
                    Dictionary<string, UrlItem> dict = DictUrls[urlConfig.lstUrls[i].sceneName];
                    if (!dict.ContainsKey(urlConfig.lstUrls[i].interfaceName))
                    {
                        dict.Add(urlConfig.lstUrls[i].interfaceName, urlConfig.lstUrls[i]);
                        DictUrls[urlConfig.lstUrls[i].sceneName] = dict;
                    }
                }
            }
        }
        return DictUrls;
    }
}


/// <summary>
/// 配置问价中的属性
/// </summary>
[Serializable]
public class UrlItem
{
    public string sceneName;        //场景名称
    public string interfaceName;    //接口名称  
    public string socket;           //ip + port  （网络套字节）
    public string api;              //后缀 api

    public string Url()
    {
        return socket + api;
    }
}

[Serializable]
public class GIpPortConfig
{
    public string globalSocket;     //若填的socket为空，则使用全局的socket
    public List<UrlItem> lstUrls = new List<UrlItem>();
}