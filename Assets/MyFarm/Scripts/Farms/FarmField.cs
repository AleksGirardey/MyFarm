using System;
using MyFarm.Scripts.GameManager;
using TMPro;
using UnityEngine;

namespace MyFarm.Scripts.Farms
{
    public class FarmField : MonoBehaviour
    {
        private static GameAsset _gameAsset => GameManager.GameManager.GameAsset;
        private static GameConfig _gameConfig => GameManager.GameManager.GameConfig;
        private static GameData _gameData => GameManager.GameManager.GameData;
        
        private bool _isUnlocked = false;
        private int _fieldIndex = -1;
        private int _priceToUnlock = -1;
        
        private FarmPlot[] _plots;

        #region Assignations

        public Transform plotsParent;
        public TextMeshProUGUI priceText;
        public GameObject lockedParent;
        #endregion
        
        public int FieldIndex
        {
            get => _fieldIndex;
            set => _fieldIndex = value;
        }

        public bool IsUnlocked => _isUnlocked;
        
        public void Unlock()
        {
            _isUnlocked = true;
            lockedParent.SetActive(false);
        }
        
        public void SetLocked(int price)
        {
            _isUnlocked = false;
            _priceToUnlock = price;
            priceText.text = price + "";
            lockedParent.SetActive(true);
        }
        
        private void Awake()
        {
            _plots = new FarmPlot[_gameConfig.defaultFarmPlotsPerField];
        }

        public FarmPlot GetFarmPlot(int index)
        {
            if (index < 0 || index >= _plots.Length) return null;

            return _plots[index];
        }

        public void SetFarmPlot(int index, int seedType, DateTime timestamp)
        {
            FarmPlot farmPlot = Instantiate(_gameAsset.farmPlotPrefab, plotsParent);

            farmPlot.gameObject.name = "Plot[" + index + "]";

            _plots[index] = farmPlot;
            farmPlot.Reset();
            farmPlot.SeedType = seedType;
            farmPlot.PlantTimestamp = timestamp;
        }

        public void Init(bool isUnlocked, int price)
        {
            if (isUnlocked) { Unlock(); }
            else SetLocked(price);

            for (int index = 0; index < _plots.Length; ++index)
            {
                SetFarmPlot(index, -1, DateTime.Now);
            }
        }

        public void OnUnlockField()
        {
            if (_isUnlocked) return;

            int playerMoney = _gameData.PlayerMoney;

            if (playerMoney < _priceToUnlock) return;

            _gameData.PlayerMoney -= _priceToUnlock;
            Unlock();
        }
    }
}