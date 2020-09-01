using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkSeas {

    public class Inventory {

        public delegate void OnItemChanged();
        public OnItemChanged onItemChangedCallback;

        private List<Item> items;
        private List<Item> augs;

        public Inventory() {
            items = new List<Item>();
            augs = new List<Item>();
        }

        public bool Add(Item item) {

            items.Add(item);

            Refresh();

            return true;

        }

        public void Remove(Item item) {

            items.Remove(item);

            Refresh();

        }

        public void Refresh() {
            onItemChangedCallback?.Invoke();
        }

        public List<Item> GetItemList() {
            return items;
        }

        public List<Item> GetAugmentList() {

            augs.Clear();

            for(int i = 0; i < items.Count; i++)
                if(items[i].type == Item.Type.Augmentation)
                    augs.Add(items[i]);

            return augs;
        }
    }
}
