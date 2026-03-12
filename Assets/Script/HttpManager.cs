using Best.HTTP.SecureProtocol.Org.BouncyCastle.Bcpg.Sig;
using LINK_SocketFramework;
using LINK_SocketFramework.HttpRequest;
using LINK_SocketFramework.SSE;
using LitJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public delegate void OnGetDataWithOneParam(object obj);

public delegate void OnGetDataWithTwoParam(object obj, object obj1);

public class HttpManager : UnitySingleton<HttpManager>
{
    SSEClient sSEClient;



    private void Start()
    {

        GetMenuList();

        //sSEClient.Connect();
    }

    public void GetMenuList(string parentId = "")
    {
        {
            string url = UrlConfigLoader.GetUrl("Default", "获取菜单列表");
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                ["parentId"] = parentId
            };
            Dictionary<string, string> header = new Dictionary<string, string>()
            {

            };
            LINK_HttpRequest.SendGetRequest<MenuListDataRoot>(url, (isSuccess, result, classdata) =>
            {
                Debug.Log("获取菜单列表" + "   " + isSuccess + "    " + result);

            }, data, header);
        }
    }
    #region SSE
    public void SendSSEData(string data)
    {
        InITSSE(data);
    }

    void InITSSE(string data)
    {
        sSEClient = new SSEClient(UrlConfigLoader.GetUrl("Default", "大模型调用测试(对话)") + "?prompt=" + data);
        sSEClient.OnMessageReceived += (message) =>
        {
            Debug.Log("SSE消息接收: " + message);
            if (message == "[DONE]")
            {
                sSEClient.Close();
            }
        };
        sSEClient.OnOpen += (p) =>
        {
            Debug.Log("SSE连接已打开");
        };
        sSEClient.OnClose += (p) =>
        {
            Debug.LogError("SSE连接已关闭");
        };
        sSEClient.OnError += (p) =>
        {
            Debug.LogError("SSE连接发生错误: " + p);
        };
    }
    #endregion

}

#region 菜单列表
[System.Serializable]
public class MenuListDataListItem
{
    public int tmunId;
    public int tmunParentId;
    public string tmunTitle;
    public string tmunTitleEn;
    public string tmunType;
    public string tmunUrl;
    public int tmunSort;
    public int tmunStatus;
    public string tmunAddtime;
    public string tmunModtime;
}


[System.Serializable]
public class MenuListDataRoot
{
    public string code;
    public string msg;
    public List<MenuListDataListItem> dataList;
}


#endregion
