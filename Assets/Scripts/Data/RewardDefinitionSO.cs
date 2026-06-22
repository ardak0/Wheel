using UnityEngine;

namespace WheelDemo.Data
{
    // One asset per reward type (cash, gold, chest...). Slices point at these
    // so adding a new reward never touches code.
    [CreateAssetMenu(fileName = "reward_", menuName = "WheelDemo/Reward Definition")]
    public class RewardDefinitionSO : ScriptableObject
    {
        [SerializeField] private string rewardId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [Tooltip("Optional Addressables key for the icon. When set (and an " +
                 "Addressables content build exists) the icon is loaded through " +
                 "Addressables; otherwise the embedded Icon above is used.")]
        [SerializeField] private string iconAddress;

        public string RewardId => rewardId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public string IconAddress => iconAddress;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(rewardId))
                rewardId = name;
        }
#endif
    }
}
