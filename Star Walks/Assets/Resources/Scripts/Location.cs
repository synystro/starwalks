using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Location", menuName = "Location")]
public class Location : ScriptableObject
{

    public new string name;
    public Sprite BG;
    [Space(10)]
    [TextArea(1,10)] 
    public string arrivalMessage;
    [Space(10)]
    public List<Choice[]> allChoices;
    [Space(10)]
    public Choice[] choices1;
    [Space(10)]
    public Choice[] choices2;
    [Space(10)]

    public int minScraps;
    public int maxScraps;

}
