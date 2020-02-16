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

public class SB_MatchMaker
{

    [Header("Properties")]
    public GameObject m_PlayerContainer;

    private SB_RoomManager RoomManager;
    private Room<IndexedDictionary<string,object>> matchRoom;

    public SB_MatchMaker(SB_RoomManager manager)
    {
        RoomManager = manager;
    }

    public async void EnterMatchMaker(Dictionary<string, object> options)
    {
        matchRoom = await RoomManager.JoinOrCreate<IndexedDictionary<string,object>>("MatchMakerRoom", options);
        matchRoom.OnMessage += OnMatchMessage;
        RoomManager.HandleOnEnterMatchMaking();
    }

    public async void Leave()
    {
        await matchRoom.Leave();
    }

    private async void OnMatchMessage(object msg)
    {
        if (msg.GetType() == typeof(System.Byte))
        {
            Debug.Log("Number of ships in matchmaker: " + msg);
        }
        else if (msg.GetType() == typeof(IndexedDictionary<string, object>))
        {
            IndexedDictionary<string, object> message = (IndexedDictionary<string, object>)msg;
            if (message.ContainsKey("ships"))
            {
                List<Ship> ships = SB_ShipHelper.ObjectToShips((List<object>)message["ships"]);
                RoomManager.ClearMatchMakerShips();
                RoomManager.AddMatchMakerShips(ships);
            }
            else
            {
                MatchMakeResponse response = GetResponseObject(msg);
                RoomManager.HandleEnterGame(response);
                await this.matchRoom.Send(1);
                RoomManager.HandleOnMatchFound();
            }  
        } 
    }

    MatchMakeResponse GetResponseObject(object msg)
    {
        IndexedDictionary<string, object> message = (IndexedDictionary<string, object>)msg;
        IndexedDictionary<string, object> roomData = (IndexedDictionary<string, object>)message["room"];

        RoomAvailable roomAvailable = new RoomAvailable();
        roomAvailable.name = roomData["name"] as string;
        roomAvailable.processId = roomData["processId"] as string;
        roomAvailable.roomId = roomData["roomId"] as string;

        MatchMakeResponse response = new MatchMakeResponse();
        response.room = roomAvailable;
        response.sessionId = message["sessionId"] as string;

        return response;
    }



}
