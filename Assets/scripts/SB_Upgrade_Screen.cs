using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SB_Upgrade_Screen : MonoBehaviour
{
    [Header("Main Properties")]
    public GameObject m_UpgradePoints;
    public GameObject m_ShipNameText;
    public GameObject m_ApplyUpgrades;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerData.CurrentShipUUID != null)
        {
            Ship currentShip = PlayerData.CurrentShip();

            m_ShipNameText.GetComponent<TMP_Text>().text = currentShip.name + " Rank: " + currentShip.rank;
            m_UpgradePoints.GetComponent<TMP_Text>().text = "Upgrade Points: " + currentShip.upgrade_points + " Spent: " + PlayerData.UpgradePointsSpent;

            if (PlayerData.UpgradePointsSpent > 0)
            {
                m_ApplyUpgrades.SetActive(true);
            }
            else
            {
                m_ApplyUpgrades.SetActive(false);
            }
        }
    }
}
