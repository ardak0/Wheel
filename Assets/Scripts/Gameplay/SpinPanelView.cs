using UnityEngine;
using UnityEngine.UI;

namespace WheelDemo.Gameplay
{
    // Spin and Leave buttons. References are auto-found in OnValidate and the
    // click handlers are added in code, so the inspector OnClick lists stay empty.
    public class SpinPanelView : MonoBehaviour
    {
        [SerializeField] private GameController gameController;
        [SerializeField] private Button uiButtonSpin;
        [SerializeField] private Button uiButtonLeave;

        private void OnEnable()
        {
            uiButtonSpin.onClick.AddListener(gameController.RequestSpin);
            uiButtonLeave.onClick.AddListener(gameController.RequestCashOut);

            gameController.StateMachine.StateChanged += OnStateChanged;
            gameController.ZoneStarted += (_, __) => Refresh();
            Refresh();
        }

        private void OnDisable()
        {
            uiButtonSpin.onClick.RemoveListener(gameController.RequestSpin);
            uiButtonLeave.onClick.RemoveListener(gameController.RequestCashOut);
            gameController.StateMachine.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(Core.GameState from, Core.GameState to) => Refresh();

        private void Refresh()
        {
            uiButtonSpin.interactable = gameController.CanSpin;
            uiButtonLeave.gameObject.SetActive(gameController.CanLeave);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameController == null) gameController = FindObjectOfType<GameController>();
            if (uiButtonSpin == null)  uiButtonSpin  = Find("ui_button_spin");
            if (uiButtonLeave == null) uiButtonLeave = Find("ui_button_leave");
        }

        private Button Find(string n)
        {
            foreach (var b in GetComponentsInChildren<Button>(true))
                if (b.name == n) return b;
            Debug.LogWarning($"[{name}] missing button '{n}'", this);
            return null;
        }
#endif
    }
}
