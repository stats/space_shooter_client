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
    private Room<IndexedDictionary<string,object>> shipBuilderRoom;

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
                shipBuilderRoom = await RoomManager.client.JoinOrCreate<IndexedDictionary<string,object>>("ShipBuilderRoom", options);
                shipBuilderRoom.OnMessage += OnShipBuilderMessage;
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
        IndexedDictionary<string, object> message = (IndexedDictionary<string, object>)msg;
        string action = message["action"].ToString();

        if (action == "ships")
        {
            PlayerData.myShips = SB_ShipHelper.ObjectToShips((List<object>)message["ships"]);
            RoomManager.HandleShipListUpdated();
        }

        if (action == "error")
        {
            RoomManager.HandleErrorMessage((string)message["message"]);
        }

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
