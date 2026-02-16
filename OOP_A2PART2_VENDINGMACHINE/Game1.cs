using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection;
using System.Text;

namespace OOP_A2PART2_VENDINGMACHINE
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //font
        private SpriteFont font;

        //background
        private Texture2D bg;

        //vending machine object
        VendingMachine vm;

        //sprite
        private Texture2D vmSprite;
        private Vector2 vmPosition;
        private int vmScale;
        //machine hitbox
        private Rectangle hitbox;
        //whether the mouse is currently over the vending machine
        private bool vmHovering;
        //1x1 texture used to draw hitboxes/debug rectangles
        private Texture2D pixel;

        //buttons===============================================================
        //pay button
        private Texture2D bGreenUp;
        private Texture2D bGreenDown;
        private bool bIsPressed;
        //button hovering
        private bool bHovering;
        //button hitbox
        private Rectangle bHitbox;
        //button scale
        private int bScale;
        //button position
        private Vector2 bPosition;
        //button text
        private string bGreenText = "PAY";
        private Vector2 bGreenTextPos;

        //kick button





        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //screen size
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();

            vmScale = 5;
            bScale = 5;

            font = Content.Load<SpriteFont>("Font");

            vm = new VendingMachine();
            vm.CreateVendingMachine();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // create a 1x1 white texture for drawing rectangles / hitboxes
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            //background
            bg = Content.Load<Texture2D>("CityBG");

            //sprite
            vmSprite = Content.Load<Texture2D>("VendingMachine");

            //button sprite
            bGreenUp = Content.Load<Texture2D>("buttonFrames/buttonGreenUp");
            bGreenDown = Content.Load<Texture2D>("buttonFrames/buttonGreenDown");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //mouse
            var mouse = Mouse.GetState();

            //machine position
            int vmX = (GraphicsDevice.Viewport.Width - vmSprite.Width * vmScale) / 2;
            int vmY = (GraphicsDevice.Viewport.Height - vmSprite.Height * vmScale) / 2;
            vmPosition = new Vector2(vmX, vmY);

            //machine hitbox
            hitbox = new Rectangle(vmX, vmY, vmSprite.Width * vmScale, vmSprite.Height * vmScale);
            vmHovering = hitbox.Contains(mouse.X, mouse.Y);

            //button position
            int bX = (GraphicsDevice.Viewport.Width - bGreenUp.Width * bScale) / 2 - 100;
            int bY = GraphicsDevice.Viewport.Height - bGreenUp.Height * bScale - 50;
            bPosition = new Vector2(bX, bY);
            
            

            //button hitbox
            bHitbox = new Rectangle((int)bPosition.X, (int)bPosition.Y, bGreenUp.Width * bScale, bGreenUp.Height * bScale);
            bHovering = bHitbox.Contains(mouse.X, mouse.Y);

            //BUTTON FUNCTIONALITY HERE
            if (bHovering && mouse.LeftButton == ButtonState.Pressed)
            {
                bIsPressed = true;
                bGreenTextPos = new Vector2(bPosition.X + (bGreenUp.Width * bScale) / 2 - font.MeasureString(bGreenText).X / 2, bPosition.Y + 32);
            }
            if (mouse.LeftButton == ButtonState.Released)
            {
                // If the button was pressed (press started inside the button),
                // trigger the vending machine give-money animation.
                if (bIsPressed)
                {
                    vm.StartGive(new Vector2(vmPosition.X + vmSprite.Width * vmScale + 10, vmPosition.Y), Color.LimeGreen);
                }

                bIsPressed = false;
                bGreenTextPos = new Vector2(bPosition.X + (bGreenUp.Width * bScale) / 2 - font.MeasureString(bGreenText).X / 2, bPosition.Y + 15);
            }

            // update vending machine animations
            vm.UpdateGive();



            // stats drawing handled in Draw()
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            //background
            _spriteBatch.Draw(bg, new Vector2(0, -495), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

            //sprite
            _spriteBatch.Draw(vmSprite, vmPosition, null, Color.White, 0f, Vector2.Zero, vmScale, SpriteEffects.None, 0f);

            //hitbox
            //show hitbox outline when hovering (debug)
            DrawCollision(hitbox, hitbox.Width, hitbox.Height, 2, Color.Red, vmHovering ? 0.25f : 0f);

            //draw button
            if (bIsPressed) _spriteBatch.Draw(bGreenDown, bPosition, null, Color.White, 0f, Vector2.Zero, bScale, SpriteEffects.None, 0f);
            else _spriteBatch.Draw(bGreenUp, bPosition, null, Color.White, 0f, Vector2.Zero, bScale, SpriteEffects.None, 0f);
            //draw button text
            _spriteBatch.DrawString(font, bGreenText, bGreenTextPos, Color.DarkGreen);

            //show button hitbox when hovering (debug)
            DrawCollision(bHitbox, bHitbox.Width, bHitbox.Height, 2, Color.Green, bHovering ? 0.25f : 0f);


            // Draw give-money text if the vending machine is animating it.
            vm.GiveMoney(_spriteBatch, font, new Vector2(vmPosition.X - 20, vmPosition.Y), Color.Green);

            // Draw vending machine stats
            string stats = vm.GetStats();
            _spriteBatch.DrawString(font, stats, new Vector2(10, 10), Color.White);


            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawCollision(Rectangle hitbox, int width, int height, int thickness, Color color, float alpha)
        {
            //hitbox
            //draw a semi-transparent fill so the hitbox area is visible
            _spriteBatch.Draw(pixel, hitbox, Color.Red * alpha);

            if (alpha != 0f)
            {
                //top
                _spriteBatch.Draw(pixel, new Rectangle(hitbox.Left, hitbox.Top, hitbox.Width, thickness), Color.Red);
                //bottom
                _spriteBatch.Draw(pixel, new Rectangle(hitbox.Left, hitbox.Bottom - thickness, hitbox.Width, thickness), Color.Red);
                //left
                _spriteBatch.Draw(pixel, new Rectangle(hitbox.Left, hitbox.Top, thickness, hitbox.Height), Color.Red);
                //right
                _spriteBatch.Draw(pixel, new Rectangle(hitbox.Right - thickness, hitbox.Top, thickness, hitbox.Height), Color.Red);
            }
        }
    }
}
