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

public class SB_RoomManager : MonoBehaviour
{

    const string WS_ENDPOINT = "ws://localhost:2567";
    const string HTTP_ENDPOINT = "http://localhost:2567";

    [Header("Main Properties")]
    public ZHG_UI_System m_UI;
    public GameObject m_Game_GRP;

    [Header("Room Events")]
    public UnityEvent onAuthenticated = new UnityEvent();
    public UnityEvent onNotAuthenticated = new UnityEvent();
    public UnityEvent onLoginSuccess = new UnityEvent();
    public UnityEvent onPlayerSettingsUpdated = new UnityEvent();
    public UnityEvent onUpdatedShipList = new UnityEvent();
    public UnityEvent onEnterMatchMaking = new UnityEvent();
    public UnityEvent onShipBuilt = new UnityEvent();
    public UnityEvent onMatchFound = new UnityEvent();
    public UnityEvent onLoginFailure = new UnityEvent();
    public UnityEvent onBattleLost = new UnityEvent();

    private Client client;
    private Room<Schema> shipBuilderRoom;
    private Room<Schema> matchRoom;
    private Room<GameState> gameRoom;

    private IndexedDictionary<Ship, GameObject> ships = new IndexedDictionary<Ship, GameObject>();
    private IndexedDictionary<Enemy, GameObject> enemies = new IndexedDictionary<Enemy, GameObject>();
    private IndexedDictionary<Bullet, GameObject> bullets = new IndexedDictionary<Bullet, GameObject>();

    //private bool PlayingMessage = false;
    //private Queue<QueuedMessage> MessageQueue = new Queue<QueuedMessage>();

