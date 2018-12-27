using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Res : MonoBehaviour
{
    public Text resName;
    [HideInInspector]
    public int cardID;
    [HideInInspector]
    public GameObject playerPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitRes(GameObject pp, int index, GameObject config)
    {
        this.cardID = index;
        this.playerPanel = pp;
        Dictionary<string, string> cardConfig = config.GetComponent<Config>().GetResConfig(index);

        this.resName.text = cardConfig["name"];
    }

    private void SetShowInfo()
    {

    }
}
