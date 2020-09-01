using UnityEngine;

namespace DarkSeas
{
    [CreateAssetMenu(fileName = "New Room System", menuName = "Room System")]
    public class RoomSystem : ScriptableObject
    {

        public int id;
        public int hudPriority;
        public int targetPriority;
        public new string name;
        public Sprite icon;
        [Space(10)]
        public GameObject roomSystemPrefab;
        [Space(10)]
        public int basePowerCap;

    }
}
