using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Net;

namespace DarkSeas {
    public static class GameManager {

        // values
        public static int MAX_POWER_LIMIT = 6;
        // delegates
        public delegate void OnPlayerJump();
        public static event OnPlayerJump onPlayerJump;
        public delegate void OnPlayerDefeat();
        public static event OnPlayerDefeat onPlayerDefeatCallback;
        public delegate void OnReceiveCrew();
        public static event OnReceiveCrew onReceiveCrew;

        // time
        public static DateTime gameStart;
        public static DateTime firstBattleStart;

        // Managers
        public static TopBarManager TopBarManager;
        public static GalaxyManager GalaxyManager;
        public static SectorManager SectorManager;
        public static LocationManager LocationManager;
        public static BattleUIManager BattleUiManager;

        // UI
        public static InventoryUI InventoryUI;

        // language
        public static string language;

        //controls
        //public static GameObject[] currentSelection = new GameObject[0];

        // ship
        public static Ship ship;
        public static GameObject shipSilhoette;
        // ship systems
        public static List<GameObject> shipSystems = new List<GameObject>();
        public static List<GameObject> shipWeapons = new List<GameObject>();
        public static Shield ShipShield;

        // battle
        public static GameObject BattleGO;
        public static bool InBattle;
        public static bool InShop;

        // zone
        public static Zone currentZone;
        public static GameObject currentLocationGO;
        public static List<Zone> zones = new List<Zone>();

        private const float resumeValue = 1f;
        private const float pauseValue = 0f;

        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T SelectRandomArrayContent<T>(T[] type) {
            return type[rng.Next(0,type.Length)];
        }

        public static int RGN(int intLimit) {
            int rgn = rng.Next(0, intLimit);
            return rgn;
        }

        public static Item RandomItem() {
            ItemsAssetPack itemAssetPack =  Resources.Load<ItemsAssetPack>("In-Game/Items/ItemAssetPack");
            Item[] items = itemAssetPack.items;
            Item item = items[RGN(items.Length)];
            return item;
        }

        public static DateTime GetNetTime() {

            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            var response = myHttpWebRequest.GetResponse();
            string todaysDates = response.Headers["date"];

            return DateTime.ParseExact(todaysDates,
            "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
            CultureInfo.InvariantCulture.DateTimeFormat,
            DateTimeStyles.AssumeUniversal);

        }

        public static TimeSpan TimeDifference(DateTime a, DateTime b) {

            TimeSpan elapsedTime = a - b;

            return a - b;

        }

        public static void OpenInventoryUI() {
                      
        }

        public static void PauseGame() {
            Time.timeScale = pauseValue;
        }

        public static void ResumeGame() {
            Time.timeScale = resumeValue;
        }

        public static void PlayerJump() {
            onPlayerJump?.Invoke();
        }

        public static void Defeat() {
            // TODO: clear GameManager settings
            onPlayerDefeatCallback?.Invoke();

        }

        public static void ReceiveCrewCallback() {
            
            onReceiveCrew?.Invoke();

        }

        public static void GetShipWeapons() {

            GameManager.shipWeapons.Clear();

            foreach (Transform weapon in ship.arsenal.transform) {

                GameManager.shipWeapons.Add(weapon.gameObject);

            }

        }

        public static void GetShipSystems() {

            GameManager.shipSystems.Clear();

            foreach (Transform system in ship.systems.transform) {

                GameManager.shipSystems.Add(system.gameObject);

            }

        }


    }
}
