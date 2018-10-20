using UnityEngine;
using UnityEditor;
using System.IO;
using Protobuf;
using Google.Protobuf;

namespace Net
{
    enum EnmCmdID
    {
        CS_LOGIN_REQ = 1,
    }

    public class protoNet : ScriptableObject
    {
        [MenuItem("Tools/MyTool/Do It in C#")]
        static void DoIt()
        {
            EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
        }

        public static IMessage Deserialize(MessageParser _type, byte[] byteData)
        {
            Stream stream = new MemoryStream(byteData);
            if (stream != null)
            {
                IMessage t = _type.ParseFrom(stream);
                stream.Close();
                return t;
            }
            stream.Close();
            return default(IMessage);
        }
        public static byte[] Serialize(IMessage _data)
        {
            MemoryStream stream = new MemoryStream();
            if (stream != null)
            {
                _data.WriteTo(stream);
                byte[] bytes = stream.ToArray();
                stream.Close();
                return bytes;
            }
            stream.Close();
            return null;
        }

        //泛型序列化和反序列化
        public void NewTTT()
        {
            StoreRequest sss = new StoreRequest();
            sss.Name = "asdf";
            sss.Num = 123;
            sss.Result = 13;
            sss.MyList.Add("adsfas");
            sss.MyList.Add("sadfas");

            //序列化
            var datas = Serialize(sss);

            //反序列化
            StoreRequest ss1 = (StoreRequest)Deserialize(StoreRequest.Parser, datas);
        }

        public void Tttt()
        {
            //序列化
            StoreRequest sss = new StoreRequest();
            sss.Name = "111";
            sss.Num = 123;
            sss.Result = 123;
            sss.MyList.Add("asdfas");
            sss.MyList.Add("asdfasd");

            //反序列化
            MemoryStream stream = new MemoryStream();
            sss.WriteTo(stream);

            byte[] bytes = stream.ToArray();
            StoreRequest ss1 = StoreRequest.Parser.ParseFrom(bytes);
        }
    }
}