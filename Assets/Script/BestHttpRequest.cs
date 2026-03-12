using System.Collections.Generic;
using BestHTTP;
using System;
using LitJson;
using UnityEngine;
using System.Text;
using BestHTTP.WebSocket;
using uPLibrary.Networking.M2Mqtt;
using System.Security.Cryptography.X509Certificates;
using uPLibrary.Networking.M2Mqtt.Messages;
using BestHTTP.ServerSentEvents;

namespace LINK_SocketFramework
{
    namespace HttpRequest
    {
        public static class LINK_HttpRequest
        {
            /// <summary>
            /// ����Get����
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="url">�����ַ</param>
            /// <param name="callback">�ص�</param>
            /// <param name="requestData">����Ĳ���</param>
            /// <param name="requestHearder">���ӵ�header</param>
            public static void SendGetRequest<T>(string url, Action<bool, string, T> callback, Dictionary<string, object> requestData = null, Dictionary<string, string> requestHearder = null)
            {
                string requestUrl = url;
                if (requestData != null && requestData.Count > 0)
                {
                    requestUrl = url + "?";
                    foreach (var item in requestData)
                        requestUrl += item.Key + "=" + item.Value + "&";
                    requestUrl = requestUrl.Trim('&');
                }
                Debug.Log(requestUrl);
                HTTPRequest request = new HTTPRequest(new Uri(requestUrl), HTTPMethods.Get, (requestdata, responseData) =>
                {
                    callback?.Invoke(responseData.IsSuccess, responseData.DataAsText, JsonUtility.FromJson<T>(responseData.DataAsText));
                });
                if (requestHearder != null && requestHearder.Count > 0)
                    SetHeaderParameter(request, requestHearder);
                request.Send();
            }


            /// <summary>
            /// 发送post请求
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="url">请求地址</param>
            /// <param name="callback">回调</param>
            /// <param name="requestData">请求参数</param>
            /// <param name="requestHearder">请求头</param>
            public static void SendPostRequest<T>(string url, Action<bool, string, T> callback, Dictionary<string, object> requestData = null, Dictionary<string, string> requestHearder = null)
            {
                HTTPRequest request = new HTTPRequest(new Uri(url), HTTPMethods.Post, (requestdata, responseData) =>
                {
                    callback?.Invoke(responseData.IsSuccess, responseData.DataAsText, JsonUtility.FromJson<T>(responseData.DataAsText));
                });
                if (requestHearder != null && requestHearder.Count > 0)
                    SetHeaderParameter(request, requestHearder);
                if (requestData != null/* && requestData.Count > 0*/)
                    request.RawData = Encoding.UTF8.GetBytes(JsonMapper.ToJson(requestData));
                request.AddHeader("Content-Type", "	application/json");
                request.Send();
            }


            /// <summary>
            /// 发送post请求
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="url">请求地址</param>
            /// <param name="callback">回调</param>
            /// <param name="requestData">请求参数</param>
            /// <param name="requestHearder">请求头</param>
            public static void SendPostRequest<T>(string url, Action<bool, string, T> callback, string requestData = null, Dictionary<string, string> requestHearder = null)
            {
                HTTPRequest request = new HTTPRequest(new Uri(url), HTTPMethods.Post, (requestdata, responseData) =>
                {
                    callback?.Invoke(responseData.IsSuccess, responseData.DataAsText, JsonUtility.FromJson<T>(responseData.DataAsText));
                });
                if (requestHearder != null && requestHearder.Count > 0)
                    SetHeaderParameter(request, requestHearder);
                if (requestData != null)
                    request.RawData = Encoding.UTF8.GetBytes(requestData);
                request.AddHeader("Content-Type", "	application/json");
                request.Send();
            }

            /// <summary>
            /// 发送Get请求
            /// </summary>
            /// <param name="url">�����ַ</param>
            /// <param name="callback">�ص�  �Ƿ�ɹ�  ���ص�byte��������</param>
            public static void SendDownLoadedRequest(string url, Action<bool, byte[]> callback)
            {
                HTTPRequest request = new HTTPRequest(new Uri(url), (requestdata, responsedata) =>
                {
                    callback?.Invoke(responsedata.IsSuccess, responsedata.Data);
                });
                request.Send();
            }

            /// <summary>
            /// 设置请求头
            /// </summary>
            /// <param name="hTTPRequest"></param>
            /// <param name="requestHearder">header</param>
            private static void SetHeaderParameter(HTTPRequest hTTPRequest, Dictionary<string, string> requestHearder = null)
            {
                if (requestHearder != null)
                {
                    foreach (var item in requestHearder)
                    {
                        hTTPRequest.SetHeader(item.Key, item.Value);
                    }
                }
            }
        }

    }

    namespace BestHttpWebSocket
    {
        //websocket
        public class LINK_WebSocket
        {
            public Action<WebSocket> OnWebsocketOpen;
            public Action<string> OnStringMessageReceived;
            public Action<byte[]> OnBinaryMessageReceived;
            public Action<string> OnWebsocketClose;
            public Action<string> OnWebsocketError;
            private WebSocket webSocket;
            private bool isInit; //�Ƿ��ʼ��

