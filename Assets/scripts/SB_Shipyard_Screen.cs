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

        int i = 0;
        foreach (Ship ship in PlayerData.myShips)
        {
            int index = i;
            GameObject child = Instantiate(Resources.Load<GameObject>("ShipSelectButton"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            TMP_Text text = child.transform.GetChild(0).GetComponent<TMP_Text>();
            text.SetText(ship.name + "\nRank: " + ship.rank);
            child.GetComponent<Button>().onClick.AddListener(delegate { SelectShip(index); });
            child.transform.SetParent(m_ShipListContent.transform);
            i++;
        }
    }

    public void SelectShip(int index)
    {
        Ship ship = PlayerData.myShips[index];
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
            m_ShipStatValuesText.text = ship.rank + "\n" + ship.level + "\n" + ship.kills + "\n" + ship.next_level + "\n" + ship.upgrade_points;
        }
        if(onShipSelected != null)
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
}
