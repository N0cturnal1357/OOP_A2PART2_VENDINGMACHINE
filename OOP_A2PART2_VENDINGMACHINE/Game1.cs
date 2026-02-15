using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OOP_A2PART2_VENDINGMACHINE
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //background
        private Texture2D bg;

        //sprite
        private Texture2D vmSprite;
        private Vector2 vmPosition;
        private int vmScale;

        //hitbox
        private Rectangle hitbox;
        //whether the mouse is currently over the vending machine
        private bool isHovering;
        //1x1 texture used to draw hitboxes/debug rectangles
        private Texture2D pixel;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //screen size
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();

            //background
            bg = Content.Load<Texture2D>("CityBG");

            //sprite
            vmSprite = Content.Load<Texture2D>("VendingMachine");

            //sprite position
            vmScale = 5;
            int vmX = (GraphicsDevice.Viewport.Width - vmSprite.Width*vmScale) / 2;
            int vmY = (GraphicsDevice.Viewport.Height - vmSprite.Height*vmScale) / 2;
            vmPosition = new Vector2(vmX, vmY);

            //hitbox
            hitbox = new Rectangle(vmX, vmY, vmSprite.Width * vmScale, vmSprite.Height * vmScale);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // create a 1x1 white texture for drawing rectangles / hitboxes
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // check mouse hover over vending machine
            var mouse = Mouse.GetState();
            isHovering = hitbox.Contains(mouse.X, mouse.Y);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            //background
            _spriteBatch.Draw(bg, new Vector2(0, -495), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

            //sprite
            _spriteBatch.Draw(vmSprite, vmPosition, null, Color.White, 0f, Vector2.Zero, vmScale, SpriteEffects.None, 0f);

            //hitbox
            // show hitbox outline when hovering (alpha > 0) otherwise hide
            DrawCollision(hitbox.Width, hitbox.Height, 2, Color.Red, isHovering ? 0.25f : 0f);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawCollision(int width, int height, int thickness, Color color, float alpha)
        {
            //hitbox
            // draw a semi-transparent fill so the hitbox area is visible
            _spriteBatch.Draw(pixel, hitbox, Color.Red * alpha);

            if (alpha != 0f)
            {
                // top
                _spriteBatch.Draw(pixel, new Rectangle(hitbox.Left, hitbox.Top, hitbox.Width, thickness), Color.Red);
                // bottom
                _spriteBatch.Draw(pixel, new Rectangle(hitbox.Left, hitbox.Bottom - thickness, hitbox.Width, thickness), Color.Red);
                // left
                _spriteBatch.Draw(pixel, new Rectangle(hitbox.Left, hitbox.Top, thickness, hitbox.Height), Color.Red);
                // right
                _spriteBatch.Draw(pixel, new Rectangle(hitbox.Right - thickness, hitbox.Top, thickness, hitbox.Height), Color.Red);
            }
        }

    }
}
