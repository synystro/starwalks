using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

namespace DarkSeas {
    [RequireComponent(typeof(BoxCollider2D))]
    public class Room : MonoBehaviour {

        public delegate void OnTakeDamage();
        public event OnTakeDamage onTakeDamageCallback;
        public delegate void OnRepair();
        public event OnTakeDamage onRepairCallback;
        
        //public delegate void OnRoomFireCheck();
        //public delegate void OnRoomBreachCheck();
        //public event OnRoomFireCheck onRoomFireCheck;
        //public event OnRoomBreachCheck onRoomBreachCheck;

        public float roomIconScale;

        [SerializeField] protected GameObject systemGO;
        [Space(10)]
        [SerializeField] protected GameObject iconPrefab;
        [Space(10)]
        public RoomSystem system;
        [Space(10)]
        [SerializeField] protected int maxHP;
        [Space(10)]
        [SerializeField] protected int currentHP;

        public List<GameObject> Tiles { get { return tiles; } }
        public List<GameObject> TilesOnFire { get { return tilesOnFire; } }
        public List<GameObject> TilesBreached { get { return tilesBreached; } }
        public GameObject Fog { get { return fog; } }
        public GameObject SystemGO { get { return systemGO; } }
        public int MaxHP { get { return maxHP; } set { maxHP = value; } }
        public int CurrentHP { get { return currentHP; } }
        public bool IsHandled { get { return roomSystem.handler != null ? true : false; } }
        [SerializeField] public bool IsDamaged { get { return maxHP > currentHP ? true : false; } }
        public bool IsOnFire { get { return TilesOnFire.Count > 0 ? true : false; } }
        public bool IsBreached { get { return TilesBreached.Count > 0 ? true : false; } }
        public bool IsLeaking { get; set; }
        [SerializeField] public bool HasCrewmember { get { return crewmembers.Count > 0 ? true : false; } }
        
        //public int DamageTaken { get { return maxHP - currentHP; } }
        //public bool TakingDamage = false;

        [Space(20)]
        [SerializeField] public List<GameObject> crewmembers = new List<GameObject>();
        [Space(10)]
        [SerializeField] public GameObject firefighter;
        [SerializeField] public List<GameObject> firefighters = new List<GameObject>();
        [Space(10)]
        [SerializeField] public GameObject breachFixer;
        [SerializeField] public List<GameObject> breachFixers = new List<GameObject>();
        [Space(10)]
        [SerializeField] public GameObject repairer;
        [SerializeField] public List<GameObject> repairers = new List<GameObject>();
        [Space(10)]
        private float extinguishRatio, extinguishFill, extinguishTime;
        private float breachFixingRatio, breachFixingFill, breachFixingTime;
        private float timeToExtinguish = 5f;
        private float timeToFixBreach = 5f;
        [SerializeField] private float repairRatio, repairFill, repairTime;
        private float timeToRepair = 15f;

        protected List<GameObject> tiles = new List<GameObject>();
        protected List<GameObject> tilesOnFire = new List<GameObject>();
        protected List<GameObject> tilesBreached = new List<GameObject>();

        protected Image roomIcon;
        protected ShipSystem roomSystem;
        //protected GameObject roomSystem;
        protected Ship shipManager;


        [SerializeField] private GameObject fog;
        private Collider2D r_collider2d;
        private BoxCollider2D boxCollider2d;
        private Sprite fogSprite;
        private PawnUIManager pawnFirefighterUI;
        private PawnUIManager pawnBreachFixerUI;
        private PawnUIManager pawnRepairUI;

        // camera
        private Camera cam;
        private CameraManager camMgr;

        public ShipSystem System { get { return roomSystem; } }
        public List<Room> neighbourRooms = new List<Room>();
        public Ship Ship { get { return shipManager; } }

        private void OnEnable() {

            SetRoomIcon();
            // refresh rooms's MaxHP and HP
            RefreshHP();
            
        }

        private void RefreshHP() {

            if (roomSystem != null) {
                
                if(roomSystem.system.name == "Weapons")
                return;

                if(!roomSystem.IsDamaged) {
                    maxHP = roomSystem.powerCap;
                    currentHP = maxHP;
                } else
                    maxHP = roomSystem.powerCap;
            }

        }

