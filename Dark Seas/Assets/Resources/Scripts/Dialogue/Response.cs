using UnityEngine;
namespace DarkSeas
{
    [CreateAssetMenu(fileName = "New Response", menuName = "Response")]
    public class Response : ScriptableObject
    {

        [Space(10)]
        public int id;
        [Space(10)]
        [TextArea(3, 10)]
        public string text;
        [Space(10)]
        public bool isFight;
        [Space(10)]
        public bool isStore;
        //[Space(10)]
        //public Battle battle;
        //[Space(10)]
        //public Store store;
        [Space(10)]
        public bool isIntruder;
        [Space(10)]
        public int scraps;
        [Space(10)]
        public Item[] itemRewards;
        [Space(10)]
        public GameObject[] crewRewards;
        [Space(10)]
        public GameObject[] intruder;
        [Space(10)]
        public Choice[] choices;

    }
}