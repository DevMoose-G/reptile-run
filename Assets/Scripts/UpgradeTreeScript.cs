using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class UpgradeGroup { 
    public UpgradeNode tongueUpgrade;
    public UpgradeNode healthUpgrade;
    public UpgradeNode damageUpgrade;
    public UpgradeNode attackSpeedUpgrade;

    public bool isFull() { 
        if(tongueUpgrade != null && healthUpgrade != null && damageUpgrade != null && attackSpeedUpgrade != null) {
            return true;
        }
        return false;
    }

    public List<UpgradeNode> toList()
    {
        return new List<UpgradeNode> { tongueUpgrade, healthUpgrade, damageUpgrade, attackSpeedUpgrade };
    }
}

[System.Serializable]
public class UpgradeTree
{
    // public static UpgradeTree current = new UpgradeTree();

    
    public List<UpgradeNode> nodes;

    

    public UpgradeTree() {
        // nodes = new Dictionary<string, UpgradeNode> { };
    }

    public static void Load() {

        var jsonData = Resources.Load<TextAsset>("upgrades_new");

        if (GameState.current.currentReptile().upgradeTree == null)
        {
            GameState.current.currentReptile().upgradeTree = JsonUtility.FromJson<UpgradeTree>(jsonData.text);
        }
    }
    
}

[System.Serializable]
public class UpgradeNode {
    public int id;
    public string category; // Attack Speed, Damage, Health, Tongue, Extra(Cut Obstacles)
    public int level;
    public float amount; // if variable associated with number (ie: health, damage), then this is amount it increases by
    public float amount2; // for tongue speed (while amount = tongue length)
    public int cost; // cost in evoPoints

    public UpgradeNode() {
        category = "";
        level = 0;
        amount = 0;
        cost = 0;
    }
}