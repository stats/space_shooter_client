using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ZombieHeadGames.UI
{
    public class ZHG_FlashMessage : MonoBehaviour
    {

        [Header("Main Properties")]
        public GameObject MessageObject;

        private TMP_Text messageText;

        private float currentTime = 0.0f;
        private float executeTime = 0.0f;
        private float displayTime = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            if (MessageObject != null)
            {
                MessageObject.SetActive(false);
                messageText = MessageObject.GetComponent<TMP_Text>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            currentTime = Time.time;
            if (executeTime != 0.0f)
            {
                if (currentTime - executeTime > displayTime)
                {
                    MessageObject.SetActive(false);
                }
            }
        }

        public void ShowMessage(string message, float delay)
        {
            messageText.text = message;
            MessageObject.SetActive(true);
            executeTime = Time.time;
            displayTime = delay;
        }
    }
}
