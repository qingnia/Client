using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fightCache
{
    public int RoleID { get; set; }
    public Direction dir { get; set; }
    public int RoomID { get; set; }
    public int CardID { get; set; }
}

public delegate void GameHisHandler(string msg);

public class GameController : MonoBehaviour
{
    public event GameHisHandler GameHisEvent;

    public GameObject playerStatusUI;
    public GameObject config;

    [HideInInspector]
    public Dictionary<int, Player> playerList = new Dictionary<int, Player>();
    public Dictionary<int, WaitPlayer> waitPlayers = new Dictionary<int, WaitPlayer>();
    public List<Protobuf.moveBroadcast> moveCache = new List<Protobuf.moveBroadcast>();
    public List<Protobuf.attackBroadcast> attackCache = new List<Protobuf.attackBroadcast>();
    private Dictionary<int, Dictionary<int, Room>> roomMap;

    //Room[][] roomMaps = new Room[100][];

    private GameObject playerPrefab;
    private GameObject playerPanelPrefab;
    private GameObject roomPrefab;
    private Config cf;

    private int roomCount = 1;
    public int actionRoleID;


    // Use this for initialization
    void Start()
    {
        roomMap = new Dictionary<int, Dictionary<int, Room>>(100);
        playerPrefab = (GameObject)Resources.Load("Prefabs/Player");
        playerPanelPrefab = (GameObject)Resources.Load("Prefabs/PlayerPanel");
        roomPrefab = (GameObject)Resources.Load("Prefabs/Room");
        SingleNet.Instance.PlayerMove += OnPlayerMove;
        SingleNet.Instance.AttackEvent += OnAttackEvent;
        cf = config.GetComponent<Config>();

        InitGame();

        GameHisEvent("游戏开始！");
    }

    // Update is called once per frame
    //玩家移动控制player,玩家状态改变控制playerpanel
    void Update()
    {
        //响应攻击消息
        TryAttack();

        //先执行移动
        PlayerTryMove();
        //只有在自己行动回合，才监听输入，否则只接收网络消息
        if (PlayerData.Instance.RoleID == actionRoleID)
        {
            //判断要不要显示攻击按钮
            SetAttackButton();

            Direction dir = GetInputDir();
            if (dir != Direction.dirNone)
            {
                Protobuf.moveRequest mr = new Protobuf.moveRequest
                {
                    Direction = (int)dir
                };
                SingleNet.Instance.SendMsgCommon(mr, "move");
            }
        }
        /*
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
        */
    }

    public void OnPlayerMove(Protobuf.moveBroadcast mb)
    {
        this.moveCache.Add(mb);
    }

    public void OnAttackEvent(Protobuf.attackBroadcast ab)
    {
        this.attackCache.Add(ab);
        GameHisEvent("接收到攻击消息");
    }

    public Direction GetInputDir()
    {
        Direction dir = Direction.dirNone;
        if (!Input.anyKey)
        {
            return dir;
        }
        string input = Input.inputString;
        switch (input)
        {
            case "w":
                dir = Direction.dirUp;
                break;
            case "s":
                dir = Direction.dirDown;
                break;
            case "a":
                dir = Direction.dirLeft;
                break;
            case "d":
                dir = Direction.dirRight;
                break;
            case "o":
                dir = Direction.dirStop;
                break;
            default:
                break;
        }
        return dir;
    }

    public void InitGame()
    {
        InitRoomMap();
        InitPlayerList();
    }

