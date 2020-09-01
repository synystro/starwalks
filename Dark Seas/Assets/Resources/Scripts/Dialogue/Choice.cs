using UnityEngine;
namespace DarkSeas
{
    [CreateAssetMenu(fileName = "New Choice", menuName = "Choice")]
    public class Choice : ScriptableObject
    {

        [Space(10)]
        public int id;
        [Space(10)]
        [TextArea(3, 10)]
        public string text;
        [Space(10)]
        public Battle battle;
        [Space(10)]
        public Store store;
        [Space(10)]
        public Response[] responses;

    }
}