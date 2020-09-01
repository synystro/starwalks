using UnityEngine;

namespace DarkSeas {
    public class PauseToggle : MonoBehaviour {
        public GameObject[] toggleObjects;

        public GameObject[] toggleButtons;

        private const float onValue = 1f;
        private const float offValue = 0.5f;
        private const float pauseValue = 0f;

        private UnityEngine.UI.Toggle toggle;
        private CanvasGroup canvasGroup;
        public BattleUIManager battleUIManager;

        void Start() {
            battleUIManager = GetComponentInParent<BattleUIManager>();

            // get components
            toggle = GetComponent<UnityEngine.UI.Toggle>();
            canvasGroup = GetComponentInParent<CanvasGroup>();
            // set alpha to "off" value
            canvasGroup.alpha = offValue;
            // add listener to this toggle button
            toggle.onValueChanged.AddListener(OnToggleValueChanged);

        }

        public void OnToggleValueChanged(bool isOn) {
            // activate/deactivate game objects according to isOn
            if (toggleObjects.Length > 0)
                for (int i = 0; i < toggleObjects.Length; i++) {
                    toggleObjects[i].SetActive(!toggleObjects[i].activeSelf);
                }

            // toggle buttons
            if (toggleButtons.Length > 0)
                for (int i = 0; i < toggleObjects.Length; i++)
                    toggleButtons[i].GetComponent<CanvasGroup>().interactable = !toggleButtons[i].GetComponent<CanvasGroup>().interactable;

            // change alpha acordding to isOn
            canvasGroup.alpha = isOn ? onValue : offValue;

            // pause/unpause the game acordding to isOn
            //Time.timeScale = isOn ? pauseValue : onValue;
            if (isOn) {
                //Inventory.i.onItemChangedCallback?.Invoke();
                //Augments.i.onItemChangedCallback?.Invoke();
                GameManager.PauseGame();
                //if(!GameManager.InBattle)
                    //battleUIManager.DisplayInventoryCanvas();
            } else {
                GameManager.ResumeGame();

                //if (!GameManager.InBattle)//|| battleUIManager.augmentCanvas.activeSelf)
                    //battleUIManager.HideInventoryCanvas();
            }

        }

    }
}