    void PlayerTryMove()
    {
        if (this.moveCache.Count <=0 )
        {
            return;
        }
        foreach (var item in moveCache)
        {
            int roleID = item.RoleID;
            if (!playerList.ContainsKey(roleID))
            {
                Debug.Log("玩家移动了，但是找不到这个人 role:" + roleID);
                return;
            }
            Player p = playerList[roleID];
            Direction dir = (Direction)item.Direction;
            
            GameHisEvent("玩家" + roleID + "向" + dir + "移动");

            Vector3 nextPos = CommonFun.NextPos(p.transform.position, dir);
            //Vector3 vec = CommonFun.Vector2MapIndex(nextPos);
            Vector3 mapIndex = CommonFun.Vector2MapIndex(nextPos);
            int x = (int)mapIndex.x;
            int y = (int)mapIndex.z;
            if (!roomMap.ContainsKey(x) || !roomMap[x].ContainsKey(y))
            {
                Room r = AddNewRoom(item.RoomID, nextPos);
                if (!roomMap.ContainsKey(x))
                {
                    roomMap[x] = new Dictionary<int, Room>();
                }
                roomMap[x][y] = r;
            }
            p.transform.position = nextPos;

            //如果有卡片，还要执行卡片的操作
            if (item.CardID > 0)
            {
                Debug.Log("玩家获得了新卡片：" + item.CardID);
            }
            if (item.NextActionRoleID > 0)
            {
                GameHisEvent("行动切换，接下来是" + item.NextActionRoleID);
                this.actionRoleID = item.NextActionRoleID;
            }
            if (item.NeedAttack)
            {
                GameHisEvent("玩家需要选择是否攻击");
            }
        }
        moveCache.Clear();
    }
    void TryAttack()
    {
        if (this.attackCache.Count <= 0)
        {
            return;
        }
        foreach (var item in attackCache)
        {
            int roleID = item.RoleID;
            int targetID = item.TargetID;

            GameHisEvent("玩家" + roleID + "攻击了" + targetID);
        }
        attackCache.Clear();
    }

    private void InitPlayerList()
    {
        foreach (var item in waitPlayers)
        {
            this.AddNewPlayer(item.Key, item.Value);
        }
    }

    private void AddNewPlayer(int roleID, WaitPlayer wp)
    {
        int characterID = playerList.Count + 1;
        //地图内玩家
        GameObject go = Instantiate(playerPrefab);
        go.transform.parent = this.transform;
        go.name = "动态" + playerList.Count;
        Player p = go.GetComponent<Player>();
        p.InitPlayer(characterID, roleID);

        //状态UI玩家
        GameObject pp = Instantiate(playerPanelPrefab);
        PlayerPanel playerPanel = pp.GetComponent<PlayerPanel>();
        //playerPanel.SetPlayer(go);
        //playerPanel.InitPlayer(n);
        pp.GetComponent<PlayerPanel>().InitPlayerPanel(go, roleID, characterID, config);

        Vector3 newPos = pp.transform.position;
        switch(characterID)
        {
            case 1:
                newPos.y -= 160;
                break;
            case 2:
                newPos.x -= 330;
                break;
            case 3:
                newPos.x += 330;
                break;
            case 4:
                break;
            case 5:
                newPos.y += 160;
                break;
            default:
                break;
        }
        pp.transform.position = newPos;
        pp.transform.SetParent(playerStatusUI.transform, false);

        playerList[roleID] = p;
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
        room.InitRoom(roomID, v3, cf);
        return room;
    }

    public Room GetRoomByPos(Vector3 pos)
    {
        Vector3 mapIndex = CommonFun.Vector2MapIndex(pos);
        int x = (int)mapIndex.x;
        int y = (int)mapIndex.z;
        if (!roomMap.ContainsKey(x) || !roomMap[x].ContainsKey(y))
        {
            return null;
        }
        return roomMap[x][y];
    }

    private void SetAttackButton()
    {
        FightRoom fr = playerStatusUI.GetComponent<FightRoom>();
        fr.attack.gameObject.SetActive(false);

        int selfRoleID = PlayerData.Instance.RoleID;
        Player p = playerList[selfRoleID];
        int selfRoomID = GetRoomIDByRoleID(selfRoleID);

        Vector3 pos = p.transform.position;
        foreach (var op in playerList)
        {
            if (op.Key == selfRoleID)
            {
                continue;
            }
            if (GetRoomIDByRoleID(op.Value.RoleID) == selfRoomID)
            {
                fr.attack.gameObject.SetActive(true);
                PlayerPanel[] pps = fr.GetComponentsInChildren<PlayerPanel>();
                foreach (var pp in pps)
                {
                    if (pp.roleID == op.Value.RoleID)
                    {
                        pp.background.color = new Color(255, 0, 0);
                    }
                }
            }
        }
    }

    private int GetRoomIDByRoleID(int roleID)
    {
        if (!playerList.ContainsKey(roleID))
        {
            Debug.Log("找不到玩家：" + roleID);
            return 0;
        }
        Player p = playerList[roleID];
        Room r = GetRoomByPos(p.gameObject.transform.position);
        return r.roomID;
    }
}
