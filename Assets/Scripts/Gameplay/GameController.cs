using System;
using System.Collections.Generic;
using UnityEngine;
using WheelDemo.Core;
using WheelDemo.Data;

namespace WheelDemo.Gameplay
{
    // The one place that owns game flow. Views call the Request* methods and
    // listen to the events below; they never run the rules themselves.
    public class GameController : MonoBehaviour
    {
        [Header("Wheel configs")]
        [SerializeField] private WheelConfigSO bronzeWheel;
        [SerializeField] private WheelConfigSO silverWheel;
        [SerializeField] private WheelConfigSO goldenWheel;

        public GameStateMachine StateMachine { get; } = new GameStateMachine();
        public RewardInventory Inventory { get; } = new RewardInventory();
        public int CurrentZone { get; private set; } = 1;

        public WheelConfigSO CurrentWheel => WheelFor(ZoneRules.GetTierForZone(CurrentZone));
        public bool CanSpin => StateMachine.Current == GameState.Idle;
        public bool CanLeave => StateMachine.Current == GameState.Idle && ZoneRules.CanLeaveAtZone(CurrentZone);

        public event Action<int, WheelConfigSO> ZoneStarted;   // (zone, wheel)
        public event Action<int> SpinResolved;                 // (sliceIndex) -> view animates, then calls back
        public event Action<RewardDefinitionSO, int> RewardWon;
        public event Action BombHit;
        public event Action<IReadOnlyDictionary<RewardDefinitionSO, int>> PlayerCashedOut;

        private SlicePicker picker;
        private int pendingIndex = -1;

        private void Awake()
        {
            picker = new SlicePicker();
        }

        private void Start() => ZoneStarted?.Invoke(CurrentZone, CurrentWheel);

        public void RequestSpin()
        {
            if (!CanSpin) return;
            pendingIndex = picker.PickSliceIndex(CurrentWheel);
            StateMachine.TransitionTo(GameState.Spinning);
            SpinResolved?.Invoke(pendingIndex);
        }

        // Called by the wheel view when its spin tween finishes.
        public void NotifySpinAnimationFinished()
        {
            if (StateMachine.Current != GameState.Spinning) return;
            StateMachine.TransitionTo(GameState.ResultReveal);
            ResolveResult();
        }

        public void RequestCashOut()
        {
            if (!CanLeave) return;
            StateMachine.TransitionTo(GameState.CashedOut);
            PlayerCashedOut?.Invoke(Inventory.Amounts);
        }

        public void RequestRestart()
        {
            var s = StateMachine.Current;
            if (s != GameState.BombExploded && s != GameState.CashedOut) return;

            Inventory.Clear();
            CurrentZone = 1;
            StateMachine.TransitionTo(GameState.Idle);
            ZoneStarted?.Invoke(CurrentZone, CurrentWheel);
        }

        private void ResolveResult()
        {
            var wheel = CurrentWheel;
            var slice = wheel.Slices[pendingIndex];

            if (slice.IsBomb)
            {
                StateMachine.TransitionTo(GameState.BombExploded);
                Inventory.Clear();
                BombHit?.Invoke();
                return;
            }

            int amount = wheel.GetScaledAmount(slice, CurrentZone);
            Inventory.Add(slice.Reward, amount);
            RewardWon?.Invoke(slice.Reward, amount);

            StateMachine.TransitionTo(GameState.RewardCollected);
            CurrentZone++;
            StateMachine.TransitionTo(GameState.Idle);
            ZoneStarted?.Invoke(CurrentZone, CurrentWheel);
        }

        private WheelConfigSO WheelFor(WheelTier tier) => tier switch
        {
            WheelTier.Golden => goldenWheel,
            WheelTier.Silver => silverWheel,
            _ => bronzeWheel
        };

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (bronzeWheel != null && !bronzeWheel.HasBomb)
                Debug.LogWarning("Bronze wheel has no bomb slice.", this);
        }
#endif
    }
}
