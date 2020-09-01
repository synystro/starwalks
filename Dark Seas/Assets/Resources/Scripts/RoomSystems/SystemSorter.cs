using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkSeas {
    public class SystemSorter : MonoBehaviour {

        [SerializeField] private ShipSystem[] shipSystemsArray;
        [SerializeField] private List<ShipSystem> shipSystems = new List<ShipSystem>();
        [SerializeField] private List<ShipSystem> orderedShipSystems = new List<ShipSystem>();

        public void Sort() {

            shipSystemsArray = GetComponentsInChildren<ShipSystem>();

            // get children that actually contain a system (weapons' children do not [the actual weapons go's])
            foreach (ShipSystem shipSystem in shipSystemsArray)
                if (shipSystem.system != null)
                    shipSystems.Add(shipSystem);

            // order by hud priority
            orderedShipSystems = shipSystems.OrderBy(shipSystem => shipSystem.system.hudPriority).ToList();

            // apply hud priority numbers to the systems' sibling indexes
            for (int i = 0; i < orderedShipSystems.Count; i++)
                orderedShipSystems[i].transform.SetSiblingIndex(i);

            GameManager.GetShipSystems();

        }

    }

}