using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SB_PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    public TMP_Text m_ShipNameText;

    public MaskImage m_PrimaryMask;
    public GameObject m_PrimaryReady;

    public MaskImage m_SpecialMask;
    public GameObject m_SpecialReady;

    public MaskImage m_ShieldMask;
    public MaskImage m_ShieldRechargeMask;
    public TMP_Text m_ShieldText;

    public MaskImage m_ExperienceMask;
    public TMP_Text m_ExperienceText;

    public void SetShipName(string name)
    {
        if(!m_ShipNameText) return;
        m_ShipNameText.text = name;
    }

    public void SetPrimary(float primary)
    {
      if(!m_PrimaryMask || ! m_PrimaryReady) return;

      m_PrimaryMask.setValue(primary);

      if(primary >= 1)
      {
        m_PrimaryReady.SetActive(true);
      }
      else
      {
        m_PrimaryReady.SetActive(false);
      }
    }

    public void SetSpecial(float special)
    {
      if(!m_SpecialMask || ! m_SpecialReady) return;

      m_SpecialMask.setValue(special);

      if(special >= 1)
      {
        m_SpecialReady.SetActive(true);
      }
      else
      {
        m_SpecialReady.SetActive(false);
      }
    }

    public void SetShields(int shield, int shieldMax)
    {
      if(!m_ShieldMask || !m_ShieldText) return;

      m_ShieldMask.setValue((float)shield / (float)shieldMax);
      m_ShieldText.text = shield + "/" + shieldMax;
    }

    public void SetExperience(int experience, int nextLevel)
    {
      if(!m_ExperienceMask || !m_ExperienceText) return;

      m_ShieldMask.setValue((float)experience/(float)nextLevel);
      m_ExperienceText.text = experience + "/" + nextLevel;
    }


}
