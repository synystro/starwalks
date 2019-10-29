using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Choice", menuName = "Choice")]
public class Choice: ScriptableObject {

    [Space(10)]
    public int id;
    [Space(10)]
    [TextArea(3,10)]
    public string text;
    [Space(10)]
    public bool hasReward;
    [Space(10)]
    public float rewardChance;
    [Space(10)]
    public float crewChance;
    [Space(10)]
    public Response[] responses;

}