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

    public void SetPlayer(Object obj)
    {
        this.player = obj;
    }

}
