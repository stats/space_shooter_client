using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Colyseus.Schema;

public class PlayerData
{
    public static string CurrentShipUUID;
    public static MapSchema<Ship> myShips;

    public static Ship shipData;

    public static int currentWave = 1;
    public static int enemiesSpawned = 0;
    public static int enemiesKilled = 0;

    public static UnlockMessage unlockMessage;

    public static int UpgradePointsSpent = 0;
    public static Dictionary<string, int> UpgradePoints = new Dictionary<string, int>()
    {
        {"Damage", 0},
        {"Range", 0},
        {"FireRate", 0},
        {"Accelleration", 0},
        {"Speed", 0},
        {"ShieldsMax", 0},
        {"ShieldsRecharge", 0}
    };

    public static void SetUnlocks(UnlockMessage u)
    {
        unlockMessage = u;
    }

    public static float XPPercentage()
    {
        return (shipData.kills - shipData.previousLevel) / (shipData.nextLevel - shipData.previousLevel);
    }
    public static void UpdateShip(Ship ship)
    {
        myShips[ship.uuid] = ship;
    }

    public static Ship CurrentShip()
    {
        if (PlayerData.CurrentShipUUID == null) return null;
        return myShips[PlayerData.CurrentShipUUID];
    }

    public static T GetCurrentShipAttribute<T>(string _name)
    {
        return (T)typeof(Ship).GetField(_name).GetValue(PlayerData.CurrentShip());
    }

    public static void ResetUpgrades()
    {
        PlayerData.UpgradePointsSpent = 0;
        PlayerData.UpgradePoints = new Dictionary<string, int>()
        {
            {"Damage", 0},
            {"Range", 0},
            {"FireRate", 0},
            {"Accelleration", 0},
            {"Speed", 0},
            {"ShieldsMax", 0},
            {"ShieldsRecharge", 0}
        };
    }
}