        public virtual void Awake() {
            // get ship component
            shipManager = GetComponentInParent<Ship>();
            // get collider
            boxCollider2d = GetComponent<BoxCollider2D>();
            r_collider2d = GetComponent<Collider2D>();
            // get the camera components
            cam = Camera.main.GetComponent<Camera>();
            camMgr = Camera.main.GetComponent<CameraManager>();

            // get room's tiles
            GetTiles();
            // get room's icon
            if (this.gameObject.name != "Corridor")
                iconPrefab = Resources.Load<GameObject>("Prefabs/UI/RoomIcon");
            // get room's fog sprite
            fogSprite = Resources.Load<Sprite>("Sprites/fog");

            // generate room's fog
            GenerateFog();

            if (system != null) {
                this.gameObject.name = system.name;
                iconPrefab.gameObject.name = shipManager.gameObject.name + " " + this.gameObject.name + " Icon";

                // instantiate system prefab
                systemGO = Instantiate(system.roomSystemPrefab, Vector3.zero, Quaternion.identity);
                roomSystem = systemGO.GetComponent<ShipSystem>();
                // attach it to the ship's systems parent go
                systemGO.transform.parent = shipManager.systems.transform;
                // set system position to ship's position
                systemGO.transform.position = shipManager.transform.position;
                // set room system on prefab clone
                roomSystem.system = system;
                // set room on system
                roomSystem.Room = this;

                //Instantiate RoomIcon
                roomIcon = Instantiate(iconPrefab, GameManager.BattleUiManager.roomIconCanvas.transform).GetComponent<Image>();
                roomIcon.transform.SetParent(shipManager.roomIconsParent.transform);
                roomIcon.sprite = system.icon;

                SetRoomIcon();

                //TODO: organise this shit below

                onTakeDamageCallback += SetRoomIconColor;
                onRepairCallback += SetRoomIconColor;
                roomIconScale = 1f;                

            }

            extinguishTime = timeToExtinguish;
            breachFixingTime = timeToFixBreach;
            repairTime = timeToRepair;

        }

        public virtual void Start() {

        }

        public virtual void Update() {
            if (roomIcon) {
                roomIcon.transform.position = Camera.main.WorldToScreenPoint(boxCollider2d.bounds.center); // Room icon follow room
            }
        }

        public virtual void TakeDamage(float damage) {

            int roomDamage = Mathf.RoundToInt(damage / 10);
            currentHP -= roomDamage;

            // on damage callback
            if (currentHP < maxHP)
                onTakeDamageCallback?.Invoke();

            // if less than zero, zero
            if (currentHP < 0)
                currentHP = 0;

            if (maxHP <= 0 && systemGO != null) {
                maxHP = 0;
                systemGO.SetActive(false);
            }
        }

        public virtual void ExtinguishOneFire() {

            print("Extinguished 1 fire!");

            if (TilesOnFire.Count > 0) {
                TilesOnFire[0].GetComponent<FloorTile>().ExtinguishFire();
                //CheckForFire();
            }

        }

        public virtual void FixOneBreach() {

            print("Repaired 1 breach!");

            if (TilesBreached.Count > 0) {
                TilesBreached[0].GetComponent<FloorTile>().RemoveBreach();
                shipManager.CheckForBreach();
            }

        }

        public virtual void RepairOneBar() {

            currentHP++;

            onRepairCallback?.Invoke();

            if (currentHP > maxHP)
                currentHP = maxHP;

            if (GameManager.BattleUiManager != null)
                GameManager.BattleUiManager.RefreshPowerUsage();

        }

        public void ExtinguishFire(GameObject _firefighter) {

            firefighters.Add(_firefighter);

            if (firefighters.Count == 1) {
                firefighter = _firefighter;
                pawnFirefighterUI = firefighter.GetComponent<PawnUIManager>();
                StartCoroutine("ExtinguishRoomFire");
            }

        }

        public void StopExtinguishingFire(GameObject _firefighter) {

            if ((firefighters.Count == 0) || (_firefighter != firefighter))
                return;

            pawnFirefighterUI.ToggleProgressBar();
            firefighters.Clear();
            firefighter = null;
            pawnFirefighterUI = null;
            extinguishFill = 0;
            StopCoroutine("ExtinguishRoomFire");

        }

        public void RepairBreach(GameObject _breachFixer) {

            breachFixers.Add(_breachFixer);

            if (breachFixers.Count == 1) {
                breachFixer = _breachFixer;
                pawnBreachFixerUI = breachFixer.GetComponent<PawnUIManager>();
                StartCoroutine("FixRoomBreach");
            }

        }

