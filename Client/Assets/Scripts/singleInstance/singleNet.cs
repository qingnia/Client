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
using Thrift.Transport;
using Thrift.Protocol;

public enum MSG_TYPE
{
    KRPC_CALL = 1,
    KRPC_REPLY = 2,
    KRPC_EXCEPTION = 3,
    KRPC_ONEWAY = 4,
}


public class TcpMsgHead
{
    public UInt32 magic = 0xA5A5A5A5;
    public UInt32 _version = 1;
    public UInt32 _data_len = 0;

    public TcpMsgHead()
    {
        magic = 0xA5A5A5A5;
        _version = 1;
        _data_len = 0;
    }

    public TcpMsgHead(byte[] retHead)
    {
        byte[] buff = new byte[4];

        Array.ConstrainedCopy(retHead, 0, buff, 0, 4);
        Array.Reverse(buff);
        magic = BitConverter.ToUInt32(buff, 0);

        Array.ConstrainedCopy(retHead, 4, buff, 0, 4);
        Array.Reverse(buff);
        _version = BitConverter.ToUInt32(buff, 0);
        
        Array.ConstrainedCopy(retHead, 8, buff, 0, 4);
        Array.Reverse(buff);
        _data_len = BitConverter.ToUInt32(buff, 0);
    }
}

public class singleNet : SingleInstance<singleNet>
{

    public static singleNet netInstance;
    private ClientAsync gameClient = new ClientAsync();
    private System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

    private static int lastSessionID;

    private singleNet() { }

    public void ConnectGameServer(string serverURL, int port)
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
        gameClient.Received += new Action<string, byte[]>((key, msg) =>
        {
            //头部消息
            TcpMsgHead msgHead = new TcpMsgHead(msg);
            //byte[] msgHead = new byte[16];
            //Array.ConstrainedCopy(msg, 0, msgHead, 0, 12);

            //thrift消息
            byte[] thriftBuff = new byte[64];
            Array.ConstrainedCopy(msg, 12, thriftBuff, 0, 36);
            ProtoBufRpcHead hhh = Thrift.Transport.TMemoryBuffer.DeSerialize<ProtoBufRpcHead>(thriftBuff);

            //正文
            int length = (int)msgHead._data_len - 36;
            byte[] response = new byte[length];
            Array.ConstrainedCopy(msg, 48, response, 0, length);
            processMsg(hhh.Function_name, response);
            //protoNet.Deserialize(getServerMsgType(hhh.Function_name), response);
            //Debug.Log(key + "对我说：" + msg);
        });
        gameClient.ConnectAsync(serverURL, port);
    }

    private void processMsg(String function, byte[] msg)
    {
        switch (function)
        {
            case "rpcMsg:add":
                CalResponse ret = CalResponse.Parser.ParseFrom(msg);
                //CalResponse ret = (CalResponse)protoNet.Deserialize(CalResponse.Parser, msg);
                Debug.Log("服务器返回：" + ret.Status);
                break;
            case "rpcMsg:login":
                Debug.Log("登陆返回");
                break;
            case "rpcMsg:chat":
                Debug.Log("聊天返回");
                break;
            default:
                break;
        }
    }

    public void SendChatMsg(string msg)
    {
        chatBroadcast tc = new chatBroadcast();
        tc.Said = msg;

        byte[] data = CreateData(tc, "chat");
        gameClient.SendAsync(data);
    }

    public void Login(int roleID, int roomID)
    {
        testLoginInfo tli = new testLoginInfo();
        tli.Roleid = roleID;
        tli.Roomid = roomID;
        
        byte[] data = CreateData(tli, "login");
        gameClient.SendAsync(data);
    }

    // Use this for initialization
    public void SendGameMsg()
    {
        testLoginInfo tli = new testLoginInfo();
        tli.Roleid = 123456;
        tli.Roomid = 1232;

        testAdd ta = new testAdd();
        System.Random ran = new System.Random();
        ta.A = ran.Next();
        ta.B = ran.Next();

        Debug.Log("计算" + ta.A + " + " + ta.B);

        chatBroadcast tc = new chatBroadcast();
        tc.Said = "aaaaaaaaaaaa";

        byte[] data = CreateData(tli, "login");
        gameClient.SendAsync(data);
    }
    
    private byte[] CreateData(IMessage pbuf, String function)
    {
        ProtoBufRpcHead pbHead = new ProtoBufRpcHead();
        pbHead.Msg_type = (int)MSG_TYPE.KRPC_CALL;
        pbHead.Session_id = lastSessionID++;
        pbHead.Function_name = "rpcMsg:" + function;

        byte[] pbdata = protoNet.Serialize(pbuf);
        ByteBuffer buff = new ByteBuffer();

        TcpMsgHead tmh = new TcpMsgHead();
        
        byte[] pbHeadBytes = Thrift.Transport.TMemoryBuffer.Serialize(pbHead);
        ProtoBufRpcHead hhh = Thrift.Transport.TMemoryBuffer.DeSerialize<ProtoBufRpcHead>(pbHeadBytes);
        tmh._data_len = (UInt32)(pbdata.Length + pbHeadBytes.Length);

        buff.WriteUint32(tmh.magic);
        buff.WriteUint32(tmh._version);
        buff.WriteUint32(tmh._data_len);

        buff.WriteBytes(pbHeadBytes);
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