using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class SaveData
{
    public static void SavePlayer(Player player)
    {
        string path = Application.persistentDataPath + "/player.json";
        PlayerData data = new PlayerData(player);
        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(path, json);
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonConvert.DeserializeObject<PlayerData>(json);
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
