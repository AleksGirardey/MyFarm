using System;
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
            if (Instance == this)
            {
                if (_gameData != null) _gameData.Load();
                return;
            }
            
            Debug.LogWarning("Two GameManager present on this scene. Deleting this one !", this);
            Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            _gameData.Save();
        }

        #endregion

        
    }
}
