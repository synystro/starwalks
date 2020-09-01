using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas {

    [RequireComponent(typeof(GridLayoutGroup))]
    public class CrewButtonsPanel : MonoBehaviour {

        public GameObject buttonPrefab;
        [Space(10)]
        private GameObject shipCrewParent;
        [Space(10)]
        public bool horizontal;

        private Transform buttonContainer;
        private int nButtons;

        private RectTransform panelRect;

        private float layoutWidth;
        private float layoutHeight;
        private float panelWidth;
        private float panelHeight;

        private GridLayoutGroup gridLayout;

        private Ship playerShip;

        private void Refresh() {
                SetupPanel();
                print("refreshed crewPanel");
        }

        private void Start() {

            buttonContainer = this.transform;

            playerShip = GameManager.ship;
            GameManager.onReceiveCrew += Refresh;

            SetupPanel();

        }

        private void ClearPanel() {

            int nButtonsToDelete = this.transform.childCount;

            for(int i = nButtonsToDelete ; i > 0; i--) {
                 DestroyImmediate(this.transform.GetChild(i-1).gameObject);
             }

        }
        
        private void SetupPanel() {

            ClearPanel();

            shipCrewParent = GameManager.ship.crew;

            gridLayout = this.GetComponent<GridLayoutGroup>();

            // get button size
            layoutWidth = gridLayout.cellSize.x;
            layoutHeight = gridLayout.cellSize.y;

            nButtons = 0;

            foreach(Transform child in shipCrewParent.transform)
                if(child.gameObject.activeSelf == true)
                    nButtons++;

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

            SetupCrew();

        }

        public void SetupCrew() {

            int currentButtonIndex = 0;

            for (int i = 0; i < playerShip.crew.transform.childCount; i++) {

                GameObject crewmemberGO;
                crewmemberGO = playerShip.crew.transform.GetChild(i).gameObject;

                // if the crewmember's gameobject is inactive for some reason, skip to next
                if(crewmemberGO.activeSelf == false) {
                    continue;
                }
                
                Crewmember crewmember = crewmemberGO.GetComponent<Crewmember>();

                GameObject crewmemberButtonGO = this.transform.GetChild(currentButtonIndex).gameObject;

                Image crewmemberIcon = crewmemberButtonGO.transform.GetChild(0).GetComponentInChildren<Image>(true);
                // enable crew icon
                crewmemberIcon.enabled = true;
                // set crew icon
                crewmemberIcon.sprite = crewmember.icon;
                crewmemberIcon.color = crewmember.GetComponent<SpriteRenderer>().color;

                RectTransform crewAttPanelRectT = crewmemberButtonGO.transform.Find("CrewAttributesPanel").GetComponent<RectTransform>();
                AttributesUIPanel attUI = crewAttPanelRectT.GetComponent<AttributesUIPanel>();
                attUI.SetCrewmember(crewmember);

                // set double click
                CustomButton crewmemberButton = crewmemberButtonGO.GetComponent<CustomButton>();
                crewmemberButton.onDoubleTap.AddListener(
                    () =>  crewAttPanelRectT.gameObject.SetActive(!crewAttPanelRectT.gameObject.activeSelf)
                );

                crewmemberButton.onDoubleTap?.Invoke();

                UnityEngine.UI.Toggle crewToggle = crewmemberButtonGO.GetComponent<UnityEngine.UI.Toggle>();
                ToggleColor toggleColor = crewmemberButtonGO.GetComponent<ToggleColor>();
                // assign toggle to the crewmember's script
                crewmember.toggle = crewToggle;
                // remove all previous functionalities from crew toggle
                crewToggle.onValueChanged.RemoveAllListeners();
                // add functionality to crew toggle
                crewToggle.onValueChanged.AddListener(crewmember.ToggleSelection);
                crewToggle.onValueChanged.AddListener(toggleColor.OnToggleValueChanged);

                // add to crew button index
                currentButtonIndex++;

            }

        }

    }

}
