using AI_test.Sprites;
using AI_test.Core;
using AI_test.AI_and_Behaviours;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using AI_test.Level_Generation;
using System.Text;
using System;

namespace AI_test
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Player player;
        public Grid grid;
        Agent agent;
        public static int ScreenHeight;
        public static int ScreenWidth;
        public static Game1 _instance;
        public static MouseState mouseState;
        //PathRequestManager pathRequestManager;
        Pathfinding pathfinding;
        AgentBT bT;
        //CycleGenerator cycleGenerator;

        public Game1()
        {
            _instance = this;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ScreenHeight = _graphics.PreferredBackBufferHeight;

            ScreenWidth = _graphics.PreferredBackBufferWidth;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ArtManager.Load(_instance);
            player = new Player(ArtManager.textures["Player"], new Vector2(300, 300));
            grid = new Grid(new Vector2(150, 150), 15, new Vector2(200, 200), ArtManager.textures["Nodes"]);
            pathfinding = new Pathfinding(grid);
            agent = new Agent(ArtManager.textures["Player"], grid.grid[8, 8].Position, 0, grid.grid[1, 1], grid);
            //bT = new AgentBT(agent);
            //cycleGenerator = new CycleGenerator(7);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            EntityManager.Update(gameTime);

            Camera.Follow(player);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.BurlyWood);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, transformMatrix: Camera.Transform);
            EntityManager.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}