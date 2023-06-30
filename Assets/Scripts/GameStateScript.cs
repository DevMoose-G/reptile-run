using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class MoveData
{
    public string name = "";
    public float coolDownTime = 0.0f;
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

    public UpgradeTree upgradeTree;
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
        moves.Add(new MoveData("Counter", 4.0f));
        moves.Add(new MoveData("Hydro Wave", 4.0f));
        moves.Add(new MoveData("Quick Attack", 4.0f));
    }

}

[System.Serializable]
public class GameState 
{
    public static GameState current = new GameState();

    public List<ReptileData> reptiles = new List<ReptileData> { };

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
    
    public void Clear()
    {
        PlayerPrefs.DeleteAll();
    }

    public float progressTowardsEvolution() {
        
        if (currentReptile().currentEvolution == 1) {
            Debug.Log(reptiles[current_reptile_idx].totalEvoPoints / reptiles[current_reptile_idx].stage1Evolution);
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
