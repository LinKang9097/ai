using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageDispatcher : Singleton<MessageDispatcher>
{
    /// <summary>
    /// 存放所有事件      key:事件名称   value：事件回调函数
    /// </summary>
    private Dictionary<string, Action<object>> eventDic = new Dictionary<string, Action<object>>();

    /// <summary>
    /// 事件订阅
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调</param>
    public void Subscribe(string eventName, Action<object> callback)
    {
        if (!eventDic.ContainsKey(eventName))
            eventDic.Add(eventName, callback);
        else
            eventDic[eventName] += callback;
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="eventName">事件名称</param>
    public void UnSubscribe(string eventName)
    {
        if (!eventDic.ContainsKey(eventName))
        {
            Debug.LogError("事件不存在"+eventName);
            return;
        }
        eventDic.Remove(eventName);
    }

    /// <summary>
    /// 发布事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="param">参数</param>
    public void Publish(string eventName, object param = null)
    {
        if (!eventDic.ContainsKey(eventName))
        {
            Debug.LogError("事件不存在" + eventName);
            return;
        }
        eventDic[eventName]?.Invoke(param);
    }
}
