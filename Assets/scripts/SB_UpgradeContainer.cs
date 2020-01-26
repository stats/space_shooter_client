using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SB_UpgradeContainer : MonoBehaviour
{

    [Header("Main Properties")]
    public string m_UpgradeKey;
    public string m_UpgradeText;

    [Header("Sub Components [Don't change]")]
    public GameObject m_SkillText;
    public GameObject m_SkillBar;
    public GameObject m_Increase;
    public GameObject m_Decrease;

    // Start is called before the first frame update
    void Start()
    {
        m_SkillText.GetComponent<TMP_Text>().text = m_UpgradeText;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (PlayerData.UpgradePoints[m_UpgradeKey] > 0)
            {
                m_Decrease.SetActive(true);
            }
            else
            {
                m_Decrease.SetActive(false);
            }

            if (PlayerData.CurrentShip() != null && PlayerData.CurrentShip().upgrade_points > 0)
            {
                m_Increase.SetActive(true);
            }
            else
            {
                m_Increase.SetActive(false);
            }
        } 
        catch
        {
            Debug.LogError("Could not process: " + m_UpgradeKey + " Upgrade Key");
        }
    }

    public void IncreaseSkill()
    {
        if(PlayerData.CurrentShip() != null && PlayerData.UpgradePointsSpent < PlayerData.CurrentShip().upgrade_points)
        {
            PlayerData.UpgradePointsSpent += 1;
            PlayerData.UpgradePoints[m_UpgradeKey] += 1;
            UpdateBarSize();
        }
    }

    public void DecreaseSkill()
    {
        if(PlayerData.UpgradePoints[m_UpgradeKey] > 0)
        {
            PlayerData.UpgradePoints[m_UpgradeKey] -= 1;
            PlayerData.UpgradePointsSpent -= 1;
            UpdateBarSize();
        }
    }

    public void UpdateBarSize()
    {
        float stat = PlayerData.GetCurrentShipAttribute<int>("upgrade_" + m_UpgradeKey) + PlayerData.UpgradePoints[m_UpgradeKey];
        if (stat > 0) Debug.Log("Stat: " + m_UpgradeKey + ", " + stat);
        m_SkillBar.GetComponent<MaskImage>().setValue(stat / 20.0f);
    }
}
