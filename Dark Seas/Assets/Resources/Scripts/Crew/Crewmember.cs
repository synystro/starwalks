using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public enum State {
    Idle,
    Moving,
    HeadingToRoom,
    Working
};

namespace DarkSeas {

    public abstract class Crewmember : MonoBehaviour {

        #region Protected / private

        protected delegate void PeriodicCheck();
        protected delegate void OnEnterRoom();
        protected delegate void OnLeftRoom();
        protected delegate void OnJobFinished();
        protected PeriodicCheck onPeriodicCheck;
        protected OnEnterRoom onEnterRoomCallback;
        protected OnLeftRoom onLeftRoomCallback;
        protected OnJobFinished onJobFinished;
        
        #region PathFinding

        [SerializeField] protected Room roomToGo;

        private bool isDestinationSet;
        private int destinationIndex = 0;
        private Vector3 tapPos;
        protected PathFinding path;
        protected List<Vector2> destinations = new List<Vector2>();

        #endregion

        [SerializeField] protected Ship ship;
        [SerializeField] protected Room currentRoom;    
        [SerializeField] protected Room previousRoom;    
        [SerializeField] protected ShipSystem roomSystem;

        protected PawnUIManager pawnUI;
        
        [SerializeField] protected bool isOnRoom;
        [SerializeField] protected bool isSelectable;
        [SerializeField] protected bool isLearner;
        [SerializeField] protected bool isMachine;

        [SerializeField] public State state {get; set; } 

        [SerializeField] private LayerMask roomLayerMask;

        private float waitingTimer;

        #endregion

        #region Public

        //public space
        public Sprite icon;
        [Space(10)]
        public bool cantMove = false;
        [Space(10)]
        public bool isSelected = false;

        public float curSpeed;
        public float passiveHealRatio = 0f; 

        public UnityEngine.UI.Toggle toggle;        

        // get stats to crewmember
        public CrewmemberStats crewmemberStats;
        #endregion

        #region getters

        public bool IsLearner { get { return isLearner; } }
        public bool IsMachine { get { return isMachine; } }

        #endregion

        public virtual void Awake() {
            // set icon
            icon = GetComponent<SpriteRenderer>().sprite;
        }

        public virtual void Start() {

            // set room layer mask
            roomLayerMask = LayerMask.GetMask("Aimable");

            // set periodic check
            onPeriodicCheck += CheckCurrentRoom;

            // set state
            state = State.Idle;

            // set crewmember's ship
            ship = GetComponentInParent<Ship>();

            ship.onShipDestroyed += Destroy;

            // get pathfinder
            path = this.GetComponent<PathFinding>();
            // get pawn ui manager
            pawnUI = this.GetComponent<PawnUIManager>();     

            isDestinationSet = false;

            // set crewmember initial stats
            SetupMemberStats();

            // set health and progress bar
            SetupUIElements();

        }

        public virtual void Update() {

            // bar follow crewmember
            UiBarFollow();

            // heal crewmember
            PassiveHealthRegen();

            // input
            if (isSelectable)
                ProcessInputs();

            // move crewmember
            Movement();

            // check which room the crewmember is inside at intervals
            waitingTimer -= Time.deltaTime;
            if (waitingTimer <= 0) {
                float maxWaitingTimer = .05f;
                waitingTimer = maxWaitingTimer;
                onPeriodicCheck?.Invoke();
            }

        }

        public virtual void CheckCurrentRoom() {

            Vector2 currentPos = this.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, -Vector2.up, 0.5f, roomLayerMask);

            isOnRoom = false;

            if (hit.collider != null) {

                if (hit.collider.gameObject.CompareTag("Room")) {

                    isOnRoom = true;

                    //currentRoomGO = hit[i].collider.gameObject;

                    currentRoom = hit.transform.GetComponent<Room>();

                    bool alreadyIn = false;

                    // check if already in the room
                    if (currentRoom.crewmembers.Contains(this.gameObject))
                        alreadyIn = true;
                        
                    if (!alreadyIn) {
                        currentRoom.AddCrewmember(this.gameObject);
                        onEnterRoomCallback?.Invoke();
                    }

                    if (currentRoom.SystemGO != null)
                        roomSystem = currentRoom.SystemGO.GetComponent<ShipSystem>();
                    else
                        roomSystem = null;

                }

            }

            if (!isOnRoom && currentRoom != null) {

                // remove crewmember from room
                StopAllActivities();
                onLeftRoomCallback?.Invoke();
                currentRoom.RemoveCrewmember(this.gameObject);

                // not in room anymore
                previousRoom = currentRoom;
                currentRoom = null;
                roomSystem = null;

            }

        }

