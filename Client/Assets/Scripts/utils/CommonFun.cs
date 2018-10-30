using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonFun : MonoBehaviour {

    public static Vector3 MapIndex2Vector(int x, int y, int floor)
    {
        Vector3 vec = new Vector3(x * 10, floor * 5, y * 10);
        return vec;
    }

    public static Vector3 Vector2MapIndex(Vector3 vec)
    {
        Vector3 ret = new Vector3(Mathf.Floor(vec.x / 10), Mathf.Floor(vec.y / 5), Mathf.Floor(vec.z / 10));
        return ret;
    }

    public static Vector3 Vector2RoomPos(Vector3 vec)
    {
        Vector3 ret = new Vector3(Mathf.Floor(vec.x / 10) * 10, Mathf.Floor(vec.y / 5) * 5, Mathf.Floor(vec.z / 10) * 10);
        return ret;
    }

    public static Vector3 NextPos(Vector3 vec, direction dir)
    {
        Vector3 ret = vec;
        switch(dir)
        {
            case direction.dirUp:
                vec.z += 10;
                break;
            case direction.dirDown:
                vec.z -= 10;
                break;
            case direction.dirLeft:
                vec.x -= 10;
                break;
            case direction.dirRight:
                vec.x += 10;
                break;
            case direction.dirStop:
                break;
        }
        return vec;
    }
}
