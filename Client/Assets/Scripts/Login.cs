using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    public string serverURL;
    public int port;

    public Text accountText;
    public Text passwordText;
    public Text roomText;

    public GameObject chatScoll;
    public GameObject waitRoom;

	// Use this for initialization
	void Start () {
        singleNet.Instance.LoginEvent += new LoginEventHandler(OnLoginRet);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ConnectServer()
    {
        Debug.Log("try login");
        singleNet.Instance.ConnectGameServer(serverURL, port);
        Invoke("TryLogin", 1);
    }

    void TryLogin()
    {
        int roleID = int.Parse(accountText.text);
        int roomID = int.Parse(roomText.text);
        string password = passwordText.text;

        singleNet.Instance.Login(roleID, roomID);
    }

    public void OnLoginRet(int status)
    {
        if (RetStatus.rsSuccess.Equals(status))
        {
            Debug.Log("login success, then get ready!");
            this.gameObject.SetActive(false);

            //激活房间界面
            chatScoll.SetActive(true);
            waitRoom.SetActive(true);
        }
        Debug.Log("login fail, retry");
    }
    
}
