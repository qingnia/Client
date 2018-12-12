using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protobuf;

public class Room : MonoBehaviour {

    private string roomName;
    public int roomID;
    public TextMesh nameContext;
    public MeshRenderer northWall, RightWall, southWall, LeftWall;
    int[] exportArray = { 0, 1, 1, 1 };


    public void InitRoom(int id, Vector3 v3, Config config)
    {
        this.roomID = id;
        Vector3 pos = CommonFun.Vector2RoomPos(v3);
        transform.position = pos;

        Dictionary<string, string> roomConfig = config.GetRoomConfig(id);
        roomName = roomConfig["name"];

        SetShowInfo();
    }

    private void SetShowInfo()
    {
        nameContext.text = roomName;

        if (exportArray[0] == 0)
        {
            northWall.gameObject.SetActive(false);
        }
        if (exportArray[1] == 0)
        {
            RightWall.gameObject.SetActive(false);
        }
        if (exportArray[2] == 0)
        {
            southWall.gameObject.SetActive(false);
        }
        if (exportArray[3] == 0)
        {
            LeftWall.gameObject.SetActive(false);
        }
    }
}
