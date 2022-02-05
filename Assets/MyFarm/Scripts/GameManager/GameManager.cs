using UnityEngine;

namespace MyFarm.Scripts.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public static GameAsset GameAsset => Instance._gameAsset;
        public static GameConfig GameConfig => Instance._gameConfig;
        public static GameData GameData => Instance._gameData;
        
        [SerializeField] private GameAsset _gameAsset;
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private GameData _gameData;

        #region UnityCalls

        private void Awake()
        {
            if (Instance == null) Instance = this;
            if (Instance == this) return;
            
            Debug.LogWarning("Two GameManager present on this scene. Deleting this one !", this);
            Destroy(gameObject);
        }

        private void Start()
        {
            Time.timeScale = 1;
            _gameData?.Load();
        }

        private void FixedUpdate()
        {
            _gameData.Save();
        }

        #endregion

        #region CheatFunctions

        public void GiveMoney()
        {
            _gameData.PlayerMoney += 100;
        }

        public void Speed(float value)
        {
            Time.timeScale = value;
        }
        
        #endregion
    }
}
