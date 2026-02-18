using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

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

        //kick animation state
        private bool isKicking;
        private Vector2 kickPosition;
        private Color kickColor;
        private string kickText;
        private float kickAlpha;
        private float kickRisePerFrame = 0.5f;
        private float kickFadePerFrame = 0.02f;

        private Vector2 position;
        private Texture2D vmSprite;
        private int vmScale;

        //items gained
        List<string> itemsGained = new List<string>();

        public void CreateVendingMachine()
        {
            hits = 0;
            totalMoneyGiven = 0;
            health = new Random().Next(10, 50);
            SetItem();
            SetPrice();

            min = 1;
            max = 12 * health;
        }

        public void SetPosition(int x, int y, Texture2D sprite)
        {
            vmSprite = sprite;
            vmScale = 5;
            //machine position
            int vmX = (x - vmSprite.Width * vmScale) / 2;
            int vmY = (y - vmSprite.Height * vmScale) / 2;
            position = new Vector2(vmX, vmY);
        }

        public void DrawMachine(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(vmSprite, position, null, Color.White, 0f, Vector2.Zero, vmScale, SpriteEffects.None, 0f);
        }
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
                $"Chance to Get Item: {chance} / {max}\n" +
                $"Items Gained: {string.Join(", ", itemsGained)}"
                ;

            return stats;
        }

        public void SetPrice(int min = 1, int max = 6) //1-5 default range
        {
            price = new Random().Next(min, max);
        }

        public int GetPrice()
        {
            return price;
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
            itemsGained.Add(item);
            SetItem();
            SetPrice();
        }

        //KICK STUFF=====================================================================
        public void Kick(SpriteBatch spriteBatch, SpriteFont font, Color color)
        {
            if (!isKicking) return;

            spriteBatch.DrawString(font, kickText, kickPosition, kickColor * kickAlpha, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
        }
        public void UpdateKick()
        {
           if (!isKicking) return;
            kickPosition.Y -= kickRisePerFrame;
            kickAlpha -= kickFadePerFrame;
            if (kickAlpha <= 0f)
            {
                kickAlpha = 0f;
                isKicking = false;
            }
        }

        public void StartKick(Color color)
        {
            if (isKicking) return; // already running

            health -= damage;
            SetChance();
            kickText = $"- {damage}";
            kickPosition = new Vector2(position.X + vmSprite.Width*vmScale + 30, position.Y);
            kickColor = color;
            kickAlpha = 1f;
            isKicking = true;

            hits++;
            damage = new Random().Next(1, 10);
        }

        //GIVE MONEY STUFF================================================================
        public void GetMoney(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Color color)
        {
            string totalGiven = $"Money Added: {this.totalMoneyGiven}";
            spriteBatch.DrawString(font, totalGiven, position, color);
        }
        public void GiveMoney(SpriteBatch spriteBatch, SpriteFont font, Color color)
        {
            //Draw the current give-money animation state
            if (!isGiving) return;

            spriteBatch.DrawString(font, giveText, givePosition, giveColor * giveAlpha, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
        }

        //Start the give-money animation (called when the button is pressed)
        public void StartGive(Color color)
        {
            if (isGiving) return; // already running

            totalMoneyGiven += price;
            giveText = $"+ {price}";
            givePosition = new Vector2(position.X - 30, position.Y);
            giveColor = color;
            giveAlpha = 1f;
            isGiving = true;

            //change to give item, if there is one
            if (containsItem)
            {
                int randNum = new Random().Next(min, max + 1);
                if (randNum <= chance)
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

        //CHANCE TO GET ITEM STUFF================================================================
        private void SetChance()
        {
            min = 1;
            max = 12 * health; //higher health = lower chance to get item
            chance = new Random().Next(min, max + 1);
        }
    }
}
