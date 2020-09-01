using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas
{
    public class GameSettings : MonoBehaviour
    {
        public GameObject BattleGO;
        public TopBarManager TopBarManager;
        public GalaxyManager GalaxyManager;
        public SectorManager SectorManager;
        public LocationManager LocationManager;
        public BattleUIManager BattleUiManager;
        [Space(10)]
        public InventoryUI InventoryUI;
        [Space(10)]
        public Ship ship;
        public GameObject shipSilhouette;
        [Space(10)]
        public List<GameObject> shipSystems = new List<GameObject>();
        [Space(10)]
        public List<GameObject> shipWeapons = new List<GameObject>();
        [Space(10)]
        public List<Zone> zones = new List<Zone>();

        public void Update() {
            if(Input.GetMouseButtonDown(1)) {
                GameManager.PlayerJump();
            }
        }

        private void Awake()
        {
            // set managers
            GameManager.TopBarManager = TopBarManager;
            GameManager.GalaxyManager = GalaxyManager;
            GameManager.SectorManager = SectorManager;
            GameManager.LocationManager = LocationManager;
            GameManager.BattleUiManager = BattleUiManager;

            // set UI elements
            GameManager.InventoryUI = InventoryUI;

            // set language
            GameManager.language = "EN-UK";

            // set battle go
            GameManager.BattleGO = BattleGO;

            // set ship
            GameManager.ship = ship;
            GameManager.shipSilhoette = shipSilhouette;

            // assign zone and set their locations
            GameManager.zones = zones;
            AddLocationsToZones();

        }

        private void AddLocationsToZones() {
            for(int i = 0; i < GameManager.zones.Count; i++) {
                GameManager.zones[i].locations.Clear();
                string zoneName = GameManager.zones[i].name.ToString();
                Location[] locations = Resources.LoadAll<Location>("In-Game/Locations/" + zoneName);

                if(locations.Length < 1) {
                    Debug.LogError("Locations missing on folder corresponding to the " + zoneName + " zone!");
                    return;
                }
                
                for(int j = 0; j < locations.Length; j++)
                    GameManager.zones[i].locations.Add(locations[j]);
            }            
        }

        private void GetShipWeapons()
        {

            GameManager.shipWeapons.Clear();

            foreach (Transform weapon in ship.arsenal.transform)
            {

                GameManager.shipWeapons.Add(weapon.gameObject);

            }

        }

        private void GetShipSystems()
        {

            GameManager.shipSystems.Clear();

            foreach (Transform system in ship.systems.transform)
            {

                GameManager.shipSystems.Add(system.gameObject);

            }

        }

    }
}
