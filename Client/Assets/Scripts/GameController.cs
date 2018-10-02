using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [HideInInspector]
    public List<Player> playerList = new List<Player>();
    private Dictionary<int, Dictionary<int, Room>> roomMap;
    //Room[][] roomMaps = new Room[100][];

    private GameObject playerPrefab;
    private GameObject roomPrefab;

    private int currentPlayerID;

    // Use this for initialization
    void Start()
    {
        currentPlayerID = 0;
        roomMap = new Dictionary<int, Dictionary<int, Room>>(100);
        playerPrefab = (GameObject)Resources.Load("Prefabs/Player");
        roomPrefab = (GameObject)Resources.Load("Prefabs/Room");
        InitPlayerList();
        InitRoomMap();
    }

    // Update is called once per frame
    void Update()
    {
        Player p = playerList[currentPlayerID];
        direction dir = p.GetInputDir();
        if (dir == direction.dirStop)
        {
            return;
        }
        PlayerTryMove(p, dir);
    }

    void PlayerTryMove(Player p, direction dir)
    {
        currentPlayerID++;
    }

    public void InitPlayerList()
    {

        for (int i = 0; i < 3; i++)
        {
            Player p = AddNewPlayer(i);
            playerList.Add(p);
            /*
            GameObject go = Instantiate(playerPrefab);
            go.transform.parent = transform;
            GameObject p = GetComponentInChildren<GameObject>();
            p.name = "111";*/
        }
    }

    private Player AddNewPlayer(int n)
    {
        GameObject go = Instantiate(playerPrefab);
        go.transform.parent = transform;
        go.name = "动态" + playerList.Count;
        Player p = go.GetComponent<Player>();
        p.InitPlayer(n);
        return p;
    }

    void InitRoomMap()
    {
        int x, y;
        for (int i = 1; i <= 3; i++)
        {
            x = 1 - i;
            y = 0;
            Vector3 vec1 = CommonFun.MapIndex2Vector(x, y, 0);
            Room r = AddNewRoom(i, vec1);
            if (!roomMap.ContainsKey(x))
            {
                roomMap[x] = new Dictionary<int, Room>(100);
            }
            roomMap[x][y] = r;
        }
    }

    private Room AddNewRoom(int roomID, Vector3 v3)
    {
        GameObject go = Instantiate(roomPrefab);
        go.transform.parent = transform.GetChild(0);
        go.name = "新房间";
        Room room = go.GetComponent<Room>();
        room.InitRoom(roomID, v3);
        return room;
    }
}
