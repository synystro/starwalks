using UnityEngine;

namespace DarkSeas
{
    public class PowerToggle : MonoBehaviour
    {

        public GameObject[] toggleObjects;
        [Space(10)]
        public GameObject[] toggleButtons;

        private const float onValue = 1f;
        private const float offValue = 0.5f;
        private const float pauseValue = 0f;

        private UnityEngine.UI.Toggle toggle;
        private CanvasGroup canvasGroup;

        private Ship playerShip;

        void Start()
        {
            toggle = GetComponent<UnityEngine.UI.Toggle>();
            canvasGroup = GetComponentInParent<CanvasGroup>();

            // set alpha to "off" value
            canvasGroup.alpha = offValue;
            // add listener to this toggle button
            toggle.onValueChanged.AddListener(OnToggleValueChanged);

        }

        public void OnToggleValueChanged(bool isOn) {

            if (isOn) {
                //GameManager.BattleUiManager.AddListenerToPowerShipWeapons();
                //GameManager.BattleUiManager.AddListenerToPowerShipSystems();
            } else {
                GameManager.BattleUiManager.AddListenerToShipWeapons();
                GameManager.BattleUiManager.AddListenerToShipSystems();
            }

            // activate/deactivate game objects according to isOn
            if (toggleObjects.Length > 0)
                for (int i = 0; i < toggleObjects.Length; i++)
                    toggleObjects[i].SetActive(!toggleObjects[i].activeSelf);
            // toggle buttons
            if (toggleButtons.Length > 0)
                for (int i = 0; i < toggleObjects.Length; i++)
                    toggleButtons[i].GetComponent<CanvasGroup>().interactable = !toggleButtons[i].GetComponent<CanvasGroup>().interactable;
            // change alpha according to isOn
            canvasGroup.alpha = isOn ? onValue : offValue;

        }

    }
}
