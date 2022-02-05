using System;
using TMPro;
using UnityEngine;

namespace MyFarm.Scripts.Ui
{
    public class Farmer : MonoBehaviour
    {
        public static Action<int> OnFarmerCountChange;

        public TextMeshProUGUI farmerCount;

        private void Awake()
        {
            OnFarmerCountChange += UpdateText;
        }

        private void UpdateText(int value)
        {
            farmerCount.text = "x" + value;
        }
    }
}