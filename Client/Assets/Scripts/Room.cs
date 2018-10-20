using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protobuf;

public class Room : MonoBehaviour {

    private string roomName;
    public TextMesh nameContext;
    public MeshRenderer northWall, RightWall, southWall, LeftWall;
    int[] exportArray = { 0, 1, 1, 1 };


    public void InitRoom(int id, Vector3 v3)
    {
        Vector3 pos = CommonFun.Vector2RoomPos(v3);
        Config cf = Config.GetInstance();
        transform.position = pos;

        Dictionary<string, string> roomConfig = cf.GetRoomConfig(id);
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
