using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        //buttons===============================================================
        //hitboxes
        private Rectangle bGreenHitbox;
        private Rectangle bRedHitbox;
        private bool isHoveringGreen;
        private bool isHoveringRed;
        private int buttonScale;

        //pay button
        private Texture2D bGreenUp;
        private Texture2D bGreenDown;
        private Texture2D bGreenCurrentSprite;
        private Vector2 greenTextPosition;

        //kick button
        private Texture2D bRedUp;
        private Texture2D bRedDown;
        private Texture2D bRedCurrentSprite;
        private Vector2 redTextPosition;

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

            vm = new VendingMachine();
            vm.CreateVendingMachine();

            buttonScale = 5;
           
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //font
            font = Content.Load<SpriteFont>("Font");

            //background
            bg = Content.Load<Texture2D>("CityBG");

            //sprite
            vmSprite = Content.Load<Texture2D>("VendingMachine");

            //button sprite
            bGreenUp = Content.Load<Texture2D>("buttonFrames/buttonGreenUp");
            bGreenDown = Content.Load<Texture2D>("buttonFrames/buttonGreenDown");
            bRedUp = Content.Load<Texture2D>("buttonFrames/buttonRedUp");
            bRedDown = Content.Load<Texture2D>("buttonFrames/buttonRedDown");

            bGreenCurrentSprite = bGreenUp;
            bRedCurrentSprite = bRedUp;
            greenTextPosition.Y = GraphicsDevice.Viewport.Height - (bGreenUp.Height * buttonScale) - 25;
            redTextPosition.Y = GraphicsDevice.Viewport.Height - (bRedUp.Height * buttonScale) - 25;

        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //mouse
            var mouse = Mouse.GetState();

            //button text position
            greenTextPosition.X = GraphicsDevice.Viewport.Width / 2 - (bGreenUp.Width * buttonScale) - 85;
            redTextPosition.X = GraphicsDevice.Viewport.Width / 2 + 215;

            //button hitboxes
            //pay button
            bGreenHitbox = new Rectangle
            (
                GraphicsDevice.Viewport.Width / 2 - (bGreenUp.Width * buttonScale) - 150,
                GraphicsDevice.Viewport.Height - (bGreenUp.Height * buttonScale) - 50,
                bGreenUp.Width * buttonScale,
                bGreenUp.Height * buttonScale
            );
            //kick button
            bRedHitbox = new Rectangle
            (
                GraphicsDevice.Viewport.Width / 2 + 150,
                GraphicsDevice.Viewport.Height - (bRedUp.Height * buttonScale) - 50,
                bRedUp.Width * buttonScale,
                bRedUp.Height * buttonScale
            );

            isHoveringGreen = bGreenHitbox.Contains(mouse.Position);
            isHoveringRed = bRedHitbox.Contains(mouse.Position);
            if (isHoveringGreen && mouse.LeftButton == ButtonState.Pressed)
            {
                bGreenCurrentSprite = bGreenDown;
                vm.StartGive(Color.Green);
                greenTextPosition.Y = GraphicsDevice.Viewport.Height - (bGreenUp.Height * buttonScale) - 15;
            }
            if (isHoveringRed && mouse.LeftButton == ButtonState.Pressed)
            {
                bRedCurrentSprite = bRedDown;
                vm.StartKick(Color.Red);
                redTextPosition.Y = GraphicsDevice.Viewport.Height - (bRedUp.Height * buttonScale) - 15;
            }
            
            if (mouse.LeftButton == ButtonState.Released)
            {
                bGreenCurrentSprite = bGreenUp;
                bRedCurrentSprite = bRedUp;
                greenTextPosition.Y = GraphicsDevice.Viewport.Height - (bGreenUp.Height * buttonScale) - 25;
                redTextPosition.Y = GraphicsDevice.Viewport.Height - (bRedUp.Height * buttonScale) - 25;
            }

            //machine position
            vm.SetPosition(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, vmSprite);

            // update vending machine animations
            vm.UpdateGive();

            // update kick animation
            vm.UpdateKick();

            // stats drawing handled in Draw()
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            //background
            _spriteBatch.Draw(bg, new Vector2(0, -495), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

            //buttons
            //pay button
            _spriteBatch.Draw
            (
                bGreenCurrentSprite, 
                new Vector2
                (
                    GraphicsDevice.Viewport.Width/2 - (bGreenUp.Width * buttonScale) - 150, 
                    GraphicsDevice.Viewport.Height - (bGreenUp.Height * buttonScale) - 50
                ), 
                null, 
                Color.White, 
                0f, 
                Vector2.Zero, 
                buttonScale, 
                SpriteEffects.None, 
                0f
            );
            _spriteBatch.DrawString(font, "PAY", greenTextPosition, Color.DarkGreen);
            //kick button
            _spriteBatch.Draw
            (
                bRedCurrentSprite,
                new Vector2
                (
                   GraphicsDevice.Viewport.Width / 2 + 150,
                   GraphicsDevice.Viewport.Height - (bRedUp.Height * buttonScale) - 50
                ),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                buttonScale,
                SpriteEffects.None,
                0f
            );
            _spriteBatch.DrawString(font, "KICK", redTextPosition, Color.DarkRed);

            //sprite
            vm.DrawMachine(_spriteBatch, font);

            // Draw give-money text if the vending machine is animating it.
            vm.GiveMoney(_spriteBatch, font, Color.Green);

            //same with kick animation
            vm.Kick(_spriteBatch, font, Color.Red);

            // Draw vending machine stats
            string stats = vm.GetStats();
            _spriteBatch.DrawString(font, stats, new Vector2(10, 10), Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
