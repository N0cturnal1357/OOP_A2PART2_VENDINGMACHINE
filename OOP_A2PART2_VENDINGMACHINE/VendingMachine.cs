using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_A2PART2_VENDINGMACHINE
{
    internal class VendingMachine
    {
        private int hits;
        private int health;
        private int price;
        private int moneyAdded;
        private bool containsItem;
        private string[] items;
        private string item;
        public void createVendingMachine(bool containsItem = false, string item = "Nothing")
        {
            hits = 0;
            moneyAdded = 0;
            health = new Random().Next(10, 50);






            this.containsItem = containsItem;
            if (containsItem)
            {
                setItem();
            }
        }

        public void setItem()
        {
            items = ["Chips", "Soda", "Candy"];
            int random = new Random().Next(0, items.Length);

            item = items[random];
        }

        public void setPrice(int min, int max)
        {
            price = new Random().Next(min, max);
        }
    }
}
