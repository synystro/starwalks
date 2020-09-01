using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas {

    [RequireComponent(typeof(Button))]
    public class RowManager : MonoBehaviour {

        public List<GameObject> rows = new List<GameObject>();

        [SerializeField] private int currentIndex;
        private Button button;
        private CanvasGroup canvasGroup;

        void Start() {

            // get components
            button = GetComponent<Button>();
            canvasGroup = GetComponentInParent<CanvasGroup>();
            // add listener to this toggle button
            button.onClick.AddListener(NextRow);

        }

        public void NextRow() {

            // deactivate current row
            rows[currentIndex].SetActive(false);

            // go to next, if there isn't one go back to the first one
            if (currentIndex < rows.Count - 1)
                currentIndex++;
            else
                currentIndex = 0;

            // activate next row
            rows[currentIndex].SetActive(true);

        }

        private void OnEnable() {

            //rows.Clear();
            currentIndex = 0;

        }

    }
}
