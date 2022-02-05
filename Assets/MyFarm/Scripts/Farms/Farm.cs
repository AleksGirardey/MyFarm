using UnityEngine;
using MyFarm.Scripts.GameManager;

namespace MyFarm.Scripts.Farms
{
    public class Farm : MonoBehaviour
    {
        public static Farm Instance;
        private static GameAsset _gameAsset => GameManager.GameManager.GameAsset;
        private static GameConfig _gameConfig => GameManager.GameManager.GameConfig;
        private static GameData _gameData => GameManager.GameManager.GameData;

        private FarmField[] _farmFields;

        #region UNITY_CALLS

        private void Awake()
        {
            CreateInstance();

            if (!Init())
                Load();
        }

        private void CreateInstance()
        {
            if (Instance == null) Instance = this;
            if (Instance == this) return;
            
            Debug.LogWarning("There is two instance of this component, deleting this one !");
            Destroy(this);
        }
        
        #endregion

        #region Init Load Save

        private bool Init()
        {
            int count = _gameConfig.defaultFarmFieldCount;
            _farmFields = new FarmField[count];
            
            if (PlayerPrefs.HasKey("IsInit")) return false;
            
            PlayerPrefs.SetInt("IsInit", 1);

            _gameData.PlayerMoney = _gameConfig.defaultPlayerStartingMoney;
            
            for (int index = 0; index < count; ++index)
            {
                FarmField field = InstantiateFarmField(index);
                field.Init(index < 1, index * _gameConfig.defaultFarmFieldPriceRatio * _gameConfig.defaultFarmFieldPrice);
            }
            
            PlayerPrefs.Save();
            return true;
        }

        private void Load()
        {
            int farmFieldUnlocked = _gameConfig.defaultFarmFieldCount;

            for (int index = 0; index < farmFieldUnlocked; ++index)
            {
                FarmField farmField = InstantiateFarmField(index);
                
                _gameData.LoadFarmField(ref farmField);
            }
        }
        
        private FarmField InstantiateFarmField(int index)
        {
            FarmField farmField = Instantiate(_gameAsset.farmFieldPrefab, transform);

            farmField.gameObject.name = "FarmField[" + index + "]";
            farmField.FieldIndex = index;

            _farmFields[index] = farmField;
            
            return farmField;
        }
        
        #endregion
        
        public FarmField GetFarmField(int index)
        {
            return index >= _farmFields.Length ? null : _farmFields[index];
        }
    }
}