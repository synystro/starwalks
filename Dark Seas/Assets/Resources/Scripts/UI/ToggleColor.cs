using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas {

    public class ToggleColor : MonoBehaviour {

        public Color normalColor;
        public Color selectedColor;

        private UnityEngine.UI.Toggle toggle;

        private Image img;

        private void Start() {

            toggle = GetComponent<UnityEngine.UI.Toggle>();

            // add listener to this toggle button
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            img = this.GetComponent<Image>();

        }

        public void OnToggleValueChanged(bool isOn) {
            
            // change colour acordding to isOn
            img.color = isOn ? selectedColor : normalColor;

        }

    }

}