    async void Update()
    {
        if (gameRoom != null)
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
                { {"action", "input" } };

            Dictionary<string, object> input = new Dictionary<string, object>();

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool sendData = false;
            if (horizontal != 0)
            {
                input.Add("horizontal", horizontal);
                sendData = true;
            }
            if (vertical != 0)
            {
                input.Add("vertical", vertical);
                sendData = true;
            }
            if (Input.GetButton("Fire1"))
            {
                input.Add("primary_attack", 1);
                sendData = true;
            }
            if (Input.GetButton("Fire2"))
            {
                input.Add("special_attack", 1);
                sendData = true;
            }

            if (sendData)
            {
                options.Add("input", input);
                try
                {
                    await gameRoom.Send(options);
                } catch
                {
                    Debug.LogWarning("GameRoom has been disposed. Stop Trying to access it.");

                }
            }
        }
    }

    public void onCheckAuthentication()
    {
        if (IsAuthenticated())
        {
            ConnectToServer();
            Invoke("CheckForRenew", getRenewTimeSeconds());
            if (onAuthenticated != null)
            {
                onAuthenticated.Invoke();
            }
        }
        else
        {
            if (onNotAuthenticated != null)
            {
                onNotAuthenticated.Invoke();
            }
        }
    }

    #region Helper Functions
    public void ConnectToServer()
    {
        if (client == null)
        {
            client = ColyseusManager.Instance.CreateClient(WS_ENDPOINT);
        }
    }

    public async void EnterShipyard()
    {
        if (shipBuilderRoom == null)
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
                { {"token", PlayerPrefs.GetString("token")} };

            shipBuilderRoom = await client.JoinOrCreate<Schema>("ShipBuilderRoom", options);
            shipBuilderRoom.OnMessage += OnShipBuilderMessage;
        }
    }

    private async void OnShipBuilderMessage(object msg)
    {
        IndexedDictionary<string, object> message = (IndexedDictionary<string, object>)msg;
        string action = message["action"].ToString();

        if (action == "ships")
        {
            /*List<Ship> ships = new List<Ship>();
            foreach(var ship in (List<object>)message["ships"])
            {
                ships.Add((Ship)ship);
            }
            PlayerData.myShips = ships;*/

            PlayerData.myShips = objectToShip((List<object>)message["ships"]);
            if (onUpdatedShipList != null)
            {
                onUpdatedShipList.Invoke();
            }
        }
        if (action == "error")
        {
            //TODO: Handle this weird error
            Debug.Log("Error: " + (string)message["message"]);
        }
        if (action == "message")
        {
            Debug.Log("Messag: " + (string)message["message"]);
        }
        if (action == "ship_upgrade_success")
        {
            Debug.Log("Upgrade Success");
            PlayerData.ResetUpgrades();
        }
        if (action == "enter_match_making")
        {
            await shipBuilderRoom.Leave();
            shipBuilderRoom = null;

            Dictionary<string, object> options = new Dictionary<string, object>()
            { {"token", PlayerPrefs.GetString("token")},
              {"rank", PlayerData.CurrentShip().rank }
            };

            matchRoom = await client.JoinOrCreate<Schema>("MatchMakerRoom", options);
            matchRoom.OnMessage += OnMatchMessage;

            if (onEnterMatchMaking != null)
            {
                onEnterMatchMaking.Invoke();
            }
        }
    }

    private async void OnMatchMessage(object msg)
    {
        if (msg.GetType() == typeof(System.Byte))
        {
            
        }
        else if (msg.GetType() == typeof(IndexedDictionary<string, object>))
        {
            MatchMakeResponse response = GetResponseObject(msg);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            gameRoom = await client.ConsumeSeatReservation<GameState>(response, headers);

            gameRoom.OnLeave += (code) =>
            {
                Debug.Log("[GameRoom] Client left: " + code);
            };

            gameRoom.OnError += (message) =>
            {
                Debug.Log("[GameRoom] Error: " + message);
            };

            gameRoom.OnMessage += OnGameMessage;
            gameRoom.State.OnChange += (changes) =>
            {
                changes.ForEach((obj) =>
                {
                    if (obj.Field == "current_wave")
                    {
                        PlayerData.currentWave = (int)obj.Value;
                    }
                    else if (obj.Field == "enemies_spawned")
                    {
                        PlayerData.enemiesSpawned = (int)obj.Value;
                    }
                    else if (obj.Field == "enemies_killed")
                    {
                        PlayerData.enemiesKilled = (int)obj.Value;
                    }
                });
            };
            gameRoom.State.ships.OnAdd += OnShipAdd;
            gameRoom.State.ships.OnRemove += OnShipRemove;

            gameRoom.State.enemies.OnAdd += OnEnemyAdd;
            gameRoom.State.enemies.OnRemove += OnEnemyRemove;

            gameRoom.State.bullets.OnAdd += OnBulletAdd;
            gameRoom.State.bullets.OnRemove += OnBulletRemove;

            await this.matchRoom.Send(1);
            if(onMatchFound != null)
            {
                onMatchFound.Invoke();
            }
        }
    }

    void OnGameMessage(object msg)
    {
        if (msg.ToString() == "The Battle Has Been Lost")
        {
            BattleLost();
        }
        else if (msg.ToString().StartsWith("Battle Starts In"))
        {
            m_UI.ShowMessage(msg.ToString(), 1.0f);
        }
        else
        {
            m_UI.ShowMessage(msg.ToString(), 1.5f);
        }
    }

    async void BattleLost()
    {
        m_UI.ShowMessage("The Battle Has Been Lost!", 3);
        clearGameObjects();
        await Task.Delay(System.TimeSpan.FromSeconds(3));
        foreach (Transform child in m_Game_GRP.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        await gameRoom.Leave();
        if(onBattleLost != null)
        {
            onBattleLost.Invoke();
        }
    }

    private void clearGameObjects()
    {
        List<Ship> shipDelete = new List<Ship>();
        foreach (KeyValuePair<Ship, GameObject> kvp in ships)
        {
            shipDelete.Add(kvp.Key);
        }
        foreach (Ship key in shipDelete)
        {
            OnShipRemove(key, key.uuid);
        }

        List<Enemy> enemyDelete = new List<Enemy>();
        foreach (KeyValuePair<Enemy, GameObject> kvp in enemies)
        {
            enemyDelete.Add(kvp.Key);
        }
        foreach (Enemy key in enemyDelete)
        {
            OnEnemyRemove(key, key.uuid);
        }

        ships.Clear();
        enemies.Clear();
        bullets.Clear();
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
                else if( item.Key == "position")
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

    private bool IsAuthenticated()
    {
        if (!PlayerPrefs.HasKey("expiresIn")) return false;
        return System.Convert.ToInt64(PlayerPrefs.GetString("expiresIn")) > System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    private void SetResponse(string jsonString)
    {
        LoginResponse loginResponse = LoginResponse.CreateFromJSON(jsonString);
        PlayerPrefs.SetString("token", loginResponse.token);
        PlayerPrefs.SetString("expiresIn", loginResponse.expiresIn.ToString());
        PlayerPrefs.SetString("username", loginResponse.username);
        PlayerPrefs.Save();
        if (onPlayerSettingsUpdated != null)
        {
            onPlayerSettingsUpdated.Invoke();
        }

    }

    private long getRenewTimeSeconds()
    {
        if (!PlayerPrefs.HasKey("expiresIn") || !PlayerPrefs.HasKey("token")) return 0;
        long renew_time = ((System.Convert.ToInt64(PlayerPrefs.GetString("expiresIn")) - System.DateTimeOffset.Now.ToUnixTimeMilliseconds()) / 1000) - 600;
        Debug.Log("Renew In " + renew_time);
        return renew_time;
    }

    public void CheckForRenew()
    {
        StartCoroutine(onRenew());
    }
    #endregion

    #region Game Room Events
    void OnShipAdd(Ship ship, string key)
    {
        Debug.Log("Created Player");
        GameObject ship_gameobject = Instantiate(Resources.Load<GameObject>("Ship"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        ShipGameObject sgo = ship_gameobject.GetComponent<ShipGameObject>();
        sgo.shipData = ship;
        sgo.UpdateComponents();
        ship_gameobject.transform.eulerAngles = new Vector3(
            ship_gameobject.transform.eulerAngles.x+180,
            ship_gameobject.transform.eulerAngles.y - 90,
            ship_gameobject.transform.eulerAngles.z + 90
        );
        ship_gameobject.transform.position = new Vector3(ship.position.x, ship.position.y, 0);
        ship_gameobject.transform.SetParent(m_Game_GRP.transform);
        if (ship.uuid == PlayerData.CurrentShipUUID)
        {
            PlayerData.shipData = ship;
            //TODO: The UI needs to get and handle this
            //ShieldText.GetComponent<TMP_Text>().text = ship.shields.ToString() + "/" + ship.max_shields.ToString();
        }

        ships.Add(ship, ship_gameobject);

        ship.OnChange += (changes) =>
        {
            changes.ForEach((obj) =>
            {
                GameObject ship_go;
                if (ships.TryGetValue(ship, out ship_go))
                {
                    if (obj.Field == "position")
                    {
                        //Debug.Log("Changing X " + obj.Value);
                        Vector3 next_position = ship_go.transform.position;
                        Position pos = (Position)obj.Value;
                        next_position.x = pos.x;
                        next_position.y = pos.y;
                        ship_go.transform.position = next_position;
                    }
                    if (ship.uuid == PlayerData.CurrentShipUUID)
                    {
                        if (obj.Field == "primary_cooldown"
                         || obj.Field == "special_cooldown"
                         || obj.Field == "kills"
                         || obj.Field == "horizontal_accelleration"
                         || obj.Field == "vertical_accelleration"
                         || obj.Field == "shields")
                        {
                            m_UI.UpdateHUD();
                        }
                    }
                }
                else
                {
                    Debug.LogError("[SB_RoomManager] Could not get ship: " + key);
                }


            });
        };
    }

    void OnShipRemove(Ship ship, string key)
    {
        GameObject ship_go;
        if (ships.TryGetValue(ship, out ship_go))
        {
            GameObject explosion_gameobject = Instantiate(Resources.Load<GameObject>("explosions/ElectricExplosion"), ship_go.transform.position, Quaternion.identity) as GameObject;
            explosion_gameobject.transform.SetParent(m_Game_GRP.transform);
            Destroy(ship_go);
            ships.Remove(ship);
        }
    }

    void OnEnemyAdd(Enemy enemy, string key)
    {
        GameObject enemy_gameobject = Instantiate(Resources.Load<GameObject>("enemies/" + enemy.model_type), new Vector3(enemy.position.x, enemy.position.y, 0), Quaternion.identity) as GameObject;
        enemy_gameobject.name = "Enemy" + enemy.model_type;
        enemy_gameobject.transform.SetParent(m_Game_GRP.transform);
        enemies.Add(enemy, enemy_gameobject);

        enemy.OnChange += (changes) =>
        {
            changes.ForEach((obj) =>
            {
                GameObject enemy_go;
                if (enemies.TryGetValue(enemy, out enemy_go))
                {
                    if (obj.Field == "position")
                    {
                        //Debug.Log("Changing X " + obj.Value);
                        Vector3 next_position = enemy_go.transform.position;
                        Position pos = (Position)obj.Value;
                        next_position.x = pos.x;
                        next_position.y = pos.y;
                        enemy_go.transform.rotation = Quaternion.FromToRotation(Vector3.up, next_position - enemy_go.transform.position);
                        enemy_go.transform.position = next_position;
                    }
                }
                else
                {
                    Debug.LogError("[SB_RoomManager] Could not get enemy game object: " + key);
                }

            });
        };
    }

    void OnEnemyRemove(Enemy enemy, string key)
    {
        GameObject enemy_go;
        if (enemies.TryGetValue(enemy, out enemy_go))
        {
            GameObject explosion_gameobject = Instantiate(Resources.Load<GameObject>("explosions/EnemyExplosion_1"), enemy_go.transform.position, Quaternion.identity) as GameObject;
            explosion_gameobject.transform.SetParent(m_Game_GRP.transform);
            Destroy(enemy_go);
            enemies.Remove(enemy);
        }

    }

    void OnBulletAdd(Bullet bullet, string key)
    {
        GameObject bullet_gameobject = Instantiate(Resources.Load<GameObject>("bullets/" + bullet.bullet_mesh), new Vector3(bullet.position.x, bullet.position.y, 0), Quaternion.identity) as GameObject;
        //TODO: Set the material for the bullet
        bullet_gameobject.name = "Bullet" + bullet.bullet_mesh;
        bullet_gameobject.transform.SetParent(m_Game_GRP.transform);
        bullets.Add(bullet, bullet_gameobject);

        bullet.OnChange += (changes) =>
        {
            changes.ForEach((obj) =>
            {
                GameObject bullet_go;
                if (bullets.TryGetValue(bullet, out bullet_go))
                {
                    if (obj.Field == "position")
                    {
                        Vector3 next_position = bullet_go.transform.position;
                        Position pos = (Position)obj.Value;
                        next_position.x = pos.x;
                        next_position.y = pos.y;
                        bullet_go.transform.position = next_position;
                    }
                }
                else
                {
                    Debug.LogError("[SB_RoomManager] Could not get bullet: " + key);
                }

            });
        };
    }

    void OnBulletRemove(Bullet bullet, string key)
    {
        GameObject bullet_go;
        if (bullets.TryGetValue(bullet, out bullet_go))
        {
            Destroy(bullet_go);
            GameObject explosion_gameobject = Instantiate(Resources.Load<GameObject>("explosions/BulletExplosion_1"), bullet_go.transform.position, Quaternion.identity) as GameObject;
            explosion_gameobject.transform.SetParent(m_Game_GRP.transform);
            bullets.Remove(bullet);
        }
    }
    #endregion

    #region Events

    public async void CallBuildShip(SB_Shipbuilder_Screen screen)
    {
        Ship shipData = screen.ShipGameObject.shipData;
        Dictionary<string, object> options = new Dictionary<string, object>()
        {
            {"action", "create" },
            {"ship", shipData }
        };
        await shipBuilderRoom.Send(options);
        if(onShipBuilt != null)
        {
            onShipBuilt.Invoke();
        }
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
        if (PlayerData.CurrentShipUUID == null)
        {
            return;
        }
        Dictionary<string, object> options = new Dictionary<string, object>()
        {
            {"action", "play" },
            {"uuid", PlayerData.CurrentShipUUID }
        };
        await shipBuilderRoom.Send(options);
    }

    public void CallCreateAccount(string username, string email, string password)
    {
        StartCoroutine(onCreateAccount(username, email, password));
    }

    public void CallQuickLogin()
    {
        StartCoroutine(onQuickLogin());
    }

    public void CallLogin(string email, string password)
    {
        StartCoroutine(onLogin(email, password));
    }

    #endregion

    #region Web Requests
    IEnumerator onRenew()
    {
        WWWForm form = new WWWForm();
        form.AddField("token", PlayerPrefs.GetString("token"));

        using (var w = UnityWebRequest.Post(HTTP_ENDPOINT + "/renew", form))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError("Renew Failure");
            }
            else
            {
                //Debug.Log("Got Renew Response");
                SetResponse(w.downloadHandler.text);
                Invoke("CheckForRenew", getRenewTimeSeconds());
            }
        }
    }

    IEnumerator onQuickLogin()
    {
        WWWForm form = new WWWForm();
        form.AddField("system_id", SystemInfo.deviceUniqueIdentifier);

        using (var w = UnityWebRequest.Post(HTTP_ENDPOINT + "/quick_login", form))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError("Quick Login Failure");
                if (onLoginFailure != null)
                {
                    onLoginFailure.Invoke();
                }
            }
            else
            {
                SetResponse(w.downloadHandler.text);
                ConnectToServer();
                Invoke("CheckForRenew", getRenewTimeSeconds());
                if (onLoginSuccess != null)
                {
                    onLoginSuccess.Invoke();
                }

            }
        }
    }

    IEnumerator onLogin(string email, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (var w = UnityWebRequest.Post(HTTP_ENDPOINT + "/login", form))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError("Login Failure");
                if(onLoginFailure != null)
                {
                    onLoginFailure.Invoke();
                }
            }
            else
            {
                SetResponse(w.downloadHandler.text);
                ConnectToServer();
                Invoke("CheckForRenew", getRenewTimeSeconds());
                if (onLoginSuccess != null)
                {
                    onLoginSuccess.Invoke();
                }
            }
        }
    }

    IEnumerator onCreateAccount(string email, string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("username", username);
        form.AddField("password", password);

        using (var w = UnityWebRequest.Post(HTTP_ENDPOINT + "/signup", form))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError("[SB_RoomManager] Create Account Failure");
            }
            else
            {
                SetResponse(w.downloadHandler.text);
                ConnectToServer();
                Invoke("CheckForRenew", getRenewTimeSeconds());
                if (onLoginSuccess != null)
                {
                    onLoginSuccess.Invoke();
                }
            }
        }
    }
    #endregion

}
