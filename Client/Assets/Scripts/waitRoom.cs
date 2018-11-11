using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PublicInfo
{
    public int roleID;
    public string name;
    public int status;
}

public class WaitRoom : MonoBehaviour {

    public LayoutGroup waitPlayerLayout;
    private static GameObject waitPlayerPrefab;
    public List<PublicInfo> waitCacheList;
    public List<GameObject> waitPlayerList = new List<GameObject>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UpdateChatHis();
    }

    private void UpdateChatHis()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        if (waitCacheList.Count <= 0)
        {
            return;
        }
        foreach (var item in waitCacheList)
        {
            Debug.Log("show new player. roleID:" + item.roleID);
            GameObject go = Instantiate(waitPlayerPrefab);
            go.GetComponent<WaitPlayer>().nameText.text = item.name;
            go.GetComponent<WaitPlayer>().roleIDText.text = item.roleID.ToString();
            go.GetComponent<WaitPlayer>().status.text = item.status.ToString();

            go.transform.SetParent(waitPlayerLayout.transform);
            go.name = "动态" + waitPlayerList.Count;
            waitPlayerList.Add(go);
            waitCacheList.Remove(item);
        }
    }

    private void Awake()
    {
        Debug.Log("room awake");
        waitCacheList = new List<PublicInfo>();
        waitPlayerPrefab = (GameObject)Resources.Load("Prefabs/waitPlayer");
        singleNet.Instance.PlayerJoinEvent += new PlayerJoinEventHandler(AddNewWaitPlayer);
    }

    public void WaitRoomOnLogin(Protobuf.playersInfo pinfos)
    {
        Debug.Log("login success, player count:" + pinfos.BaseInfos);
        foreach(var p in pinfos.BaseInfos)
        {
            PublicInfo pi;
            pi.roleID = p.RoleID;
            pi.name = p.Name;
            pi.status = p.Status;
            this.waitCacheList.Add(pi);
        }
    }

    void AddNewWaitPlayer(PublicInfo pinfo)
    {
        PublicInfo pi = new PublicInfo();
        this.waitCacheList.Add(pi);
    }
}
