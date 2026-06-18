using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WheelDemo.Gameplay
{
    // Tiny helper component on the reward row prefab.
    public class RewardRow : MonoBehaviour
    {
        [SerializeField] private Image uiImageRewardIcon;
        [SerializeField] private TMP_Text uiTextRewardAmountValue;

        public void SetIcon(Sprite icon) => uiImageRewardIcon.sprite = icon;
        public void SetAmount(int amount) => uiTextRewardAmountValue.text = amount.ToString();
    }
}