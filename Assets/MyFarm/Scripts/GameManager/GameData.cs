using System;
using MyFarm.Scripts.Farms;
using MyFarm.Scripts.Ui;
using UnityEngine;

namespace MyFarm.Scripts.GameManager
{
    [CreateAssetMenu(fileName = "GameData", menuName = "MyFarm/Config/GameData")]
    public class GameData : ScriptableObject
    {
        private static string entry_playerMoney = "PlayerMoney";
        private static string entry_playerFarmers = "PlayerFarmers";
        private static string entry_farmField = "FarmField";
        private static string entry_farmSlotType = "SlotType";
        private static string entry_cropTimestamp = "CropTimestamp";

        private int _playerMoney = 20;
        private int _farmers = 0;

        private bool _isDirty = false;

        #region Getter Setter

        public int PlayerMoney
        {
            get => _playerMoney;
            set
            {
                _isDirty = true;

                bool gain = value > _playerMoney;
                _playerMoney = value;
                Money.OnPlayerMoneyChanged?.Invoke(_playerMoney, gain);
            }
        }

        public int Farmers
        {
            get => _farmers;
            set
            {
                _isDirty = true;
                
                _farmers = value;
                Farmer.OnFarmerCountChange?.Invoke(_farmers);
            }
        }

        public bool IsDirty
        {
            set => _isDirty = value;
        }

        private string GetFarmPrefix(int farmIndex)
        {
            return entry_farmField + "_" + farmIndex + "_";
        }
        
        #endregion
        
        public void Save()
        {
            if (!_isDirty) return;
            
            PlayerPrefs.SetInt(entry_playerMoney, _playerMoney);
            PlayerPrefs.SetInt(entry_playerFarmers, _farmers);
            
            for (int farmIndex = 0; farmIndex < GameManager.GameConfig.defaultFarmFieldCount; ++farmIndex)
            {
                SaveFarmField(farmIndex);
            }
            
            PlayerPrefs.Save();
            _isDirty = false;
        }

        public void Load()
        {
            _playerMoney = PlayerPrefs.GetInt(entry_playerMoney, GameManager.GameConfig.defaultPlayerStartingMoney);
            _farmers = PlayerPrefs.GetInt(entry_playerFarmers, 0);
        }

        private void SaveFarmField(int farmIndex)
        {
            int plotPerField = GameManager.GameConfig.defaultFarmPlotsPerField; 
            string typePrefix = GetFarmPrefix(farmIndex) + entry_farmSlotType + "_"; 
            string timestampPrefix = GetFarmPrefix(farmIndex) + entry_cropTimestamp + "_";
            
            FarmField farmField = Farm.Instance.GetFarmField(farmIndex);
            
            int type = -1;
            DateTime timestamp = DateTime.Now;

            if (farmField.IsUnlocked)
                PlayerPrefs.SetInt(GetFarmPrefix(farmIndex), 1);
            
            for (int plotIndex = 0; plotIndex < plotPerField; ++plotIndex)
            {
                FarmPlot plot = farmField.GetFarmPlot(plotIndex);

                if (plot != null)
                {
                    type = plot.SeedType;
                    timestamp = plot.PlantTimestamp;
                }
                
                PlayerPrefs.SetInt(typePrefix + plotIndex, type);
                PlayerPrefs.SetString(timestampPrefix + plotIndex, timestamp.ToBinary().ToString());
            }
        }

        public void LoadFarmField(ref FarmField farmField)
        {
            int plotPerField = GameManager.GameConfig.defaultFarmPlotsPerField; 
            string typePrefix = GetFarmPrefix(farmField.FieldIndex) + entry_farmSlotType + "_"; 
            string timestampPrefix = GetFarmPrefix(farmField.FieldIndex) + entry_cropTimestamp + "_"; 
            
            bool fieldUnlock = IsFieldUnlock(farmField.FieldIndex); 
            
            if (fieldUnlock) farmField.Unlock();
            else farmField.SetLocked(farmField.FieldIndex * GameManager.GameConfig.defaultFarmFieldPrice * GameManager.GameConfig.defaultFarmFieldPriceRatio);

            for (int index = 0; index < plotPerField; ++index)
            {
                int type = PlayerPrefs.GetInt(typePrefix + index, -1);
                string timestampBinary = PlayerPrefs.GetString(timestampPrefix + index, DateTime.Now.ToBinary().ToString());
                DateTime timestamp = DateTime.FromBinary(Convert.ToInt64(timestampBinary));

                farmField.SetFarmPlot(index, type, timestamp);
            }
        }

        private bool IsFieldUnlock(int index)
        {
            return PlayerPrefs.HasKey(GetFarmPrefix(index));
        }
        
        public void Reset()
        {
            PlayerPrefs.DeleteAll();
            _playerMoney = GameManager.GameConfig.defaultPlayerStartingMoney;
            _farmers = 0;
        }
    }
}