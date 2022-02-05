using UnityEngine;

namespace MyFarm.Scripts.Seeds
{
    [CreateAssetMenu(fileName = "Seed", menuName = "MyFarm/Seed")]
    public class Seed : ScriptableObject
    {
        public string displayName = "defaultName";
        
        [Space]
        [Header("Farming")]
        [Tooltip("Expressed in milliseconds")] public float growthTime = 120000;
        [Tooltip("Expressed in milliseconds")] public float decayTime = 600000;

        [Space]
        [Header("Buy/Sell Price")]
        public int buyPrice = 4;
        public int sellingPrice = 10;

        [Space] [Header("Visual")]
        public Sprite sprite;
        public Sprite shopSprite;
    }
}