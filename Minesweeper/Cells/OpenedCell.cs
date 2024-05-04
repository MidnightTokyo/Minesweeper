using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Cells
{
    public class OpenedCell : BaseCell
    {
        public static readonly string StaticTextureName = "Textures/Tiles/OpenedCell";

        public override string TextureName => StaticTextureName;

        public OpenedCell()
        {
        }
    }
}
