using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightRoom : MonoBehaviour
{
    public Button attack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanAttack()
    {
        return this.attack.enabled;
    }

    public void ClickAttack()
    {
        Debug.Log("攻击");
    }
}
