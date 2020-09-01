using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace DarkSeas
{
    public class Ship : MonoBehaviour {

        public delegate void OnShipDestroyed();
        public event OnShipDestroyed onShipDestroyed;

        public new string name;
        [Space(10)]
        public float hull, maxHull;
        public float armor;
        public float evasion;
        [Space(10)]
        public float pilotEV;
        public float engineEV;
        [Space(10)]
        public int reactor;
        [Space(20)]
        public int fuel;
        public int ammo;
        public int scraps;
        [Space(20)]
        public int weaponsCapacity;
        [Space(20)]
        public GameObject command;
        public GameObject arsenal;
        public GameObject shield;
        public GameObject engine;
        public GameObject oxygen;
        public GameObject medbay;
        public GameObject cloaking;
        public GameObject robotics;
        [Space(20)]
        public CustomGrid pathGrid;
        public GameObject body;
        public GameObject shell;
        public GameObject interior;
        public GameObject crew;
        public GameObject droids;
        public GameObject systems;
        public GameObject roomsParent;
        public GameObject fog;
        [Space(20)]
        public Vector2[] weaponSlotsPositions;
        [Space(20)]
        public bool IsBreached;
        public List<GameObject> rooms = new List<GameObject>();
        [Space(20)]
        public int tileHP;
        [Space(20)]
        public bool isPlayer;
        [Space(10)]
        public bool isHostile;
        [Space(10)]
        public bool isScannerWorking;
        [Space(20)]
        public GameObject collectiblePrefab;
        [Space(10)]
        [Header("Active Effects")]
        [Space(10)]
        public ShipEffects shipEffects;

        public WeaponsRoom WeaponsRoom { get; private set; }

        public GameObject roomIconsParent;

        private Inventory cargo;
        private Inventory augments;

        private const int MAX_CREWMEMBERS_NUMBER = 10;

        private void OnEnable() {

            // iterate through all shipStats variables and set them to zero
            Type type = typeof(ShipEffects);
            FieldInfo[] properties = type.GetFields();
            foreach (FieldInfo property in properties) {
                property.SetValue(shipEffects, 0);
            }

        }

        private void OnDestroy() {
            Destroy(roomIconsParent);
        }

        private void Awake()
        {
            // initialise cargo
            cargo = new Inventory();

            // if player ship, set inventory to gamemanager's inventoryui
            if (isPlayer)
                if (GameManager.InventoryUI != null && GameManager.InventoryUI.gameObject.activeSelf)
                    GameManager.InventoryUI.SetInventory(cargo);
                else
                    Debug.LogWarning("The InventoryUI is either not set or it's inactive for the " + this.name + " ship");

            // initialise augments
            augments = new Inventory();

            // interior starts visible
            isScannerWorking = true;

            // add ship rooms to rooms list
            for (int i = 0; i < roomsParent.transform.childCount; i++)
                rooms.Add(roomsParent.transform.GetChild(i).gameObject);

            // get weapons room
            WeaponsRoom = GetComponentInChildren<WeaponsRoom>();

            // create rooms icons parent on battle ui manager
            roomIconsParent = new GameObject();
            roomIconsParent.name = this.name;
            GameObject roomIconsCanvasGO = GameManager.BattleUiManager.roomIconCanvas;
            roomIconsParent.transform.SetParent(roomIconsCanvasGO.transform);

            // generate grid
            if (isPlayer)
                pathGrid.Generate(this);

        }

        private void Start()
        {
            // get ship's essential systems
            arsenal = GetComponentInChildren<Weapons>().gameObject;
            shield = GetComponentInChildren<Shield>().gameObject;
            engine = GetComponentInChildren<Engine>().gameObject;
            command = GetComponentInChildren<Command>().gameObject;
            oxygen = GetComponentInChildren<Oxygen>().gameObject;

            // get ship's optional systems

            // med bay
            if (GetComponentInChildren<Medbay>() != null)
                medbay = GetComponentInChildren<Medbay>().gameObject;

            // cloaking device
            if (GetComponentInChildren<Cloaking>() != null)
                cloaking = GetComponentInChildren<Cloaking>().gameObject;

            // robotics
            if (GetComponentInChildren<Robotics>() != null)
                robotics = GetComponentInChildren<Robotics>().gameObject;

            // set hull to maximum capacity
            hull = maxHull;

            // if it's the player, add listener for deafeat and set weapons and shield to the gamemanager
            if (isPlayer) {

                GameManager.onPlayerDefeatCallback += OnPlayerDefeat;

                //call ship systems
                GameManager.GetShipWeapons();
                //GameManager.GetShipSystems();
                GetComponentInChildren<SystemSorter>().Sort();
                GameManager.ShipShield = shield.GetComponent<Shield>();

                if (GameManager.BattleUiManager != null) {
                    GameManager.BattleUiManager.Refresh();
                }

            }

        }

        private void OnPlayerDefeat() {

            print("PLAYER DEFEATED");
            GameManager.onPlayerDefeatCallback -= OnPlayerDefeat;

        }

        public void ScrapsTransaction(int _scraps) {
            scraps += _scraps;
            
            // if less than zero, zero
            if (scraps <= 0)
                scraps = 0;
        }

        public void ReceiveItem(Item item) {
                cargo.Add(item);
                print(item.name + " added to " + this.name + "'s ship cargo");
        }

        public void ReceiveItems(Item[] items) {
            for (int i = 0; i < items.Length; i++) {
                print(cargo);
                cargo.Add(items[i]);
                print(items[i] + " added to " + this.name + "'s ship cargo");
            }
        }

        public void ReceiveCrew(GameObject[] crewmembers) {

            int crewmembersToReceive = crewmembers.Length;

            for (int i = 0; i < crewmembersToReceive; i++) {

                int currentCrewCount = crew.transform.childCount;

                if (currentCrewCount < MAX_CREWMEMBERS_NUMBER) {

                    GameObject crewmember = Instantiate(crewmembers[i], transform.position, Quaternion.identity);
                    crewmember.transform.SetParent(crew.transform);
                    print(crewmembers[i].name + " added to your ship!");

                } else {

                    // SHIP CREW IS FULL
                    print("Ship is full of crewmembers! Max of " + MAX_CREWMEMBERS_NUMBER);

                }

            }

            // refresh UI and add functionality to crew buttons
            //if(GameManager.InBattle)
            GameManager.ReceiveCrewCallback();

        }

        public void RefreshShipArmor() {

            // set bonus evasion
            float finalArmor = (armor + shipEffects.Armor) * (1 + shipEffects.Armor_P);

            armor = finalArmor;

            if (armor > 100f)
                armor = 100f;
            else if (armor < 0f)
                armor = 0f;

        }

        public void RefreshShipEvasion() {

            // set bonus evasion
            float finalCaptainEV = (pilotEV + shipEffects.PilotEV) * (1 + shipEffects.CaptainEV_P);
            float finalEngineEV = (engineEV + shipEffects.EngineEV) * (1 + shipEffects.EngineEV_P);
            float finalCloakingEV = shipEffects.CloakingEV * (1 + shipEffects.CloakingEV_P);

            float bonusEV = finalCaptainEV + finalEngineEV + finalCloakingEV;

            evasion = bonusEV;

            if (evasion > 100f)
                evasion = 100f;
            else if (evasion < 0f)
                evasion = 0f;

        }

        public void CheckForBreach() {

            IsBreached = false;

            // check for breach on every room
            for (int i = 0; i < rooms.Count; i++) {
                Room room = rooms[i].GetComponent<Room>();
                if (room.IsBreached) {
                    IsBreached = true;
                }
            }

        }

        public void TakeDamage(float damage)
        {
            // damage ship's hull
            hull -= damage;

            // if it's the player
            if (isPlayer) {

                // update hull bar
                if(GameManager.BattleUiManager != null)
                    GameManager.BattleUiManager.RefreshHullIntegrity();

                // check for player's defeat
                if (hull <= 0) {
                    GameManager.Defeat();
                }

            }

            // handle destruction

            if (hull <= 0) {
                onShipDestroyed?.Invoke();
            }

        }

        public void SetAlpha(float alpha) {
            SpriteRenderer[] children = body.GetComponentsInChildren<SpriteRenderer>();
            Color newColor;
            foreach (SpriteRenderer child in children) {
                if (child.GetComponent<Shield>() == null) {
                    newColor = child.color;
                    newColor.a = alpha;
                    child.color = newColor;
                }
            }
        }

        public Inventory Cargo { get { return cargo; } }
        public Inventory Augments { get { return augments; } }

    }
}
