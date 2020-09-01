using UnityEngine;

namespace DarkSeas {

    [CreateAssetMenu(fileName = "ItemAssetPack", menuName = "ItemAssetPAck")]
    public class ItemsAssetPack : ScriptableObject {

        [SerializeField] public Item[] items;     

    }

}
