using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class LocationManager : MonoBehaviour {

    public Location location;
    public GameObject BG;
    [Space(10)]
    public GameObject uiManagerGO;
    [Space(10)]

    private List<GameObject> uiChoices = new List<GameObject>();
    private UIManager uiManager;

    private TextMeshProUGUI eventMessage;

    void Start() {

        // set BG image;
        BG.GetComponent<SpriteRenderer>().sprite = location.BG;

        // set uiManager;
        uiManager = uiManagerGO.GetComponent<UIManager>();

        // set text locations.
        eventMessage = uiManager.eventTextGO.GetComponent<TextMeshProUGUI>();

        // display arrival message on UI.
        eventMessage.text = location.arrivalMessage;

        // group up uiManager's choices.
        uiChoices.Add(uiManager.eventChoice1);
        uiChoices.Add(uiManager.eventChoice2);
        uiChoices.Add(uiManager.eventChoice3);

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
            EmptyChoices();

            return;
        }

        int random = Random.Range(0, 3);
        Response response = choice.responses[random];
        eventMessage.text = response.text;

        NextChoices(response);

    }

    void NextChoices(Response response) {

        if (response.choices.Length == 0) {

            EmptyChoices();

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

    void EmptyChoices() {

        for (int i = 0; i < uiChoices.Count; i++) {
            uiChoices[i].GetComponent<TextMeshProUGUI>().text = "";
            uiChoices[i].transform.parent.gameObject.SetActive(false);
        }

    }

    void AddListener(GameObject go, Choice choice) {

        go.GetComponent<Button>().onClick.AddListener(delegate () { GiveResponse(choice); });

    }
    /*
    void AddListener(GameObject go) {

        go.GetComponent<Button>().onClick.AddListener(delegate () { ExitLocation(); });

    }
    */
    void RemoveListener(GameObject go) {

        go.GetComponent<Button>().onClick.RemoveAllListeners();

    }

    public void ExitLocation() {

        print("exiting location.");
        // exit this location.

    }

}
