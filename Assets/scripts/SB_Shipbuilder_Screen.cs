using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SB_Shipbuilder_Screen : MonoBehaviour
{
    [Header("Main Properties")]
    public TMP_Text m_LoadoutText;
    public TMP_Text m_ShipNameText;

    [Header("Screens")]
    public GameObject m_ShipsScreen;
    public GameObject m_PrimaryScreen;
    public GameObject m_SpecialScreen;
    public GameObject m_MaterialScreen;

    [Header("Ship Components")]
    public TMP_InputField m_NameInput;
    public string m_ShipType = "explorer1";
    public string m_ShipMaterial = "cindertron_recruit1";
    public string m_Primary = "Cannon";
    public string m_Special = "EmergencyBrake";

    private ShipGameObject shipGameObject;
    public ShipGameObject ShipGameObject { get { return shipGameObject; } }
    private GameObject shipDisplay;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void UpdateButtonStates()
    {
        Debug.Log("Update Button States");
        foreach (Transform child in m_ShipsScreen.transform)
        {
            child.gameObject.GetComponent<SB_UnlockableButton>().UpdateState();
        }
        foreach (Transform child in m_PrimaryScreen.transform)
        {
            child.gameObject.GetComponent<SB_UnlockableButton>().UpdateState();
        }
        foreach (Transform child in m_SpecialScreen.transform)
        {
            child.gameObject.GetComponent<SB_UnlockableButton>().UpdateState();
        }
        foreach (Transform child in m_MaterialScreen.transform)
        {
            child.gameObject.GetComponent<SB_UnlockableButton>().UpdateState();
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
        if(shipDisplay != null)
        {
            shipDisplay.SetActive(true);
        }
    }

    public void SetShipType(string type)
    {
        m_ShipType = type;
        UpdateShip();
    }

    public void SetShipMaterial(string mat)
    {
        m_ShipMaterial = mat;
        UpdateShip();
    }

    public void SetPrimary(string primary)
    {
        m_Primary = primary;
        UpdateShip();
    }

    public void SetSpecial(string special)
    {
        m_Special = special;
        UpdateShip();
    }

    public void UpdateShip()
    {
        Ship shipData = new Ship();
        shipData.name = m_NameInput.text;
        shipData.shipType = m_ShipType;
        shipData.shipMaterial = m_ShipMaterial;
        shipData.primaryWeapon = m_Primary;
        shipData.specialWeapon = m_Special;

        if (shipData.shipType == null)
        {
            shipData.shipType = "explorer1";
        }
        if(shipData.shipMaterial == null)
        {
            shipData.shipMaterial = "cindertron_recruit1";

        }

        string t, m;

        t = char.ToUpper(shipData.shipType[0]) + shipData.shipType.Substring(1);
        t = t.Replace("1", " Mark I");
        t = t.Replace("2", " Mark II");
        t = t.Replace("3", " Mark III");
        t = t.Replace("4", " Mark IV");
        t = t.Replace("5", " Mark V");
        m = shipData.shipMaterial.Replace("_", " ");
        m = char.ToUpper(m[0]) + m.Substring(1);

        m_LoadoutText.text = "Type: " + t + "\nPaint: " + m + "\nPrimary: " + shipData.primaryWeapon + "\nSpecial: " + shipData.specialWeapon;
        

        if (shipDisplay == null)
        {
            shipDisplay = Instantiate(Resources.Load<GameObject>("ship"), new Vector3(-600, 0, 0), Quaternion.identity) as GameObject;
            shipDisplay.name = "Ship Builder Test Ship";
        }

        shipDisplay.SetActive(true);
        shipGameObject = shipDisplay.GetComponent<ShipGameObject>();
        shipGameObject.shipData = shipData;
        shipGameObject.UpdateComponents();
        if (m_ShipNameText != null)
        {
            m_ShipNameText.text = shipData.name;
        }
    }

    public void HideAllTabs()
    {
        transform.Find("ShipTypesTab").gameObject.SetActive(false);
        transform.Find("ShipMaterialTab").gameObject.SetActive(false);
        transform.Find("ShipPrimaryTab").gameObject.SetActive(false);
        transform.Find("ShipSpecialTab").gameObject.SetActive(false);
    }

    public void ShowTab(string name)
    {
        HideAllTabs();
        Transform tab = transform.Find(name);
        if(tab != null)
        {
            tab.gameObject.SetActive(true);
        }
    }
}
