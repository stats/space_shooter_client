using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SB_Game_HUD : MonoBehaviour
{

    [Header("Players")]
    public Dictionary<string, SB_PlayerHUD> m_Players = new Dictionary<string, SB_PlayerHUD>();

    public TMP_Text m_WaveText;

    /** TODO: Make sure the the individual huds are added here **/
    public GameObject m_HUDContainer;

    public void HideHUD()
    {
        gameObject.SetActive(false);
    }

    public void ShowHUD()
    {
        gameObject.SetActive(true);
    }

    public void AddPlayerHUD(string uuid, SB_PlayerHUD hud)
    {
        if (!m_HUDContainer) return;
        m_Players.Add(uuid, hud);
        hud.transform.SetParent(m_HUDContainer.transform);
    }

    public SB_PlayerHUD GetPlayerHUD(string uuid)
    {
        return m_Players[uuid];
    }

    public void RemovePlayerHUD(string uuid)
    {
        SB_PlayerHUD hud = m_Players[uuid];
        m_Players.Remove(uuid);
        /** TODO: Maybe replace this with a destroyed image **/
        Object.Destroy(hud);
    }

    public void SetWaveText(string text)
    {
        if (!m_WaveText) return;
        m_WaveText.text = text;
    }

    public void ResetHUD()
    {
        m_Players.Clear();
        foreach(Transform child in m_HUDContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

}
