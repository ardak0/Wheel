using System;
using System.Collections.Generic;
using UnityEngine;

namespace WheelDemo.Data
{
    public enum WheelTier
    {
        Bronze, // normal zone, has the bomb
        Silver, // every 5th zone, no bomb
        Golden  // every 30th zone, no bomb, special rewards
    }

    [Serializable]
    public class SliceEntry
    {
        [SerializeField] private RewardDefinitionSO reward;
        [SerializeField, Min(0)] private int baseAmount = 1;
        [SerializeField, Min(0.01f)] private float weight = 1f;
        [SerializeField] private bool isBomb;

        public RewardDefinitionSO Reward => reward;
        public int BaseAmount => baseAmount;
        public float Weight => weight;
        public bool IsBomb => isBomb;
    }

    // Everything a designer needs to author one wheel. Slice list and the
    // per-zone reward growth curve are all here, editable in the inspector.
    [CreateAssetMenu(fileName = "wheel_config_", menuName = "WheelDemo/Wheel Config")]
    public class WheelConfigSO : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private WheelTier tier;

        [Header("Art")]
        [SerializeField] private Sprite baseSprite;
        [SerializeField] private Sprite indicatorSprite;

        [Header("Slices (clockwise from the indicator)")]
        [SerializeField] private List<SliceEntry> slices = new List<SliceEntry>();

        [Header("Reward scaling")]
        [Tooltip("X = zone index, Y = multiplier on slice base amounts.")]
        [SerializeField] private AnimationCurve rewardScaleByZone = AnimationCurve.Linear(1, 1, 100, 10);

        public WheelTier Tier => tier;
        public Sprite BaseSprite => baseSprite;
        public Sprite IndicatorSprite => indicatorSprite;
        public IReadOnlyList<SliceEntry> Slices => slices;
        public int SliceCount => slices.Count;

        public bool HasBomb
        {
            get
            {
                for (int i = 0; i < slices.Count; i++)
                    if (slices[i].IsBomb) return true;
                return false;
            }
        }

        public int GetScaledAmount(SliceEntry slice, int zoneIndex)
        {
            float m = rewardScaleByZone.Evaluate(zoneIndex);
            return Mathf.Max(1, Mathf.RoundToInt(slice.BaseAmount * m));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (tier != WheelTier.Bronze && HasBomb)
                Debug.LogWarning($"[{name}] {tier} wheel has a bomb slice, but safe/super wheels must be bomb-free.", this);
            if (tier == WheelTier.Bronze && !HasBomb)
                Debug.LogWarning($"[{name}] Bronze wheel has no bomb slice.", this);
        }
#endif
    }
}
