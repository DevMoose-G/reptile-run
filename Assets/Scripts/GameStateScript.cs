using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class EggData
{
    public string species = "";
    public int crownCost = 0;
    public float HATCH_TIMER = 0;
    public float hatchTimer = 0;

    public string getModelLocation()
    {
        if (species == "Gecko")
        {
            return "Eggs/GeckoEgg";
        }
        else if (species == "Chameleon")
        {
            return "Eggs/CamilaEgg";
        }
        return "";
    }
}

[System.Serializable]
public class MoveData
{
    public string name = "";
    public float coolDownTime = 0.0f;
    public float timer = 0.0f;
    public float damageMultiplier = 1.0f;
    public bool auto = false;

    public MoveData(string n, float cooldown)
    {
        this.name = n;
        this.coolDownTime = cooldown;
    }
}

[System.Serializable]
public class ReptileData {
    public string name = "";
    public string species = "";

    public int evoPoints = 0;
    public int totalEvoPoints = 0; // the total accumulated evo points

    [System.NonSerialized]
    public UpgradeTree upgradeTree;

    public List<int> nodes_obtained;

    public int currentEvolution = 1; // stage 1 of evolution

    public float tongueSpeed = 20.0f;
    public float tongueRetractionSpeed = 35.0f;
    public float tonguePeakLength = 12.0f;

    public float MAX_HEALTH = 2.0f;
    public float attackSpeed = 3.5f; // num of seconds it takes to hit an attack
    public float damage = 1.0f;

    public float stage1Evolution = 3500f;
    public float stage2Evolution = 9800f;

    public List<MoveData> moves = new List<MoveData>();

    public Dictionary<string, int> stage_levels = new Dictionary<string, int>(); // each reptile has progress on what level they are on

    public ReptileData()
    {
        // stage level first
        stage_levels.Add("ForestStage", 1);
        evoPoints = 0;
        name = "Gary";
        species = "Gecko";

        moves.Add(new MoveData("Bite", 2.0f));
    }

    public string getModelLocation()
    {
        if(species == "Gecko")
        {
            if (currentEvolution == 1)
            {
                return "Evolutions/Gecko_Stage1";
            }
            else if (currentEvolution == 2)
            {
                return "Evolutions/Gecko_Stage2";
            }
            else if (currentEvolution == 3)
            {
                return "Evolutions/Gecko_Stage3";
            }
        } else if (species == "Chameleon")
        {

        }

        return "";
    }

    public UpgradeGroup GetUpgradeGroup()
    {
        UpgradeGroup cat_nodes = new UpgradeGroup();

        // look for upgrades
        int i = 0;
        while (!cat_nodes.isFull() && i < upgradeTree.nodes.Count) // while there are nodes to look through and the group has not been filled
        {
            UpgradeNode node = upgradeTree.nodes[i];
            if (!nodes_obtained.Contains(node.id))
            { // if not previously bought
                if (node.category == "Tongue" && cat_nodes.tongueUpgrade == null)
                    cat_nodes.tongueUpgrade = node;
                if (node.category == "Health" && cat_nodes.healthUpgrade == null)
                    cat_nodes.healthUpgrade = node;
                if (node.category == "Damage" && cat_nodes.damageUpgrade == null)
                    cat_nodes.damageUpgrade = node;
                if (node.category == "AttackSpeed" && cat_nodes.attackSpeedUpgrade == null)
                    cat_nodes.attackSpeedUpgrade = node;
            }
            i++;
        }

        return cat_nodes;
    }

    // not used so far
    public List<UpgradeNode> FirstAvailableNodes(int num)
    {
        List<UpgradeNode> avail_nodes = new List<UpgradeNode>();
        int i = 0;
        while (num > 0 && i < upgradeTree.nodes.Count)
        {
            UpgradeNode node = upgradeTree.nodes[i];
            if (!nodes_obtained.Contains(node.id))
            { // if available and not previously bought
                avail_nodes.Add(node);
                num--;
            }
            i++;
        }
        return avail_nodes;
    }

}

[System.Serializable]
public class GameState 
{
    public static GameState current = new GameState();

    public List<ReptileData> reptiles = new List<ReptileData> { };
    public List<EggData> eggs = new List<EggData> { };

    public int current_reptile_idx = 0;
    
    public int crowns;

    public GameState() {

        // load gary gecko as first reptile
        reptiles.Add(new ReptileData());

        current_reptile_idx = 0;
    }

    public ReptileData currentReptile()
    {
        return reptiles[current_reptile_idx];
    }

    public bool addEgg(string species)
    {
        EggData egg = new EggData();
        egg.species = species;
        switch (species)
        {
            case "Gecko":
                egg.HATCH_TIMER = 4.0f;
                egg.hatchTimer = 4.0f;
                egg.crownCost = 2;
                break;
            case "Chameleon":
                egg.HATCH_TIMER = 4.0f;
                egg.hatchTimer = 4.0f;
                egg.crownCost = 6;
                break;
        }

        if(egg.crownCost > crowns)
        {
            return false;
        }
        crowns -= egg.crownCost;

        eggs.Add(egg);
        return true;
    }
    
    public void Clear()
    {
        PlayerPrefs.DeleteAll();
    }

    public float progressTowardsEvolution() {
        
        if (currentReptile().currentEvolution == 1) {
            return reptiles[current_reptile_idx].totalEvoPoints / reptiles[current_reptile_idx].stage1Evolution;
        } else if (currentReptile().currentEvolution == 2)
        {
            return (reptiles[current_reptile_idx].totalEvoPoints - reptiles[current_reptile_idx].stage1Evolution) / reptiles[current_reptile_idx].stage2Evolution;
        }

        return -1;
    }

    public void subtractEvoPoints(int points) {
        if (points < 0)
        {
            Debug.LogError("Subtracting negative amount of evo points. Use the addEvoPoints method instead.");
        }
        reptiles[current_reptile_idx].evoPoints -= points;
    }
    
    public void addEvoPoints(int points)
    {
        if (points < 0) {
            Debug.LogError("Adding negative amount of evo points. Use the subtractEvoPoints method instead.");
        }
        reptiles[current_reptile_idx].evoPoints += points;
        reptiles[current_reptile_idx].totalEvoPoints += points;
    }

}