        public void StopFixingBreach(GameObject _breachfixer) {

            if ((breachFixers.Count == 0) || (_breachfixer != breachFixer))
                return;
                
            // remove repairer and toggle its progress bar
            pawnBreachFixerUI.ToggleProgressBar();
            breachFixers.RemoveAt(0);

            // check if the room is fixed
            if(!IsBreached || breachFixers.Count == 0) {
                breachFixers.Clear();
                breachFixer = null;
                pawnBreachFixerUI = null;
                breachFixingFill = 0;
                StopCoroutine("FixRoomBreach");
            } else if(breachFixers.Count > 0) {
            // if it's not fixed, transfer the job to another unit if there's any
                breachFixer = breachFixers[0];
                pawnBreachFixerUI = breachFixer.GetComponent<PawnUIManager>();
            }

        }

        public void Repair(GameObject _repairer) {

            repairers.Add(_repairer);

            if (repairers.Count == 1) {
                repairer = _repairer;
                pawnRepairUI = repairer.GetComponent<PawnUIManager>();
                StartCoroutine("RepairRoom");
            }


        }

        public void StopRepairProgress(GameObject _repairer) {
            
            if ((repairers.Count == 0) || (_repairer != repairer))
                return;
                
            // remove repairer and toggle its progress bar
            pawnRepairUI.ToggleProgressBar();
            repairers.RemoveAt(0);

            // check if the room is fixed
            if(!IsDamaged || repairers.Count == 0) {
                repairers.Clear();
                repairer = null;
                pawnRepairUI = null;
                repairFill = 0;
                StopCoroutine("RepairRoom");
            } else if(repairers.Count > 0) {
            // if it's not fixed, transfer the job to another unit if there's any
                repairer = repairers[0];
                pawnRepairUI = repairer.GetComponent<PawnUIManager>();
            }

        }

        public void AddCrewmember(GameObject crewmember) {

            crewmembers.Add(crewmember);

            if (!shipManager.isScannerWorking) {
                CrewmembersSetFog();
            }

        }

        public void RemoveCrewmember(GameObject crewmember) {

            crewmembers.Remove(crewmember);

            if(firefighters.Contains(crewmember))
                firefighters.Remove(crewmember);

            if(repairers.Contains(crewmember))
                repairers.Remove(crewmember);

            if(breachFixers.Contains(crewmember))
                breachFixers.Remove(crewmember);

            if (!shipManager.isScannerWorking) {
                CrewmembersSetFog();
            }
        }

        public void SetAlpha(float alphaValue) {
            SpriteRenderer[] children = this.GetComponentsInChildren<SpriteRenderer>();
            Color newColor;
            foreach (SpriteRenderer child in children) {
                newColor = child.color;
                newColor.a = alphaValue;
                child.color = newColor;
            }
        }

        private void CrewmembersSetFog() {

            // if AI, return
            if (shipManager.isHostile)
                return;

            if (crewmembers.Count > 0 && fog != null) {
                fog.SetActive(false);
            } else {
                fog.SetActive(true);
            }

        }

        private void GetTiles() {

            for (int i = 0; i < this.transform.childCount; i++) {
                if (this.transform.GetChild(i).gameObject.GetComponent<FloorTile>()) {
                    GameObject tile = this.transform.GetChild(i).gameObject;
                    tile.GetComponent<FloorTile>().shipManager = shipManager;
                    tiles.Add(tile);
                }
            }

        }

        private void GenerateFog() {

            fog = new GameObject();
            fog.name = this.name + "_Fog";
            fog.transform.SetParent(this.transform);
            fog.transform.localPosition = boxCollider2d.offset;
            fog.transform.localRotation = new Quaternion(0, 0, 0, 0);
            fog.transform.localScale = boxCollider2d.size;

            SpriteRenderer fogSR = fog.AddComponent<SpriteRenderer>();
            fogSR.sprite = fogSprite;
            fogSR.sortingOrder = 99;

            // put it on ship's fog parent 
            fog.transform.SetParent(shipManager.fog.transform);

        }

        private void SetRoomIcon() {

            if (system == null)
                return;

            //AdjustRoomIconScale();

            SetRoomIconColor();

            roomIcon.transform.localScale = new Vector3(1f, 1f, 1f);
            roomIcon.transform.Translate(0, 0, 0);

        }

