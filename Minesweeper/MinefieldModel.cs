using Minesweeper.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class MinefieldModel
    {
        private BaseCell[,] _mainCells;
        private MineCell[,] _mineCells;
        private FlagCell[,] _flagCells;
        private bool _firstOpen;
        private bool _lose;

        private Random _random;

        public MinefieldModel(int fieldWidth, int fieldHeight)
        {
            StartGame(fieldHeight, fieldWidth);
        }

        public void GetFieldSize(out int x, out int y)
        {
            x = _mainCells.GetLength(0);
            y = _mainCells.GetLength(1);
        }

        public void StartGame(int fieldWidth, int fieldHeight)
        {
            _mainCells = new BaseCell[fieldWidth, fieldHeight];
            _mineCells = new MineCell[fieldWidth, fieldHeight];
            _flagCells = new FlagCell[fieldWidth, fieldHeight];
            _firstOpen = true;
            _lose = false;
            _random = new Random();

            ClearField();
        }

        public BaseCell GetMainCell(int x, int y)
        {
            return _mainCells[x, y];
        }

        public MineCell GetMineCell(int x, int y)
        {
            return _mineCells[x, y];
        }

        public FlagCell GetFlagCell(int x, int y)
        {
            return _flagCells[x, y];
        }
        
        public bool IsLose()
        {
            return _lose;
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
            while (mines > 0)
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

        public void OpenCell(int posX, int posY)
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
            }
            else
            {
                OpenEmptyCellsNearbyCell(posX, posY);
            }
        }

        public void MarkCell(int posX, int posY)
        {
            if (_flagCells[posX, posY] != null)
            {
                _flagCells[posX, posY] = null;
            }
            else if (_mainCells[posX, posY] is ClosedCell)
            {
                _flagCells[posX, posY] = new FlagCell();
            }
        }

        private void OpenEmptyCell(int posX, int posY)
        {
            _mainCells[posX, posY] = new OpenedCell();

            if (_flagCells[posX, posY] != null)
            {
                _flagCells[posX, posY] = null;
            }
        }

        private void OpenEmptyCellsNearbyCell(int posX, int posY)
        {
            if (GetMinesCountNearbyCell(posX, posY) > 0)
            {
                OpenEmptyCell(posX, posY);
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
                        OpenEmptyCell(x, y);
                        if (GetMinesCountNearbyCell(x, y) < 1)
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

        public int GetMinesCountNearbyCell(int posX, int posY)
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

        public bool CheckEnd()
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
