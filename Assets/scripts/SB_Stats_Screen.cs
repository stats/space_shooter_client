using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class SB_Stats_Screen : MonoBehaviour
{

    [Header("Main Properties")]
    public TMP_Text m_WelcomeText;
    public TMP_Text m_StatsTitle;
    public TMP_Text m_StatsDescr;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUsername();
    }

    public void UpdateUsername()
    {
        if (m_WelcomeText)
        {
            m_WelcomeText.text = PlayerPrefs.GetString("username") + " Statistics";
        }
    }

    public void DisplayStats(Statistics stats)
    {
        m_StatsTitle.text = "";
        m_StatsDescr.text = "";
       
        foreach ( DictionaryEntry item in stats.stats.Items)
        {
            m_StatsTitle.text = m_StatsTitle.text + "\n" + item.Key.ToString().Replace("_", " ") + ":";
            m_StatsDescr.text = m_StatsDescr.text + "\n" + item.Value;
        }
    }
}
