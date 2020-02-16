using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace ZombieHeadGames.UI
{

    public class ZHG_UI_System : MonoBehaviour
    {
        #region Variables
        [Header("Main Properties")]
        public ZHG_UI_Screen m_StartScreen;
        public ZHG_FlashMessage m_FlashMessage;
        public SB_Game_HUD m_HUD;

        [Header("System Events")]
        public UnityEvent onSwitchedScreen = new UnityEvent();
        public UnityEvent onUISystemInitialized = new UnityEvent();
        public UnityEvent onCloseCurrentScreen = new UnityEvent();

        [Header("Fader Properties")]
        public Image m_Fader;
        public float m_FadeInDuration = 1f;
        public float m_FadeOutDuration = 1f;

        private Component[] screens = new Component[0];

        private ZHG_UI_Screen previousScreen;
        private ZHG_UI_Screen PreviousScreen { get { return previousScreen; } }

        private ZHG_UI_Screen currentScreen;
        private ZHG_UI_Screen CurrentScreen { get { return currentScreen; } } 

        #endregion


        #region Main Methods
        // Start is called before the first frame update
        void Start()
        {
            screens = GetComponentsInChildren<ZHG_UI_Screen>(true);
            InitializeScreens();

            if(m_StartScreen)
            {
                SwitchScreens(m_StartScreen);
            }

            if (onUISystemInitialized != null)
            {
                onUISystemInitialized.Invoke();
            }

            if (m_Fader)
            {
                m_Fader.gameObject.SetActive(true);
            }
            FadeIn();
        }

        #endregion

        #region Helper Methods

        public void SwitchScreens(ZHG_UI_Screen aScreen)
        {
            if (aScreen)
            {
                if(currentScreen)
                {
                    currentScreen.CloseScreen();
                    previousScreen = currentScreen;
                }

                currentScreen = aScreen;
                currentScreen.gameObject.SetActive(true);
                currentScreen.StartScreen();
            }

            if(onSwitchedScreen != null)
            {
                onSwitchedScreen.Invoke();
            }
        }

        public void CloseCurrentScreen()
        {
            if(currentScreen)
            {
                currentScreen.CloseScreen();
                currentScreen = null;
                previousScreen = currentScreen;
            }

            if(onCloseCurrentScreen != null)
            {
                onCloseCurrentScreen.Invoke();
            }
        }

        public void FadeIn()
        {
            if (m_Fader)
            {
                m_Fader.CrossFadeAlpha(0f, m_FadeInDuration, false);
            }
        }

        public void FadeOut()
        {
            if (m_Fader)
            {
                m_Fader.CrossFadeAlpha(1f, m_FadeOutDuration, false);
            }
        }

        public void GoToPreviousScreen()
        {
            if(previousScreen)
            {
                SwitchScreens(previousScreen);
            }
        }

        void InitializeScreens()
        {
            foreach(var screen in screens)
            {
                screen.gameObject.SetActive(true);
            }
        }

        public void ShowMessage(string message, float delay)
        {
            if(m_FlashMessage != null)
            {
                m_FlashMessage.ShowMessage(message, delay);
            }
        }

        public void ShowHUD()
        {
            if(m_HUD != null)
            {
                Debug.Log("Show HUD");
                m_HUD.ShowHUD();
            }
        }

        public void HideHUD()
        {
            if (m_HUD != null)
            {
                m_HUD.HideHUD();
                m_HUD.ResetHUD();
            }
        }
        #endregion 
    }
}
