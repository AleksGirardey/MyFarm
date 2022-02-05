using System;
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
        public TextMeshProUGUI sellText;
        public Image growthFiller;
        public Image decayFiller;

        public ParticleSystem HarvestReadyParticleSystem;
        
        #endregion
        
        private int _seedType;
        private DateTime _plantTimestamp;
        private bool _harvestReady;

        public bool HarvestReady
        {
            get { return _harvestReady; }
            private set
            {
                if (value)
                {
                    HarvestReadyParticleSystem.Play();
                    sellText.enabled = true;
                    sellText.text = "Sell for " + _seed.sellingPrice + "$";
                }
                else
                {
                    sellText.enabled = false;
                    HarvestReadyParticleSystem.Stop();
                }

                _harvestReady = value;
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

        public DateTime PlantTimestamp
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
                seedNameText.gameObject.SetActive(false);
                seedImage.sprite = _gameAsset.emptyFarmPlotSprite;
                seedImage.fillAmount = 1;
                return;
            }
            
            seedNameText.gameObject.SetActive(true);
            seedNameText.text = _seed.displayName;
            seedImage.fillAmount = 0;
            seedImage.sprite = _seed.sprite;
        }

        private void UpdateGrowth()
        {
            if (_seedType == -1) return;

            DateTime harvestTime = _plantTimestamp.AddSeconds(_seed.growthTime / Time.timeScale);
            DateTime decayTime = harvestTime.AddSeconds(_seed.decayTime / Time.timeScale);
            DateTime now = DateTime.Now;

            
            progressionFiller.SetActive(true);

            if (now < harvestTime) // Growing
            {
                HarvestReady = false;
                TimeSpan timeSpanHarvest = harvestTime - _plantTimestamp;
                TimeSpan timeSpanNow = now - _plantTimestamp;
                float ratio = Mathf.Clamp01((float) (timeSpanNow.TotalSeconds / timeSpanHarvest.TotalSeconds));

                seedImage.fillAmount = ratio;
                growthFiller.fillAmount = ratio;
                decayFiller.fillAmount = 0;

                statusNameText.text = GROWING_STATUS_TEXT;

                TimeSpan timeSpanLeft = harvestTime - now;
                
                int minLeft = Convert.ToInt32(timeSpanLeft.Minutes);
                int secLeft = Convert.ToInt32(timeSpanLeft.Seconds);
                
                string timeLeft = "";

                if (minLeft > 0) timeLeft += (minLeft + "min");
                if (secLeft > 0) timeLeft += (secLeft + "s");

                timerText.text = timeLeft;
            } else if (now >= harvestTime && now < decayTime) // Ready but decaying
            {
                HarvestReady = true;
                TimeSpan timeSpanDecay = decayTime - harvestTime;
                TimeSpan timeSpanNow = now - harvestTime;
                float ratio = Mathf.Clamp01((float) (timeSpanNow.TotalSeconds / timeSpanDecay.TotalSeconds));

                seedImage.fillAmount = 1;
                growthFiller.fillAmount = 1;
                decayFiller.fillAmount = ratio;

                statusNameText.text = READY_STATUS_TEXT;

                TimeSpan timeSpanLeft = decayTime - now;
                
                int minLeft = Convert.ToInt32(timeSpanLeft.Minutes);
                int secLeft = Convert.ToInt32(timeSpanLeft.Seconds);
                
                string timeLeft = "";

                if (minLeft > 0) timeLeft += (minLeft + "min");
                if (secLeft > 0) timeLeft += (secLeft + "s");

                timerText.text = timeLeft;
            }
            else // Decayed
            {
                if (_gameData.Farmers > 0)
                {
                    _gameData.Farmers -= 1;
                    OnHarvestClick();
                }
                else
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
            PlantTimestamp = DateTime.Now;
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

            _gameData.PlayerMoney -= requiredMoney;
            PlantTimestamp = DateTime.Now;
            SeedType = seedToPlant;
        }
    }
}