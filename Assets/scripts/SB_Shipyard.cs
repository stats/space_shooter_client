using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

using Colyseus;
using Colyseus.Schema;
using GameDevWare.Serialization;
using ZombieHeadGames.UI;
using System.Threading.Tasks;

public class SB_Shipyard
{
    private Room<ShipBuilderState> shipBuilderRoom;

    private SB_RoomManager RoomManager;

    public SB_Shipyard(SB_RoomManager manager)
    {
        RoomManager = manager;
    }

    public async void EnterShipyard()
    {
        if (shipBuilderRoom == null)
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
      {{"token", PlayerPrefs.GetString("token")}};
            try
            {
                shipBuilderRoom = await RoomManager.client.JoinOrCreate<ShipBuilderState>("ShipBuilderRoom", options);
                shipBuilderRoom.OnMessage += OnShipBuilderMessage;

                Dictionary<string, object> data = new Dictionary<string, object>()
                {{"action", "unlocked" } };
                await shipBuilderRoom.Send(data);
            }
            catch
            {
                /** Most likely the server down **/
                RoomManager.HandleServerDown();
            }
        }
    }

    public async void OnShipBuilderMessage(object msg)
    {
        if(msg is Statistics)
        {
            Statistics stats = msg as Statistics;
            RoomManager.HandleStats(stats);
            
        } 
        else if (msg is UnlockMessage)
        {
            UnlockMessage unlocks = msg as UnlockMessage;
            PlayerData.SetUnlocks(unlocks);
        }
        else if (msg is ShipList)
        {
            ShipList sl = msg as ShipList;
            Debug.Log("Ship Count: " + sl.ships.Count);
            PlayerData.myShips = sl.ships;
            RoomManager.HandleShipListUpdated();
        }
        else if (msg is ErrorMessage)
        {
            ErrorMessage er = msg as ErrorMessage;
            RoomManager.HandleErrorMessage(er.message);
        }
        else
        {
            IndexedDictionary<string, object> message = (IndexedDictionary<string, object>)msg;

            string action = message["action"].ToString();

            if (action == "message")
            {
                RoomManager.HandleMessage((string)message["message"]);
            }

            if (action == "ship_upgrade_success")
            {
                PlayerData.ResetUpgrades();
                RoomManager.HandleUpgradeSuccess();
            }

            if (action == "enter_match_making")
            {
                await shipBuilderRoom.Leave();
                shipBuilderRoom = null;

                Dictionary<string, object> options = new Dictionary<string, object>()
            { {"token", PlayerPrefs.GetString("token")},
                {"rank", PlayerData.CurrentShip().rank }
            };
                RoomManager.HandleEnterMatchMaking(options);
            }
        }
        
    }

    public async void CallBuildShip(SB_Shipbuilder_Screen screen)
    {
        Ship shipData = screen.ShipGameObject.shipData;
        Dictionary<string, object> options = new Dictionary<string, object>()
    {
        {"action", "create" },
        {"ship", shipData }
    };
        await shipBuilderRoom.Send(options);
        RoomManager.HandleOnShipBuilt();
    }

    async public void CallGetStats()
    {
        Dictionary<string, object> options = new Dictionary<string, object>()
        { {"action", "stats" } };
        await shipBuilderRoom.Send(options);
    }

    async public void CallGetUnlocked()
    {
        Dictionary<string, object> options = new Dictionary<string, object>()
        { {"action", "unlocked" } };
        await shipBuilderRoom.Send(options);
    }

    async public void CallDestroyShip()
    {
        if (PlayerData.CurrentShipUUID == null)
        {
            return;
        }
        Dictionary<string, object> options = new Dictionary<string, object>()
    {
      {"action", "delete" },
      {"uuid", PlayerData.CurrentShipUUID }
    };
        await shipBuilderRoom.Send(options);
        PlayerData.CurrentShipUUID = null;
    }

    async public void CallUpgradeShip()
    {
        if (PlayerData.CurrentShipUUID == null)
        {
            return;
        }
        Dictionary<string, object> options = new Dictionary<string, object>()
    {
      {"action", "upgrade" },
      {"uuid", PlayerData.CurrentShipUUID },
      {"upgrades", PlayerData.UpgradePoints }
    };
        await shipBuilderRoom.Send(options);
    }

    async public void CallPlayShip()
    {
        if (PlayerData.CurrentShipUUID == null || shipBuilderRoom == null) return;
 
        Dictionary<string, object> options = new Dictionary<string, object>()
        {
          {"action", "play" },
          {"uuid", PlayerData.CurrentShipUUID }
        };
        await shipBuilderRoom.Send(options);
    }

}
