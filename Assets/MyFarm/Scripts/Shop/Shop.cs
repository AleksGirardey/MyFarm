using System;
using MyFarm.Scripts.GameManager;
using MyFarm.Scripts.Seeds;
using UnityEngine;

namespace MyFarm.Scripts.Shop
{
    public class Shop : MonoBehaviour
    {
        private static GameAsset _gameAsset => GameManager.GameManager.GameAsset;
        private static GameConfig _gameConfig => GameManager.GameManager.GameConfig;
        private static GameData _gameData => GameManager.GameManager.GameData;
        
        public static Action<ShopPanel> OnShopPanelClick;
        public static ShopPanel shopPanelSelected = null;

        public Transform shopPanelsParent;
        
        private void Start()
        {
            InitShopPanels();
        }

        private void InitShopPanels()
        {
            int count = _gameConfig.availableSeeds.Length;

            for (int index = 0; index < count; ++index)
            {
                ShopPanel panel = Instantiate(_gameAsset.shopPanelPrefab, shopPanelsParent);
                Seed seed = _gameConfig.availableSeeds[index];

                panel.gameObject.name = "ShopPanel[" + seed.displayName + "]";
                panel.seedImage.sprite = seed.shopSprite;
                panel.seedName.text = seed.displayName;
                panel.seedPrice.text = seed.buyPrice + "$";
            }
        }
    }
}