﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject playerStatusUI;

    [HideInInspector]
    public List<Player> playerList = new List<Player>();
    private Dictionary<int, Dictionary<int, Room>> roomMap;

    //Room[][] roomMaps = new Room[100][];

    private GameObject playerPrefab;
    private GameObject playerPanelPrefab;
    private GameObject roomPrefab;

    private int currentPlayerID;
    private int roomCount = 1;


    // Use this for initialization
    void Start()
    {
        currentPlayerID = -1;
        roomMap = new Dictionary<int, Dictionary<int, Room>>(100);
        playerPrefab = (GameObject)Resources.Load("Prefabs/Player");
        playerPanelPrefab = (GameObject)Resources.Load("Prefabs/PlayerPanel");
        roomPrefab = (GameObject)Resources.Load("Prefabs/Room");

        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlayerID < 0)
        {
            return;
        }
        Player p = playerList[currentPlayerID];
        Direction dir = p.GetInputDir();
        if (dir == Direction.dirStop)
        {
            return;
        }

        PlayerTryMove(p, dir);
    }

    public void InitGame()
    {
        currentPlayerID = 0;
        InitPlayerList();
        InitRoomMap();
    }

    void PlayerTryMove(Player p, Direction dir)
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
        //地图内玩家
        GameObject go = Instantiate(playerPrefab);
        go.transform.parent = this.transform;
        go.name = "动态" + playerList.Count;
        Player p = go.GetComponent<Player>();
        p.InitPlayer(n);

        //状态UI玩家
        GameObject pp = Instantiate(playerPanelPrefab);
        PlayerPanel playerPanel = pp.GetComponent<PlayerPanel>();
        //playerPanel.SetPlayer(go);
        //playerPanel.InitPlayer(n);
        pp.transform.SetParent(playerStatusUI.transform, false);
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
