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

    private IndexedDictionary<Ship, GameObject> ships = new IndexedDictionary<Ship, GameObject>();
    private IndexedDictionary<Enemy, GameObject> enemies = new IndexedDictionary<Enemy, GameObject>();
    private IndexedDictionary<Bullet, GameObject> bullets = new IndexedDictionary<Bullet, GameObject>();

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

        if(response == null)
        {
            Dictionary<string, object> options = new Dictionary<string, object>()
            { {"token", PlayerPrefs.GetString("token")} };
            gameRoom = await RoomManager.JoinOrCreate<GameState>("GameRoom", options);
        }
        else
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            gameRoom = await RoomManager.ConsumeSeatReservation<GameState>(response, headers);
        }

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
            if (obj.Field == "current_wave" || obj.Field == "enemies_spawned" || obj.Field == "enemies_killed")
            {
                RoomManager.SetWaveText("Wave: " + gameRoom.State.current_wave + " Enemies Killed: " + gameRoom.State.enemies_killed + "/" + gameRoom.State.enemies_spawned);
            }
        });
        };
        gameRoom.State.ships.OnAdd += OnShipAdd;
        gameRoom.State.ships.OnRemove += OnShipRemove;

        gameRoom.State.enemies.OnAdd += OnEnemyAdd;
        gameRoom.State.enemies.OnRemove += OnEnemyRemove;

        gameRoom.State.bullets.OnAdd += OnBulletAdd;
        gameRoom.State.bullets.OnRemove += OnBulletRemove;
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
            RoomManager.ShowMessage(msg.ToString(), 1.5f);
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

    void OnShipAdd(Ship ship, string key)
    {
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

        ships.Add(ship, ship_gameobject);

        /** UI HUD Stuff **/
        GameObject hudGameObject = Object.Instantiate(Resources.Load<GameObject>("PlayerHUD"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        SB_PlayerHUD hud = hudGameObject.GetComponent<SB_PlayerHUD>();
        hud.SetShipName(ship.name);
        hud.SetPrimary((float)ship.primary_cooldown / (float)ship.primary_cooldown_max);
        hud.SetSpecial((float)ship.special_cooldown / (float)ship.special_cooldown_max);
        hud.SetShields(ship.shields, ship.max_shields);
        hud.SetShieldRecharge((float)ship.shields_recharge_cooldown / (float)ship.shields_recharge_time);
        hud.SetExperience((int)(ship.kills - ship.previous_level), (int)(ship.next_level - ship.previous_level));

        RoomManager.AddPlayerHUD(ship.uuid, hud);

        ship.OnChange += (changes) =>
        {
            changes.ForEach((obj) =>
        {
            GameObject ship_go;
            if (ships.TryGetValue(ship, out ship_go))
            {
                if (obj.Field == "position")
                {
                    Vector3 next_position = ship_go.transform.position;
                    Position pos = (Position)obj.Value;
                    next_position.x = pos.x;
                    next_position.y = pos.y;
                    ship_go.transform.position = next_position;
                }
                if (obj.Field == "primary_cooldown")
                {
                    RoomManager.UpdatePlayerHUDPrimary(ship.uuid, (float)ship.primary_cooldown / (float)ship.primary_cooldown_max);
                }
                if (obj.Field == "special_cooldown")
                {
                    RoomManager.UpdatePlayerHUDSpecial(ship.uuid, (float)ship.special_cooldown / (float)ship.special_cooldown_max);
                }
                if (obj.Field == "shields" || obj.Field == "shields_max")
                {
                    RoomManager.UpdatePlayerHUDShields(ship.uuid, ship.shields, ship.max_shields);
                }
                if (obj.Field == "shields_recharge_time" || obj.Field == "shields_recharge_cooldown")
                {

                    hud.SetShieldRecharge((float)ship.shields_recharge_cooldown / (float)ship.shields_recharge_time);
                }
                if (obj.Field == "kills")
                {
                    RoomManager.UpdatePlayerHUDExperience(ship.uuid, (int)(ship.kills - ship.previous_level), (int)(ship.next_level - ship.previous_level));
                }
                if (obj.Field == "level")
                {
                    RoomManager.ShowMessage(ship.name + " is now level " + ship.level + ". " + (int)(ship.next_level - ship.previous_level) + " kills to next level.", 3);
                }
                if (obj.Field == "bullet_invulnerable" || obj.Field == "collision_invulnerable")
                {
                    if (ship.bullet_invulnerable == true && ship.collision_invulnerable == true)
                    {
                        ship_go.GetComponent<ShipGameObject>().ActivateForceField();
                    } 
                    else if (ship.collision_invulnerable == true)
                    {
                        ship_go.GetComponent<ShipGameObject>().ActivateRammingShield();
                    }
                    else
                    {
                        ship_go.GetComponent<ShipGameObject>().DeactivateShields();
                    }
                }
                if(obj.Field == "invisible")
                {
                    ship_go.GetComponent<ShipGameObject>().SetInvisibility(ship.invisible);
                }
            }
        });
        };
    }

    void OnShipRemove(Ship ship, string key)
    {
        GameObject ship_go;
        if (ships.TryGetValue(ship, out ship_go))
        {
            GameObject explosion_gameobject = Object.Instantiate(Resources.Load<GameObject>("explosions/ElectricExplosion"), ship_go.transform.position, Quaternion.identity) as GameObject;
            explosion_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);
            Object.Destroy(ship_go);
            ships.Remove(ship);
            RoomManager.RemovePlayerHUD(ship.uuid);
        }
    }

    void OnEnemyAdd(Enemy enemy, string key)
    {
        GameObject enemy_gameobject = Object.Instantiate(Resources.Load<GameObject>("enemies/" + enemy.model_type), new Vector3(enemy.position.x, enemy.position.y, 0), Quaternion.identity) as GameObject;
        enemy_gameobject.name = "Enemy" + enemy.model_type;
        enemy_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);
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
                    Quaternion from = enemy_go.transform.rotation;
                    enemy_go.transform.rotation = Quaternion.Lerp(from, Quaternion.FromToRotation(Vector3.up, next_position - enemy_go.transform.position), 0.1f);
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
            GameObject explosion_gameobject = Object.Instantiate(Resources.Load<GameObject>("explosions/EnemyExplosion_1"), enemy_go.transform.position, Quaternion.identity) as GameObject;
            explosion_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);
            Object.Destroy(enemy_go);
            enemies.Remove(enemy);
        }
    }

    void OnBulletAdd(Bullet bullet, string key)
    {
        GameObject bullet_gameobject = Object.Instantiate(Resources.Load<GameObject>("bullets/" + bullet.bullet_mesh), new Vector3(bullet.position.x, bullet.position.y, 0), Quaternion.identity) as GameObject;
        //TODO: Set the material for the bullet
        bullet_gameobject.name = "Bullet" + bullet.bullet_mesh;
        bullet_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);
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
        if (bullets.TryGetValue(bullet, out bullet_go))
        {
            Object.Destroy(bullet_go);
            GameObject explosion_gameobject;
            if (bullet.blast_radius != 0)
            {
                explosion_gameobject = SB_Explosion.GetExplosion(bullet.blast_radius/15, bullet_go.transform.position);
            }
            else
            {
                explosion_gameobject = SB_Explosion.GetExplosion(5, bullet_go.transform.position);
            }
            explosion_gameobject.transform.SetParent(RoomManager.m_Game_GRP.transform);
            bullets.Remove(bullet);
        }
    }

}
