using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class LocationManager : MonoBehaviour {
    [Space(10)]
    public Location location;
    public GameObject BG;
    [Space(10)]
    public GameObject sector;
    [Space(10)]
    public GameObject uiManagerGO;

    private List<GameObject> uiChoices = new List<GameObject>();

    private GameObject exitChoice;

    private UIManager uiManager;

    private SectorManager sm;

    private TextMeshProUGUI eventMessage;

    void Start() {

        // find sector manager.
        sm = GameObject.FindGameObjectWithTag("SectorManager").GetComponent<SectorManager>();

        // set BG image.
        BG.GetComponent<SpriteRenderer>().sprite = location.BG;

        // set uiManager.
        //uiManagerGO = GameObject.FindGameObjectWithTag("UIManager");
        uiManager = uiManagerGO.GetComponent<UIManager>();

        // activate UI.
        uiManager.eventCanvas.SetActive(true);

        // set text locations.
        eventMessage = uiManager.eventTextGO.GetComponent<TextMeshProUGUI>();

        // display arrival message on UI.
        eventMessage.text = location.arrivalMessage;

        // group up uiManager's choices.
        uiChoices.Add(uiManager.eventChoice1);
        uiChoices.Add(uiManager.eventChoice2);
        uiChoices.Add(uiManager.eventChoice3);

        // set UI's exit button.
        exitChoice = uiChoices[1];

        FirstChoices();
        
    }

    void FirstChoices() {

        for(int i = 0; i < location.choices1.Length; i++) {

            Choice currentChoice = location.choices1[i];
            uiChoices[i].GetComponent<TextMeshProUGUI>().text = location.choices1[i].text;
            AddListener(uiChoices[i], currentChoice);

        }

    }

    void GiveResponse(Choice choice) {

        if (choice.responses.Length == 0) {

            EmptyResponse();
            NoChoices();

            return;
        }

        int random = Random.Range(0, 3);
        Response response = choice.responses[random];
        eventMessage.text = response.text;

        NextChoices(response);

    }

    void NextChoices(Response response) {

        if (response.choices.Length == 0) {

            NoChoices();

            return;
        }

        for (int i = 0; i < response.choices.Length; i++) {

            Choice currentChoice = response.choices[i];
            uiChoices[i].GetComponent<TextMeshProUGUI>().text = response.choices[i].text;
            AddListener(uiChoices[i], currentChoice);

        }

    }

    void EmptyResponse() {

        eventMessage.text = "";
        uiManager.eventMessagePanel.SetActive(false);

    }

    void ReactivateUIelements() {

        uiManager.eventMessagePanel.SetActive(true);

        for (int i = 0; i < uiChoices.Count; i++) {
            uiChoices[i].GetComponent<TextMeshProUGUI>().text = "";
            uiChoices[i].transform.parent.gameObject.SetActive(true);
        }

    }

    void NoChoices() {

        for (int i = 0; i < uiChoices.Count; i++) {
            uiChoices[i].GetComponent<TextMeshProUGUI>().text = "";
            uiChoices[i].transform.parent.gameObject.SetActive(false);
        }

        MoveOn();

    }

    void MoveOn() {

        exitChoice.transform.parent.gameObject.SetActive(true);
        exitChoice.GetComponent<TextMeshProUGUI>().text = "Engage...";
        AddListenerToExit(exitChoice);

    }

    void AddListener(GameObject go, Choice choice) {

        go.GetComponent<Button>().onClick.AddListener(delegate () { GiveResponse(choice); });

    }
    
    void AddListenerToExit(GameObject go) {

        go.GetComponent<Button>().onClick.AddListener(delegate () { ExitLocation(); });

    }
    
    void RemoveListener(GameObject go) {

        go.GetComponent<Button>().onClick.RemoveAllListeners();

    }

    public void ExitLocation() {

        ReactivateUIelements();

        // deactivate UI canvas group.
        uiManager.eventCanvas.SetActive(false);

        sm.sector.SetActive(true);
        sm.currentPoint.GetComponent<Point>().wasVisited = true;
        Destroy(this.gameObject);

    }

}
