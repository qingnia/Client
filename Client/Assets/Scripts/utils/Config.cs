using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : SingleInstance<Config>
{

    private static Config cf = new Config();

    private Dictionary<int, Dictionary<string, string>> roomConfig = new Dictionary<int, Dictionary<string, string>>();

    private Config() {}

    public static Config GetInstance()
    {
        return cf;
    }

    public Dictionary<string, string> GetRoomConfig(int roomID)
    {
        if (roomConfig.Count == 0)
        {
            roomConfig = CSVFileHelper.OpenCSV("Assets/Tables/Room.csv");
        }
        return roomConfig[roomID];
    }

    public Dictionary<string, string> GetPlayerConfig(int playerID)
    {
        if (roomConfig.Count == 0)
        {
            roomConfig = CSVFileHelper.OpenCSV("Assets/Tables/Player.csv");
        }
        return roomConfig[playerID];
    }
}
