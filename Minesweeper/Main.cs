using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Minesweeper.Cells;
using System.Collections.Generic;

namespace Minesweeper
{
    public class Main : Game
    {
        public const int CELL_SIZE_PX = 32;

        public const int WIDTH = 480;
        public const int HEIGHT = 480;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MouseState _lastMouseState;

        //private min _minefield;

        private readonly Dictionary<string, Texture2D> _cellTextures;
        private Texture2D _numbersTexture;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _cellTextures = new Dictionary<string, Texture2D>();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Window.Title = "MonoGame XNA Minesweeper 2D";

            _graphics.PreferredBackBufferWidth = WIDTH;
            _graphics.PreferredBackBufferHeight = HEIGHT;
            
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), _spriteBatch);

            //Load Cell 2D Textures
            _cellTextures.Add(ClosedCell.StaticTextureName, Content.Load<Texture2D>(ClosedCell.StaticTextureName));
            _cellTextures.Add(OpenedCell.StaticTextureName, Content.Load<Texture2D>(OpenedCell.StaticTextureName));
            _cellTextures.Add(MineCell.StaticTextureName, Content.Load<Texture2D>(MineCell.StaticTextureName));
            _cellTextures.Add(FlagCell.StaticTextureName, Content.Load<Texture2D>(FlagCell.StaticTextureName));

            //Load Numbers 2D Texture
            _numbersTexture = Content.Load<Texture2D>("Textures/Tiles/Numbers");

            //Add game board
            //_minefield = new Minefield(this, 15, 15, _cellTextures, _numbersTexture);
            //_minefield.Initialize();
            //Components.Add(_minefield);

            MinefieldView view = new MinefieldView(this, 15, 15, _cellTextures, _numbersTexture);
            view.Initialize();
            Components.Add(view);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            MouseState mouseState = Mouse.GetState();

            if (_lastMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                _minefield.LeftMouseClick(mouseState.Position);

            if (_lastMouseState.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released)
                _minefield.RightMouseClick(mouseState.Position);

            _lastMouseState = mouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}