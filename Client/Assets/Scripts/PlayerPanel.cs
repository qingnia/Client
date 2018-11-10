using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanel : MonoBehaviour {

    private Object player;
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

    public void setPlayer(Object obj)
    {
        this.player = obj;
    }

}
