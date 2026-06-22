using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace WheelDemo.Gameplay
{
    // Builds slices each zone and spins to the picked index. The base sprite and
    // icons rotate together; the indicator stays fixed at the top.
    public class WheelView : MonoBehaviour
    {
        [SerializeField] private GameController gameController;
        [SerializeField] private RectTransform sliceContainer; // rotates: base + icons live under here
        [SerializeField] private Image wheelBaseImage;          // the tier base, child of sliceContainer
        [SerializeField] private Image indicatorImage;          // pointer, NOT under sliceContainer
        [SerializeField] private SliceView slicePrefab;
        [Tooltip("Slice ring radius as a share of the container's half-extent. " +
                 "Relative (not a fixed pixel value) so it scales with the wheel " +
                 "across 20:9 / 16:9 / 4:3 instead of overflowing.")]
        [SerializeField, Range(0.1f, 1f)] private float radiusFraction = 0.62f;
        [SerializeField] private float angleOffset = 0f;        // nudge icons into the pockets

        [Header("Spin tween")]
        [SerializeField] private int fullSpins = 5;
        [SerializeField] private float spinDuration = 3.2f;
        [SerializeField] private Ease spinEase = Ease.OutCubic;

        private ComponentPool<SliceView> slicePool;
        private float sliceAngle;

        private void OnEnable()
        {
            gameController.ZoneStarted += Rebuild;
            gameController.SpinResolved += SpinTo;
        }

        private void OnDisable()
        {
            gameController.ZoneStarted -= Rebuild;
            gameController.SpinResolved -= SpinTo;
        }

        private void OnDestroy()
        {
            // Kill the spin tween so its OnComplete lambda can't fire after this
            // object is gone (it captures sliceContainer and gameController).
            if (sliceContainer != null) sliceContainer.DOKill();
        }

        private void Rebuild(int zone, Data.WheelConfigSO wheel)
        {
            if (wheelBaseImage != null && wheel.BaseSprite != null)
                wheelBaseImage.sprite = wheel.BaseSprite;
            if (indicatorImage != null && wheel.IndicatorSprite != null)
                indicatorImage.sprite = wheel.IndicatorSprite;

            slicePool ??= new ComponentPool<SliceView>(slicePrefab, sliceContainer);
            slicePool.ReleaseAll();

            sliceContainer.localRotation = Quaternion.identity;

            int n = wheel.SliceCount;
            sliceAngle = 360f / n;

            // Derive the ring radius from the container's current size so the
            // layout holds on any aspect ratio instead of a hard-coded 150px.
            float radius = CurrentRadius();

            for (int i = 0; i < n; i++)
            {
                var slice = wheel.Slices[i];
                var view = slicePool.Get();
                view.name = $"ui_slice_{i}";

                float ang = i * sliceAngle + angleOffset;
                var rt = (RectTransform)view.transform;
                rt.localRotation = Quaternion.Euler(0, 0, -ang);
                rt.anchoredPosition = new Vector2(
                    Mathf.Sin(ang * Mathf.Deg2Rad) * radius,
                    Mathf.Cos(ang * Mathf.Deg2Rad) * radius);

                int amount = wheel.GetScaledAmount(slice, zone);
                var icon = slice.Reward != null ? slice.Reward.Icon : null;
                view.Bind(icon, amount, slice.IsBomb);
            }
        }

        // Half of the container's shorter side, scaled by radiusFraction. Using
        // the live rect means the ring tracks the wheel under any CanvasScaler
        // setup, so icons never spill outside the base on wide/tall screens.
        private float CurrentRadius()
        {
            var rect = sliceContainer.rect;
            float halfExtent = Mathf.Min(rect.width, rect.height) * 0.5f;
            return halfExtent * radiusFraction;
        }

        private void SpinTo(int index)
        {
            float target = fullSpins * 360f + index * sliceAngle;

            sliceContainer.DOLocalRotate(new Vector3(0, 0, target), spinDuration, RotateMode.FastBeyond360)
                .SetEase(spinEase)
                .OnComplete(() =>
                {
                    var e = sliceContainer.localEulerAngles;
                    sliceContainer.localRotation = Quaternion.Euler(0, 0, e.z % 360f);
                    gameController.NotifySpinAnimationFinished();
                });
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameController == null) gameController = FindObjectOfType<GameController>();
        }
#endif
    }
}