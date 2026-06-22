using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelDemo.Data;
using WheelDemo.Loading;

namespace WheelDemo.Gameplay
{
    // Top bar listing what the player has banked this run. One row per reward.
    public class RewardBarView : MonoBehaviour
    {
        [SerializeField] private GameController gameController;
        [SerializeField] private RectTransform rowContainer;
        [SerializeField] private RewardRow rowPrefab;

        private readonly Dictionary<RewardDefinitionSO, RewardRow> rows = new();
        private ComponentPool<RewardRow> rowPool;

        private void OnEnable()
        {
            gameController.Inventory.RewardChanged += OnRewardChanged;
            gameController.Inventory.Cleared += OnCleared;
        }

        private void OnDisable()
        {
            gameController.Inventory.RewardChanged -= OnRewardChanged;
            gameController.Inventory.Cleared -= OnCleared;
        }

        private void OnRewardChanged(RewardDefinitionSO reward, int total)
        {
            if (!rows.TryGetValue(reward, out var row))
            {
                rowPool ??= new ComponentPool<RewardRow>(rowPrefab, rowContainer);
                row = rowPool.Get();
                row.transform.SetAsLastSibling();
                rows[reward] = row;

                // Prefer an Addressables-loaded icon when the reward declares a
                // key and a loading backend is registered; otherwise use the
                // embedded sprite. The captured row may be recycled before the
                // async load completes, so guard with the reward still owning it.
                row.SetIcon(reward.Icon); // embedded fallback (and while loading)
                if (IconLoader.HasBackend && !string.IsNullOrEmpty(reward.IconAddress))
                {
                    var capturedRow = row;
                    IconLoader.LoadIcon(reward.IconAddress, sprite =>
                    {
                        if (sprite != null && rows.TryGetValue(reward, out var current) && current == capturedRow)
                            capturedRow.SetIcon(sprite);
                    });
                }
            }
            row.SetAmount(total);
        }

        private void OnCleared()
        {
            rowPool?.ReleaseAll();
            rows.Clear();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameController == null) gameController = FindObjectOfType<GameController>();
        }
#endif
    }
}
