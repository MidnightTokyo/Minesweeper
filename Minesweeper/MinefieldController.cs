using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class MinefieldController
    {
        private MinefieldModel _minefieldModel;
        private MinefieldView _minefieldView;

        public MinefieldController(MinefieldModel minefieldModel)
        {
            _minefieldModel = minefieldModel;
        }

        public void SetView(MinefieldView minefieldView)
        {
            _minefieldView = minefieldView;
        }

        public void LeftMouseClick(int posX, int posY)
        {
            if (_minefieldModel.IsLose())
            {
                _minefieldModel.GetFieldSize(out int x, out int y);
                _minefieldModel.StartGame(x, y);
                return;
            }

            _minefieldModel.OpenCell(posX, posY);

            if (_minefieldModel.CheckEnd())
            {
                _minefieldModel.GetFieldSize(out int x, out int y);
                _minefieldModel.StartGame(x, y);
            }
        }

        public void RightMouseClick(int posX, int posY)
        {
            _minefieldModel.MarkCell(posX, posY);
        }
    }
}
