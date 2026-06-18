using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WheelDemo.Gameplay
{
    // One wedge on the wheel: icon + amount. WheelView spawns and positions these.
    public class SliceView : MonoBehaviour
    {
        [SerializeField] private Image uiImageSliceIcon;
        [SerializeField] private TMP_Text uiTextSliceAmountValue;

        public void Bind(Sprite icon, int amount, bool isBomb)
        {
            uiImageSliceIcon.sprite = icon;
            uiImageSliceIcon.enabled = icon != null;
            uiTextSliceAmountValue.text = isBomb ? string.Empty : $"x{amount}";
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (uiImageSliceIcon == null)
                uiImageSliceIcon = transform.Find("ui_image_slice_icon")?.GetComponent<Image>();
            if (uiTextSliceAmountValue == null)
                uiTextSliceAmountValue = transform.Find("ui_text_slice_amount_value")?.GetComponent<TMP_Text>();
        }
#endif
    }
}