        private void Destroy() {

            // make sure to destroy this pawn and everything that belongs to it

            StopAllActivities();
            Destroy(pawnUI.healthBar);
            Destroy(pawnUI.progressBar);
            Destroy(this.gameObject);

        }

        private void SetupMemberStats()
        {
            // set current speed to default speed.
            curSpeed = crewmemberStats.speed;

            crewmemberStats.health = crewmemberStats.maxHealth; // set health to equal max health
            
            // gather all skills into a dictionary and basically sets every stat related variable the crewmember has
            crewmemberStats.GatherSkills();
            
        }

        private void SetupUIElements()
        {
            //instantiate healthbar
            pawnUI.healthBar = Instantiate(pawnUI.healthBarPrefab, GameManager.BattleUiManager.crewmemberCanvas.transform).GetComponent<Image>();
            pawnUI.healthBarFill = new List<Image>(pawnUI.healthBar.GetComponentsInChildren<Image>()).Find(img => img != pawnUI.healthBar);
            pawnUI.healthBar.name = crewmemberStats.name + "'s healthbar, of the" + ship.name + " ship";

            //instantiate actionbar
            pawnUI.progressBar = Instantiate(pawnUI.progressBarPrefab, GameManager.BattleUiManager.crewmemberCanvas.transform).GetComponent<Image>();
            pawnUI.progressBarFill = new List<Image>(pawnUI.progressBar.GetComponentsInChildren<Image>()).Find(img => img != pawnUI.progressBar);
            pawnUI.progressBar.name = crewmemberStats.name + "'s progressbar, of the" + ship.name + " ship";
            // hidden by default
            pawnUI.healthBar.gameObject.SetActive(false);
            pawnUI.progressBar.gameObject.SetActive(false);

        }

