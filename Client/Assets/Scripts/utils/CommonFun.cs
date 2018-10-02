using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonFun : MonoBehaviour {

    public static Vector3 MapIndex2Vector(int x, int y, int floor)
    {
        Vector3 vec = new Vector3(x * 10, floor * 5, y * 10);
        return vec;
    }
}
