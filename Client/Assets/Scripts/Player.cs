using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private string playerName;
    private int playerID;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitPlayer(int id)
    {
        playerID = id;
        Vector3 randomVec = transform.position;
        randomVec.x += id * 2;
        transform.position = randomVec;
        playerName = "123";
    }

    public direction GetInputDir()
    {
        direction dir = direction.dirStop;
        if (!Input.anyKey)
        {
            return dir;
        }
        string input = Input.inputString;
        switch (input)
        {
            case "w":
                dir = direction.dirUp;
                break;
            case "s":
                dir = direction.dirDown;
                break;
            case "a":
                dir = direction.dirLeft;
                break;
            case "d":
                dir = direction.dirRight;
                break;
            default:
                break;
        }
        return dir;
    }
}
