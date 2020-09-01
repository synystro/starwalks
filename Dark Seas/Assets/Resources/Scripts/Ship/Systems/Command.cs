using UnityEngine;

namespace DarkSeas
{
    public class Command : ShipSystem
    {
        [SerializeField] private float EVASION_RATIO = 2.5f;

        //[SerializeField] private bool isScanner;

        [SerializeField] private bool isScanningEnemy;

        public override void Start()
        {
            base.Start();

            // set ship's starting command evasion
            //RefreshEvasion();            
            
        }

        public override void Update()
        {
            if (IsPowered) {

                if(!ship.isScannerWorking)
                    DisplayShipInterior();

                if (powerConsumption > 1 && !isScanningEnemy) {
                    DisplayEnemyShipInterior();
                    isScanningEnemy = true;
                } else if (powerConsumption < 2 && isScanningEnemy) {
                    HideEnemyShipInterior();
                }

            } else {
                // reset handler skill level
                crewmemberSkillLevel = 0;
                // if scanner was on, turn it off
                if (ship.isScannerWorking) {
                    HideShipInterior();
                }

                // if enemy scanner was on, turn it off
                if (isScanningEnemy) {
                    HideEnemyShipInterior();
                }

            }
        }

        public override void RefreshToUI()
        {
            RefreshEvasion();
        }

        public void RefreshEvasion()
        {
            if (handler != null && IsPowered)
                crewmemberSkillLevel = handler.crewmemberStats.pilotLevel;
            else
                crewmemberSkillLevel = skillInput;

            ship.pilotEV = EVASION_RATIO * (powerConsumption + crewmemberSkillLevel);

            battleUiManager.RefreshEvasion();
        }

        private void DisplayShipInterior() {

            for(int i = 0; i < ship.rooms.Count; i++) {
                ship.rooms[i].GetComponent<Room>().Fog.SetActive(false);
            }

            // set interior visible to true
            ship.isScannerWorking = true;

        }

        private void HideShipInterior() {

            for (int i = 0; i < ship.rooms.Count; i++) {

                Room room = ship.rooms[i].GetComponent<Room>();
                if (room == null)
                    Debug.LogError(ship.rooms[i] + " is missing ROOM component!");
                // if the room has no crewmembers, activate the fog in order to make the room hidden
                if(!room.HasCrewmember)
                    room.Fog.SetActive(true);

            }

            // set interior visible to false
            ship.isScannerWorking = false;

        }

        private void DisplayEnemyShipInterior() {

            for (int i = 0; i < GameManager.LocationManager.EnemyShips.Count; i++) {

                Ship enemyShip = GameManager.LocationManager.EnemyShips[i].GetComponent<Ship>();

                for (int j = 0; j < enemyShip.rooms.Count; j++) {
                    enemyShip.rooms[j].GetComponent<Room>().Fog.SetActive(false);
                }

            }

            // scanning enemies
            isScanningEnemy = true;

        }

        private void HideEnemyShipInterior() {

            for (int i = 0; i < GameManager.LocationManager.EnemyShips.Count; i++) {

                Ship enemyShip = GameManager.LocationManager.EnemyShips[i].GetComponent<Ship>();

                for (int j = 0; j < enemyShip.rooms.Count; j++) {
                    enemyShip.rooms[j].GetComponent<Room>().Fog.SetActive(true);
                }

            }
            // not scanning enemies anymore
            isScanningEnemy = false;

        }

    }
}
