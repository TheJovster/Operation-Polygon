using OperationPolygon.Combat;
using OperationPolygon.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentAssaultRifleAmmo;
    public int currentSMGAmmo;
    public int currentLMGAmmo;
    public int currentSniperRifleAmmo;
    public int currentGrenades;

    public int currentHealthPacks;
    public int currentFrags;

    public GameData() //data that we can use to save - the system is very flexible
    {
        this.currentAssaultRifleAmmo = 0;
        this.currentSMGAmmo = 0;
        this.currentLMGAmmo = 0;
        this.currentSniperRifleAmmo = 0;
        this.currentGrenades = 0;
        //I can't save health data at the moment - I need to separate player and EnemyHealth scripts.
    }
}
