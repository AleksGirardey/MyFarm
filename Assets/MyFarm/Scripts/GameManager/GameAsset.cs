using MyFarm.Scripts.Farms;
using MyFarm.Scripts.Shop;
using UnityEngine;

namespace MyFarm.Scripts.GameManager
{
    [CreateAssetMenu(fileName = "GameAsset", menuName = "MyFarm/Config/GameAsset")]
    public class GameAsset : ScriptableObject
    {
        [Header("Utilities")]
        public Sprite emptySprite;
        
        [Header("Farming")]
        
        public FarmField farmFieldPrefab;
        public FarmPlot farmPlotPrefab;

        [Header("Shop")]
        public ShopPanel shopPanelPrefab;
    }
}