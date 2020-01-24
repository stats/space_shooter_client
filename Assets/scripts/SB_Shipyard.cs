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
    private Room<Schema> shipBuilderRoom;

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

            shipBuilderRoom = await RoomManager.client.JoinOrCreate<Schema>("ShipBuilderRoom", options);
            shipBuilderRoom.OnMessage += OnShipBuilderMessage;
        }
    }

    public async void OnShipBuilderMessage(object msg)
    {
        IndexedDictionary<string, object> message = (IndexedDictionary<string, object>)msg;
        string action = message["action"].ToString();

        if (action == "ships")
        {
            PlayerData.myShips = objectToShip((List<object>)message["ships"]);
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

    private List<Ship> objectToShip(List<object> ships_as_objects)
    {
        List<Ship> ship_list = new List<Ship>();
        foreach (IDictionary<string, object> ship in ships_as_objects)
        {
            ship_list.Add(shipFromObject(ship));
        }
        return ship_list;
    }

    private Ship shipFromObject(IDictionary<string, object> ship)
    {
        Ship tmpShip = new Ship();
        System.Type shipType = typeof(Ship);
        foreach (var item in ship)
        {
            var field = shipType.GetField(item.Key);
            if (field != null && item.Value != null)
            {
                if (item.Value.GetType() == typeof(System.Double))
                {
                    field.SetValue(tmpShip, System.Convert.ToSingle(item.Value));
                }
                else if (item.Key == "position")
                {
                    Position position = new Position();
                    position.x = 0;
                    position.y = 0;
                    field.SetValue(tmpShip, position);
                }
                else
                {
                    field.SetValue(tmpShip, item.Value);
                }

            }
        }
        return tmpShip;
    }
}