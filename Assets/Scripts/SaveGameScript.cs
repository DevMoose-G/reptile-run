using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveGameScript
{

    //it's static so we can call it from anywhere 
    public static void Save()
    {
        
        string json = JsonUtility.ToJson(GameState.current);

        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/game.rr", false);
        writer.Write(json);
        writer.Close();
    }

    public static void Load()
    {
        UpgradeTree.Load();
        if (System.IO.File.Exists(Application.persistentDataPath + "/game.rr") == false)
        {
            return;
        }

        StreamReader reader = new StreamReader(Application.persistentDataPath + "/game.rr");
        string jsonData = reader.ReadToEnd();
        Debug.Log(jsonData);
        reader.Close();
        GameState.current = JsonUtility.FromJson<GameState>(jsonData);

        /*
        if (File.Exists(Application.persistentDataPath + "/gameData.rr"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameData.rr", FileMode.Open);
            SaveGameScript.gameState = (GameState)bf.Deserialize(file);
            file.Close();
        }
        */
    }

    public static void Clear()
    {
        Debug.Log("CLEARING DATA");
        GameState.current = new GameState();
        
        if (System.IO.File.Exists(Application.persistentDataPath + "/game.rr"))
        {
            // If file found, delete it
            System.IO.File.Delete(Application.persistentDataPath + "/game.rr");
        }
    }
}
