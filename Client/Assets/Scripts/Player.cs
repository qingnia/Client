using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    
    private int roleID;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void InitPlayer(int id)
    {
        roleID = id;
        Vector3 randomVec = transform.position;
        randomVec.x += id * 2;
        transform.position = randomVec;
    }

}
