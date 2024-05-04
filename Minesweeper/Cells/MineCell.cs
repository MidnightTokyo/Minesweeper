using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Cells
{
    public class MineCell : BaseCell
    {
        public static readonly string StaticTextureName = "Textures/Tiles/MineCell";

        public override string TextureName => StaticTextureName;

        public MineCell()
        {
        }
    }
}
