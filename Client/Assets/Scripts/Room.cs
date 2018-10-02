using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    public MeshRenderer northWall, RightWall, southWall, LeftWall;
    int[] exportArray = { 0, 1, 1, 1 };

    public void InitRoom(int id, Vector3 v3)
    {
        transform.position = v3;
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