            public LINK_WebSocket(string Websocketurl, bool autoConnect = false)
            {
                webSocket = new WebSocket(new Uri(Websocketurl));
                webSocket.OnOpen += OnOpen;
                webSocket.OnMessage += OnMessageReceived;
                webSocket.OnBinary += OnbinaryMessageReceived;
                webSocket.OnClosed += OnWebSocketClosed;
                webSocket.OnErrorDesc += OnErrorDesc;
                isInit = true;
            }

            public void Open()
            {
                if (isInit)
                    webSocket.Open();
                else
                    Debug.LogError("WebSocket未初始化");
            }

            public void Close()
            {
                if (webSocket.IsOpen)
                {
                    Debug.LogError("websocket关闭");
                    webSocket.Close();
                }
            }

            /// <summary>
            /// 发送websocket数据
            /// </summary>
            /// <param name="sendData"></param>
            public void SendMessage(string sendData)
            {
                webSocket.Send(sendData);
            }

            /// <summary>
            /// 发送websocket数据
            /// </summary>
            /// <param name="data"></param>
            public void SendMessage(byte[] data)
            {
                webSocket.Send(data);
            }

            private void OnOpen(WebSocket webSocket)
            {
                OnWebsocketOpen?.Invoke(webSocket);
            }
            private void OnMessageReceived(WebSocket webSocket, string message)
            {
                OnStringMessageReceived?.Invoke(message);
            }

            private void OnbinaryMessageReceived(WebSocket webSocket, byte[] message)
            {
                OnBinaryMessageReceived?.Invoke(message);
            }

            private void OnWebSocketClosed(WebSocket webSocket, UInt16 code, string message)
            {
                OnWebsocketClose?.Invoke(message);
            }

            private void OnErrorDesc(WebSocket ws, string error)
            {
                OnWebsocketError?.Invoke(error);
            }
        }
    }

    namespace M2Mqtt
    {
        public class LINK_Mqtt
        {
            MqttClient mqttClient;
            public Action<string, string> OnReceivceMqttMsg;    //topic   ���ص���Ϣ
            public Action<object, EventArgs> OnClientDisconnect;
            public bool isConnected
            {
                get => mqttClient.IsConnected;
            }
            public LINK_Mqtt(string hostName, int Port, string userName, string passWord, string clientid)
            {
                mqttClient = new MqttClient(hostName, Port, false, new X509Certificate());
                mqttClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
                mqttClient.MqttMsgDisconnected += Client_MqttDisconnected;
                mqttClient.Connect(clientid, userName, passWord);
            }


            /// <summary>
            /// 订阅主题
            /// </summary>
            /// <param name="topics"></param>
            public void SubscribeTopic(string[] topics)
            {
                byte[] bases = new byte[topics.Length];
                for (int i = 0; i < bases.Length; i++)
                {
                    bases[i] = MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE;
                }
                mqttClient.Subscribe(topics, bases);
            }

            public void PublishMsg(string topic, string Msg)
            {
                byte[] byteArray = Encoding.Default.GetBytes(Msg);
                mqttClient.Publish(topic, byteArray);
            }

            public void PublishMsg(string topic, byte[] Msg)
            {
                mqttClient.Publish(topic, Msg);
            }

            /// <summary>
            /// 取消订阅主题
            /// </summary>
            /// <param name="topics"></param>
            public void UnsubscribeTopics(string[] topics)
            {
                mqttClient.Unsubscribe(topics);
            }

            /// <summary>
            /// 关闭MQTT链接
            /// </summary>
            public void Close()
            {
                if (isConnected)
                    mqttClient.Disconnect();
            }

            private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
            {
                OnReceivceMqttMsg?.Invoke(e.Topic, System.Text.Encoding.UTF8.GetString(e.Message));
            }

            private void Client_MqttDisconnected(object sender, EventArgs e)
            {
                OnClientDisconnect?.Invoke(sender, e);
            }
        }
    }

    namespace SSE
    {
        public class SSEClient
        {
            private EventSource sse;
            public Action<string> OnOpen;
            public Action<string> OnMessageReceived;
            public Action<string> OnError;
            public Action<string> OnClose;

            public SSEClient(string sseUrl)
            {
                sse = new EventSource(new Uri(sseUrl));
                sse.OnOpen += OnEventSourceOpened;
                sse.OnMessage += OnEventSourceReceived;
                sse.OnError += OnEventSourceError;
                sse.OnClosed += OnEventSourceClosed;
                sse.Open();
            }

            public void Close()
            {
                sse.Close();
            }

            void OnEventSourceOpened(EventSource source)
            {
                OnOpen?.Invoke("连接成功");
            }

            void OnEventSourceReceived(EventSource source, Message msg)
            {
                OnMessageReceived?.Invoke(msg.Data);
            }

            void OnEventSourceError(EventSource source, string error)
            {
                OnError?.Invoke(error);
            }

            void OnEventSourceClosed(EventSource source)
            {
                OnError?.Invoke("连接关闭");
            }

        }
    }
}


