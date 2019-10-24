using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DansCSharpLibrary.Serialization;
using System.IO;
using System;
using System.Collections.Generic;

namespace ClassicGraphics
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D tex;

        PictureManager pm;

        Picture background;
        readonly int PIXELSIZE = 4;
        readonly int PIXELWIDTH = 128;
        readonly int PIXELHEIGHT = 128;
        bool doDraw;
        bool blink;

        List<Keys> accountedKeys;

        int[] worldSize;
        Wall[,] map;
        double[] pos;
        byte facing;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = PIXELWIDTH * PIXELSIZE;
            graphics.PreferredBackBufferWidth = PIXELHEIGHT * PIXELSIZE;
            Content.RootDirectory = "Content";
            accountedKeys = new List<Keys>();

            pm = new PictureManager();

            worldSize = new int[] { 10, 10 };
            map = new Wall[worldSize[0], worldSize[1]];
            pos = new double[] { 4, 4 };
            facing = 0;

            for (int i = 0; i < worldSize[0]; i++)
            {
                map[0, i] = new Wall("save/turtle.pic");
                map[worldSize[0]-1, i] = new Wall("save/alt.pic");
                map[i, 0] = new Wall("save/turtle.pic");
                map[i, worldSize[0]-1] = new Wall("save/alt.pic");
            }
            

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            
            if (!Directory.Exists("save"))
            {
                Directory.CreateDirectory("save");
                float[,][] col = new float[PIXELWIDTH, PIXELHEIGHT][];
                for (float x = 0; x < PIXELWIDTH; x++)
                {
                    for (float y = 0; y < PIXELHEIGHT; y++)
                    {
                        float colo = 0.15F;
                        if (y < PIXELHEIGHT / 2) { colo = 0.30F; }
                        col[(int)x, (int)y] = new float[] { colo, colo, colo };
                    }
                }
                pm.Add("save/background.pic", new Picture(col));
            }

            tex = Content.Load<Texture2D>("wP");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pm.Add("save/background.pic");
            Texture2D texture = Content.Load<Texture2D>("blue");
            pm.Add("save/blue.pic", TexToPic(texture));
            texture = Content.Load<Texture2D>("red");
            pm.Add("save/red.pic", TexToPic(texture));
            texture = Content.Load<Texture2D>("green");
            pm.Add("save/green.pic", TexToPic(texture));
            texture = Content.Load<Texture2D>("yellow");
            pm.Add("save/yellow.pic", TexToPic(texture));
            texture = Content.Load<Texture2D>("Turtle Icon");
            pm.Add("save/turtle.pic", TexToPic(texture));
            texture = Content.Load<Texture2D>("Alt");
            pm.Add("save/alt.pic", TexToPic(texture));

            background = pm.Get("save/background.pic");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Q))
                Exit();

            blink = !blink;

            // Get newly pressed keys
            List<Keys> newKeys = new List<Keys>();
            Keys[] keys = Keyboard.GetState().GetPressedKeys();

            for (int i = 0; i < keys.Length; i++)
            {
                if (!accountedKeys.Contains(keys[i]))
                {
                    newKeys.Add(keys[i]);
                    accountedKeys.Add(keys[i]);
                }
            }
            List<Keys> pressedKeys = new List<Keys>();
            for (int i = 0; i < keys.Length; i++) { pressedKeys.Add(keys[i]); };

            for (int i = 0; i < accountedKeys.Count; i++)
            {
                if (!pressedKeys.Contains(accountedKeys[i]))
                {
                    accountedKeys.RemoveAt(i);
                    i--;
                }
            }


            if (newKeys.Contains(Keys.P))
            {
                doDraw = true;
            }
            if (newKeys.Contains(Keys.O))
            {
                doDraw = false;
            }
            if (accountedKeys.Contains(Keys.A))
            {
                facing -= 2;
            }
            if (accountedKeys.Contains(Keys.D))
            {
                facing += 2;
            }
            if (accountedKeys.Contains(Keys.W))
            {
                double DTOR = Math.PI / 180;
                double[] dC = { Math.Cos((double)facing / byte.MaxValue * 360 * DTOR), Math.Sin((double)facing / byte.MaxValue * 360 * DTOR) };
                pos[0] += dC[0] * 0.08;
                pos[1] += dC[1] * 0.08;
            }
            if (accountedKeys.Contains(Keys.S))
            {
                double DTOR = Math.PI / 180;
                double[] dC = { Math.Cos((double)facing / byte.MaxValue * 360 * DTOR), Math.Sin((double)facing / byte.MaxValue * 360 * DTOR) };
                pos[0] -= dC[0] * 0.08;
                pos[1] -= dC[1] * 0.08;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (accountedKeys.Contains(Keys.K))
            {
                GraphicsDevice.Clear(Color.Black);
            }

            if (!accountedKeys.Contains(Keys.L))
            {

                if (doDraw)
                {

                    DrawBackground();
                    
                    DrawWalls();


                }
            }

            if (blink)
            {
                DrawBlink();

                DrawCoords();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void DrawBlink()
        {
            Rectangle rect = new Rectangle(new Point(30, graphics.PreferredBackBufferHeight - 30), new Point(30, 30));
            spriteBatch.Draw(tex, rect, Color.Purple);
        }

        void DrawBackground()
        {
            Rectangle rect;
            for (int x = 0; x < PIXELWIDTH; x++)
            {
                for (int y = 0; y < PIXELHEIGHT; y++)
                {
                    rect = new Rectangle(new Point(x * PIXELSIZE, y * PIXELSIZE), new Point(PIXELSIZE, PIXELSIZE));
                    float[] colour = background.c[x, y];
                    spriteBatch.Draw(tex, rect, new Color(colour[0], colour[1], colour[2]));
                }
            }
        }

        void DrawCoords()
        {
            byte[] x = BitConverter.GetBytes(pos[0]);
            byte[] y = BitConverter.GetBytes(pos[1]);

            for (int i = 0; i < x.Length; i++)
            {
                Rectangle rect = new Rectangle(new Point(i * PIXELSIZE, graphics.PreferredBackBufferHeight - (2 * PIXELSIZE)), new Point(PIXELSIZE, PIXELSIZE));
                spriteBatch.Draw(tex, rect, new Color((float)x[i] / byte.MaxValue, (float)x[i] / byte.MaxValue, (float)x[i] / byte.MaxValue));
                rect = new Rectangle(new Point(i * PIXELSIZE, graphics.PreferredBackBufferHeight - (3 * PIXELSIZE)), new Point(PIXELSIZE, PIXELSIZE));
                spriteBatch.Draw(tex, rect, new Color((float)y[i] / byte.MaxValue, (float)y[i] / byte.MaxValue, (float)y[i] / byte.MaxValue));
            }
        }
        
        void DrawWalls()
        {
            // Degree to Radians constant
            double convertDToR = Math.PI / 180;
            Rectangle rect;

            // Convert facing from byte to degrees
            double dir = (double)facing;
            dir /= byte.MaxValue;
            dir *= 359;

            // x < PIXELWIDTH
            for (int x = 0; x < PIXELWIDTH; x++)
            {

                // newDir is ray dir in degrees
                double dT = (((double)(x - (PIXELWIDTH / 2)) / PIXELWIDTH) * 60);
                double newDir = dir + dT;
                newDir %= 360;

                bool inBounds(double[] pos, Object[,] arr)
                {
                    return !(pos[0] < 0 || pos[1] < 0 || pos[0] >= arr.GetLength(0) || pos[1] >= arr.GetLength(1));
                }
                if (newDir == 90 || newDir == 270 || newDir == 0 || newDir == 180)
                {
                    // Going straight in one direction
                    if (newDir == 90 || newDir == 270)
                    {
                        sbyte sign = (sbyte)(newDir / 90);
                        sign -= 2;
                        sign *= -1;
                        double[] newPos = { pos[0], pos[1] };

                        bool didDraw = false;
                        
                        while (inBounds(newPos, map))
                        {
                            Wall w = map[(int)newPos[0], (int)newPos[1]];
                            if (w != null)
                            {
                                if (sign < 0)
                                {
                                    newPos[1]++;
                                }
                                newPos[1] = (int)newPos[1];
                                double[] dis = { newPos[0] - pos[0], newPos[1] - pos[1] };
                                didDraw = true;
                                Picture pic = pm.Get(w.tile);
                                printTileLine(x, newPos[0] % 1, GetFauxDistance(newPos, pos, (dT * convertDToR)), pic);
                                break;
                            }

                            newPos[1] += sign;
                        }
                        if (!didDraw)
                        {
                            Picture pic = pm.Get("save/blue.pic");
                            printTileLine(x, newPos[0] % 1, GetFauxDistance(newPos, pos, (dT * convertDToR)), pic);
                        }
                    }
                    else if (newDir == 0)
                    {
                        sbyte sign = (sbyte)(newDir / 90);
                        sign -= 1;
                        sign *= -1;
                        double[] newPos = { pos[0], pos[1] };

                        bool didDraw = false;
                        while (inBounds(newPos, map))
                        {
                            Wall w = map[(int)newPos[0], (int)newPos[1]];
                            if (w != null)
                            {
                                newPos[0] = (int)newPos[0];
                                didDraw = true;
                                Picture pic = pm.Get(w.tile);
                                printTileLine(x, newPos[1] % 1, GetFauxDistance(newPos, pos, (dT * convertDToR)), pic);
                                break;
                            }

                            newPos[0] += sign;
                        }
                        if (!didDraw)
                        {
                            Picture pic = pm.Get("save/blue.pic");
                            printTileLine(x, newPos[1] % 1, GetFauxDistance(newPos, pos, (dT * convertDToR)), pic);
                        }
                    }
                }
                else
                {
                    // newDir to radians
                    newDir *= convertDToR;

                    // y = m*x + b
                    double slope = Math.Tan(newDir);
                    double yInt = pos[1] - (slope * pos[0]);

                    // +- x and +- y
                    int[] sign = { GetSign(Math.Cos(newDir)), GetSign(Math.Sin(newDir)) };


                    // -- Fingers for two finger algorith --
                    // Initial position of X
                    double[] newXF = new double[2];
                    // Update x to next integer
                    newXF[0] = (int)(pos[0] + (0.5D * sign[0]) + 0.5D);
                    newXF[1] = slope * newXF[0] + yInt;
                    // Initial position of Y
                    double[] newYF = new double[2];
                    // Update y to next integer
                    newYF[1] = (int)(pos[1] + (0.5D * sign[1]) + 0.5D);
                    newYF[0] = (newYF[1] - yInt) / slope;

                    // What the Wall is if found
                    Wall w = null;
                    // Where to draw found wall
                    double[] wallPos = { -1, -1 };

                    bool found = false;
                    double drawPos = -1;
                    while (inBounds(newXF, map) && inBounds(newYF, map) && !found)
                    {
                        // Test spot for empty or not
                        if (GetWall(newXF, true) != null)
                        {
                            found = true;
                            wallPos = newXF;
                            drawPos = wallPos[1] % 1;
                            w = GetWall(newXF, true);
                            break;
                        }
                        else if (GetWall(newYF, false) != null)
                        {
                            found = true;
                            wallPos = newYF;
                            drawPos = wallPos[0] % 1;
                            w = GetWall(newYF, false);
                            break;
                        }

                        // If newXF is 'ahead' on line
                        if (GetSign(newXF[0].CompareTo(newYF[0])) != sign[0])
                        {
                            newXF[0] += sign[0];
                            newXF[1] = slope * newXF[0] + yInt;
                        }
                        // Otherwise increment newYF
                        else
                        {
                            newYF[1] += sign[1];
                            newYF[0] = (newYF[1] - yInt)/ slope;
                        }
                    }

                    if (!found)
                    {
                        while (inBounds(newXF, map))
                        {
                            // Test spot for empty or not

                            if (GetWall(newXF, true) != null)
                            {
                                found = true;
                                wallPos = newXF;
                                drawPos = wallPos[1] % 1;
                                w = GetWall(newXF, true);
                                break;
                            }
                            // Increment newXF
                            newXF[0] += sign[0];
                            newXF[1] = slope * newXF[0] + yInt;
                        }
                        while (inBounds(newYF, map))
                        {
                            // Test spot for empty or not
                            if (GetWall(newYF, false) != null)
                            {
                                found = true;
                                wallPos = newYF;
                                drawPos = wallPos[0] % 1;
                                w = GetWall(newYF, false);
                                break;
                            }
                            // Increment newYF
                            newYF[1] += sign[1];
                            newYF[0] = (newYF[1] - yInt) / slope;
                        }
                    }

                    { }

                    if (w != null)
                    {
                        Picture pic = pm.Get(w.tile);
                        printTileLine(x, drawPos, GetFauxDistance(wallPos, pos, (dT * convertDToR)), pic);

                    }
                    // Did not find a wall, draw default blue
                    else
                    {
                        Picture pic = pm.Get("save/blue.pic");
                        printTileLine(x, 0.5, 2, pic);
                    }

                }
            }
        }

        internal double GetFauxDistance(double[] p1, double[] p2, double dT)
        {
            double[] taxiDis = { p2[0] - p1[0], p2[1] - p1[1] };
            double[] powDis = { Math.Pow(taxiDis[0], 2), Math.Pow(taxiDis[1], 2) };
            double absDis = Math.Sqrt(powDis[0] + powDis[1]);
            double fDis = Math.Cos(dT) * absDis;

            return fDis;
        }

        internal int roundDef(double a)
        {
            if (a < 0)
            {
                return (-1) * ((int)(-a + 0.5D));
            }
            return (int)(a + 0.5D);
        }

        internal double specCompTo(double a, double b)
        {
            return a - b;
        }

        internal Wall GetWall(double[] spot, bool xEdge)
        {
            if (xEdge)
            {
                if (0 <= (int)(spot[0] - 0.1) && (int)(spot[0] - 0.1) < map.GetLength(0))
                {

                    if (0 <= (int)(spot[0] + 0.1) && (int)(spot[0] + 0.1) < map.GetLength(0))
                    {
                        if (map[(int)(spot[0] - 0.1), (int)spot[1]] != null && map[(int)(spot[0] + 0.1), (int)spot[1]] != null)
                        {
                            return null;
                        }
                    }

                    if (map[(int)(spot[0] - 0.1), (int)spot[1]] != null)
                    {
                        return map[(int)(spot[0] - 0.1), (int)spot[1]];
                    }
                    else
                    {
                        if (0 <= (int)(spot[0] + 0.1) && (int)(spot[0] + 0.1) < map.GetLength(0))
                        {
                            return map[(int)(spot[0] + 0.1), (int)spot[1]];
                        }
                    }
                }
            }
            else
            {
                if (0 <= (int)(spot[1] - 0.1) && (int)(spot[1] - 0.1) < map.GetLength(1))
                {
                    if (0 <= (int)spot[1] && (int)(spot[1] + 0.1) < map.GetLength(1))
                    {
                        if (map[(int)spot[0], (int)(spot[1] + 0.1)] != null && map[(int)(spot[0]), (int)(spot[1] - 0.1)] != null)
                        return null;
                    }

                    if (map[(int)(spot[0]), (int)(spot[1] - 0.1)] != null)
                    {
                        return map[(int)spot[0], (int)(spot[1] - 0.1)];
                    }
                    else
                    {
                        if (0 <= (int)spot[1] && (int)(spot[1] + 0.1) < map.GetLength(1))
                        {
                            return map[(int)spot[0], (int)(spot[1] + 0.1)];
                        }
                    }
                }
            }

            return null;
        }

        // Returns       1 for d > 0,         0 for d == 0,      -1 for d < 0
        public sbyte GetSign(double d)
        {
            if (d == 0) { return 0; }
            return (sbyte)(d / (Math.Abs(d)));
        }


        void printTileLine(int drawX, double tileX, double distance, Picture pic)
        {

            if (distance < 0.1D)
            {
                distance = 0.1D;
            }

            double height = (PIXELHEIGHT / distance);
            
            for (double i = ((-1) * (height/2)); i < height/2; i++)
            {
                Rectangle rect = new Rectangle(new Point(drawX * PIXELSIZE, (int)(i + (PIXELHEIGHT/2))*PIXELSIZE), new Point(PIXELSIZE, PIXELSIZE));

                int picXPos = (int)(tileX * pic.c.GetLength(0));
                int picYPos = (int)(((i + (height/2)) / height)  * (pic.c.GetLength(1)));

                if (picXPos < 0 || picXPos >= pic.c.GetLength(0) || picYPos < 0 || picYPos >= pic.c.GetLength(1))
                {
                    continue;
                }

                float[] rawCol = pic.c[picXPos, picYPos];
                Color col = new Color(rawCol[0], rawCol[1], rawCol[2]);

                float darkness = (float)(1 / Math.Sqrt(Math.Max(1, distance / 3)));

                col.B = (byte)Math.Min(byte.MaxValue, (((float)col.B) * darkness));
                col.G = (byte)Math.Min(byte.MaxValue, (((float)col.G) * darkness));
                col.R = (byte)Math.Min(byte.MaxValue, (((float)col.R) * darkness));

                spriteBatch.Draw(tex, rect, col);
            }
        }

        void printPixel(String tile, float pos, float distance)
        {
            double height = ((1/Math.Sqrt(distance))/2);

        }

        static Color GetColor(Color[] col, int[] coord, int width)
        {
            return col[coord[0] + (width * coord[1])];
        }

        static Color[] GetColors(Texture2D texture)
        {
            Color[] col = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(col);
            return col;
        }

        static Picture TexToPic(Texture2D texture)
        {
            float[,][] space = new float[texture.Width, texture.Height][];
            Color[] col = GetColors(texture);

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    Color curr = col[x + (y * texture.Width)];
                    space[x, y] = new float[] { ((float)curr.R) /((float)byte.MaxValue),
                                                ((float)curr.G) / ((float)byte.MaxValue),
                                                ((float)curr.B) / ((float)byte.MaxValue) };
                }
            }

            return new Picture(space);
        }
    }
}