using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace DodgeTheBlocksEnhanced
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D playerTexture, blockTexture, powerUpTexture;
        private Vector2 playerPosition;
        private List<Vector2> fallingBlocks;
        private List<Vector2> powerUps;
        private Random random;
        private float speed = 5f;
        private float fallSpeed = 2f;
        private int score = 0;
        private int lives = 3;
        private SpriteFont font;
        private bool gameOver = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            playerPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 50);
            fallingBlocks = new List<Vector2>();
            powerUps = new List<Vector2>();
            random = new Random();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Player texture (Blue)
            playerTexture = new Texture2D(GraphicsDevice, 40, 40);
            Color[] playerColor = new Color[40 * 40];
            for (int i = 0; i < playerColor.Length; i++) playerColor[i] = Color.Blue;
            playerTexture.SetData(playerColor);

            // Falling block texture (Red)
            blockTexture = new Texture2D(GraphicsDevice, 30, 30);
            Color[] blockColor = new Color[30 * 30];
            for (int i = 0; i < blockColor.Length; i++) blockColor[i] = Color.Red;
            blockTexture.SetData(blockColor);

            // Power-up texture (Green)
            powerUpTexture = new Texture2D(GraphicsDevice, 20, 20);
            Color[] powerUpColor = new Color[20 * 20];
            for (int i = 0; i < powerUpColor.Length; i++) powerUpColor[i] = Color.Green;
            powerUpTexture.SetData(powerUpColor);

            font = Content.Load<SpriteFont>("ScoreFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    ResetGame();
                return;
            }

            KeyboardState state = Keyboard.GetState();

            // Move player
            if (state.IsKeyDown(Keys.Left)) playerPosition.X -= speed;
            if (state.IsKeyDown(Keys.Right)) playerPosition.X += speed;

            // Keep player within bounds
            playerPosition.X = Math.Clamp(playerPosition.X, 0, GraphicsDevice.Viewport.Width - 40);

            // Spawn falling blocks at random
            if (random.NextDouble() < 0.02)
                fallingBlocks.Add(new Vector2(random.Next(0, GraphicsDevice.Viewport.Width - 30), 0));

            // Spawn power-ups occasionally
            if (random.NextDouble() < 0.005)
                powerUps.Add(new Vector2(random.Next(0, GraphicsDevice.Viewport.Width - 20), 0));

            // Move falling blocks
            for (int i = 0; i < fallingBlocks.Count; i++)
                fallingBlocks[i] += new Vector2(0, fallSpeed);

            // Move power-ups
            for (int i = 0; i < powerUps.Count; i++)
                powerUps[i] += new Vector2(0, fallSpeed);

            // Collision detection with blocks
            Rectangle playerRect = new Rectangle(playerPosition.ToPoint(), new Point(40, 40));
            for (int i = fallingBlocks.Count - 1; i >= 0; i--)
            {
                Rectangle blockRect = new Rectangle(fallingBlocks[i].ToPoint(), new Point(30, 30));
                if (playerRect.Intersects(blockRect))
                {
                    fallingBlocks.RemoveAt(i);
                    lives--;
                    if (lives <= 0)
                        gameOver = true;
                }
            }

            // Collision detection with power-ups
            for (int i = powerUps.Count - 1; i >= 0; i--)
            {
                Rectangle powerUpRect = new Rectangle(powerUps[i].ToPoint(), new Point(20, 20));
                if (playerRect.Intersects(powerUpRect))
                {
                    powerUps.RemoveAt(i);
                    ActivatePowerUp();
                }
            }

            // Increase difficulty over time
            if (score % 100 == 0) fallSpeed += 0.1f;

            score++;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Draw player
            _spriteBatch.Draw(playerTexture, playerPosition, Color.White);

            // Draw falling blocks
            foreach (var block in fallingBlocks)
                _spriteBatch.Draw(blockTexture, block, Color.White);

            // Draw power-ups
            foreach (var powerUp in powerUps)
                _spriteBatch.Draw(powerUpTexture, powerUp, Color.White);

            // Draw score and lives
            _spriteBatch.DrawString(font, $"Score: {score}", new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(font, $"Lives: {lives}", new Vector2(10, 40), Color.White);

            if (gameOver)
            {
                _spriteBatch.DrawString(font, "Game Over! Press Enter to Restart", new Vector2(100, 200), Color.Red);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ResetGame()
        {
            score = 0;
            lives = 3;
            fallingBlocks.Clear();
            powerUps.Clear();
            playerPosition = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height - 50);
            gameOver = false;
            fallSpeed = 2f;
        }

        private void ActivatePowerUp()
        {
            int effect = random.Next(0, 2);
            if (effect == 0)
            {
                // Slow time effect
                fallSpeed *= 0.7f;
            }
            else
            {
                // Shrink player
                playerTexture = new Texture2D(GraphicsDevice, 20, 20);
                Color[] playerColor = new Color[20 * 20];
                for (int i = 0; i < playerColor.Length; i++) playerColor[i] = Color.Blue;
                playerTexture.SetData(playerColor);
            }
        }
    }
}