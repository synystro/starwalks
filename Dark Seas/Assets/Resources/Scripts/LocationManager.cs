using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace DarkSeas {
    [RequireComponent(typeof(BoxCollider2D))]
    public class LocationManager : MonoBehaviour {
        [SerializeField] private List<GameObject> enemyShips = new List<GameObject>();
        [Space(10)]
        public Location location;
        [Space(10)]
        public GameObject BG;
        [Space(10)]
        public GameObject sector;
        [Space(10)]
        public GameObject battleGO;
        [Space(10)]
        public GameObject uiManagerGO;
        [Space(10)]
        public GameObject eventCanvasGroupGO;
        [Space(10)]
        public GameObject storeCanvasGroupGO;

        private List<GameObject> uiChoices = new List<GameObject>();

        [SerializeField] private List<Choice> currentChoices = new List<Choice>();

        private GameObject centeredChoice;
        private EventUIManager uiManager;

        private SectorManager sm;
        private BattleUIManager battleUiManager;

        private TextMeshProUGUI eventMessage;

        public List<Vector3> enemyPositions = new List<Vector3>();
        public List<Vector3> enemyRotations = new List<Vector3>();

        public List<GameObject> EnemyShips { get { return enemyShips; } }

        private bool hasStarted;

        private void OnEnable() {
            if (hasStarted)
                Start();
        }

        private void Start() {
            hasStarted = true;

            // get managers
            sm = GameManager.SectorManager;
            battleUiManager = GameManager.BattleUiManager;

            // set BG image
            //BG.GetComponent<SpriteRenderer>().sprite = location.BG;

            // set uiManager
            uiManager = uiManagerGO.GetComponent<EventUIManager>();

            // activate event canvas UI
            uiManager.eventCanvas.SetActive(true);

            // activate event choices panel
            uiManager.eventChoicesPanel.SetActive(true);

            // set text locations
            eventMessage = uiManager.eventTextGO.GetComponent<TextMeshProUGUI>();

            // group up uiManager's choices
            uiChoices.Add(uiManager.eventChoice1);
            uiChoices.Add(uiManager.eventChoice2);
            uiChoices.Add(uiManager.eventChoice3);

            // set choice's centered button
            centeredChoice = uiChoices[1];

            // if there's a location
            if (location != null) {
                SetupEvent();

                // hide battle canvas
                battleUiManager.HideBattleCanvas();

            }

        }

        private void SetupEvent() {

            // display arrival message on UI based on the language set
            if (GameManager.language == "EN-UK")
                eventMessage.text = location.arrivalMessageEnglishUK;
                if(location.arrivalMessageEnglishUK == "") {
                    eventMessage.text = location.name;
                    NoChoices();
                    return;
            } else if (GameManager.language == "PT-BR") {
                eventMessage.text = location.arrivalMessage;
                if (location.arrivalMessage == "") {
                    eventMessage.text = location.name;
                    NoChoices();
                    return;
                }
            }

            // erase previous uiChoices
            //uiChoices.Clear();
            //currentChoices.Clear();

            // group up uiManager's choices
            //uiChoices.Add(uiManager.eventChoice1);
            //uiChoices.Add(uiManager.eventChoice2);
            //uiChoices.Add(uiManager.eventChoice3);

            // set choice's centered button
            // centeredChoice = uiChoices[1];

            // set choices inactive
            HideChoices();

            // display first choices
            FirstChoices();

        }

        private void FirstChoices() {

            // display this location's first choices based on the language set
            if (GameManager.language == "EN-UK") {

                for (int i = 0; i < location.choices1.Length; i++) {

                    Choice currentChoice = location.choicesEnglishUK[i];
                    uiChoices[i].transform.parent.gameObject.SetActive(true);
                    uiChoices[i].GetComponent<TextMeshProUGUI>().text = currentChoice.text;
                    AddListener(uiChoices[i], currentChoice);

                    currentChoices.Add(currentChoice);

                }
            } else if (GameManager.language == "PT-BR") {
                for (int i = 0; i < location.choices1.Length; i++) {

                    Choice currentChoice = location.choices1[i];
                    uiChoices[i].transform.parent.gameObject.SetActive(true);
                    uiChoices[i].GetComponent<TextMeshProUGUI>().text = currentChoice.text;
                    AddListener(uiChoices[i], currentChoice);

                }
            } else
                Debug.LogError("No language found for " + this.name + " script.");
        }

        private void GiveResponse(Choice choice) {

            Response response;

            if (choice.responses.Length == 0) {

                EmptyResponse();
                NoChoices();

                return;
            }

            int random = Random.Range(0, choice.responses.Length);
            response = choice.responses[random];
            eventMessage.text = response.text;

            #region handle rewards/losses

            // receive/lose scraps
            if (response.scraps != 0)
                GameManager.ship.ScrapsTransaction(response.scraps);
            // receive item(s)
            if (response.itemRewards.Length > 0)
                GameManager.ship.ReceiveItems(response.itemRewards);
            if (response.crewRewards.Length > 0)
                GameManager.ship.ReceiveCrew(response.crewRewards);

            #endregion

            NextChoices(response);

        }

        private void NextChoices(Response response) {

            if (response.choices.Length == 0) {

                NoChoices();

                return;
            }

            for (int i = 0; i < uiChoices.Count; i++) {

                Transform uiChoicesPanel = uiChoices[i].transform.parent;
                RemoveListener(uiChoices[i]);
                currentChoices.Clear();

                if (i < response.choices.Length) {
                    Choice currentChoice = response.choices[i];
                    uiChoicesPanel.gameObject.SetActive(true);
                    uiChoices[i].GetComponent<TextMeshProUGUI>().text = response.choices[i].text;
                    AddListener(uiChoices[i], currentChoice);

                    currentChoices.Add(currentChoice);

                } else {
                    uiChoices[i].GetComponent<TextMeshProUGUI>().text = "";
                    RemoveListener(uiChoices[i]);

                    uiChoicesPanel.gameObject.SetActive(false);
                }

            }

        }

        private void EmptyResponse() {

            eventMessage.text = "";
            uiManager.eventMessagePanel.SetActive(false);

        }

        private void ReactivateUIelements() {

            uiManager.eventMessagePanel.SetActive(true);

            for (int i = 0; i < uiChoices.Count; i++) {
                uiChoices[i].GetComponent<TextMeshProUGUI>().text = "";
                uiChoices[i].transform.parent.gameObject.SetActive(true);
            }

        }

        private void HideChoices() {

            for (int i = 0; i < uiChoices.Count; i++) {
                uiChoices[i].transform.parent.gameObject.SetActive(false);
            }

        }

        private void NoChoices() {

            RemoveChoices();

            if (GameManager.InBattle || GameManager.InShop) {
                uiManager.eventChoicesPanel.SetActive(false);
                return;
            }

            MoveOn();

        }

        private void RemoveChoices() {

            for (int i = 0; i < uiChoices.Count; i++) {
                uiChoices[i].GetComponent<TextMeshProUGUI>().text = "";
                RemoveListener(uiChoices[i]);
                uiChoices[i].transform.parent.gameObject.SetActive(false);
            }

        }

        private void MoveOn() {

            centeredChoice.transform.parent.gameObject.SetActive(true);
            centeredChoice.GetComponent<TextMeshProUGUI>().text = "Engage...";
            AddListenerToExit(centeredChoice);

        }

        private void AddListener(GameObject go, Choice choice) {

            if (choice.battle != null)
                go.GetComponent<Button>().onClick.AddListener(delegate () { StartBattle(choice.battle); });
            else if(choice.store != null)
                go.GetComponent<Button>().onClick.AddListener(delegate () { OpenStore(choice.store); } );
            else
                go.GetComponent<Button>().onClick.AddListener(delegate () { GiveResponse(choice); });

        }

        private void AddListenerToContinue(GameObject go) {

            go.GetComponent<Button>().onClick.AddListener(delegate () {

                ReactivateUIelements();
                // deactivate event UI canvas group
                uiManager.eventCanvas.SetActive(false);
                // unlock camera
                Camera.main.GetComponent<CameraManager>().isLocked = false;
                // deactivate battle ui
                battleUiManager.battleCanvas.SetActive(true);

            });

        }

        private void AddListenerToExit(GameObject go) {

            go.GetComponent<Button>().onClick.AddListener(delegate () { ExitLocation(); });

        }

        private void RemoveListener(GameObject go) {

            go.GetComponent<Button>().onClick.RemoveAllListeners();

        }
        private void StartBattle(Battle battle) {

            GameManager.InBattle = true;

            EmptyResponse();
            NoChoices();

            // set battle to the battle GO
            BattleManager.battle = battle;

            // unlock camera
            Camera.main.GetComponent<CameraManager>().isLocked = false;

            // make sure enemy ships list is clear before adding any
            enemyShips.Clear();

            // spawn enemy ships
            for (int i = 0; i < battle.enemyShips.Count; i++) {

                GameObject shipGO = Instantiate(battle.enemyShips[i], enemyPositions[i], Quaternion.identity);
                shipGO.transform.eulerAngles = enemyRotations[i];
                enemyShips.Add(shipGO);
                Ship ship = shipGO.GetComponent<Ship>();
                // set crew rotation to normal (MAKE THIS BETTER LATER)
                ship.crew.transform.eulerAngles -= enemyRotations[i];
                ship.droids.transform.eulerAngles -= enemyRotations[i];
                
                ship.isHostile = true;
                shipGO.name += "_0" + (i + 1).ToString(); // e.g. FederationShip-01_i (i = index number)
                shipGO.AddComponent<EnemyAI>();
                shipGO.transform.SetParent(GameManager.BattleGO.transform);

            }

            if (battle.enemyShips.Count <= 0)
                GameManager.InBattle = false;

            // activate player's ship
            GameManager.ship.gameObject.SetActive(true);

            // refresh battle ui
            //GameManager.BattleUiManager.Refresh(); THIS BUGS

            // activate battle ui canvas
            GameManager.BattleUiManager.DisplayBattleCanvas();

            // unpause game
            GameManager.ResumeGame();

        }

        private void OpenStore(Store store) {

            GameManager.InShop = true;

            EmptyResponse();
            NoChoices();

            // start store here
            GameObject storeGO = new GameObject();
            storeGO.name = store.name;
            storeGO.transform.SetParent(GameManager.LocationManager.gameObject.transform);
            storeGO.AddComponent<StoreInventory>().store = store;

            // enable store UI
            DisplayStoreCanvas();

        }

        private void DisplayVictoryMessage(string victoryText) {
            // deactivate battle ui
            battleUiManager.battleCanvas.SetActive(false);

            // re-enable message text and display accordingly
            uiManager.eventMessagePanel.SetActive(true);
            eventMessage.text = victoryText;
            // activate choices panel
            uiManager.eventChoicesPanel.gameObject.SetActive(true);
            // display continue button
            centeredChoice.transform.parent.gameObject.SetActive(true);
            centeredChoice.GetComponent<TextMeshProUGUI>().text = "Acknowledged.";
            // add listener to continue...
            AddListenerToContinue(centeredChoice);

        }

        public void BattleWon() {

            GameManager.InBattle = false;
            // lock camera
            Camera.main.GetComponent<CameraManager>().isLocked = true;
            // get enemyShips before battle is destroyed
            List<GameObject> enemyShips = BattleManager.battle.enemyShips;

            string victoryText;

            // set victory text based on the number of enemy enemyShips
            if (enemyShips.Count == 1)
                victoryText = "You have defeated the " + enemyShips[0].GetComponent<Ship>().name + " vessel!";
            else if (enemyShips.Count == 2)
                victoryText = "You have defeated the " + enemyShips[0].GetComponent<Ship>().name + " and " + enemyShips[1].GetComponent<Ship>().name + " vessels!";
            else if (enemyShips.Count == 3)
                victoryText = "You have defeated the " + enemyShips[0].GetComponent<Ship>().name + ", the " + enemyShips[1].GetComponent<Ship>().name + " and the " + enemyShips[2].GetComponent<Ship>().name + " vessels!";
            else {
                victoryText = "I think a bug just happened. Sorry about that, captain.";
                Debug.LogError("Unpredicted number of enemy enemyShips: " + enemyShips.Count);
            }

            DisplayVictoryMessage(victoryText);

        }

        public void ExitLocation() {
            // reactivate UI elements gameobjects (choices, responses, etc)
            ReactivateUIelements();

            // remove choices
            RemoveChoices();

            // deactivate player's ship.
            GameManager.ship.gameObject.SetActive(false);

            // destroy enemy ships (if there's any)
            if (enemyShips.Count > 0)
                for (int i = 0; i < enemyShips.Count; i++)
                    Destroy(enemyShips[i]);
            enemyShips.Clear();

            // deactivate UI canvas group.
            uiManager.eventCanvas.SetActive(false);

            // reactivate sector and mark currentpoint as visited
            sm.sector.SetActive(true);
            Point point = sm.currentPoint.GetComponent<Point>();
            point.IsVisited = true;

            // not in battle anymore
            GameManager.InBattle = false;

            // deactivate current location
            GameManager.LocationManager.gameObject.SetActive(false);

            // unpause game
            GameManager.ResumeGame();

        }

        public void DisplayEventCanvas() { eventCanvasGroupGO.SetActive(true); }

        public void HideEventCanvas() { eventCanvasGroupGO.SetActive(false); }

        public void DisplayStoreCanvas() { storeCanvasGroupGO.SetActive(true); }

        public void HideStoreCanvas() { storeCanvasGroupGO.SetActive(false); }

    }
}
