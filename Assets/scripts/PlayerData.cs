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

    public static UnlockMessage unlocks;

    public static int UpgradePointsSpent = 0;
    public static Dictionary<string, int> UpgradePoints = new Dictionary<string, int>()
    {
        {"damage", 0},
        {"range", 0},
        {"fire_rate", 0},
        {"accelleration", 0},
        {"speed", 0},
        {"shields_max", 0},
        {"shields_recharge", 0}
    };

    public static void SetUnlocks(UnlockMessage u)
    {
        unlocks = u;
    }

    public static float XPPercentage()
    {
        //Debug.Log("XP" + shipData.kills + ", " + shipData.previous_level + ", " + shipData.next_level);
        return (shipData.kills - shipData.previous_level) / (shipData.next_level - shipData.previous_level);
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
            {"damage", 0},
            {"range", 0},
            {"fire_rate", 0},
            {"accelleration", 0},
            {"speed", 0},
            {"shields_max", 0},
            {"shields_recharge", 0}
        };
    }
}
