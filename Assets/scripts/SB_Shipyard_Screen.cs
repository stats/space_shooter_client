using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class SB_Shipyard_Screen : MonoBehaviour
{

    [Header("Main Properties")]
    public TMP_Text m_WelcomeText;
    public TMP_Text m_ShipNameText;
    public GameObject m_ShipListContent;
    public GameObject m_ShipDisplayContainer;
    public TMP_Text m_ShipStatValuesText;
    public TMP_Text m_LoadoutText;

    [Header("Upgrade Properties")]
    public TMP_Text m_UpgradeAvailableText;
    public GameObject m_UpgradeAvailable;

    [Header("Delete Properties")]
    public GameObject m_DeleteConfirm;

    [Header("Events")]
    public UnityEvent onShipSelected = new UnityEvent();

    private ShipGameObject shipGameObject;
    private GameObject shipDisplay;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUsername();
    }

    public void HideShipDisplay()
    {
        if (m_ShipDisplayContainer != null)
        {
            m_ShipDisplayContainer.SetActive(false);
        }

        if(m_DeleteConfirm)
        {
            m_DeleteConfirm.SetActive(false);
        }
    }

    public void UpdateUsername()
    {
        if(m_WelcomeText)
        {
            m_WelcomeText.text = "Welcome to the Shipyard, " + PlayerPrefs.GetString("username");
        }
    }

    public void UpdateShips()
    {
        foreach (Transform child in m_ShipListContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        PlayerData.myShips.ForEach(delegate (string uuid, Ship ship)
        {
            GameObject child = Instantiate(Resources.Load<GameObject>("ShipSelectButton"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            TMP_Text text = child.transform.GetChild(0).GetComponent<TMP_Text>();
            text.SetText(ship.name + "\nRank: " + ship.rank);
            child.GetComponent<Button>().onClick.AddListener(delegate { SelectShip(ship.uuid); });
            child.transform.SetParent(m_ShipListContent.transform);
        });
    }

    public void SelectShip(string uuid)
    {
        Ship ship = PlayerData.myShips[uuid];
        if(shipDisplay == null)
        {
            shipDisplay = Instantiate(Resources.Load<GameObject>("ship"), new Vector3(-600, 0, 0), Quaternion.identity) as GameObject;
            shipDisplay.name = "Shipyard Test Ship";
        }

        shipDisplay.SetActive(true);
        shipGameObject = shipDisplay.GetComponent<ShipGameObject>();
        shipGameObject.shipData = ship;
        PlayerData.CurrentShipUUID = ship.uuid;
        shipGameObject.UpdateComponents();
        m_ShipDisplayContainer.SetActive(true);
        if(m_ShipNameText != null)
        {
            m_ShipNameText.text = ship.name;
        }
        if(m_ShipStatValuesText != null)
        {
            m_ShipStatValuesText.text = ship.level + "\n" + ship.rank + "\n" + ship.kill_score + "\n" +
                                        ship.kills + "/" + ship.next_level + "\n\n" + 
                                        ship.damage + " (" + ship.upgrade_damage + ")\n" +
                                        ship.fire_rate + " (" + ship.upgrade_fire_rate + ")\n" + 
                                        ship.range + " (" + ship.upgrade_range + ")\n\n" +
                                        ship.max_shields + " (" + ship.upgrade_shields_max + ")\n" + 
                                        ship.shields_recharge_time + " (" + ship.upgrade_shields_recharge + ")\n\n" + 
                                        ship.speed + " (" + ship.upgrade_speed + ")\n" + 
                                        ship.accelleration + " (" + ship.upgrade_accelleration + ")";
        }
        if(m_UpgradeAvailable)
        {
            if(ship.upgrade_points > 0)
            {
                m_UpgradeAvailableText.text = ship.upgrade_points + "";
                m_UpgradeAvailable.SetActive(true);
            } else
            {
                m_UpgradeAvailable.SetActive(false);
            }
        }
        if (m_LoadoutText)
        {
            string t, m;

            t = char.ToUpper(ship.ship_type[0]) + ship.ship_type.Substring(1);
            t = t.Replace("1", " Mark I");
            t = t.Replace("2", " Mark II");
            t = t.Replace("3", " Mark III");
            t = t.Replace("4", " Mark IV");
            t = t.Replace("5", " Mark V");
            m = ship.ship_material.Replace("_", " ");
            m = char.ToUpper(m[0]) + m.Substring(1);

            m_LoadoutText.text = "Type: " + t + "\nPaint: " + m + "\nPrimary: " + ship.primary_weapon + "\nSpecial: " + ship.special_weapon;

        }
        if (onShipSelected != null)
        {
            onShipSelected.Invoke();
        }
    }

    public void HideShip()
    {
        if(shipDisplay != null)
        {
            shipDisplay.SetActive(false);
        }
    }

    public void ShowShip()
    {
        if (shipDisplay != null)
        {
            shipDisplay.SetActive(true);
        }
    }

    public void ShowDeleteConfirm()
    {
        m_DeleteConfirm.SetActive(true);
    }


    public void HideDeleteConfirm()
    {
        m_DeleteConfirm.SetActive(false);
    }
}
