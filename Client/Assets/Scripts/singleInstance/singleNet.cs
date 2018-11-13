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

public delegate void ChatEventHandler(int roleID, string msg);
public delegate void LoginEventHandler(playersInfo pinfos);
public delegate void PlayerJoinEventHandler(PublicInfo pinfo);
public delegate void PlayerStatusModifyEventHandler(statusBroadcast sb);

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

    public TcpMsgHead(byte[] retHead, int msgStart)
    {
        byte[] buff = new byte[4];

        Array.ConstrainedCopy(retHead, msgStart, buff, 0, 4);
        Array.Reverse(buff);
        magic = BitConverter.ToUInt32(buff, 0);

        Array.ConstrainedCopy(retHead, msgStart + 4, buff, 0, 4);
        Array.Reverse(buff);
        _version = BitConverter.ToUInt32(buff, 0);
        
        Array.ConstrainedCopy(retHead, msgStart + 8, buff, 0, 4);
        Array.Reverse(buff);
        _data_len = BitConverter.ToUInt32(buff, 0);
    }
}

public class SingleNet : SingleInstance<SingleNet>
{
    public static SingleNet netInstance;
    private ClientAsync gameClient = new ClientAsync();
    private System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

    private static int lastSessionID;

    private SingleNet() { }

    public event ChatEventHandler ChatEvent;
    public event LoginEventHandler LoginEvent;
    public event PlayerJoinEventHandler PlayerJoinEvent;
    public event PlayerStatusModifyEventHandler PlayerStatusModify;

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
            //循环处理，解决粘包问题
            int msgStart = 0;
            while (msg.Length > msgStart)
            {
                //头部消息
                TcpMsgHead msgHead = new TcpMsgHead(msg, msgStart);
                if (msgHead._data_len <= 0)
                {
                    break;
                }

                //thrift消息
                byte[] thriftBuff = new byte[64];
                Array.ConstrainedCopy(msg, msgStart + 12, thriftBuff, 0, (int)msgHead._data_len);
                ProtoBufRpcHead hhh = Thrift.Transport.TMemoryBuffer.DeSerialize<ProtoBufRpcHead>(thriftBuff);

                int thriftLength = Thrift.Transport.TMemoryBuffer.Serialize(hhh).Length;
                int length = (int)msgHead._data_len - thriftLength;
                byte[] response = new byte[length];
                Array.ConstrainedCopy(msg, msgStart + 12 + thriftLength, response, 0, length);
                ProcessMsg(hhh.Function_name, response);

                msgStart = msgStart + 12 + (int)msgHead._data_len;
            }
        });
        gameClient.ConnectAsync(serverURL, port);
    }

    private void ProcessMsg(String function, byte[] msg)
    {
        commonResponse ret;
        switch (function)
        {
            case "rpcMsg:add":
                ret = commonResponse.Parser.ParseFrom(msg);
                //CalResponse ret = (CalResponse)protoNet.Deserialize(CalResponse.Parser, msg);
                Debug.Log("服务器返回：" + ret.Status);
                break;
            case "rpcMsg:login":
                playersInfo pinfos = playersInfo.Parser.ParseFrom(msg);
                LoginEvent(pinfos);
                Debug.Log("登陆返回");
                break;
            case "rpcMsg:chat":
                Debug.Log("聊天发送成功");
                break;
            case "rpcMsg:statusBroad":
                statusBroadcast sb = statusBroadcast.Parser.ParseFrom(msg);
                PlayerStatusModify(sb);
                Debug.Log("有玩家更新状态");
                break;
            case "rpcMsg:chatBroad":
                chatBroadcast tcccc = chatBroadcast.Parser.ParseFrom(msg);
                //ChatControl.AddChatHis(tcccc.Said);
                Debug.Log("聊天返回: " + tcccc.Said);

                ChatEvent(123456, tcccc.Said);
                break;
            default:
                break;
        }
    }

    public void SendChatMsg(string msg)
    {
        chatBroadcast tc = new chatBroadcast
        {
            Said = msg
        };

        byte[] data = CreateData(tc, "chat");
        gameClient.SendAsync(data);
    }

    public void Login(int roleID, int roomID)
    {
        testLoginInfo tli = new testLoginInfo
        {
            Roleid = roleID,
            Roomid = roomID
        };

        byte[] data = CreateData(tli, "login");
        gameClient.SendAsync(data);
    }

    public void SendMsgCommon(IMessage im, string function)
    {
        byte[] data = CreateData(im, function);
        gameClient.SendAsync(data);
    }

    // Use this for initialization
    public void SendGameMsg()
    {
        testLoginInfo tli = new testLoginInfo
        {
            Roleid = 123456,
            Roomid = 1232
        };

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
    private byte[] SerializeInfoObjToByteArray(ValueType infoStruct)
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