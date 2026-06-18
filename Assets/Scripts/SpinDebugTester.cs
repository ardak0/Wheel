using UnityEngine;
using WheelDemo.Gameplay;

// Throwaway tester: no UI needed. Press Space to spin, L to leave, R to restart.
// Tick "Auto Spin" in the inspector to spin on a timer instead.
public class SpinDebugTester : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private bool autoSpin = false;
    [SerializeField] private float autoInterval = 0.8f;
    [SerializeField] private float fakeSpinTime = 0.2f;

    private float nextAuto;

    private void Awake()
    {
        if (gameController == null) gameController = GetComponent<GameController>();
        if (gameController == null) gameController = FindObjectOfType<GameController>();
    }

    private void Start()
    {
        gameController.ZoneStarted += (z, w) => Debug.Log($"--- Zone {z} ({w.Tier}) | can leave: {gameController.CanLeave}");
        gameController.RewardWon += (r, a) => Debug.Log($"   won {a}x {r.DisplayName}");
        gameController.BombHit += () =>
        {
            Debug.Log("   BOOM — rewards wiped");
            if (autoSpin) Invoke(nameof(Restart), 1f);
        };
        gameController.PlayerCashedOut += amounts =>
        {
            Debug.Log("   CASHED OUT with:");
            foreach (var kv in amounts) Debug.Log($"      {kv.Value}x {kv.Key.DisplayName}");
        };
        gameController.SpinResolved += _ => Invoke(nameof(FinishSpin), fakeSpinTime);

        Debug.Log($"Ready. Zone {gameController.CurrentZone}. Space=spin  L=leave  R=restart");
    }

    private void Update()
    {
        if (autoSpin)
        {
            if (gameController.CanSpin && Time.time >= nextAuto)
            {
                nextAuto = Time.time + autoInterval;
                gameController.RequestSpin();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && gameController.CanSpin)
        {
            gameController.RequestSpin();
        }

        if (Input.GetKeyDown(KeyCode.L) && gameController.CanLeave) gameController.RequestCashOut();
        if (Input.GetKeyDown(KeyCode.R)) Restart();
    }

    private void FinishSpin() => gameController.NotifySpinAnimationFinished();
    private void Restart() => gameController.RequestRestart();
}