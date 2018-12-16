using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour {

    public Text characterName, characterSex, characterAge;
    public Text strength, speed, knowledge, spirit;
    public Image background;

    private Object player;
    public int characterID, roleID;
    private string playerName;
    Dictionary<examType, int> et2Level;
    Dictionary<examType, string[]> etLevel2Value;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    private void Awake()
    {
        et2Level = new Dictionary<examType, int>();
        etLevel2Value = new Dictionary<examType, string[]>();
    }

    public void SetPlayer(Object obj)
    {
        this.player = obj;
    }

    public void InitPlayerPanel(GameObject obj, int index, int roleID, GameObject config)
    {
        this.player = obj;
        this.characterID = index;
        this.roleID = roleID;
        Dictionary<string, string> playerConfig = config.GetComponent<Config>().GetPlayerConfig(index);
        //Dictionary<string, string> playerConfig = Config.Instance.GetPlayerConfig(index);

        this.characterName.text = playerConfig["name"];
        this.characterAge.text = playerConfig["age"];

        string ttt = playerConfig["originStrengthLevel"];
        et2Level[examType.etStrength] = int.Parse(playerConfig["originStrengthLevel"]);
        et2Level[examType.etSpeed] = int.Parse(playerConfig["originSpeedLevel"]);
        et2Level[examType.etKnowledge] = int.Parse(playerConfig["originKnowledgeLevel"]);
        et2Level[examType.etSpirit] = int.Parse(playerConfig["originSpiritLevel"]);
        etLevel2Value[examType.etStrength] = playerConfig["strengthValue"].Split('|');
        etLevel2Value[examType.etSpeed] = playerConfig["speedValue"].Split('|');
        etLevel2Value[examType.etKnowledge] = playerConfig["knowledgeValue"].Split('|');
        etLevel2Value[examType.etSpirit] = playerConfig["spiritValue"].Split('|');

        SetShowInfo();
    }

    private void SetShowInfo()
    {
        strength.text = etLevel2Value[examType.etStrength][et2Level[examType.etStrength]];
        speed.text = etLevel2Value[examType.etSpeed][et2Level[examType.etSpeed]];
        knowledge.text = etLevel2Value[examType.etKnowledge][et2Level[examType.etKnowledge]];
        spirit.text = etLevel2Value[examType.etSpirit][et2Level[examType.etSpirit]];
    }

    public void OnClick()
    {
        FightRoom fr = GetComponentInParent<FightRoom>();
        if (fr.CanAttack())
        {
            Protobuf.attackRequest ar = new Protobuf.attackRequest
            {
                TargetID = this.roleID,
                Option = 0,
            }
            SingleNet.Instance.SendMsgCommon(ar, "attack");
        }
    }
}
