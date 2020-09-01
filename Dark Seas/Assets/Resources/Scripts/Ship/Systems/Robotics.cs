using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas {

    public class Robotics : ShipSystem {
        
        public float repairRatio;
        private const float BASE_REPAIR_RATIO = 0.5f;

        public override void Start() {

            base.Start();

        }

        public override void Update() {

            repairRatio = BASE_REPAIR_RATIO * powerConsumption;
            
        }
    }

}