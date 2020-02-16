using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SB_MatchShip : MonoBehaviour
{

    [Header("Properties")]
    public TMP_Text m_Text;
    public Image m_Image;

    private Ship _shipData;

    public void SetShip(Ship ship)
    {
        _shipData = ship;
        m_Text.text = _shipData.name + "\nLevel " + _shipData.level;
        m_Image.sprite = Resources.Load<Sprite>("ships_thumbnails/" + ship.ship_type);
    }
}
