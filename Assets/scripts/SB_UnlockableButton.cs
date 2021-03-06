﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnlockType
{
    Ship, Primary, Special, Material
}

public class SB_UnlockableButton : MonoBehaviour
{

    [Header("Properties")]
    public string m_Key;
    public Sprite m_Sprite;
    public UnlockType m_Type;
    public SB_Shipbuilder_Screen m_ShipBuilderScreen;

    private Button _button;
    private Image _image;

    // Start is called before the first frame update
    void Awake()
    {
        _button = transform.GetComponent<Button>();
        _image = transform.GetComponent<Image>();

        _image.sprite = m_Sprite;
        switch(m_Type)
        {
            case UnlockType.Ship: _button.onClick.AddListener(delegate { m_ShipBuilderScreen.SetShipType(m_Key); }); break;
            case UnlockType.Primary: _button.onClick.AddListener(delegate { m_ShipBuilderScreen.SetPrimary(m_Key); }); break;
            case UnlockType.Special: _button.onClick.AddListener(delegate { m_ShipBuilderScreen.SetSpecial(m_Key); }); break;
            case UnlockType.Material: _button.onClick.AddListener(delegate { m_ShipBuilderScreen.SetShipMaterial(m_Key); }); break;
        }

    }
   
    public void UpdateState()
    {
        if (PlayerData.unlockMessage != null)
        {
            //Debug.Log("Key: " + m_Key + " Contains Key: " + PlayerData.unlockMessage.unlocks.ContainsKey(m_Key));

            if (PlayerData.unlockMessage.unlocks.ContainsKey(m_Key) == false)
            {
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                transform.parent.gameObject.SetActive(true);
                UnlockItem ui = PlayerData.unlockMessage.unlocks[m_Key];
                if(ui.unlocked)
                { 
                    _button.interactable = true;
                } 
                else
                {
                    _button.interactable = false;
                }
            }
        }
        else
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}
