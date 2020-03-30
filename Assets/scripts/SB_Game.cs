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

public class SB_Game
{
    private SB_RoomManager RoomManager;

    private Room<GameState> gameRoom;

    private IndexedDictionary<string, GameObject> ships = new IndexedDictionary<string, GameObject>();
    private IndexedDictionary<string, GameObject> enemies = new IndexedDictionary<string, GameObject>();
    private IndexedDictionary<string, GameObject> bullets = new IndexedDictionary<string, GameObject>();
    private IndexedDictionary<string, GameObject> drops = new IndexedDictionary<string, GameObject>();

    private IndexedDictionary<string, SB_HealthBar> EnemyHealthBars = new IndexedDictionary<string, SB_HealthBar>();

    public SB_Game(SB_RoomManager manager)
    {
        RoomManager = manager;
    }

    public void LeaveGame()
    {
        BattleLost();
    }

    public async Task<bool> HandleInput(Dictionary<string, object> options)
    {
        if (gameRoom == null) return false;

        await gameRoom.Send(options);
        return true;
    }

    public async void HandleEnterGame(MatchMakeResponse response)
    {

        if (response == null)
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
            {
                {"token", PlayerPrefs.GetString("token")},
                {"waveRank", PlayerData.CurrentShip().rank - 5 }
            };
            gameRoom = await RoomManager.JoinOrCreate<GameState>("GameRoom", options);
            SetupGameRoomListeners();
        }
        else
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            gameRoom = await RoomManager.ConsumeSeatReservation<GameState>(response, headers);
            SetupGameRoomListeners();
        }

        gameRoom.OnLeave += async (code) =>
        {
            Debug.Log("[GameRoom] On Leave: " + code + " : Attempting Reconnect");
            try
            {
                gameRoom = await RoomManager.ReconnectToGame<GameState>(gameRoom.Id, gameRoom.SessionId);
                SetupGameRoomListeners();
                Debug.Log("[GameRoom] Reconnection Success: " + gameRoom.SessionId);
            }
            catch
            {
                Debug.Log("[GameRoom] On Leave: Could not reconnnect");
            }
        };

    }

    private void SetupGameRoomListeners()
    { 
        gameRoom.OnError += (message) =>
        {
            Debug.Log("[GameRoom] Error: " + message);
        };

        gameRoom.OnMessage += OnGameMessage;
        gameRoom.State.OnChange += (changes) =>
        {
            changes.ForEach((obj) =>
        {
            if (obj.Field == "currentWave" || obj.Field == "enemiesSpawned" || obj.Field == "enemiesKilled")
            {
                RoomManager.SetWaveText("Wave: " + gameRoom.State.currentWave + " Enemies Killed: " + gameRoom.State.enemiesKilled + "/" + gameRoom.State.enemiesSpawned);
            }
        });
        };
        gameRoom.State.ships.OnAdd += OnShipAdd;
        gameRoom.State.ships.OnRemove += OnShipRemove;

        gameRoom.State.enemies.OnAdd += OnEnemyAdd;
        gameRoom.State.enemies.OnRemove += OnEnemyRemove;

        gameRoom.State.bullets.OnAdd += OnBulletAdd;
        gameRoom.State.bullets.OnRemove += OnBulletRemove;

        gameRoom.State.drops.OnAdd += OnDropAdd;
        gameRoom.State.drops.OnRemove += OnDropRemove;
    }

    void OnGameMessage(object msg)
    {
        if (msg.ToString() == "The Battle Has Been Lost")
        {
            BattleLost();
        }
        else if (msg.ToString().StartsWith("Battle Starts In"))
        {
            RoomManager.ShowMessage(msg.ToString(), 1.0f);
        }
        else
        {
            if(msg.ToString().StartsWith("Boss Incoming"))
            {
                Debug.Log("Announcing Boss");
                RoomManager.AnnounceBoss();
            }
            else
            {
                RoomManager.ShowMessage(msg.ToString(), 1.5f);
            }
        }
    }

    public async void BattleLost()
    {
        RoomManager.ShowMessage("The Battle Has Been Lost!", 3);
        await gameRoom.Leave();
        clearGameObjects();
        await Task.Delay(System.TimeSpan.FromSeconds(3));
        foreach (Transform child in RoomManager.m_Game_GRP.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        RoomManager.HandleOnBattleLost();
    }

    private void clearGameObjects()
    {
        List<string> shipDelete = new List<string>();
        foreach (KeyValuePair<string, GameObject> kvp in ships)
        {
            shipDelete.Add(kvp.Key);
        }
        foreach (string key in shipDelete)
        {
            RemoveShip(key);
        }

        List<string> enemyDelete = new List<string>();
        foreach (KeyValuePair<string, GameObject> kvp in enemies)
        {
            enemyDelete.Add(kvp.Key);
        }
        foreach (string key in enemyDelete)
        {
            RemoveEnemy(key);
        }

        ships.Clear();
        enemies.Clear();
        bullets.Clear();
        drops.Clear();
    }

    void OnShipAdd(Ship ship, string key)
    {
        if (ships.ContainsKey(ship.uuid))
        {
            Debug.Log("Scene already contains " + ship.uuid);
            return;
        }

        GameObject ship_gameobject = Object.Instantiate(Resources.Load<GameObject>("Ship"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        ShipGameObject sgo = ship_gameobject.GetComponent<ShipGameObject>();
        sgo.shipData = ship;
        sgo.UpdateComponents();
        ship_gameobject.transform.eulerAngles = new Vector3(
          ship_gameobject.transform.eulerAngles.x + 180,
          ship_gameobject.transform.eulerAngles.y - 90,
          ship_gameobject.transform.eulerAngles.z + 90
        );
        ship_gameobject.transform.position = new Vector3(ship.position.x, ship.position.y, 0);
        ship_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);

        ships.Add(ship.uuid, ship_gameobject);

        /** UI HUD Stuff **/
        GameObject hudGameObject = Object.Instantiate(Resources.Load<GameObject>("PlayerHUD"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        SB_PlayerHUD hud = hudGameObject.GetComponent<SB_PlayerHUD>();
        hud.SetShipName(ship.name);
        hud.SetPrimary((float)ship.primaryCooldown / (float)ship.primaryCooldownMax);
        hud.SetSpecial((float)ship.specialCooldown / (float)ship.specialCooldownMax);
        hud.SetShield(ship.shield, ship.maxShield);
        hud.SetShieldRecharge((float)ship.shieldRechargeTime / (float)ship.shieldRechargeCooldown);
        hud.SetExperience((int)(ship.kills - ship.previousLevel), (int)(ship.nextLevel - ship.previousLevel));

        RoomManager.AddPlayerHUD(ship.uuid, hud);

        ship.OnChange += (changes) =>
        {
            changes.ForEach((obj) =>
            {
                GameObject ship_go;
                if (ships.TryGetValue(ship.uuid, out ship_go))
                {
                    if (obj.Field == "position")
                    {
                        Vector3 next_position = ship_go.transform.position;
                        Position pos = (Position)obj.Value;
                        next_position.x = pos.x;
                        next_position.y = pos.y;
                        ship_go.transform.position = next_position;
                    }
                    if (obj.Field == "primaryCooldown")
                    {
                        RoomManager.UpdatePlayerHUDPrimary(ship.uuid, (float)ship.primaryCooldown / (float)ship.primaryCooldownMax);
                    }
                    if (obj.Field == "specialCooldown")
                    {
                        RoomManager.UpdatePlayerHUDSpecial(ship.uuid, (float)ship.specialCooldown / (float)ship.specialCooldownMax);
                    }
                    if (obj.Field == "shield" || obj.Field == "shieldMax")
                    {
                        RoomManager.UpdatePlayerHUDShield(ship.uuid, ship.shield, ship.maxShield);
                    }
                    if (obj.Field == "shieldRechargeTime" || obj.Field == "shieldRechargeCooldown")
                    {

                        hud.SetShieldRecharge((float)ship.shieldRechargeTime / (float)ship.shieldRechargeCooldown);
                    }
                    if (obj.Field == "kills")
                    {
                        RoomManager.UpdatePlayerHUDExperience(ship.uuid, (int)(ship.kills - ship.previousLevel), (int)(ship.nextLevel - ship.previousLevel));
                    }
                    if (obj.Field == "level")
                    {
                        RoomManager.ShowMessage(ship.name + " is now level " + ship.level + ". " + (int)(ship.nextLevel - ship.previousLevel) + " kills to next level.", 3);
                    }
                    if (obj.Field == "bulletInvulnerable" || obj.Field == "collisionInvulnerable")
                    {
                        if (ship.bulletInvulnerable == true && ship.collisionInvulnerable == true)
                        {
                            ship_go.GetComponent<ShipGameObject>().ActivateForceField();
                        } 
                        else if (ship.collisionInvulnerable == true)
                        {
                            ship_go.GetComponent<ShipGameObject>().ActivateRammingShield();
                        }
                        else
                        {
                            ship_go.GetComponent<ShipGameObject>().DeactivateShield();
                        }
                    }
                    if(obj.Field == "invisible")
                    {
                        ship_go.GetComponent<ShipGameObject>().SetInvisibility(ship.invisible);
                    }
                }
                else
                {
                    Debug.Log("Could not get the ship");
                }
            });
        };
    }

    void OnShipRemove(Ship ship, string key)
    {
        RemoveShip(ship.uuid);
    }

    void RemoveShip(string uuid)
    {
        GameObject ship_go;
        if (ships.TryGetValue(uuid, out ship_go))
        {
            GameObject explosion_gameobject = Object.Instantiate(Resources.Load<GameObject>("explosions/ElectricExplosion"), ship_go.transform.position, Quaternion.identity) as GameObject;
            explosion_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);
            Object.Destroy(ship_go);
            ships.Remove(uuid);
            RoomManager.RemovePlayerHUD(uuid);
        }
    }

    void OnEnemyAdd(Enemy enemy, string key)
    {
        if (enemies.ContainsKey(enemy.uuid))
        {
            Debug.Log("Enemy already added " + enemy.uuid);
            return;
        }

        var angle = Quaternion.identity.eulerAngles;
        angle.z = (float)enemy.angle * 180 / Mathf.PI;
        GameObject enemy_gameobject = Object.Instantiate(Resources.Load<GameObject>("enemies/" + enemy.modelType), new Vector3(enemy.position.x, enemy.position.y, 0), Quaternion.Euler(angle)) as GameObject;
        enemy_gameobject.name = "Enemy" + enemy.modelType;
        enemy_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);
        enemies.Add(enemy.uuid, enemy_gameobject);

        SetEnemyShield(enemy, enemy_gameobject);

        AddEnemyHealthBar(enemy.uuid, (int)enemy.health, enemy_gameobject.transform.position);

        enemy.OnChange += (changes) =>
        {
            changes.ForEach((obj) =>
            {
                GameObject enemy_go;
                if (enemies.TryGetValue(enemy.uuid, out enemy_go))
                {
                    if (obj.Field == "position")
                    {
                        //Debug.Log("Changing X " + obj.Value);
                        Vector3 next_position = enemy_go.transform.position;
                        Position pos = (Position)obj.Value;
                        next_position.x = pos.x;
                        next_position.y = pos.y;
                        next_position.z = 0;
                        enemy_go.transform.position = next_position;
                        MoveEnemyHealthBar(enemy.uuid, next_position);
                    }
                    if (obj.Field == "angle")
                    {
                        angle = enemy_go.transform.rotation.eulerAngles;
                        angle.z = (float)obj.Value * 180 / Mathf.PI;
                        enemy_go.transform.rotation = Quaternion.Euler(angle);
                    }
                    if(obj.Field == "health")
                    {
                        SetHealthBarValue(enemy.uuid, Mathf.RoundToInt((float)obj.Value));
                    }
                    if (obj.Field == "bulletInvulnerable" || obj.Field == "collisionInvulnerable")
                    {
                        SetEnemyShield(enemy, enemy_go);
                    }
                }
                else
                {
                    Debug.LogError("[SB_RoomManager] Could not get enemy game object: " + key);
                }
            });
        };
    }

    void SetEnemyShield(Enemy enemy, GameObject enemy_go)
    {
        try { 
            if (enemy.bulletInvulnerable == true && enemy.collisionInvulnerable == true)
            {
                enemy_go.transform.Find("ForceField").gameObject.SetActive(true);
                enemy_go.transform.Find("RammingShield").gameObject.SetActive(false);
            }
            else if (enemy.collisionInvulnerable == true)
            {
                enemy_go.transform.Find("ForceField").gameObject.SetActive(false);
                enemy_go.transform.Find("RammingShield").gameObject.SetActive(true);
            }
            else
            {
                enemy_go.transform.Find("ForceField").gameObject.SetActive(false);
                enemy_go.transform.Find("RammingShield").gameObject.SetActive(false);
            }
        } 
        catch
        {
            //Do nothing
        }
    }

    void OnEnemyRemove(Enemy enemy, string key)
    {
        RemoveEnemyHealthBar(enemy.uuid);
        RemoveEnemy(enemy.uuid);
    }

    void RemoveEnemy(string uuid)
    {
        GameObject enemy_go;
        if (enemies.TryGetValue(uuid, out enemy_go))
        {
            int r = Random.Range(1, 3);
            GameObject explosion_gameobject = Object.Instantiate(Resources.Load<GameObject>("explosions/EnemyExplosion_" + r), enemy_go.transform.position, Quaternion.identity) as GameObject;
            explosion_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);
            Object.Destroy(enemy_go);
            enemies.Remove(uuid);
        }
    }

    void AddEnemyHealthBar(string uuid, int hp, Vector3 pos)
    {
        if(EnemyHealthBars.ContainsKey(uuid) == false)
        {
            pos.z = 0;
            SB_HealthBar health = Object.Instantiate(Resources.Load<SB_HealthBar>("HealthBar"), pos, Quaternion.identity) as SB_HealthBar;
            health.gameObject.transform.SetParent(RoomManager.m_WorldSpaceHUD.transform);
            RectTransform rect = health.gameObject.transform.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(70, 9);
            health.SetMaxHealth(hp);
            EnemyHealthBars.Add(uuid, health);
        }
    }

    void RemoveEnemyHealthBar(string uuid)
    {
        if(EnemyHealthBars.ContainsKey(uuid) )
        {
            Object.Destroy(EnemyHealthBars[uuid].gameObject);
            EnemyHealthBars.Remove(uuid);
        }
    }

    void MoveEnemyHealthBar(string uuid, Vector3 pos)
    {
        if(EnemyHealthBars.ContainsKey(uuid))
        {
            SB_HealthBar health = EnemyHealthBars[uuid];
            pos = pos + (Vector3.up * 30.0f);
            pos.z = -200;
            health.gameObject.transform.position = pos;
        }
    }

    void SetHealthBarValue(string uuid, int hp)
    {
        if(EnemyHealthBars.ContainsKey(uuid))
        {
            EnemyHealthBars[uuid].SetHealth(hp);
        }
    }

    void OnBulletAdd(Bullet bullet, string key)
    {

        if (bullets.ContainsKey(bullet.uuid)) return;

        GameObject bullet_gameobject = Object.Instantiate(Resources.Load<GameObject>("bullets/" + bullet.bulletMesh), new Vector3(bullet.position.x, bullet.position.y, 0), Quaternion.identity) as GameObject;
        //TODO: Set the material for the bullet
        bullet_gameobject.name = "Bullet" + bullet.bulletMesh;
        bullet_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);
        bullets.Add(bullet.uuid, bullet_gameobject);

        bullet.OnChange += (changes) =>
        {
            changes.ForEach((obj) =>
        {
            GameObject bullet_go;
            if (bullets.TryGetValue(bullet.uuid, out bullet_go))
            {
                if (obj.Field == "position")
                {
                    Vector3 next_position = bullet_go.transform.position;
                    Position pos = (Position)obj.Value;
                    next_position.x = pos.x;
                    next_position.y = pos.y;
                    bullet_go.transform.rotation = Quaternion.FromToRotation(Vector3.up, next_position - bullet_go.transform.position);
                    bullet_go.transform.position = next_position;
                }
            }
        });
        };
    }

    void OnBulletRemove(Bullet bullet, string key)
    {
        GameObject bullet_go;
        if (bullets.TryGetValue(bullet.uuid, out bullet_go))
        {
            Object.Destroy(bullet_go);
            GameObject explosion_gameobject;
            if (bullet.blastRadius != 0)
            {
                explosion_gameobject = SB_Explosion.GetExplosion(bullet.blastRadius/15, bullet_go.transform.position);
            }
            else
            {
                explosion_gameobject = SB_Explosion.GetExplosion(1, bullet_go.transform.position);
            }
            explosion_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);
            bullets.Remove(bullet.uuid);
        }
    }

    void OnDropAdd(Drop drop, string key)
    {

        if (drops.ContainsKey(drop.uuid)) return;

        GameObject drop_gameobject = Object.Instantiate(Resources.Load<GameObject>("drops/" + drop.modelType), new Vector3(drop.position.x, drop.position.y, 0), Quaternion.identity) as GameObject;
        drop_gameobject.name = "Drop" + drop.modelType;
        drop_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);
        drops.Add(drop.uuid, drop_gameobject);

        drop.OnChange += (changes) =>
        {
            changes.ForEach((obj) =>
            {
                GameObject drop_go;
                if (drops.TryGetValue(drop.uuid, out drop_go))
                {
                    if (obj.Field == "position")
                    {
                        Vector3 next_position = drop_go.transform.position;
                        Position pos = (Position)obj.Value;
                        next_position.x = pos.x;
                        next_position.y = pos.y;
                        drop_go.transform.position = next_position;
                    }
                }
            });
        };
    }

    void OnDropRemove(Drop drop, string key)
    {
        GameObject drop_go;
        if (drops.TryGetValue(drop.uuid, out drop_go))
        {
            Object.Destroy(drop_go);
            drops.Remove(drop.uuid);
        }
    }

}
