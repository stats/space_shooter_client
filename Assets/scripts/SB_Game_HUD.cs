using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SB_Game_HUD : MonoBehaviour
{
    [Header("HUD Components")]

    [Header("Player 1 Components")]
    public TMP_Text m_shipname;

    public MaskImage m_primary;
    public GameObject m_primary_ready;
    
    public MaskImage m_special;
    public GameObject m_special_ready;

    public MaskImage m_shield_amount;
    public MaskImage m_shield_recharge;
    public GameObject m_shield_active;
    public GameObject m_shield_critical;

    public MaskImage m_xp;


    [Header("Player 2 Components")]


    [Header("Player 3 Components")]


    [Header("Player 4 Components")]

    public TMP_Text m_wave_text;

    private void Start()
    {
        hideHUD();    
    }

    public void hideHUD()
    {
        gameObject.SetActive(false);
    }

    public void showHUD()
    {
        UpdateHUD();
        gameObject.SetActive(true);
    }

    public void UpdateHUD()
    {
        if (PlayerData.shipData == null) return;
        var data = PlayerData.shipData;
        if(m_shipname)
        {
            m_shipname.text = PlayerData.shipData.name;
        }

        if(m_primary)
        {
            m_primary.setValue(PlayerData.shipData.primary_cooldown / PlayerData.shipData.primary_cooldown_max);
            if(m_primary_ready && PlayerData.shipData.primary_cooldown >= PlayerData.shipData.primary_cooldown_max)
            {
                m_primary_ready.SetActive(true);
            } else
            {
                m_primary_ready.SetActive(false);
            }
        }

        if (m_special)
        {
            m_special.setValue(PlayerData.shipData.special_cooldown / PlayerData.shipData.special_cooldown_max);
            if (m_special_ready && PlayerData.shipData.special_cooldown >= PlayerData.shipData.special_cooldown_max)
            {
                m_special_ready.SetActive(true);
            }
            else
            {
                m_special_ready.SetActive(false);
            }
        }

        if (m_shield_amount)
        {
            if(m_shield_critical && m_shield_active)
            {
                if(PlayerData.shipData.shields == 1)
                {
                    m_shield_critical.SetActive(true);
                    m_shield_active.SetActive(false);
                    m_shield_amount.setValue(0);
                } else
                {
                    m_shield_critical.SetActive(false);
                    m_shield_active.SetActive(true);
                    m_shield_amount.setValue(PlayerData.shipData.shields / PlayerData.shipData.max_shields);
                }
            }

            
            
            if (m_shield_recharge && PlayerData.shipData.shields_recharge_time != 0) 
            {
                m_shield_recharge.setValue(PlayerData.shipData.shields_recharge_cooldown / PlayerData.shipData.shields_recharge_time);
            }
        }

        if(m_xp)
        {
            m_xp.setValue(PlayerData.XPPercentage() );
        }

        if (m_wave_text)
        {
            m_wave_text.text = "Wave: " + PlayerData.currentWave + " Enemies Killed: " + PlayerData.enemiesKilled + "/" + PlayerData.enemiesSpawned;
        }

    }
}
