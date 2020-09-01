using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas
{
    public class Toggle : MonoBehaviour
    {

        public List<GameObject> toggleObjects = new List<GameObject>();

        public List<GameObject> toggleButtons = new List<GameObject>();

        private const float onValue = 1f;
        private const float offValue = 0.5f;
        private const float pauseValue = 0f;

        private UnityEngine.UI.Toggle toggle;
        private CanvasGroup canvasGroup;

        void Start()
        {

            // get components
            toggle = GetComponent<UnityEngine.UI.Toggle>();
            canvasGroup = GetComponentInParent<CanvasGroup>();
            // set alpha to "off" value
            canvasGroup.alpha = offValue;
            // add listener to this toggle button
            toggle.onValueChanged.AddListener(OnToggleValueChanged);

        }

        public void OnToggleValueChanged(bool isOn)
        {

            // activate/deactivate game objects according to isOn
            if (toggleObjects.Count > 0)
                for (int i = 0; i < toggleObjects.Count; i++)
                    toggleObjects[i].SetActive(!toggleObjects[i].activeSelf);
            // toggle buttons
            if (toggleButtons.Count > 0)
                for (int i = 0; i < toggleObjects.Count; i++)
                    toggleButtons[i].GetComponent<CanvasGroup>().interactable = !toggleButtons[i].GetComponent<CanvasGroup>().interactable;
            // change alpha acordding to isOn
            canvasGroup.alpha = isOn ? onValue : offValue;

        }

    }
}
