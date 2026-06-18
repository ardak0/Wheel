using UnityEngine;
using UnityEngine.UI;

namespace WheelDemo.Gameplay
{
    // Shows on bomb. Give up -> restart. (Revive with currency is the optional bonus.)
    public class BombPopupView : MonoBehaviour
    {
        [SerializeField] private GameController gameController;
        [SerializeField] private GameObject root;
        [SerializeField] private Button uiButtonGiveUp;

        private void OnEnable()
        {
            gameController.BombHit += Show;
            uiButtonGiveUp.onClick.AddListener(OnGiveUp);
            if (root != null) root.SetActive(false);
        }

        private void OnDisable()
        {
            gameController.BombHit -= Show;
            uiButtonGiveUp.onClick.RemoveListener(OnGiveUp);
        }

        private void Show() { if (root != null) root.SetActive(true); }

        private void OnGiveUp()
        {
            if (root != null) root.SetActive(false);
            gameController.RequestRestart();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameController == null) gameController = FindObjectOfType<GameController>();
            if (uiButtonGiveUp == null) uiButtonGiveUp = FindButton("ui_button_give_up");
        }

        private Button FindButton(string n)
        {
            foreach (var b in GetComponentsInChildren<Button>(true))
                if (b.name == n) return b;
            return null;
        }
#endif
    }
}
