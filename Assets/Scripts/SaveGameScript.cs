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
        /*
        string json = JsonUtility.ToJson(GameState.current);

        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/game.rr", true);
        writer.Write(json);
        writer.Close();
        */

        GameState.current.Save();

        /*
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located 
        FileStream file = File.Create(Application.persistentDataPath + "/gameData.rr"); //you can call it anything you want 
        bf.Serialize(file, SaveGameScript.gameState);
        file.Close();
        */
    }

    public static void Load()
    {
        /*
        StreamReader reader = new StreamReader(Application.persistentDataPath + "/game.rr");
        string jsonData = reader.ReadToEnd();
        Debug.Log(jsonData);
        GameState.current = JsonUtility.FromJson<GameState>(jsonData);
        reader.Close();
        */

        GameState.current.Load();

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
}
