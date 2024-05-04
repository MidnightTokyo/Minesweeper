using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Minesweeper.Cells;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class Minefield : DrawableGameComponent
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

        private SpriteBatch _spriteBatch;
        private Vector2 _fieldStartPosition;

        private Dictionary<string, Texture2D> _cellTextures;
        private Texture2D _numbersTexture;

        private Rectangle[,] _fieldGrid;
        private BaseCell[,] _mainCells;
        private MineCell[,] _mineCells;
        private FlagCell[,] _flagCells;
        private bool _firstOpen;
        private bool _lose;

        private Random _random;

        public Minefield(Game game, int fieldWidth, int fieldHeight, Dictionary<string, Texture2D> cellTextures, Texture2D numbersTexture) : base(game)
        {
            _spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            Vector2 windowCenter = new Vector2(Main.WIDTH / 2, Main.HEIGHT / 2);
            _fieldStartPosition = new Vector2(windowCenter.X - ((fieldWidth * Main.CELL_SIZE) / 2), windowCenter.Y - ((fieldHeight * Main.CELL_SIZE) / 2));

            _numbersTexture = numbersTexture;
            _cellTextures = cellTextures;

            _fieldGrid = new Rectangle[fieldWidth, fieldHeight];
            _mainCells = new BaseCell[fieldWidth, fieldHeight];
            _mineCells = new MineCell[fieldWidth, fieldHeight];
            _flagCells = new FlagCell[fieldWidth, fieldHeight];
            _firstOpen = true;
            _lose = false;

            _random = new Random();
        }

        private void MakeGrid()
        {
            Rectangle rectangle;

            for (int x = 0; x < _fieldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _fieldGrid.GetLength(1); y++)
                {
                    rectangle = new Rectangle((int)(_fieldStartPosition.X + (x * Main.CELL_SIZE)), (int)(_fieldStartPosition.Y + (y * Main.CELL_SIZE)), Main.CELL_SIZE, Main.CELL_SIZE);
                    _fieldGrid[x, y] = rectangle;
                }
            }
        }

        public override void Initialize()
        {
            MakeGrid();

            ClearField();

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Rectangle rectangle;
            BaseCell cell;
            for (int x = 0; x < _mainCells.GetLength(0); x++)
            {
                for (int y = 0; y < _mainCells.GetLength(1); y++)
                {
                    rectangle = _fieldGrid[x, y];
                    cell = _mainCells[x, y];

                    _spriteBatch.Draw(_cellTextures[cell.TextureName], rectangle, Color.White);

                    if (_lose && _mineCells[x, y] != null)
                    {
                        MineCell mineCell = _mineCells[x, y];
                        _spriteBatch.Draw(_cellTextures[mineCell.TextureName], rectangle, Color.White);
                    }

                    if (_flagCells[x,y] != null)
                    {
                        _spriteBatch.Draw(_cellTextures[FlagCell.StaticTextureName], rectangle, Color.White);
                    }

                    if (cell is OpenedCell && _mineCells[x, y] == null)
                    {
                        int mineCount = GetMineCountNerbyCell(x, y);

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

        public void LeftMouseClick(Point pos)
        {
            if (_lose)
            {
                ClearField();
                return;
            }

            for (int x = 0; x < _fieldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _fieldGrid.GetLength(1); y++)
                {
                    if (_fieldGrid[x, y].Contains(pos))
                    {
                        if (_mainCells[x, y] is OpenedCell) return;

                        if (OpenCell(x, y))
                        {
                            OpenEmptyCellsNearbyCell(x, y);

                            if (CheckEnd())
                                Game.Exit();
                        }

                        return;
                    }
                }
            }
        }

        public void RightMouseClick(Point pos)
        {
            for (int x = 0; x < _fieldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _fieldGrid.GetLength(1); y++)
                {
                    if (_fieldGrid[x, y].Contains(pos))
                    {
                        if (_flagCells[x, y] != null)
                        {
                            _flagCells[x, y] = null;
                        }
                        else if (_mainCells[x, y] is ClosedCell)
                        {
                            _flagCells[x, y] = new FlagCell();
                        }

                        return;
                    }
                }
            }
        }

        private bool OpenCell(int posX, int posY)
        {
            _mainCells[posX, posY] = new OpenedCell();

            if (_firstOpen)
            {
                _firstOpen = false;
                PlaceMines();
            }

            if (_flagCells[posX, posY] != null)
            {
                _flagCells[posX, posY] = null;
            }

            if (_mineCells[posX, posY] != null)
            {
                _lose = true;
                return false;
            }

            return true;
        }

        private void OpenEmptyCellsNearbyCell(int posX, int posY)
        {
            if (GetMineCountNerbyCell(posX,posY) > 0)
            {
                OpenCell(posX, posY);
                return;
            }

            for (int x = posX - 1; x < posX + 2; x++)
            {
                for (int y = posY - 1; y < posY + 2; y++)
                {
                    if (x == posX && y == posY) continue;

                    if (x < 0 || x >= _mainCells.GetLength(0) || y < 0 || y >= _mainCells.GetLength(1))
                        continue;

                    if (_mainCells[x, y] is OpenedCell) continue;

                    if (_mineCells[x, y] == null)
                    {
                        OpenCell(x, y);
                        if (GetMineCountNerbyCell(x, y) < 1)
                        {
                            OpenEmptyCellsNearbyCell(x, y);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private int GetMineCountNerbyCell(int posX, int posY)
        {
            int mines = 0;

            for (int x = posX - 1; x < posX + 2; x++)
            {
                for (int y = posY - 1; y < posY + 2; y++)
                {
                    if (x == posX && y == posY) continue;

                    if (x < 0 || x >= _mineCells.GetLength(0) || y < 0 || y >= _mineCells.GetLength(1)) 
                        continue;

                    if (_mineCells[x, y] != null) 
                        mines++;
                }
            }

            return mines;
        }

        private void ClearField()
        {
            _firstOpen = true;
            _lose = false;

            ClosedCell cell;

            for (int x = 0; x < _mainCells.GetLength(0); x++)
            {
                for (int y = 0; y < _mainCells.GetLength(1); y++)
                {
                    cell = new ClosedCell();
                    _mainCells[x, y] = cell;
                }
            }

            _mineCells = new MineCell[_mineCells.GetLength(0), _mineCells.GetLength(1)];
            _flagCells = new FlagCell[_flagCells.GetLength(0), _flagCells.GetLength(1)];
        }

        private void PlaceMines()
        {
            int mines = ((int)(_mineCells.GetLength(0) * _mineCells.GetLength(1) * 0.15));

            MineCell mineCell;
            while(mines > 0)
            {
                for (int x = 0; x < _mineCells.GetLength(0); x++)
                {
                    for (int y = 0; y < _mineCells.GetLength(1); y++)
                    {
                        mineCell = _mineCells[x, y];

                        if (mineCell != null) continue;

                        if (_random.Next(0, 2) != 1) continue;

                        mineCell = new MineCell();
                        
                        _mineCells[x, y] = mineCell;

                        mines--;

                        x = _random.Next(0, _mineCells.GetLength(0));

                        if (mines < 1) return;
                    }
                }
            }
        }

        private bool CheckEnd()
        {
            for (int x = 0; x < _mainCells.GetLength(0); x++)
            {
                for (int y = 0; y < _mainCells.GetLength(1); y++)
                {
                    if (_mainCells[x, y] is ClosedCell && _mineCells[x, y] == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
