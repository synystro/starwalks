using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas
{
    [CreateAssetMenu(fileName = "New Battle", menuName = "Battle")]
    public class Battle : ScriptableObject
    {

        [Space(10)]
        public int id;
        [Space(10)]
        [TextArea(3, 10)]
        public string text;
        [Space(10)]
        public List<GameObject> enemyShips = new List<GameObject>();

    }
}