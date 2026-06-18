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

        public string RewardId => rewardId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(rewardId))
                rewardId = name;
        }
#endif
    }
}
