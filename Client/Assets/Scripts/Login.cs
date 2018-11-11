using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    public string serverURL;
    public int port;
    public bool loginActive;

    public Text accountText;
    public Text passwordText;
    public Text roomText;

    public GameObject chatScoll;
    public GameObject waitRoom;

    private Protobuf.playersInfo pinfos;

	// Use this for initialization
	void Start () {
        singleNet.Instance.LoginEvent += new LoginEventHandler(OnLoginRet);
        this.loginActive = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (this.gameObject.activeSelf != loginActive)
        {
            Debug.Log("login success, then get ready!");
            this.gameObject.SetActive(loginActive);

            if (!loginActive)
            {
                //激活房间界面
                chatScoll.SetActive(true);
                waitRoom.SetActive(true);
                waitRoom.GetComponent<WaitRoom>().WaitRoomOnLogin(this.pinfos);
            }
        }
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

    public void OnLoginRet(Protobuf.playersInfo pinfos)
    {
        if ((int)RetStatus.rsSuccess == pinfos.Cr.Status)
        {
            loginActive = false;
            this.pinfos = pinfos;
            return;
        }
        Debug.Log("login fail, retry");
    }
    
}
