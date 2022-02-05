using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyFarm.Scripts.Shop
{
    public class ShopPanel : MonoBehaviour
    {
        #region Assignations

        public Image seedImage;
        public TextMeshProUGUI seedName;
        public TextMeshProUGUI seedPrice;
        public GameObject signBorder;

        #endregion

        public int SeedType { get; set; }

        private void Awake()
        {
            Shop.OnShopPanelClick += OnShopPanelClick;
        }

        private void OnShopPanelClick(ShopPanel panel)
        {
            if (panel == this) return;

            signBorder.SetActive(false);
        }
        
        public void OnClick()
        {
            if (Shop.shopPanelSelected == this)
            {
                signBorder.SetActive(false);
                Shop.shopPanelSelected = null;
                return;
            }

            signBorder.SetActive(true);
            Shop.shopPanelSelected = this;
            Shop.OnShopPanelClick.Invoke(this);
        }
    }
}