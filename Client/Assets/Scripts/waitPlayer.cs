﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitPlayer : MonoBehaviour {

    private PublicInfo m_info;
    public Text nameText;
    public Text roleIDText;
    public Text status;
    public Text characterID;

    private string playerName;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitWaitPlayer(PublicInfo pinfo)
    {
        this.m_info = pinfo;
    }
}
