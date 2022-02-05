using MyFarm.Scripts.Seeds;
using UnityEngine;

namespace MyFarm.Scripts.GameManager
{
    [CreateAssetMenuAttribute(fileName = "GameConfig", menuName = "MyFarm/Config/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public int defaultPlayerStartingMoney = 20;
        
        [Header("Farm fields")]
        public int defaultFarmUnlocked = 1;
        public int defaultFarmPlotsPerField = 4;
        public int defaultFarmFieldCount = 4;
        [Space]
        public int defaultFarmFieldPrice = 100;
        public int defaultFarmFieldPriceRatio = 2;
        [Header("Seeds")]
        public Seed[] availableSeeds;
    }
}