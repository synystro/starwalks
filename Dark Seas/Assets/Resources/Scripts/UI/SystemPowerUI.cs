using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas {

    public class SystemPowerUI : MonoBehaviour {

        public Button addOnePowerButton;
        public Button removeOnePowerButton;
        public ShipSystem system;

        private int maxPowerLimit;
        private TMPro.TextMeshProUGUI powerValueText;

        private float timeOutTimer;
        private const float timeOut = 3f;
        
        private void Awake() {
            maxPowerLimit = GameManager.MAX_POWER_LIMIT;
            powerValueText = this.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            addOnePowerButton.onClick.AddListener(() => { AddOnePowerBar(); } );
            removeOnePowerButton.onClick.AddListener(() => { RemoveOnePowerBar(); });
        }

        private void Update() {

            powerValueText.text = system.powerConsumption.ToString();

            timeOutTimer += Time.fixedDeltaTime;

            if(timeOutTimer >= timeOut) {
                RefreshTimeOut();
                this.gameObject.SetActive(false);
            }

        }

        private void RefreshTimeOut() {
            timeOutTimer = 0;
        }

        public void AddOnePowerBar() {
            system.AddOnePowerBar();
            RefreshTimeOut();
        }

        public void RemoveOnePowerBar() {
            system.RemoveOnePowerBar();
            RefreshTimeOut();
        }

    }

}
