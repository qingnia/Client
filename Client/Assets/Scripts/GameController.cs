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
    private int roomCount = 1;

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
        Vector3 nextPos = CommonFun.NextPos(p.transform.position, dir);
        //Vector3 vec = CommonFun.Vector2MapIndex(nextPos);
        Vector3 mapIndex = CommonFun.Vector2MapIndex(nextPos);
        int x = (int)mapIndex.x;
        int y = (int)mapIndex.z;
        if (! roomMap.ContainsKey(x) || !roomMap[x].ContainsKey(y))
        {
            Room r = AddNewRoom(roomCount, nextPos);
            if (!roomMap.ContainsKey(x))
            {
                roomMap[x] = new Dictionary<int, Room>();
            }
            roomMap[x][y] = r;
        }
        p.transform.position = nextPos;
        currentPlayerID++;
        if (currentPlayerID >= playerList.Count)
        {
            currentPlayerID = 0;
        }
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
        for (int i = 0; i <= 2; i++)
        {
            x = 0 - i;
            y = 0;
            Vector3 vec = CommonFun.MapIndex2Vector(x, y, 0);
            Room r = AddNewRoom(i+1, vec);
            if (!roomMap.ContainsKey(x))
            {
                roomMap[x] = new Dictionary<int, Room>();
            }
            roomMap[x][y] = r;
        }
    }

    private Room AddNewRoom(int roomID, Vector3 v3)
    {
        roomCount++;
        GameObject go = Instantiate(roomPrefab);
        go.transform.parent = transform.GetChild(0);
        go.name = "新房间";
        Room room = go.GetComponent<Room>();
        room.InitRoom(roomID, v3);
        return room;
    }
}