        private void UiBarFollow()
        {
            //healthbar follow crewmember
            pawnUI.healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0.5f, 0));
            //progressbar follow crewmember
            pawnUI.progressBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.5f, 0, 0));
        }

        private void ApplySkillToSystemRoom() {

            if(roomSystem != null)
                if(roomSystem.handler == null)
                    roomSystem.handler = this;

        }

        private void ProcessInputs() {

            if(isSelected)
                CheckTap();

        }

        private void CheckTap() {

            if (!EventSystem.current.IsPointerOverGameObject()) {

                // mouse 0 click.
                if (Input.GetMouseButtonDown(0))
                    tapPos = Input.mousePosition;
                // mouse 0 release.
                if (Input.GetMouseButtonUp(0)) {

                    // dividing by 100 so it matches unit sizing (i.e. 100f = 1 unit).
                    Vector3 swipeForce = (tapPos - Input.mousePosition) / 100f;
                    float swipeResist = Camera.main.GetComponent<CameraManager>().swipeResist;

                    // check if swipe force is more than swipe resist
                    if (Mathf.Abs(swipeForce.x) > swipeResist || Mathf.Abs(swipeForce.y) > swipeResist)
                        return;

                    //TODO: cancel coroutinesssssssssss

                    // get tap screen to world position.
                    tapPos = Camera.main.ScreenToWorldPoint(tapPos);

                    // find path.
                    if (path.FindPath(this.transform.position, tapPos)) {
                        ClearDestination();
                        path.FindPath(this.transform.position, tapPos);
                    } else {
                        ToggleSelection(isSelected);
                    }

                }
            }
        }

        private void FixedUpdate() {

            #region Follow Path

            if (destinations.Count < 1)
                return;

            if (destinationIndex < destinations.Count) {
                if (destinations[destinationIndex] != (Vector2)transform.position) {
                    transform.position = Vector3.MoveTowards(transform.position, destinations[destinationIndex], curSpeed * Time.deltaTime);
                } else {
                    destinationIndex++;
                }
            }

            if ((Vector2)transform.position == destinations[destinations.Count - 1]) {
                ClearDestination();
            }

            #endregion


        }

        private void Movement() {
            // check if cannot move.
            if (cantMove) {
                curSpeed = 0;
                return;
            }

            // set destination
            if (!isDestinationSet && path.FinalPath != null)
                SetDestination();
        }

        private void PassiveHealthRegen() {

            if (crewmemberStats.health < crewmemberStats.maxHealth && passiveHealRatio > 0)
                Heal(passiveHealRatio * Time.deltaTime);

            if (pawnUI.healthBar)
                RefreshHealthBar();

            if (pawnUI.healthBar.transform.gameObject.activeSelf)
                pawnUI.healthBarFill.fillAmount = crewmemberStats.health / crewmemberStats.maxHealth;
        }

        private void ToggleCanMove() {

            cantMove = !cantMove;

            // make it cannot move.

        }

        private void RefreshHealthBar() {
            //check health status
            if (crewmemberStats.health > crewmemberStats.maxHealth) {
                crewmemberStats.health = crewmemberStats.maxHealth; //check if health is bigger than maxhealth
            } else if (crewmemberStats.health == crewmemberStats.maxHealth && pawnUI.healthBar.transform.gameObject.activeSelf == true) {
                pawnUI.healthBar.transform.gameObject.SetActive(false);// turn off health bar
            } else if (crewmemberStats.health < crewmemberStats.maxHealth == pawnUI.healthBar.transform.gameObject.activeSelf == false) {
                pawnUI.healthBar.transform.gameObject.SetActive(true); // turn on health bar
            }
        }

        private void SetDestination() {

            foreach (Node node in path.FinalPath)
                destinations.Add(new Vector2(node.position.x, node.position.y));

            isDestinationSet = true;
            state = State.Moving;

        }

        protected void ClearDestination() {

            state = State.Idle;

            // remove target
            path.targetTransform = null;
            // remove entire path.
            path.FinalPath = null;
            // destination is not set anymore;
            isDestinationSet = false;
            // clear destination list.
            destinations.Clear();
            // reset destination index.
            destinationIndex = 0;

        }

        protected void ExtinguishFire() {

            state = State.Working;

            currentRoom.ExtinguishFire(this.gameObject);

        }

        protected void StopFireExtinguishing() {

            // if already not working, skip
            if (state != State.Working)
                return;

            // stop fire extinguishing process
            currentRoom.StopExtinguishingFire(this.gameObject);

            // stop extinguishing fire
            state = State.Idle;
            
            // clear any destination if it's a droid
            if(!isSelectable)
                ClearDestination();

            onJobFinished?.Invoke();

        }

        protected void SystemRepair() {

            state = State.Working;

            currentRoom.Repair(this.gameObject);

        }

        protected void StopSystemRepairProgress() {

            // if already not repairing, skip
            if(state != State.Working)
                return;

            // stop room's repair progress
            currentRoom.StopRepairProgress(this.gameObject);

            print("stopping system repair progress on room " + currentRoom);

            // go idle
            state = State.Idle;

            // clear any destination if it's a droid
            if(!isSelectable)
                ClearDestination();

            onJobFinished?.Invoke();

        }

        protected void FixBreach() {

            state = State.Working;

            currentRoom.RepairBreach(this.gameObject);

        }

        protected void StopBreachFixProgress() {

            // if already not fixing breach, skip
            if(state != State.Working)
                return;

            // stop room's fix breach progress
            currentRoom.StopFixingBreach(this.gameObject);

            // go idle
            state = State.Idle;

            // clear any destination if it's a droid
            if(!isSelectable)
                ClearDestination();

            onJobFinished?.Invoke();

        }

        protected void StopAllActivities() {

            #region handle

            if(roomSystem)
                if (roomSystem.handler == this) {
                roomSystem.handler = null;
                roomSystem.RefreshToUI();
                }

            #endregion

            #region system repair

            if (currentRoom.repairer == this.gameObject)
                StopSystemRepairProgress();

            #endregion

            #region breach repair

            if(currentRoom.breachFixer == this.gameObject)
                StopBreachFixProgress();

            #endregion

            #region fire extinguish

            if(currentRoom.firefighter == this.gameObject)
                StopFireExtinguishing();

            #endregion
                
        }

        public void TakeDamage(float damage) {

            crewmemberStats.health -= damage;

            //check if crewmember is dead
            if (crewmemberStats.health <= 0) {

                if(state == State.Working) {
                    StopAllActivities();
                    // remove crewmember from their room
                    currentRoom.crewmembers.Remove(this.gameObject);
                }

                Destroy(this.gameObject);
                Destroy(pawnUI.healthBar.gameObject);
                Destroy(pawnUI.progressBar.gameObject);
                Debug.Log(name + ", from the " + ship.name + " sub, has been killed");
            }
        }

        public void Heal(float heal) {

            crewmemberStats.health += heal;

            if (crewmemberStats.health > crewmemberStats.maxHealth)
                crewmemberStats.health = crewmemberStats.maxHealth;

        }

        public void ToggleSelection(bool _isSelected) {

            int crewNumber = ship.crew.transform.childCount;

            for (int i = 0; i < crewNumber; i++) {
                GameObject crewmemberGO = ship.crew.transform.GetChild(i).gameObject;
                if (crewmemberGO != this.gameObject) {
                    Crewmember crewmember = crewmemberGO.GetComponent<Crewmember>();
                    crewmember.isSelected = false;
                    if (crewmember.toggle != null)
                        crewmember.toggle.isOn = false;
                }
            }

            if (toggle != null)
                toggle.isOn = isSelected = _isSelected;
            else
                isSelected = _isSelected;
        }

    }
}