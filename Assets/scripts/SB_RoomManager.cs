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
    
    [Header("Server Settings")]
    public bool m_Production = false;

    [Header("Main Properties")]
    public ZHG_UI_System m_UI;
    public GameObject m_Game_GRP;

    [Header("Match Maker Properties")]
    public GameObject m_MatchMakerShipContainer;

    [Header("Game Room Properties")]
    public GameObject m_GameMenu;

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
    public UnityEvent onServerDown = new UnityEvent();

    public Client client;

    private SB_Shipyard shipyard;
    private SB_MatchMaker matchmaker;
    private SB_Game game;

    private string WS_ENDPOINT = "ws://localhost:2567";
    private string HTTP_ENDPOINT = "http://localhost:2567";

    void Awake()
    {
        shipyard = new SB_Shipyard(this);
        matchmaker = new SB_MatchMaker(this);
        game = new SB_Game(this);

        if (m_Production == true)
        {
            WS_ENDPOINT = "wss://cindertron7.com";
            HTTP_ENDPOINT = "https://cindertron7.com";
        }
    }

    async void Update()
    {
        bool input_sent = await HandleGameInput();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void EnterShipYard()
    {
        if (shipyard == null) return; 
        shipyard.EnterShipyard();
    }

    public void CallUpgradeShip()
    {
        if (shipyard == null) return;
        shipyard.CallUpgradeShip();
    }

    public void CallPlayShip()
    {
        if (shipyard == null) return;
        shipyard.CallPlayShip();
    }

    public void CallLeaveGame()
    {
        game.BattleLost();
    }

    public void CallDestroyShip()
    {
        shipyard.CallDestroyShip();
    }

    public void CallBuildShip(SB_Shipbuilder_Screen screen)
    {
        shipyard.CallBuildShip(screen);
    }

    public async Task<Room<T>> JoinOrCreate<T>(string name, Dictionary<string, object> options)
    {
        return await client.JoinOrCreate<T>(name, options);
    }

    public async Task<Room<T>> ConsumeSeatReservation<T>(MatchMakeResponse response, Dictionary<string, string> headers)
    {
        return await client.ConsumeSeatReservation<T>(response, headers);
    }

    public void HandleServerDown()
    {
        if (onServerDown == null) return;
        onServerDown.Invoke();
    }

    public void HandleShipListUpdated()
    {
        if (onUpdatedShipList == null) return;
        onUpdatedShipList.Invoke();
    }

    public void HandleErrorMessage(string message)
    {
        Debug.LogError("Error: " + message);
    }

    public void HandleMessage(string message)
    {
        ShowMessage(message, 3);
    }

    public void HandleUpgradeSuccess()
    {
        ShowMessage("Ship Upgrade Successfull", 3);
    }

    public void HandleEnterMatchMaking(Dictionary<string, object> options)
    {
        matchmaker.EnterMatchMaker(options);
    }

    public void HandleSkipMatchMaking()
    {
        matchmaker.Leave();
        game.HandleEnterGame(null);
        HandleOnMatchFound();
    }

    public void HandleEnterGame(MatchMakeResponse response)
    {
        game.HandleEnterGame(response);
    }

    public void HandleOnShipBuilt()
    {
        if (onShipBuilt == null) return;
        onShipBuilt.Invoke();
    }

    public void HandleOnEnterMatchMaking()
    {
        if (onEnterMatchMaking == null) return;
        onEnterMatchMaking.Invoke();
    }

    public void HandleOnMatchFound()
    {
        if (onMatchFound == null) return;
        onMatchFound.Invoke();
    }

    public void HandleOnBattleLost()
    {
        if (onBattleLost == null) return;
        onBattleLost.Invoke();
    }

    public void SetWaveText(string text)
    {
        m_UI.m_HUD.SetWaveText(text);
    }

    public void ShowMessage(string message, float duration)
    {
        m_UI.ShowMessage(message, duration);
    }

    public void AddPlayerHUD(string uuid, SB_PlayerHUD hud)
    {
        m_UI.m_HUD.AddPlayerHUD(uuid, hud);
    }

    public void UpdatePlayerHUDPrimary(string uuid, float primary)
    {
        m_UI.m_HUD.GetPlayerHUD(uuid).SetPrimary(primary);
    }

    public void UpdatePlayerHUDSpecial(string uuid, float special)
    {
        m_UI.m_HUD.GetPlayerHUD(uuid).SetSpecial(special);
    }

    public void UpdatePlayerHUDShields(string uuid, int shields, int shieldsMax)
    {
        m_UI.m_HUD.GetPlayerHUD(uuid).SetShields(shields, shieldsMax);
    }

    public void UpdatePlayerHUDExperience(string uuid, int experience, int nextLevel)
    {
        m_UI.m_HUD.GetPlayerHUD(uuid).SetExperience(experience, nextLevel);
    }

    public void RemovePlayerHUD(string uuid)
    {
        m_UI.m_HUD.RemovePlayerHUD(uuid);
    }

    public void ToggleGameMenu()
    {
        m_GameMenu.SetActive(!m_GameMenu.activeSelf);
    }

    private async Task<bool> HandleGameInput()
    {
        if (game == null) return false;

        Dictionary<string, object> options = new Dictionary<string, object>()
      {{"action", "input"}};

        Dictionary<string, object> input = new Dictionary<string, object>();

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Cancel"))
        {
            ToggleGameMenu();
        }

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
                await game.HandleInput(options);
                return true;
            }
            catch
            {
                Debug.LogWarning("GameRoom has been disposed. Stop trying to access it");
            }
        }
        return false;
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

    public void ConnectToServer()
    {
        if (client == null)
        {
            client = ColyseusManager.Instance.CreateClient(WS_ENDPOINT);
        }
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

    public void AddMatchMakerShips(List<Ship> ships)
    {
        foreach (Ship ship in ships)
        {
            GameObject matchShip = Instantiate(Resources.Load<GameObject>("MatchShip"), new Vector3(0, 0, 0), Quaternion.identity);
            matchShip.GetComponent<SB_MatchShip>().SetShip(ship);
            matchShip.transform.SetParent(m_MatchMakerShipContainer.transform);
        }
    }

    public void ClearMatchMakerShips()
    {
        foreach (Transform child in m_MatchMakerShipContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

}
