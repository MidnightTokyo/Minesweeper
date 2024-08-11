using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Minesweeper.Cells;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class MinefieldView : DrawableGameComponent
    {
        private readonly Dictionary<int, Color> _numberColors = new Dictionary<int, Color>()
        {
            { 1, Color.Blue },
            { 2, Color.Green },
            { 3, Color.Red },
            { 4, Color.DarkGreen },
            { 5, Color.Brown },
            { 6, Color.Cyan },
            { 7, Color.Black },
            { 8, Color.White }
        };

        private MinefieldModel _minefieldModel;
        private MinefieldController _minefieldController;

        private SpriteBatch _spriteBatch;
        private Vector2 _fieldStartPosition;
        private MouseState _lastMouseState;

        private Rectangle[,] _fieldGrid;

        private Dictionary<string, Texture2D> _cellTextures;
        private Texture2D _numbersTexture;

        public MinefieldView(Game game, MinefieldModel minefieldModel, MinefieldController minefieldController, Dictionary<string, Texture2D> cellTextures, Texture2D numbersTexture) : base(game)
        {

            _minefieldModel = minefieldModel;
            _minefieldController = minefieldController;
            _minefieldController.SetView(this);

            _spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            _minefieldModel.GetFieldSize(out int fieldWidth, out int fieldHeight);

            Vector2 windowCenter = new Vector2(Main.VIRTUAL_WIDTH / 2, Main.VIRTUAL_HEIGHT / 2);
            Vector2 fieldCenter = new Vector2((fieldWidth * Main.CELL_SIZE_PX) / 2, (fieldHeight * Main.CELL_SIZE_PX) / 2);

            _fieldStartPosition = new Vector2(windowCenter.X - fieldCenter.X, windowCenter.Y - fieldCenter.Y);

            _fieldGrid = new Rectangle[fieldWidth, fieldHeight];
            MakeGrid();

            _cellTextures = cellTextures;
            _numbersTexture = numbersTexture;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        private void MakeGrid()
        {
            Rectangle rectangle;

            for (int x = 0; x < _fieldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _fieldGrid.GetLength(1); y++)
                {
                    rectangle = new Rectangle((int)(_fieldStartPosition.X + (x * Main.CELL_SIZE_PX)), (int)(_fieldStartPosition.Y + (y * Main.CELL_SIZE_PX)), Main.CELL_SIZE_PX, Main.CELL_SIZE_PX);
                    _fieldGrid[x, y] = rectangle;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Main.ScaledMatrix);

            Rectangle rectangle;
            BaseCell mainCell;
            MineCell mineCell;
            FlagCell flagCell;
            for (int x = 0; x < _fieldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _fieldGrid.GetLength(1); y++)
                {
                    rectangle = _fieldGrid[x, y];

                    mainCell = _minefieldModel.GetMainCell(x, y);

                    if (mainCell == null) continue;

                    _spriteBatch.Draw(_cellTextures[mainCell.TextureName], rectangle, Color.White);
                    
                    mineCell = _minefieldModel.GetMineCell(x, y);
                    if (_minefieldModel.IsLose() && mineCell != null)
                    {
                        _spriteBatch.Draw(_cellTextures[mineCell.TextureName], rectangle, Color.White);
                    }

                    flagCell = _minefieldModel.GetFlagCell(x, y);

                    if (flagCell != null)
                    {
                        _spriteBatch.Draw(_cellTextures[flagCell.TextureName], rectangle, Color.White);
                    }

                    if (mainCell is OpenedCell && mineCell == null)
                    {
                        int mineCount = _minefieldModel.GetMinesCountNearbyCell(x, y);

                        if (mineCount > 0 && mineCount < 9)
                        {
                            _spriteBatch.Draw(_numbersTexture, rectangle, new Rectangle(16 * mineCount, 0, 16, 16), _numberColors[mineCount]);
                        }
                    }
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePostion = new Vector2(mouseState.X, mouseState.Y);

            mousePostion = Vector2.Transform(mousePostion, Matrix.Invert(Main.ScaledMatrix));

            if (_lastMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                for (int x = 0; x < _fieldGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < _fieldGrid.GetLength(1); y++)
                    {
                        if (_fieldGrid[x, y].Contains(mousePostion))
                        {
                            _minefieldController.LeftMouseClick(x, y);
                            break;
                        }
                    }
                }
            }

            if (_lastMouseState.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released)
            {
                for (int x = 0; x < _fieldGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < _fieldGrid.GetLength(1); y++)
                    {
                        if (_fieldGrid[x, y].Contains(mousePostion))
                        {
                            _minefieldController.RightMouseClick(x, y);
                            break;
                        }
                    }
                }
            }

            _lastMouseState = mouseState;

            base.Update(gameTime);
        }
    }
}
