using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScoll : MonoBehaviour {

    public GameObject gameScene;
    public GameObject gameHisContent;
    private GameObject hisPrefab;

    // Use this for initialization
    void Start () {
        hisPrefab = (GameObject)Resources.Load("Prefabs/His");
        gameScene.GetComponent<GameController>().GameHisEvent += Handle_GameHisEvent;
    }

    void Handle_GameHisEvent(string msg)
    {
        GameObject his = Instantiate(hisPrefab);
        His h = his.GetComponent<His>();
        h.hisStr.text = msg;
        his.transform.SetParent(gameHisContent.transform);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
