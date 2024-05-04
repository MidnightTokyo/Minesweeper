using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Cells
{
    public class FlagCell : BaseCell
    {
        public static readonly string StaticTextureName = "Textures/Tiles/FlagCell";

        public override string TextureName => StaticTextureName;

        public FlagCell()
        {
        }
    }
}
