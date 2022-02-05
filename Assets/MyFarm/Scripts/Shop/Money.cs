using System;
using TMPro;
using UnityEngine;

namespace MyFarm.Scripts.Shop
{
    public class Money : MonoBehaviour
    {
        public static Action<int> OnPlayerMoneyChanged;

        public TextMeshProUGUI moneyText;

        private void Awake()
        {
            OnPlayerMoneyChanged += UpdateMoneyText;
        }

        private void Start()
        {
            UpdateMoneyText(GameManager.GameManager.GameConfig.defaultPlayerStartingMoney);
        }

        private void UpdateMoneyText(int value)
        {
            moneyText.text = value + "$";
        }
    }
}