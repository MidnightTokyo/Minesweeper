using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minesweeper.Cells;
using System;
using System.Collections.Generic;
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

        private SpriteBatch _spriteBatch;
        private Vector2 _fieldStartPosition;

        private Rectangle[,] _fieldGrid;

        private Dictionary<string, Texture2D> _cellTextures;
        private Texture2D _numbersTexture;

        public MinefieldView(Game game, MinefieldModel minefieldModel, Dictionary<string, Texture2D> cellTextures, Texture2D numbersTexture) : base(game)
        {
            _minefieldModel = minefieldModel;

            _spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            _minefieldModel.GetFieldSize(out int fieldWidth, out int fieldHeight);

            Vector2 windowCenter = new Vector2(Main.WIDTH / 2, Main.HEIGHT / 2);
            Vector2 fieldCenter = new Vector2((fieldWidth * Main.CELL_SIZE_PX) / 2, (fieldHeight * Main.CELL_SIZE_PX) / 2);

            _fieldStartPosition = new Vector2(windowCenter.X - fieldCenter.X, windowCenter.Y - fieldCenter.Y);

            _fieldGrid = new Rectangle[fieldWidth, fieldHeight];
        }

        public override void Initialize()
        {
            MakeGrid();

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
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Rectangle rectangle;
            BaseCell cell;
            for (int x = 0; x < _fieldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _fieldGrid.GetLength(1); y++)
                {
                    rectangle = _fieldGrid[x, y];
                    cell = _minefieldModel.GetMainCell(x,y);

                    if (cell == null) continue;

                    _spriteBatch.Draw(_cellTextures[cell.TextureName], rectangle, Color.White);

                    /*if (_lose && _mineCells[x, y] != null)
                    {
                        MineCell mineCell = _mineCells[x, y];
                        _spriteBatch.Draw(_cellTextures[mineCell.TextureName], rectangle, Color.White);
                    }*/

                    if (_flagCells[x, y] != null)
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
    }
}
