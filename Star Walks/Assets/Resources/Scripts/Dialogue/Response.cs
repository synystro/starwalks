using UnityEngine;

[CreateAssetMenu(fileName = "New Response", menuName = "Response")]
public class Response : ScriptableObject {

    [Space(10)]
    public int id;
    [Space(10)]
    [TextArea(3, 10)]
    public string text;
    [Space(10)]
    public bool isFight;
    [Space(10)]
    public bool isIntruder;
    [Space(10)]
    public int scraps;
    [Space(10)]
    public Item[] rewards;
    [Space(10)]
    public Crew[] crew;
    [Space(10)]
    public GameObject[] intruder;
    [Space(10)]
    public Choice[] choices;

}