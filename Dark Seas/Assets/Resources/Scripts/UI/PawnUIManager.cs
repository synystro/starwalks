using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas {

    public class PawnUIManager : MonoBehaviour {

        public Image healthBar, healthBarFill, progressBar, progressBarFill;
        public GameObject healthBarPrefab, progressBarPrefab;

        public void HideHealthBar() {

        }

        public void ToggleProgressBar() {

            progressBar.gameObject.SetActive(!progressBar.gameObject.activeSelf);

        }

    }
}