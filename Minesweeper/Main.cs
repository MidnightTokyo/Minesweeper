using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Minesweeper.Cells;
using System;
using System.Collections.Generic;

namespace Minesweeper
{
    public class Main : Game
    {
        public const int CELL_SIZE_PX = 32;

        public const int VIRTUAL_WIDTH = 480;
        public const int VIRTUAL_HEIGHT = 480;

        private int _lastWidth;
        private int _lastHeight;

        public static Matrix ScaledMatrix { private set; get; }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //private MouseState _lastMouseState;

        private MinefieldView _minefieldView;

        private readonly Dictionary<string, Texture2D> _cellTextures;
        private Texture2D _numbersTexture;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _cellTextures = new Dictionary<string, Texture2D>();

            ScaledMatrix = Matrix.CreateScale(1.0f);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Window.Title = "MonoGame XNA Minesweeper 2D";
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += ClientSizeChanged;

            _graphics.PreferredBackBufferWidth = VIRTUAL_WIDTH;
            _graphics.PreferredBackBufferHeight = VIRTUAL_HEIGHT;
            
            _graphics.ApplyChanges();

            base.Initialize();
        }

        private void ClientSizeChanged(object sender, EventArgs e)
        {
            if (_lastWidth != Window.ClientBounds.Width)
            {
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            }
            else if (_lastHeight != Window.ClientBounds.Height)
            {
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Height;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            }
            _graphics.ApplyChanges();

            float scaleX = (float)GraphicsDevice.Viewport.Width / VIRTUAL_WIDTH;
            float scaleY = (float)GraphicsDevice.Viewport.Height / VIRTUAL_HEIGHT;

            _lastWidth = Window.ClientBounds.Width;
            _lastHeight = Window.ClientBounds.Height;

            ScaledMatrix = Matrix.CreateScale(scaleX, scaleY, 1.0f);
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

            MinefieldModel minefieldModel = new MinefieldModel(15, 15);
            MinefieldController minefieldController = new MinefieldController(minefieldModel);
            _minefieldView = new MinefieldView(this, minefieldModel, minefieldController, _cellTextures, _numbersTexture);
            _minefieldView.Initialize();
            Components.Add(_minefieldView);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

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