using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMsg : MonoBehaviour {
    
    public Text playerName;
    public Text chatMsg;

    private int playerRoleID;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    /*
    public void InitPlayer(int roleID, string msg)
    {
        playerRoleID = roleID;
        playerName.text = roleID.ToString();
        chatMsg.text = msg;
    }*/
}
