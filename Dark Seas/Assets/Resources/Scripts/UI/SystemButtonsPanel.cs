using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas {

    [RequireComponent(typeof(GridLayoutGroup))]
    public class SystemButtonsPanel : MonoBehaviour {

        public GameObject buttonPrefab;
        [Space(10)]
        [SerializeField] private GameObject contentParent;
        [Space(10)]
        public bool horizontal;

        private int nButtons;

        private RectTransform panelRect;

        private float layoutWidth;
        private float layoutHeight;
        private float panelWidth;
        private float panelHeight;

        private GridLayoutGroup gridLayout;
        
        private void Start() {

            contentParent = GameManager.ship.systems;

            gridLayout = this.GetComponent<GridLayoutGroup>();

            // get button size
            layoutWidth = gridLayout.cellSize.x;
            layoutHeight = gridLayout.cellSize.y;

            nButtons = contentParent.transform.childCount;

            panelRect = this.transform.parent.GetComponent<RectTransform>();

            if(horizontal) {
                panelWidth = layoutWidth * nButtons;
                panelHeight = layoutHeight;
            }
            else {
                panelWidth = layoutWidth;
                panelHeight = layoutHeight * nButtons;
            }

            panelRect.sizeDelta = new Vector2(panelWidth, panelHeight);

            for(int i = 0; i < nButtons; i++) {

                GameObject button = Instantiate(buttonPrefab);
                RectTransform buttonRT = button.GetComponent<RectTransform>();
                button.transform.SetParent(this.transform);
                buttonRT.localScale = new Vector3(1f,1f,1f);
                button.name = button.name + " " + i.ToString();

            }

            GameManager.BattleUiManager.SetupSystems();

        }

    }

}
