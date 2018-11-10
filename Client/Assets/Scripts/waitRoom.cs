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

    }

    private void UpdateChatHis()
    {
        if (waitCacheList.Count <= 0)
        {
            return;
        }
        foreach (var item in waitCacheList)
        {
            GameObject go = Instantiate(waitPlayerPrefab);
            go.GetComponent<WaitPlayer>().nameText.text = item.name;
            go.GetComponent<WaitPlayer>().roleIDText.text = item.roleID.ToString();
            go.GetComponent<WaitPlayer>().status.text = item.status.ToString();

            go.transform.parent = waitPlayerLayout.transform;
            go.name = "动态" + waitPlayerList.Count;
            waitPlayerList.Add(go);
            waitCacheList.Remove(item);
        }
    }

    private void Awake()
    {
        waitCacheList = new List<PublicInfo>();
        waitPlayerPrefab = (GameObject)Resources.Load("Prefabs/waitPlayer");
        singleNet.Instance.PlayerJoinEvent += new PlayerJoinEventHandler(AddNewWaitPlayer);
    }

    void AddNewWaitPlayer(PublicInfo pinfo)
    {
        PublicInfo pi = new PublicInfo();
        this.waitCacheList.Add(pi);
    }
}
