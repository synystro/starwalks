using UnityEngine;
using UnityEditor;

namespace DarkSeas {

    [CustomEditor(typeof(CargoManager))]
    public class InventoryEditor : Editor {
        public override void OnInspectorGUI() {
            
            if (GUILayout.Button("Add 1 random item")) {
                Item randomItem = GameManager.RandomItem();
                ((CargoManager)target).GetInventory.Add(randomItem);
            };

            DrawDefaultInspector();
        }
    }

}