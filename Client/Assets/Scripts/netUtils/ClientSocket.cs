using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System;
 
namespace Net
{
    public class ClientSocket
    {
        private static byte[] result = new byte[1024];
        private static Socket clientSocket;
        //是否已连接的标识
        public bool IsConnected = false;

        public ClientSocket()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 连接指定IP和端口的服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void ConnectServer(string ip, int port)
        {
            IPAddress mIp = IPAddress.Parse(ip);
            IPEndPoint ip_end_point = new IPEndPoint(mIp, port);

            try
            {
                clientSocket.Connect(ip_end_point);
                IsConnected = true;
                Debug.Log("连接服务器成功");
            }
            catch
            {
                IsConnected = false;
                Debug.Log("连接服务器失败");
                return;
            }
            //服务器下发数据长度
            //int receiveLength = clientSocket.Receive(result);
            //ByteBuffer buffer = new ByteBuffer(result);
            //int len = buffer.ReadShort();
            //string data = buffer.ReadString();
            //Debug.Log("服务器返回数据：" + data);
        }

        /// <summary>
        /// 发送数据给服务器
        /// </summary>
        public void SendMessage(byte[] data)
        {
            string msgData = data.ToString();
            if (IsConnected == false)
                return;
            try
            {
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteString(msgData);
                clientSocket.Send(WriteMessage(buffer.ToBytes()));
            }
            catch
            {
                IsConnected = false;
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }



        /// <summary>
        /// 接收指定客户端Socket的消息,原服务器函数，估计可以用来接收消息
        /// 原本无限循环在子线程，测试时要去掉
        /// </summary>
        /// <param name="clientSocket"></param>
        private static void RecieveMessage(object clientSocket)
        {
            Socket mClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    int receiveNumber = mClientSocket.Receive(result);
                    //Console.WriteLine("接收客户端{0}消息， 长度为{1}", mClientSocket.RemoteEndPoint.ToString(), receiveNumber);
                    ByteBuffer buff = new ByteBuffer(result);
                    //数据长度
                    int len = buff.ReadShort();
                    //数据内容
                    string data = buff.ReadString();
                    //Console.WriteLine("数据内容：{0}", data);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                    mClientSocket.Shutdown(SocketShutdown.Both);
                    mClientSocket.Close();
                    break;
                }
            }
        }

        /// <summary>
        /// 数据转换，网络发送需要两部分数据，一是数据长度，二是主体数据
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static byte[] WriteMessage(byte[] message)
        {
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                ushort msglen = (ushort)message.Length;
                writer.Write(msglen);
                writer.Write(message);
                writer.Flush();
                return ms.ToArray();
            }
        }
    }
}