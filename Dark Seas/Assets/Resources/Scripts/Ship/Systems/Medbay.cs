namespace DarkSeas {
    public class Medbay : ShipSystem {
        public float healRatio;

        private const float BASE_HEAL_RATIO = 0.25f;

        public override void Start() {

            base.Start();

        }

        public override void Update() {
            healRatio = powerConsumption * powerConsumption;
        }
    }
}
