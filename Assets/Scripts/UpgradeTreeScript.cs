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

    public List<int> nodes_obtained;

    public UpgradeTree() {
        // nodes = new Dictionary<string, UpgradeNode> { };
    }

    public static void Load() {

        var jsonData = Resources.Load<TextAsset>("upgrades_new");

        if (GameState.current.upgradeTree == null)
        {
            GameState.current.upgradeTree = JsonUtility.FromJson<UpgradeTree>(jsonData.text);
        }
    }

    public UpgradeGroup GetUpgradeGroup() {
        UpgradeGroup cat_nodes = new UpgradeGroup();

        // look for upgrades
        int i = 0;
        while (!cat_nodes.isFull() && i < nodes.Count) // while there are nodes to look through and the group has not been filled
        {
            UpgradeNode node = nodes[i];
            if (!nodes_obtained.Contains(node.id)) { // if not previously bought
                if(node.category == "Tongue" && cat_nodes.tongueUpgrade == null)
                    cat_nodes.tongueUpgrade = node;
                if (node.category == "Health" && cat_nodes.healthUpgrade == null)
                    cat_nodes.healthUpgrade = node;
                if (node.category == "Damage" && cat_nodes.damageUpgrade == null)
                    cat_nodes.damageUpgrade = node;
                if (node.category == "AttackSpeed" && cat_nodes.attackSpeedUpgrade == null)
                    cat_nodes.attackSpeedUpgrade = node;
            }
            i ++;
        }

        return cat_nodes;
    }

    public List<UpgradeNode> FirstAvailableNodes(int num) {
        List<UpgradeNode> avail_nodes = new List<UpgradeNode>();
        int i = 0;
        while(num > 0 && i < nodes.Count) 
        {
            UpgradeNode node = nodes[i];
            if (!nodes_obtained.Contains(node.id)) { // if available and not previously bought
                avail_nodes.Add(node);
                num--;
            }
            i++;
        }
        return avail_nodes;
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