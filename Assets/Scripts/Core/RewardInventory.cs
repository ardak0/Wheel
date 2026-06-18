using System;
using System.Collections.Generic;
using WheelDemo.Data;

namespace WheelDemo.Core
{
    // Rewards for the current run. Plain C#, changed only through these methods,
    // observed via events so the UI never touches the dictionary directly.
    public class RewardInventory
    {
        private readonly Dictionary<RewardDefinitionSO, int> amounts = new();

        public IReadOnlyDictionary<RewardDefinitionSO, int> Amounts => amounts;

        public event Action<RewardDefinitionSO, int> RewardChanged; // (reward, newTotal)
        public event Action Cleared;

        public void Add(RewardDefinitionSO reward, int amount)
        {
            if (reward == null || amount <= 0) return;
            amounts.TryGetValue(reward, out int current);
            amounts[reward] = current + amount;
            RewardChanged?.Invoke(reward, amounts[reward]);
        }

        public void Clear()
        {
            amounts.Clear();
            Cleared?.Invoke();
        }
    }
}
