using UnityEngine;
    public class Battery : MonoBehaviour, IDrainable<float>, ICollectable
    {
        [SerializeField] private float batteryLife = 100f;

        public float BatteryLife
        {
            get { return batteryLife; }
            set { batteryLife = value; }
        }

        public void Collect()
        {
            //pool manager handle collected 
                        gameObject.SetActive(false);

        }

        public void Drain(float drainAmount)
        {
            BatteryLife -= drainAmount;
        }

        public bool DeadBattery()
        {
            return BatteryLife <= 0;
        }
    }
