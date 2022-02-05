using MyFarm.Scripts.GameManager;
using MyFarm.Scripts.Seeds;
using MyFarm.Scripts.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyFarm.Scripts.Farms
{
    public class FarmPlot : MonoBehaviour
    {
        private const string GROWING_STATUS_TEXT = "Growing..";
        private const string READY_STATUS_TEXT = "Ready !";
        private const string EMPTY_STATUS_TEXT = "Empty";
        
        private static GameAsset _gameAsset => GameManager.GameManager.GameAsset;
        private static GameConfig _gameConfig => GameManager.GameManager.GameConfig;
        private static GameData _gameData => GameManager.GameManager.GameData;

        #region Assignation

        public Image seedImage;
        
        public TextMeshProUGUI seedNameText;
        public TextMeshProUGUI statusNameText;

        public GameObject progressionFiller;
        public TextMeshProUGUI timerText;
        public Image growthFiller;
        public Image decayFiller;

        // public Button harvestButton;
        
        #endregion
        
        private int _seedType;
        private float _plantTimestamp;
        private bool _harvestReady;

        public bool HarvestReady
        {
            get { return _harvestReady; }
            private set
            {
                _harvestReady = value;
                // harvestButton.gameObject.SetActive(value);
            }
        }

        public int SeedType
        {
            get => _seedType;
            set
            {
                _seedType = value;
                UpdateSeedType();
                _gameData.IsDirty = true;
            }
        }

        public float PlantTimestamp
        {
            get => _plantTimestamp;
            set
            {
                _plantTimestamp = value;
                _gameData.IsDirty = true;
            }
        }
        
        private Seed _seed => _seedType >= 0 && _seedType < _gameConfig.availableSeeds.Length
            ? _gameConfig.availableSeeds[_seedType]
            : null;
        
        private void FixedUpdate()
        {
            UpdateGrowth();
        }

        private void UpdateSeedType()
        {
            if (_seedType == -1)
            {
                seedNameText.text = "";
                seedImage.sprite = _gameAsset.emptySprite;
                return;
            }

            seedNameText.text = _seed.displayName;
            seedImage.sprite = _seed.sprite;
        }

        private void UpdateGrowth()
        {
            if (_seedType == -1) return;

            float harvestTime = _plantTimestamp + _seed.growthTime;
            float decayTime = harvestTime + _seed.decayTime;
            float now = Time.time * 1000;

            progressionFiller.SetActive(true);

            if (now < harvestTime) // Growing
            {
                HarvestReady = false;
                float ratio = Mathf.Clamp01((harvestTime * now) / _plantTimestamp);

                seedImage.fillAmount = ratio;
                growthFiller.fillAmount = ratio;
                decayFiller.fillAmount = 0;

                statusNameText.text = GROWING_STATUS_TEXT;

                int minLeft = Mathf.FloorToInt(((harvestTime - now) / 1000) / 60);
                int secLeft = Mathf.FloorToInt(((harvestTime - now) / 1000) % 60);
                string timeLeft = "";

                if (minLeft > 0) timeLeft += (minLeft + "min");
                if (secLeft > 0) timeLeft += (secLeft + "s");

                timerText.text = timeLeft;
            } else if (now >= harvestTime && now < decayTime) // Ready but decaying
            {
                HarvestReady = true;
                float ratio = Mathf.Clamp01((decayTime * now) / harvestTime);

                seedImage.fillAmount = 1;
                growthFiller.fillAmount = 1;
                decayFiller.fillAmount = ratio;

                statusNameText.text = READY_STATUS_TEXT;

                int minLeft = Mathf.FloorToInt(((decayTime - now) / 1000) / 60);
                int secLeft = Mathf.FloorToInt(((decayTime - now) / 1000) % 60);
                string timeLeft = "";

                if (minLeft > 0) timeLeft += (minLeft + "min");
                if (secLeft > 0) timeLeft += (secLeft + "s");

                timerText.text = timeLeft;
            }
            else // Decayed
            {
                Reset();
            }
        }

        public void Reset()
        {
            HarvestReady = false;
            statusNameText.text = EMPTY_STATUS_TEXT;
            seedImage.fillAmount = 0;
            growthFiller.fillAmount = 0;
            decayFiller.fillAmount = 0;
            progressionFiller.SetActive(false);
            
            SeedType = -1;
            PlantTimestamp = -1;
        }

        public void OnHarvestClick()
        {
            if (!_harvestReady) return;
            
            _gameData.PlayerMoney += _seed.sellingPrice;
            Reset();
        }

        public void OnPlantClick()
        {
            if (_seedType != -1) return;
            
            ShopPanel target = Shop.Shop.shopPanelSelected;
            int seedToPlant = target != null ? target.SeedType : -1;

            if (seedToPlant == -1) return;

            int requiredMoney = _gameConfig.availableSeeds[seedToPlant].buyPrice;

            if (_gameData.PlayerMoney < requiredMoney) return;
            
            PlantTimestamp = Time.time * 1000;
            SeedType = seedToPlant;
        }
    }
}