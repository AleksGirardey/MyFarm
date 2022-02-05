using MyFarm.Scripts.Farms;
using MyFarm.Scripts.Shop;
using UnityEngine;

namespace MyFarm.Scripts.GameManager
{
    [CreateAssetMenu(fileName = "GameData", menuName = "MyFarm/Config/GameData")]
    public class GameData : ScriptableObject
    {
        private static string entry_playerMoney = "PlayerMoney";
        // private static string entry_farmFieldUnlocked = "FarmFieldUnlocked";
        private static string entry_farmField = "FarmField";
        private static string entry_farmSlotType = "SlotType";
        private static string entry_cropTimestamp = "CropTimestamp";

        private int _playerMoney = 20;

        private bool _isDirty = false;

        #region Getter Setter

        public int PlayerMoney
        {
            get => _playerMoney;
            set
            {
                _isDirty = true;
                _playerMoney = value;
                Money.OnPlayerMoneyChanged?.Invoke(_playerMoney);
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
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
            
            for (int farmIndex = 0; farmIndex < GameManager.GameConfig.defaultFarmFieldCount; ++farmIndex)
            {
                SaveFarmField(farmIndex);
            }
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey(entry_playerMoney)) _playerMoney = PlayerPrefs.GetInt(entry_playerMoney);
        }

        private void SaveFarmField(int farmIndex)
        {
            int plotPerField = GameManager.GameConfig.defaultFarmPlotsPerField; 
            string typePrefix = GetFarmPrefix(farmIndex) + entry_farmSlotType + "_"; 
            string timestampPrefix = GetFarmPrefix(farmIndex) + entry_cropTimestamp + "_";
            
            FarmField farmField = Farm.Instance.GetFarmField(farmIndex);

            int type = -1;
            float timestamp = -1f;

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
                PlayerPrefs.SetFloat(timestampPrefix + plotIndex, timestamp);
            }
        }

        public void LoadFarmField(ref FarmField farmField)
        {
            int plotPerField = GameManager.GameConfig.defaultFarmPlotsPerField; 
            string typePrefix = GetFarmPrefix(farmField.FieldIndex) + entry_farmSlotType + "_"; 
            string timestampPrefix = GetFarmPrefix(farmField.FieldIndex) + entry_cropTimestamp + "_"; 
            
            bool fieldUnlock = IsFieldUnlock(farmField.FieldIndex); 
            
            if (fieldUnlock) farmField.Unlock();
            else farmField.SetLocked(farmField.FieldIndex * 100);

            for (int index = 0; index < plotPerField; ++index)
            {
                int type = PlayerPrefs.GetInt(typePrefix + index, -1);
                float timestamp = PlayerPrefs.GetFloat(timestampPrefix + index, -1f);

                farmField.SetFarmPlot(index, type, timestamp);
            }
        }

        public bool IsFieldUnlock(int index)
        {
            return PlayerPrefs.HasKey(GetFarmPrefix(index));
        }
        
        public void Reset()
        {
            PlayerPrefs.DeleteAll();
            _playerMoney = GameManager.GameConfig.defaultPlayerStartingMoney;
        }
    }
}