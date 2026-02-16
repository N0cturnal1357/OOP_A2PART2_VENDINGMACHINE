using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OOP_A2PART2_VENDINGMACHINE
{
    internal class VendingMachine
    {
        private int hits;
        private int damage;
        private int health;
        private int price;
        private int totalMoneyGiven;
        private bool containsItem;
        private string[] items;
        private string item;
        private int chance;
        private int min;
        private int max;

        //give-money animation state
        private bool isGiving;
        private Vector2 givePosition;
        private Color giveColor;
        private string giveText;
        private float giveAlpha;
        private float giveRisePerFrame = 0.5f;
        private float giveFadePerFrame = 0.02f;

        public void CreateVendingMachine()
        {
            hits = 0;
            totalMoneyGiven = 0;
            health = new Random().Next(10, 50);
            SetItem();
            SetPrice();

            min = 1;
            max = 100; //1-100 chance to get item
        }

        // Return a stats string so drawing can be done from the game's Draw() method
        public string GetStats()
        {
            string stats = 
                $"Health: {health}\n" +
                $"Hits: {hits}\n" +
                $"Damage: {damage}\n" +
                $"Price: {price}\n" +
                $"Total Money Given: {totalMoneyGiven}\n" +
                $"Contains Item: {containsItem}\n" +
                $"Item: {item}\n" +
                $"Chance to Get Item: {min}/{max}%" 
                ;

            return stats;
        }

        public void SetPrice(int min = 1, int max = 11) //1-10
        {
            price = new Random().Next(min, max);
        }

        public void SetItem()
        {
            items = new string[] { "Chips", "Soda", "Candy" };
            int random = new Random().Next(0, items.Length);

            int choose = new Random().Next(0, 1);
            if (choose == 0) containsItem = true;
            else containsItem = false;

            if (containsItem) item = items[random];
            else item = "Nothing";
        }
       
        public void GiveItem()
        {
            if (!containsItem) return;

            SetItem();
            SetPrice();
        }

        public void Kick()
        {
            hits++;
            damage = new Random().Next(1, 10);
        }
        public void GetMoney(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Color color)
        {
            string totalGiven = $"Money Added: {this.totalMoneyGiven}";
            spriteBatch.DrawString(font, totalGiven, position, color);
        }
        //GIVE MONEY STUFF================================================================
        public void GiveMoney(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Color color)
        {
            //Draw the current give-money animation state
            if (!isGiving) return;

            spriteBatch.DrawString(font, giveText, givePosition, giveColor * giveAlpha, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
        }

        //Start the give-money animation (called when the button is pressed)
        public void StartGive(Vector2 position, Color color)
        {
            if (isGiving) return; // already running

            totalMoneyGiven += price;
            giveText = $"+ {price}";
            givePosition = position;
            giveColor = color;
            giveAlpha = 1f;
            isGiving = true;

            //change to give item, if there is one
            if (containsItem)
            {
                //1 in 100 chance to recieve what is in the machine (no guarentee to be what you wanted tho)
                chance = new Random().Next(min, max+1);
                if (chance == max)
                {
                    GiveItem();
                }
                else return;
            }
        }

        //Update the give-money animation call each Update() frame from Game1
        public void UpdateGive()
        {
            if (!isGiving) return;

            givePosition.Y -= giveRisePerFrame;
            giveAlpha -= giveFadePerFrame;
            if (giveAlpha <= 0f)
            {
                giveAlpha = 0f;
                isGiving = false;
                SetPrice(); //Set a new price for the next purchase
            }
        }
    }
}
