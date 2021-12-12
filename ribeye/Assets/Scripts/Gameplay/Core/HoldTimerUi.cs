using System;
using TMPro;
using UnityEngine;

namespace Gameplay.Core
{
    public class HoldTimerUi : MonoBehaviour
    {
        public static HoldTimerUi ui;
        
        public TextMeshProUGUI tmp;

        private void Start()
        {
            ui = this;
        }

        private HeldButton watched;

        public void watch(HeldButton x)
        {
            watched = x;
        }

        private void Update()
        {
            if (!watched)
            {
                tmp.text = "";
                return;
            }
            
            print(watched.holdTimeLeft);
            
            if (watched.holdTimeLeft > 0f)
            {
                tmp.text = String.Format("{0,2:N2}", watched.holdTimeLeft);
            }
            else
            {
                tmp.text = "";
            }
        }
    }
}