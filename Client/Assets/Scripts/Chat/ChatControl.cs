using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatCache
{
    public int RoleID { get; set; }
    public string Msg { get; set; }

    public ChatCache(int roleID, string msg)
    {
        this.RoleID = roleID;
        this.Msg = msg;
    }
}

public class ChatControl : MonoBehaviour {

    private static GameObject chatMsgPrefab;
    public InputField inputField;
    public Text inputText;
    public LayoutGroup chatContent;

    public List<ChatCache> chatMsgList;

    public List<GameObject> chatList = new List<GameObject>();

    // Use this for initialization
    void Start () {

        /*
        inputField.onEndEdit.AddListener(delegate
        {
            string msg = inputText.text;
            SendChatMsg(msg);
        });*/
    }
	
	// Update is called once per frame
	void Update () {
        UpdateChatHis();
	}

    private void Awake()
    {
        chatMsgList = new List<ChatCache>();
        chatMsgPrefab = (GameObject)Resources.Load("Prefabs/ChatMsg");
        singleNet.Instance.chatEvent += new ChatEventHandler(AddChatList);
    }

    public void AddChatList(int roleID, string msg)
    {
        ChatCache cm = new ChatCache(roleID, msg);
        this.chatMsgList.Add(cm);
    }

    private void UpdateChatHis()
    {
        if (chatMsgList.Count <= 0)
        {
            return;
        }
        foreach (var item in chatMsgList)
        {
            GameObject go = Instantiate(chatMsgPrefab);
            go.GetComponent<ChatMsg>().playerName.text = item.RoleID.ToString();
            go.GetComponent<ChatMsg>().chatMsg.text = item.Msg;
            go.transform.parent = chatContent.transform;
            go.name = "动态" + chatList.Count;
            chatList.Add(go);
            chatMsgList.Remove(item);
        }
    }

    public void SendChatMsg(string msg)
    {
        Debug.Log("发聊天消息");
        singleNet.Instance.SendChatMsg(msg);
    }
}
