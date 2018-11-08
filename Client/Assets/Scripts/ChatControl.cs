using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatControl : MonoBehaviour {

    private GameObject chatMsgPrefab;
    private Transform chatContent;
    public InputField inputField;
    public Text inputText;

    // Use this for initialization
    void Start () {
        chatMsgPrefab = (GameObject)Resources.Load("Prefabs/ChatMsg");
        chatContent = transform.Find("ChatContent");

        inputField.onEndEdit.AddListener(delegate
        {
            string msg = inputText.text;
            SendChatMsg(msg);
        });
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    private void SendChatMsg(string msg)
    {
        Debug.Log("发聊天消息");
        singleNet.Instance.SendChatMsg(msg);
    }
}
