using System;
using TMPro;
using UnityEngine;

namespace MyFarm.Scripts.Shop
{
    public class FarmerPanel : MonoBehaviour
    {
        #region Assignations
        
        public TextMeshProUGUI farmerPriceText;

        #endregion

        private void Start()
        {
            farmerPriceText.text = GameManager.GameManager.GameConfig.defaultFarmerPrice + "$";
        }

        public void OnClick()
        {
            int farmerCost = GameManager.GameManager.GameConfig.defaultFarmerPrice;

            if (GameManager.GameManager.GameData.PlayerMoney < farmerCost) return;
            
            GameManager.GameManager.GameData.PlayerMoney -= farmerCost;
            GameManager.GameManager.GameData.Farmers += 3;
        }
    }
}