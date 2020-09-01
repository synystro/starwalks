using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas
{
    [CreateAssetMenu(fileName = "New Zone", menuName = "Zone")]
    public class Zone : ScriptableObject
    {
        //public new string name;
        public Sprite BG;
        [Space(10)]
        [TextArea(1, 10)]
        public string arrivalMessage;
        [Space(10)]
        public List<Location> locations = new List<Location>();

    }
}
