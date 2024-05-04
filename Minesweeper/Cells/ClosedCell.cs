using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Cells
{
    public class ClosedCell : BaseCell
    {
        public static readonly string StaticTextureName = "Textures/Tiles/ClosedCell";

        public override string TextureName => StaticTextureName;

        public ClosedCell()
        {
        }
    }
}