        private void SetRoomIconColor() {

            if (!roomIcon)
                return;

            Image roomIconImg = roomIcon.GetComponent<Image>();

            // had to use this due to a unity bug (in order to refresh the changed color)
            roomIconImg.DisableSpriteOptimizations();

            if (IsDamaged)
                roomIconImg.color = GameManager.BattleUiManager.damagedColor;
            else
                roomIconImg.color = GameManager.BattleUiManager.roomIconColor;

        }

        // private void AdjustRoomIconScale() {

        //     roomIconScale -= camMgr.scrollValue * -1.5f;

        //     if (roomIconScale > 1.3f)
        //         roomIconScale = 1.3f;
        //     else if (roomIconScale < 0.4f)
        //         roomIconScale = 0.4f;

        //     GameManager.BattleUiManager.roomIconCanvas.GetComponent<CanvasScaler>().scaleFactor = roomIconScale;

        // }

        IEnumerator ExtinguishRoomFire() {

            while (true) {
                // activate firefighter UI
                pawnFirefighterUI.progressBar.transform.gameObject.SetActive(true);

                // reset extinguish ratio
                extinguishRatio = 0;

                // add +1/s per crew in the room to the extinguish ratio
                for (int i = 0; i < firefighters.Count; i++) {
                    firefighters[i].GetComponent<Crewmember>().state = State.Working;
                    extinguishRatio += 1;
                }

                //Debug.Log("Extinguishing at " + extinguishRatio + " ratio.");

                // increase progress bar fill
                extinguishFill += extinguishRatio * Time.deltaTime;

                // fill and refresh the progress bar
                pawnFirefighterUI.progressBarFill.fillAmount = extinguishFill / extinguishTime;

                if (extinguishFill >= extinguishTime) {
                    extinguishFill = 0;
                    ExtinguishOneFire();
                }

                if(!IsOnFire)
                    StopExtinguishingFire(firefighter);

                yield return 0;

            }

        }

        IEnumerator FixRoomBreach() {

            while(true) {
                // activate breach fixer ui
                pawnBreachFixerUI.progressBar.transform.gameObject.SetActive(true);

                // reset breach ratio
                breachFixingRatio = 0;

                for (int i = 0; i < breachFixers.Count; i++) {

                    Crewmember crewmember = breachFixers[i].GetComponent<Crewmember>();

                    crewmember.state = State.Working;

                    if (crewmember.IsLearner)
                        crewmember.crewmemberStats.repairXP += Time.deltaTime;

                    breachFixingRatio += crewmember.crewmemberStats.repairLevel;

                }
                
                    //Debug.Log("Fixing breach at " + breachFixingRatio + " ratio.");

                    breachFixingFill += breachFixingRatio * Time.deltaTime;

                    if (pawnBreachFixerUI != null)
                        pawnBreachFixerUI.progressBarFill.fillAmount = breachFixingFill / breachFixingTime;
                    else
                        Debug.LogError("pawnRepairUI is missing for " + breachFixer.name + ". Was this intended?");

                    if (breachFixingFill >= breachFixingTime) {
                        breachFixingFill = 0;
                        FixOneBreach();
                    }

                    if(!IsBreached)
                        StopFixingBreach(breachFixer);

                    yield return 0;

                }

        }

        IEnumerator RepairRoom() {
            
            while(true) {
                // activate repairer ui
                pawnRepairUI.progressBar.transform.gameObject.SetActive(true);

                // reset repair ratio
                repairRatio = 0;

                for (int i = 0; i < repairers.Count; i++) {

                    Crewmember crewmember = repairers[i].GetComponent<Crewmember>();

                    crewmember.state = State.Working;

                    if (crewmember.IsLearner)
                        crewmember.crewmemberStats.repairXP += Time.deltaTime;

                    repairRatio += crewmember.crewmemberStats.repairLevel;

                }
                
                    //Debug.Log("Repairing at " + repairRatio + " ratio.");

                    repairFill += repairRatio * Time.deltaTime;

                    if (pawnRepairUI != null)
                        pawnRepairUI.progressBarFill.fillAmount = repairFill / repairTime;
                    else
                        Debug.LogError("pawnRepairUI is missing for " + repairer.name + ". Was this intended?");

                    if (repairFill >= repairTime) {
                        repairFill = 0;
                        RepairOneBar();
                    }

                    if(!IsDamaged)
                        StopRepairProgress(repairer);

                    yield return 0;

                }

        }

    }

}
