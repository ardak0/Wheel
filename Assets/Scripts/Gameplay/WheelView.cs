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
        [SerializeField] private float radius = 150f;
        [SerializeField] private float angleOffset = 0f;        // nudge icons into the pockets

        [Header("Spin tween")]
        [SerializeField] private int fullSpins = 5;
        [SerializeField] private float spinDuration = 3.2f;
        [SerializeField] private Ease spinEase = Ease.OutCubic;

        private SliceView[] spawned = System.Array.Empty<SliceView>();
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

        private void Rebuild(int zone, Data.WheelConfigSO wheel)
        {
            if (wheelBaseImage != null && wheel.BaseSprite != null)
                wheelBaseImage.sprite = wheel.BaseSprite;
            if (indicatorImage != null && wheel.IndicatorSprite != null)
                indicatorImage.sprite = wheel.IndicatorSprite;

            foreach (var s in spawned)
                if (s != null) Destroy(s.gameObject);

            sliceContainer.localRotation = Quaternion.identity;

            int n = wheel.SliceCount;
            sliceAngle = 360f / n;
            spawned = new SliceView[n];

            for (int i = 0; i < n; i++)
            {
                var slice = wheel.Slices[i];
                var view = Instantiate(slicePrefab, sliceContainer);
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
                spawned[i] = view;
            }
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