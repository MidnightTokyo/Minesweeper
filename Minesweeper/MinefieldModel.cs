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
        }

        public BaseCell GetMainCell(int x, int y)
        {
            return _mainCells[x, y];
        }

        public FlagCell GetFlagCell(int x, int y)
        {
            return _flagCells[x, y];
        }
    }
}
