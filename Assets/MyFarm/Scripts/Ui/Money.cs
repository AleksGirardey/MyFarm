using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace MyFarm.Scripts.Ui
{
    public class Money : MonoBehaviour
    {
        public static Action<int, bool> OnPlayerMoneyChanged;

        public TextMeshProUGUI moneyText;
        public UIParticleSystem GainMoneyParticleSystem;
        public UIParticleSystem LooseMoneyParticleSystem;

        private void Awake()
        {
            OnPlayerMoneyChanged += UpdateMoneyText;
        }

        private void Start()
        {
            UpdateMoneyText(GameManager.GameManager.GameData.PlayerMoney, true);
        }

        private void UpdateMoneyText(int value, bool gain)
        {
            if (gain) GainMoneyParticleSystem.StartParticleEmission();
            else LooseMoneyParticleSystem.StartParticleEmission();

            moneyText.text = value + "$";
        }
    }
}