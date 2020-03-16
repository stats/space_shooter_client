using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_BossAnnounce : MonoBehaviour
{

    GameObject text;
    float timer = 0;
    float flashTimer = 0.8f;
    bool isOn = false;

    void Start()
    {
        text = transform.Find("TextField").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(isOn)
        {
            timer += Time.deltaTime;
            if(timer >= 5.0)
            {
                isOn = false;
                text.SetActive(false);
                timer = 0.0f;
                flashTimer = 0.5f;
            } else if (timer >= flashTimer)
            {
                text.SetActive(!text.activeSelf);
                if(text.activeSelf)
                {
                    flashTimer += 0.8f;
                } else
                {
                    flashTimer += 0.3f;
                }
            }
        }
    }

    public void Show()
    {
        Debug.Log("Show Called");
        isOn = true;
        text.SetActive(true);
        timer = 0;
    }
}
