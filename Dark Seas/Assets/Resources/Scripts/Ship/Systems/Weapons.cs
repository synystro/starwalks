namespace DarkSeas
{
    public class Weapons : ShipSystem
    {
        public int capacity;

        public override void Start()
        {

            base.Start();

            // rotate ship weapons if the ship is rotated
            this.transform.eulerAngles += ship.transform.eulerAngles;

            Refresh();

        }

        public void Refresh() {

            // reset capacity and power consumption
            capacity = 0;
            powerConsumption = 0;

            int weaponsHP = Room.CurrentHP;

            // get capacity
            for (int i = 0; i < this.transform.childCount; i++) {
                ShipSystem weaponSystem = this.transform.GetChild(i).GetComponent<ShipSystem>();
                capacity += weaponSystem.powerCap;
                powerConsumption += weaponSystem.powerConsumption;       
            }

            powerCap = capacity;

        }

    }
}
    
