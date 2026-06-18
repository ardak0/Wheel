using System;
using System.Collections.Generic;

namespace WheelDemo.Core
{
    public enum GameState
    {
        Idle,            // at rest: spin or cash out
        Spinning,        // tween running, input locked
        ResultReveal,    // resolving the landed slice
        RewardCollected, // reward added, moving to next zone
        BombExploded,    // run over
        CashedOut        // left with rewards
    }

    // Transitions are whitelisted, so illegal flows (cash out mid-spin etc.)
    // throw instead of silently corrupting state.
    public class GameStateMachine
    {
        private static readonly Dictionary<GameState, GameState[]> Allowed = new()
        {
            { GameState.Idle,            new[] { GameState.Spinning, GameState.CashedOut } },
            { GameState.Spinning,        new[] { GameState.ResultReveal } },
            { GameState.ResultReveal,    new[] { GameState.RewardCollected, GameState.BombExploded } },
            { GameState.RewardCollected, new[] { GameState.Idle } },
            { GameState.BombExploded,    new[] { GameState.Idle } },
            { GameState.CashedOut,       new[] { GameState.Idle } },
        };

        public GameState Current { get; private set; } = GameState.Idle;
        public event Action<GameState, GameState> StateChanged;

        public bool CanTransitionTo(GameState next) =>
            Allowed.TryGetValue(Current, out var t) && Array.IndexOf(t, next) >= 0;

        public void TransitionTo(GameState next)
        {
            if (!CanTransitionTo(next))
                throw new InvalidOperationException($"Illegal transition {Current} -> {next}");

            var prev = Current;
            Current = next;
            StateChanged?.Invoke(prev, next);
        }
    }
}
