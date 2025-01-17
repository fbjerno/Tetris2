﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris2
{
    public class Block
    {
        public Point[] position = new Point[4];
        public Color color;
        public int RotationIndex = 0;

        public float timer = 0.5f;
        public float addTimer = 0.5f;

        public Block()
        {
            CreateBlock();
        }

        public void Update(GameTime gameTime)
        {
            MoveBlockDown(gameTime);
            Controls();
        }

        public void BlockLayout(int whatBlock)
        {
            switch (whatBlock)
            {
                case 0: // IBlock
                    position[0] = new Point(0, 0);
                    position[1] = new Point(-1, 0);
                    position[2] = new Point(1, 0);
                    position[3] = new Point(2, 0);
                    color = Color.CadetBlue;
                    break;
                case 1: // LBlock
                    position[0] = new Point(0, 0);
                    position[1] = new Point(-1, 0);
                    position[2] = new Point(1, 0);
                    position[3] = new Point(1, -1);
                    color = Color.Orange;
                    break;
                case 2: // JBlock
                    position[0] = new Point(0, 0);
                    position[1] = new Point(-1, 0);
                    position[2] = new Point(-1, -1);
                    position[3] = new Point(1, 0);
                    color = Color.Blue;
                    break;
                case 3: // OBlock
                    position[0] = new Point(0, 0);
                    position[1] = new Point(0, -1);
                    position[2] = new Point(1, 0);
                    position[3] = new Point(1, -1);
                    color = Color.Yellow;
                    break;
                case 4: // TBlock
                    position[0] = new Point(0, 0);
                    position[1] = new Point(-1, 0);
                    position[2] = new Point(1, 0);
                    position[3] = new Point(0, -1);
                    color = Color.Purple;
                    break;
                case 5: // ZBlock
                    position[0] = new Point(0, 0);
                    position[1] = new Point(1, 0);
                    position[2] = new Point(0, -1);
                    position[3] = new Point(-1, -1);
                    color = Color.Red;
                    break;
                case 6: // SBlock
                    position[0] = new Point(0, 0);
                    position[1] = new Point(-1, 0);
                    position[2] = new Point(0, -1);
                    position[3] = new Point(1, -1);
                    color = Color.LawnGreen;
                    break;
                default: break;
            }
        }

        public void CreateBlock()
        {
            int rnd = Data.RandomNumber(0, 7);

            BlockLayout(rnd);
            for (int i = 0; i < position.Length; i++)
            {
                position[i] += Data.blockSpawnOffset;
            }
        }

        public void Rotate()
        {
            if (color != Color.Yellow)
            {
                for (int i = 1; i < 4; i++)
                {
                    Point testRotation = position[i];

                    testRotation -= position[0];

                    testRotation = new Point(-testRotation.Y, testRotation.X);

                    testRotation += position[0];

                    if (testRotation.X >= Data.gameWidth && color != Color.CadetBlue || testRotation.X >= Data.gameWidth - 1 && color == Color.CadetBlue)
                    {
                        return;
                    }

                    if (testRotation.X <= -1 && color != Color.CadetBlue || testRotation.X <= -2 && color == Color.CadetBlue)
                    {
                        return;
                    }

                    if (testRotation.Y >= Data.gameHeight || testRotation.Y >= Data.gameHeight - 1 && color == Color.CadetBlue)
                    {
                        return;
                    }

                    if (color != Color.CadetBlue && Data.tileMap[testRotation.X, testRotation.Y].isSolid)
                    {
                        return;
                    }

                    if (color == Color.CadetBlue && testRotation.X !< Data.gameWidth && testRotation.X !> -1 && Data.tileMap[testRotation.X + 1, testRotation.Y].isSolid)
                    {
                        return;
                    }
                }

                if (RotationIndex == 0)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        Point point = position[i];

                        point -= position[0];

                        point = new Point(-point.Y, point.X);

                        point += position[0];

                        position[i] = point;
                    }

                    if (color == Color.CadetBlue)
                    {
                        RotationIndex++;
                    }
                }
                else if (RotationIndex == 1)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        Point point = position[i];

                        point -= position[0];

                        point = new Point(point.Y, -point.X);

                        point += position[0];

                        position[i] = point;
                    }

                    RotationIndex = 0;
                }
            }
        }

        public void Controls()
        {
            var oldPosition = position[1];

            if (Input.HasBeenPressed(Keys.Left) && CanMoveLeft())
            {
                for (int i = 0; i < position.Length; i++)
                {
                    position[i].X -= 1;
                }
            }
            if(Input.HasBeenPressed(Keys.Right) && CanMoveRight())
            {
                for (int i = 0; i < position.Length; i++)
                {
                    position[i].X += 1;
                }
            }

            if (Input.HasBeenPressed(Keys.Down))
            {
                while (CanMoveDown())
                {
                    for (int i = 0; i < position.Length; i++)
                    {
                        position[i].Y += 1;
                    }
                }
                addTimer = 0;
            }

            if (Input.HasBeenPressed(Keys.R))
            {
                Rotate();
            }

            if (oldPosition != position[1])
            {
                if (Input.HasBeenPressed(Keys.Down))
                    Audio.PlayRandomDropSFX();
                else
                    Audio.PlayRandomSFX();
            }
        }

        public void MoveBlockDown(GameTime gameTime)
        {
            if (timer <= 0 && CanMoveDown())
            {
                for (int i = 0; i < position.Length; i++)
                {
                    position[i].Y += 1;
                }

                timer = 0.5f;
            }
            else
            {
                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public bool CanMoveDown()
        {
            for (int i = 0; i < position.Length; i++)
            {
                if (position[i].Y == Data.gameHeight - 1 || Data.tileMap[position[i].X, position[i].Y + 1].isSolid)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanMoveLeft()
        {
            for (int i = 0; i < position.Length; i++)
            {
                if (position[i].X == 0 || Data.tileMap[position[i].X - 1, position[i].Y].isSolid)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanMoveRight()
        {
            for (int i = 0; i < position.Length; i++)
            {
                if (position[i].X == Data.gameWidth - 1 || Data.tileMap[position[i].X + 1, position[i].Y].isSolid)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsBlockMoving(GameTime gameTime)
        {
            if (!CanMoveDown() && addTimer <= 0)
            {
                Data.AddBlockToTileMap();
                addTimer = 0.5f;
                return false;
            }
            else if (!CanMoveDown())
            {
                addTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < position.Length; i++)
            {
                spriteBatch.Draw(Data.tileTexture, new Vector2 (position[i].X * Data.tileMapLocation + Data.tileMapOffset, position[i].Y * Data.tileMapLocation + Data.tileMapOffset), color);
            }
        }
    }
}
