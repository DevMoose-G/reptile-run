using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameState 
{
    public static GameState current = new GameState();
    public int evoPoints = 0;
    public int totalEvoPoints = 0; // the total accumulated evo points
    public int crowns; 
    public int currentEvolution = 1; // stage 1 of evolution
    public UpgradeTree upgradeTree;

    public float tongueSpeed = 20.0f;
    public float tongueRetractionSpeed = 35.0f;
    public float tonguePeakLength = 12.0f;

    public float MAX_HEALTH = 2.0f;
    public float attackSpeed = 3.5f; // num of seconds it takes to hit an attack
    public float damage = 1.0f;

    public float stage1Evolution = 2500f;
    public float stage2Evolution = 12500f;

    public GameState() { 
        evoPoints = 0;
    }
    
    public void Clear()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("evoPoints", evoPoints);
        PlayerPrefs.SetInt("totalEvoPoints", totalEvoPoints);
        PlayerPrefs.SetInt("crowns", crowns);
        PlayerPrefs.SetInt("currentEvolution", currentEvolution);

        PlayerPrefs.SetFloat("tongueSpeed", tongueSpeed);
        PlayerPrefs.SetFloat("tongueRetractionSpeed", tongueRetractionSpeed);
        PlayerPrefs.SetFloat("tonguePeakLength", tonguePeakLength);

        PlayerPrefs.SetFloat("MAX_HEALTH", MAX_HEALTH);
        PlayerPrefs.SetFloat("attackSpeed", attackSpeed);
        PlayerPrefs.SetFloat("damage", damage);

        PlayerPrefs.SetString("upgradeTreeNodes", string.Join(",", upgradeTree.nodes_obtained));
    }

    public bool Load()
    {
        UpgradeTree.Load();

        int playerPrefsSet = PlayerPrefs.GetInt("evoPoints", -1);
        if(playerPrefsSet == -1)
        {
            return false;
        }

        evoPoints = PlayerPrefs.GetInt("evoPoints", evoPoints);
        totalEvoPoints = PlayerPrefs.GetInt("totalEvoPoints", totalEvoPoints);
        crowns = PlayerPrefs.GetInt("crowns", crowns);
        currentEvolution = PlayerPrefs.GetInt("currentEvolution", currentEvolution);

        tongueSpeed = PlayerPrefs.GetFloat("tongueSpeed", tongueSpeed);
        tongueRetractionSpeed = PlayerPrefs.GetFloat("tongueRetractionSpeed", tongueRetractionSpeed);
        tonguePeakLength = PlayerPrefs.GetFloat("tonguePeakLength", tonguePeakLength);

        MAX_HEALTH = PlayerPrefs.GetFloat("MAX_HEALTH", MAX_HEALTH);
        attackSpeed = PlayerPrefs.GetFloat("attackSpeed", attackSpeed);
        damage = PlayerPrefs.GetFloat("damage", damage);

        string upgradeNodesStr = PlayerPrefs.GetString("upgradeTreeNodes", "");
        string[] upgradeNodesStrings = upgradeNodesStr.Split(new char[] { ',' });
        for (int i = 0; i < upgradeNodesStrings.Length; i++)
        {
            if(int.TryParse(upgradeNodesStrings[i], out int upgradeNodeID))
                upgradeTree.nodes_obtained.Add(upgradeNodeID);
        }

        // Debug.Log(upgradeNodesStrings);
        /*
        // converts '1,2,5,8' to [1, 2, 5, 8] for checking what upgrades you already obtained
        string upgradeNodesStr = PlayerPrefs.GetString("upgradeTreeNodes", "");
        string[] upgradeNodesStrings = upgradeNodesStr.Split(new char[] { ',' });
        Debug.Log(upgradeNodesStrings);
        for(int i = 0; i < upgradeNodesStrings.Length; i++)
        {
            upgradeTree.nodes_obtained.Add(int.Parse(upgradeNodesStrings[i]));
        }
        */

        return true;
    }

    public float progressTowardsEvolution() {
        
        if (currentEvolution == 1) {
            Debug.Log(totalEvoPoints / stage1Evolution);
            return totalEvoPoints / stage1Evolution;
        } else if (currentEvolution == 2)
        {
            return (totalEvoPoints - stage1Evolution) / stage2Evolution;
        }

        return -1;
    }

    public void subtractEvoPoints(int points) {
        if (points < 0)
        {
            Debug.LogError("Subtracting negative amount of evo points. Use the addEvoPoints method instead.");
        }
        evoPoints -= points;
    }
    
    public void addEvoPoints(int points)
    {
        if (points < 0) {
            Debug.LogError("Adding negative amount of evo points. Use the subtractEvoPoints method instead.");
        }
        evoPoints += points;
        totalEvoPoints += points;
    }

}
