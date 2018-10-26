using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Protobuf;
using Google.Protobuf;
using System.IO;
using TCPHelper;
using System.Net;
using Net;
using System.Runtime.InteropServices;
public class TcpMsgHead
{
    UInt32 magic = 0xA5A5A5A5;
    UInt32 _version = 1;
    UInt32 _data_len = 0;

    public TcpMsgHead()
    {
        magic = 0xA5A5A5A5;
        _version = 1;
        _data_len = 0;
    }
}



public class singleNet : SingleInstance<singleNet>
{

    public static singleNet netInstance;
    private ClientAsync gameClient = new ClientAsync();
    private System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

    private singleNet() { }

    public void connectGameServer(string serverURL, int port)
    {
        gameClient.Completed += new Action<System.Net.Sockets.TcpClient, EnSocketAction>((c, enAction) =>
        {
            IPEndPoint iep = c.Client.RemoteEndPoint as IPEndPoint;
            string key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
            switch (enAction)
            {
                case EnSocketAction.Connect:
                    Debug.Log("已经与" + key + "建立连接");
                    break;
                case EnSocketAction.SendMsg:
                    Debug.Log(DateTime.Now + "：向" + key + "发送了一条消息");
                    break;
                case EnSocketAction.Close:
                    Debug.Log("服务端连接关闭");
                    break;
                default:
                    break;
            }
        });
        gameClient.Received += new Action<string, string>((key, msg) =>
        {
            Debug.Log(key + "对我说：" + msg);
        });
        gameClient.ConnectAsync(serverURL, port);
        //while (true)
        //{
        //  string msg = Console.ReadLine();
        //client.SendAsync(msg);
        //}
    }
    // Use this for initialization
    public void sendGameMsg()
    {
        /*
        CSLoginInfo mLoginInfo = new CSLoginInfo();
        mLoginInfo.UserName = "linshuhe";
        mLoginInfo.Password = "123456";
        CSLoginReq mReq = new CSLoginReq();
        mReq.LoginInfo = mLoginInfo;
        */
        testLoginInfo tli = new testLoginInfo();
        tli.Roleid = 123456;
        tli.Roomid = 1232;

        byte[] data = CreateData((int)EnmCmdID.CS_LOGIN_REQ, tli);
        gameClient.SendAsync(data.ToString());
    }

    private byte[] CreateData(int typeId, IMessage pbuf)
    {
        byte[] pbdata = protoNet.Serialize(pbuf);
        ByteBuffer buff = new ByteBuffer();

        TcpMsgHead tmh = new TcpMsgHead();
        //buff.WriteBytes(StructToBytes(tmh));

        byte[] headBytes = System.Text.Encoding.Default.GetBytes(tmh.ToString());
        buff.WriteBytes(headBytes);
        buff.WriteBytes(pbdata);
        return WriteMessage(buff.ToBytes());
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

    //将一个结构序列化为字节数组
    private byte[] serializeInfoObjToByteArray(ValueType infoStruct)
    {
        if (infoStruct == null)
        {
            return null;
        }

        try
        {
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, infoStruct);

            byte[] bytes = new byte[(int)stream.Length];
            stream.Position = 0;
            int count = stream.Read(bytes, 0, (int)stream.Length);
            stream.Close();
            return bytes;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public Byte[] StructToBytes(System.Object structure)
    {
        Int32 size = Marshal.SizeOf(structure);
        Console.WriteLine(size);
        IntPtr buffer = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(structure, buffer, false);
            Byte[] bytes = new Byte[size];
            Marshal.Copy(buffer, bytes, 0, size);
            return bytes;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public System.Object BytesToStruct(Byte[] bytes, Type strcutType)
    {
        Int32 size = Marshal.SizeOf(strcutType);
        IntPtr buffer = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.Copy(bytes, 0, buffer, size);
            return Marshal.PtrToStructure(buffer, strcutType);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}