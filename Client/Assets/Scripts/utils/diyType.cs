using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    dirUp, dirRight, dirDown, dirLeft, dirStop,
}

public enum RetStatus
{
    rsSuccess, rsFail,
}

public enum PlayerStatus
{
    psEnter, psReady, psStart, psIngame, psDead,
}
