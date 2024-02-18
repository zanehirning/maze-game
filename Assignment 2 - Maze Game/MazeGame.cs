using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assignment_2___Maze_Game.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Formats.Asn1.AsnWriter;
using static Assignment_2___Maze_Game.Maze;

namespace Assignment_2___Maze_Game
{
    public class MazeGame : Game
    {
        private Character character = new Character(0, 0); //may not need
        private GraphicsDeviceManager graphics;

        private GraphicsDeviceManager m_graphics;
        private SpriteBatch m_spriteBatch;

        private Maze _maze = new Maze(5);
        private List<List<Maze.Cell>> _gameMaze;
        private Stack<Maze.Cell> shortestPath;
        private List<Maze.Cell> previouslyVisited = new List<Maze.Cell>();

        private int tileSize; //Size that will be divided by maze size to get tile size
        private int BUSH_HEIGHT = 100; //MAZE_SIZE/TILESIZE
        private int characterWidth;
        private int characterHeight;
        private int characterWidthCellPos;
        private int characterHeightCellPos;
        private int textureSize; //Breadcrumbs and shortest path textures
        private const int MAZE_SIZE = 700; // 700 x 700

        private int widthMid;
        private int heightMid;
        private int mazeStartPosX;
        private int mazeStartPosY;

        private Boolean showHint = false;
        private Boolean showBreadcrumbs = false;
        private Boolean showShortestPath = false;
        private Boolean showHighscore = false;
        private Boolean showCredits = false;
        private int highScore;
        private int currentScore;
        private double currentTime;

        private const String CREDITS_STRING_1 = "Credits:";
        private const String CREDITS_STRING_2 = "Created by: Zane Hirning";

        private const String F1_COMMAND_STRING = "F1: New Game 5x5";
        private const String F2_COMMAND_STRING = "F2: New Game 10x10";
        private const String F3_COMMAND_STRING = "F3: New Game 15x15";
        private const String F4_COMMAND_STRING = "F4: New Game 20x20";
        private const String F5_COMMAND_STRING = "F5: Display High Scores";
        private const String F6_COMMAND_STRING = "F6: Display Credits";

        private String highScoreString;
        private String currentScoreString;
        private String timeString;

        //Textures
        private Texture2D m_texTileOne;
        private Texture2D m_texTileTwo;
        private Texture2D m_texSamurai;
        private Texture2D m_texBushWall;
        private Texture2D m_texFighter;
        private Texture2D m_texBloodDroplet;
        private Texture2D m_texBreadcrumbBlur;
        private Texture2D m_texBackground;
        private SpriteFont m_font;

        //Rectangle bounds
        private Rectangle m_rectTileTwo;
        private Rectangle m_rectBushWall;
        private Rectangle m_rectSamurai;
        private Rectangle m_rectFighter;
        private Rectangle m_rectBloodDroplet;
        private Rectangle m_rectBreadcrumbBlur;
        private Rectangle m_rectBackground;

        //Keyboard
        private KeyboardInput m_inputKeyboard;

        public MazeGame()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _gameMaze = _maze.getMaze();
            shortestPath = _maze.dfs();
        }

        protected override void Initialize()
        {
            m_graphics.PreferredBackBufferWidth = 1920;
            m_graphics.PreferredBackBufferHeight = 1080;
            m_graphics.ApplyChanges();

            highScore = 0;
            currentScore = 0;
            currentTime = 0;
            calculate(); // calculate proportions

            highScoreString = $"Highscore: {highScore}";
            currentScoreString = $"Current Score: {currentScore}";
            timeString = $"Time: {string.Format("{0:0.###} seconds", (currentTime/1000).ToString())}"; 

            m_inputKeyboard = new KeyboardInput();

            m_inputKeyboard.registerCommand(Keys.W, true, new IInputDevice.CommandDelegate(onMoveUp));
            m_inputKeyboard.registerCommand(Keys.S, true, new IInputDevice.CommandDelegate(onMoveDown));
            m_inputKeyboard.registerCommand(Keys.A, true, new IInputDevice.CommandDelegate(onMoveLeft));
            m_inputKeyboard.registerCommand(Keys.D, true, new IInputDevice.CommandDelegate(onMoveRight));

            m_inputKeyboard.registerCommand(Keys.Up, true, new IInputDevice.CommandDelegate(onMoveUp));
            m_inputKeyboard.registerCommand(Keys.Down, true, new IInputDevice.CommandDelegate(onMoveDown));
            m_inputKeyboard.registerCommand(Keys.Left, true, new IInputDevice.CommandDelegate(onMoveLeft));
            m_inputKeyboard.registerCommand(Keys.Right, true, new IInputDevice.CommandDelegate(onMoveRight));

            m_inputKeyboard.registerCommand(Keys.I, true, new IInputDevice.CommandDelegate(onMoveUp));
            m_inputKeyboard.registerCommand(Keys.K, true, new IInputDevice.CommandDelegate(onMoveDown));
            m_inputKeyboard.registerCommand(Keys.J, true, new IInputDevice.CommandDelegate(onMoveLeft));
            m_inputKeyboard.registerCommand(Keys.L, true, new IInputDevice.CommandDelegate(onMoveRight));

            m_inputKeyboard.registerCommand(Keys.B, true, new IInputDevice.CommandDelegate(toggleBreadCrumbs));
            m_inputKeyboard.registerCommand(Keys.H, true, new IInputDevice.CommandDelegate(toggleHint));
            m_inputKeyboard.registerCommand(Keys.P, true, new IInputDevice.CommandDelegate(toggleShortestPath));

            m_inputKeyboard.registerGameCommand(Keys.F1, true, new IInputDevice.GameDelegate(resetGame), 5);
            m_inputKeyboard.registerGameCommand(Keys.F2, true, new IInputDevice.GameDelegate(resetGame), 10);
            m_inputKeyboard.registerGameCommand(Keys.F3, true, new IInputDevice.GameDelegate(resetGame), 15);
            m_inputKeyboard.registerGameCommand(Keys.F4, true, new IInputDevice.GameDelegate(resetGame), 20);
            m_inputKeyboard.registerCommand(Keys.F5, true, new IInputDevice.CommandDelegate(toggleHighscore));
            m_inputKeyboard.registerCommand(Keys.F6, true, new IInputDevice.CommandDelegate(toggleCredits));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);

            m_texTileOne = this.Content.Load<Texture2D>("Images/Tile_1");
            m_texTileTwo = this.Content.Load<Texture2D>("Images/Tile_2");
            m_texSamurai = this.Content.Load<Texture2D>("Images/samurai_single");
            m_texBushWall = this.Content.Load<Texture2D>("Images/Bush_Wall");
            m_texFighter = this.Content.Load<Texture2D>("Images/fighter_single");
            m_texBloodDroplet = this.Content.Load<Texture2D>("Images/Blood_Droplet");
            m_texBreadcrumbBlur = this.Content.Load<Texture2D>("Images/Breadcrumb_blur");
            m_texBackground = this.Content.Load<Texture2D>("Images/background");
            m_font = this.Content.Load<SpriteFont>("Font/Font1");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Game Over
            if (character.x == _gameMaze.Count - 1 && character.y == _gameMaze[0].Count - 1)
            {
                resetGame(_gameMaze.Count);
            }
            currentTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            timeString = $"Time: {string.Format("{0:0.#} seconds", (currentTime / 1000).ToString("F1"))}";
            currentScoreString = $"Current Score: {currentScore}";
            processInput();      

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            m_spriteBatch.Begin();
            drawBackground();
            for (int row = 0; row < _gameMaze.Count; row++)
            {
                for (int col = 0; col < _gameMaze[0].Count; col++)
                {
                    drawCell(_gameMaze[row][col]);
                }
            }
            m_spriteBatch.Draw(m_texFighter, m_rectFighter, Color.White);
            if (showBreadcrumbs)
            {
                drawBreadcrumbs();
            }
            if (showShortestPath)
            {
                drawShortestPath();
            }
            if (showHint)
            {
                drawHint();
            }
            if (showHighscore)
            {
                drawHighscore();
            }
            if (showCredits)
            {
                drawCredits();
            }
            drawCurrentScore();
            drawTime();
            drawKeybinds();
            drawCharacter();
            m_spriteBatch.End();
            base.Draw(gameTime);
        }

        private void resetGame(int dimension)
        {
            _maze = new Maze(dimension);
            _gameMaze = _maze.getMaze();
            shortestPath = _maze.dfs();
            previouslyVisited.Clear();
            character.x = 0;
            character.y = 0;
            highScore = currentScore > highScore ? currentScore : highScore;
            currentScore = 0;
            currentTime = 0;
            calculate();
            highScoreString = $"Highscore: {highScore}";
        }

        private void calculate()
        {
            widthMid = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;
            heightMid = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
            tileSize = MAZE_SIZE / _gameMaze.Count;
            // Seemingly random numbers are how I wanted it to look, then made into a ratio :)
            BUSH_HEIGHT = Convert.ToInt32(tileSize * .1429);
            characterHeight = Convert.ToInt32(tileSize * .6857);
            characterWidth = Convert.ToInt32(characterHeight * .625); //aspect ration 5:8
            characterWidthCellPos = (tileSize - characterWidth) / 2; //Also doubles as the middle for my textures
            characterHeightCellPos = (tileSize - characterHeight) / 2;
            textureSize = Convert.ToInt32(tileSize * .4571);
            mazeStartPosX = widthMid - (MAZE_SIZE/ 2);
            mazeStartPosY = heightMid- (MAZE_SIZE/ 2);

            m_rectTileTwo = new Rectangle(mazeStartPosX, mazeStartPosY, tileSize, tileSize);
            m_rectBushWall = new Rectangle(mazeStartPosX, mazeStartPosY, tileSize, BUSH_HEIGHT);
            m_rectSamurai = new Rectangle(mazeStartPosX + characterWidthCellPos, mazeStartPosY + characterHeightCellPos, characterWidth, characterHeight);
            m_rectFighter = new Rectangle(mazeStartPosX + characterWidthCellPos + (tileSize * (_gameMaze.Count - 1)), mazeStartPosY + characterHeightCellPos + (tileSize * (_gameMaze.Count - 1)), characterWidth, characterHeight);
            m_rectBloodDroplet = new Rectangle(characterWidthCellPos, characterWidthCellPos, textureSize, textureSize);
            m_rectBreadcrumbBlur = new Rectangle(characterWidthCellPos, characterWidthCellPos, textureSize, textureSize);
            m_rectBackground = new Rectangle(0, 0, widthMid * 2, heightMid * 2);
        }

        #region Drawing 
        private void drawCell(Maze.Cell cell)
        {
            //Cell
            m_spriteBatch.Draw(
                m_texTileTwo,
                new Rectangle(mazeStartPosX + (cell.y * m_rectTileTwo.Width), mazeStartPosY + (cell.x * m_rectTileTwo.Height), m_rectTileTwo.Width, m_rectTileTwo.Height),
                Color.White
            );

            //Walls
            if (cell.topNeighbor == null)
            {
                m_spriteBatch.Draw(
                    m_texBushWall,
                    new Rectangle(mazeStartPosX + (cell.y * m_rectTileTwo.Width), mazeStartPosY + (cell.x * m_rectTileTwo.Height), m_rectBushWall.Width, m_rectBushWall.Height),
                    Color.White
                );
            }
            if (cell.bottomNeighbor == null)
            {
                m_spriteBatch.Draw(
                    m_texBushWall,
                    new Rectangle(mazeStartPosX + (cell.y * m_rectTileTwo.Width), mazeStartPosY + ((cell.x + 1) * m_rectTileTwo.Height) - m_rectBushWall.Height, m_rectBushWall.Width, m_rectBushWall.Height),
                    Color.White
                );
            }
            if (cell.leftNeighbor == null)
            {
                float rotationAngle = MathHelper.PiOver2; // 90 degrees in radians
                m_spriteBatch.Draw(
                    m_texBushWall,
                    new Rectangle(mazeStartPosX + (cell.y * m_rectTileTwo.Width) + (m_rectBushWall.Height / 2), mazeStartPosY + (cell.x * m_rectTileTwo.Height) + (m_rectBushWall.Width / 2), m_rectBushWall.Width, m_rectBushWall.Height),
                    null,
                    Color.White,
                    rotationAngle,
                    new Vector2(m_texBushWall.Width / 2, m_texBushWall.Height / 2),
                    SpriteEffects.None,
                    0
                );
            }
            if (cell.rightNeighbor == null)
            {
                float rotationAngle = -MathHelper.PiOver2; // -90 degrees in radians
                m_spriteBatch.Draw(
                    m_texBushWall,
                    new Rectangle(mazeStartPosX + ((cell.y + 1) * m_rectTileTwo.Width) - (m_rectBushWall.Height / 2), mazeStartPosY + (cell.x * m_rectTileTwo.Height) + (m_rectBushWall.Width / 2), m_rectBushWall.Width, m_rectBushWall.Height),
                    null,
                    Color.White,
                    rotationAngle,
                    new Vector2(m_texBushWall.Width / 2, m_texBushWall.Height / 2),
                    SpriteEffects.None,
                    0
                );
            }
        }
        private void drawCharacter()
        {
            //may move calculations into character class
            m_spriteBatch.Draw(
                m_texSamurai,
                new Rectangle(mazeStartPosX + (characterWidthCellPos + (tileSize * character.y)), mazeStartPosY + (characterHeightCellPos + (tileSize * character.x)), m_rectSamurai.Width, m_rectSamurai.Height),
                Color.White
            );
        }
        private void drawBreadcrumbs()
        {
            foreach (Maze.Cell cell in previouslyVisited)
            {
                m_spriteBatch.Draw(
                        m_texBreadcrumbBlur,
                        new Rectangle(mazeStartPosX + characterWidthCellPos + (cell.y * m_rectTileTwo.Width), mazeStartPosY + characterWidthCellPos + (cell.x * m_rectTileTwo.Height), m_rectBreadcrumbBlur.Width, m_rectBreadcrumbBlur.Height),
                        Color.Red * .2f
                );
            }
        }
        private void drawShortestPath()
        {
            List<Maze.Cell> shortestPathList = shortestPath.ToList();
            foreach (Maze.Cell cell in shortestPathList)
            {
                //dont draw the last one
                if (cell.x != _gameMaze.Count-1 || cell.y != _gameMaze.Count-1)
                {
                    m_spriteBatch.Draw(
                        m_texBloodDroplet,
                        new Rectangle(mazeStartPosX + characterWidthCellPos + (cell.y * m_rectTileTwo.Width), mazeStartPosY + characterWidthCellPos + (cell.x * m_rectTileTwo.Height), m_rectBloodDroplet.Width, m_rectBloodDroplet.Height),
                        Color.White
                    );
                }
            }
        }
        private void drawHint()
        {
            if (shortestPath.Count > 0)
            {
                Maze.Cell hintCell = shortestPath.Peek();
                if (hintCell != null && (hintCell.x != _gameMaze.Count - 1 || hintCell.y != _gameMaze.Count - 1))
                {
                    m_spriteBatch.Draw(
                        m_texBloodDroplet,
                        new Rectangle(mazeStartPosX + characterWidthCellPos + (hintCell.y * m_rectTileTwo.Width), mazeStartPosY + characterWidthCellPos + (hintCell.x * m_rectTileTwo.Height), m_rectBloodDroplet.Width, m_rectBloodDroplet.Height),
                        Color.White
                    );
                }
            }
        }
        private void drawBackground()
        {
            m_spriteBatch.Draw(m_texBackground, GraphicsDevice.Viewport.Bounds, Color.White);
        }
        private void drawKeybinds()
        {
            Vector2 f1StringSize = m_font.MeasureString(F1_COMMAND_STRING) * 1.0f;
            Vector2 f2StringSize = m_font.MeasureString(F2_COMMAND_STRING) * 1.0f;
            Vector2 f3StringSize = m_font.MeasureString(F3_COMMAND_STRING) * 1.0f;
            Vector2 f4StringSize = m_font.MeasureString(F4_COMMAND_STRING) * 1.0f;
            Vector2 f5StringSize = m_font.MeasureString(F5_COMMAND_STRING) * 1.0f;
            Vector2 f6StringSize = m_font.MeasureString(F6_COMMAND_STRING) * 1.0f;
            float totalStringSizeY = f1StringSize.Y + f2StringSize.Y + f3StringSize.Y + f4StringSize.Y + f5StringSize.Y + f6StringSize.Y;
            drawOutlineText(
                m_spriteBatch,
                m_font, F1_COMMAND_STRING,
                Color.Black, Color.White,
                new Vector2(
                    widthMid + (MAZE_SIZE / 2) + 10,
                    heightMid + f1StringSize.Y - totalStringSizeY),
                1.0f);

            drawOutlineText(
                m_spriteBatch,
                m_font, F2_COMMAND_STRING,
                Color.Black, Color.White,
                new Vector2(
                    widthMid + (MAZE_SIZE / 2) + 10,
                    heightMid + f1StringSize.Y + f2StringSize.Y - totalStringSizeY),
                1.0f);

            drawOutlineText(
                m_spriteBatch,
                m_font, F3_COMMAND_STRING,
                Color.Black, Color.White,
                new Vector2(
                    widthMid + (MAZE_SIZE / 2) + 10,
                    heightMid + f1StringSize.Y + f2StringSize.Y + f3StringSize.Y - totalStringSizeY),
                1.0f);

            drawOutlineText(
                m_spriteBatch,
                m_font, F4_COMMAND_STRING,
                Color.Black, Color.White,
                new Vector2(
                    widthMid + (MAZE_SIZE / 2) + 10,
                    heightMid + f1StringSize.Y + f2StringSize.Y + f3StringSize.Y + f4StringSize.Y - totalStringSizeY),
                1.0f);

            drawOutlineText(
                m_spriteBatch,
                m_font, F5_COMMAND_STRING,
                Color.Black, Color.White,
                new Vector2(
                    widthMid + (MAZE_SIZE / 2) + 10,
                    heightMid + f1StringSize.Y + f2StringSize.Y + f3StringSize.Y + f4StringSize.Y + f5StringSize.Y - totalStringSizeY),
                1.0f);

            drawOutlineText(
                m_spriteBatch,
                m_font, F6_COMMAND_STRING,
                Color.Black, Color.White,
                new Vector2(
                    widthMid + (MAZE_SIZE / 2) + 10,
                    heightMid),
                1.0f);
        }
        private void drawHighscore()
        {
            Vector2 highScoreStringSize = m_font.MeasureString(highScoreString) * 1.0f;
            drawOutlineText(
                m_spriteBatch,
                m_font, highScoreString,
                Color.Black, Color.White,
                new Vector2(
                    widthMid - highScoreStringSize.X / 2,
                    heightMid - (MAZE_SIZE / 2 + ((_gameMaze.Count / 5) * tileSize)) - highScoreStringSize.Y),
                1.0f);
        }
        private void drawCredits()
        {
            Vector2 creditsStringSize1 = m_font.MeasureString(CREDITS_STRING_1) * 1.0f;
            Vector2 creditsStringSize2 = m_font.MeasureString(CREDITS_STRING_2) * 1.0f;

            drawOutlineText(
                m_spriteBatch,
                m_font, CREDITS_STRING_1,
                Color.Black, Color.White,
                new Vector2(
                    widthMid - creditsStringSize1.X / 2,
                    heightMid + (MAZE_SIZE / 2 + ((_gameMaze.Count / 5) * tileSize)) - creditsStringSize1.Y - creditsStringSize2.Y),
                1.0f);

            drawOutlineText(
                m_spriteBatch,
                m_font, CREDITS_STRING_2,
                Color.Black, Color.White,
                new Vector2(
                    widthMid - creditsStringSize2.X / 2,
                    heightMid + (MAZE_SIZE / 2 + ((_gameMaze.Count / 5) * tileSize)) - creditsStringSize2.Y),
                1.0f);
        }
        private void drawCurrentScore()
        {
            Vector2 currentScoreStringSize = m_font.MeasureString(currentScoreString) * 1.0f;
            drawOutlineText(
                m_spriteBatch,
                m_font, currentScoreString,
                Color.Black, Color.White,
                new Vector2(
                    mazeStartPosX + ((_gameMaze.Count - (_gameMaze.Count / 5)) * tileSize) - currentScoreStringSize.X / 2,
                    mazeStartPosY - currentScoreStringSize.Y),
                1.0f);
        }
        private void drawTime()
        {
            Vector2 timeStringSize = m_font.MeasureString(timeString) * 1.0f;
            drawOutlineText(
                m_spriteBatch,
                m_font, timeString,
                Color.Black, Color.White,
                new Vector2(
                    mazeStartPosX + ((_gameMaze.Count / 5) * tileSize) - timeStringSize.X / 2,
                    mazeStartPosY - timeStringSize.Y),
                1.0f);
        }
        protected static void drawOutlineText(SpriteBatch spriteBatch, SpriteFont font, string text, Color outlineColor, Color frontColor, Vector2 position, float scale)
        {
            //Demo code outline drawing
            const float PIXEL_OFFSET = 1.0f;

            spriteBatch.DrawString(font, text, position - new Vector2(PIXEL_OFFSET * scale, 0), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(PIXEL_OFFSET * scale, 0), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position - new Vector2(0, PIXEL_OFFSET * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(0, PIXEL_OFFSET * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);

            spriteBatch.DrawString(font, text, position, frontColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        #endregion

        #region InputHandlers
        protected void processInput()
        {
            m_inputKeyboard.Update();
        }

        private void onMoveUp()
        {
            if (_gameMaze[character.x][character.y].topNeighbor != null)
            {
                character.x -= 1;
                if (_gameMaze[character.x][character.y] == shortestPath.Peek())
                {
                    if (!previouslyVisited.Contains(_gameMaze[character.x][character.y])) 
                    {
                        currentScore += 5;
                    }
                    shortestPath.Pop();
                    previouslyVisited.Add(_gameMaze[character.x + 1][character.y]);
                }
                else
                {
                    if (!previouslyVisited.Contains(_gameMaze[character.x][character.y]))
                    {
                        currentScore -= 3;
                    }
                    shortestPath.Push(_gameMaze[character.x + 1][character.y]);
                    previouslyVisited.Add(_gameMaze[character.x + 1][character.y]);
                }
            }
        }
        private void onMoveDown()
        {
            if (_gameMaze[character.x][character.y].bottomNeighbor != null)
            {
                character.x += 1;
                if (_gameMaze[character.x][character.y] == shortestPath.Peek())
                {
                    if (!previouslyVisited.Contains(_gameMaze[character.x][character.y]))
                    {
                        currentScore += 5;
                    }
                    shortestPath.Pop();
                    previouslyVisited.Add(_gameMaze[character.x - 1][character.y]);
                }
                else
                {
                    if (!previouslyVisited.Contains(_gameMaze[character.x][character.y]))
                    {
                        currentScore -= 3;
                    }
                    shortestPath.Push(_gameMaze[character.x - 1][character.y]);
                    previouslyVisited.Add(_gameMaze[character.x - 1][character.y]);
                }
            }
        }
        private void onMoveRight()
        {
            if (_gameMaze[character.x][character.y].rightNeighbor != null)
            {
                character.y += 1;
                if (_gameMaze[character.x][character.y] == shortestPath.Peek())
                {
                    if (!previouslyVisited.Contains(_gameMaze[character.x][character.y]))
                    {
                        currentScore += 5;
                    }
                    shortestPath.Pop();
                    previouslyVisited.Add(_gameMaze[character.x][character.y - 1]);
                }
                else
                {
                    if (!previouslyVisited.Contains(_gameMaze[character.x][character.y]))
                    {
                        currentScore -= 3;
                    }
                    shortestPath.Push(_gameMaze[character.x][character.y - 1]);
                    previouslyVisited.Add(_gameMaze[character.x][character.y - 1]);
                }
            }
        }
        private void onMoveLeft()
        {
            if (_gameMaze[character.x][character.y].leftNeighbor != null)
            {
                character.y -= 1;
                if (_gameMaze[character.x][character.y] == shortestPath.Peek())
                {
                    if (!previouslyVisited.Contains(_gameMaze[character.x][character.y]))
                    {
                        currentScore += 5;
                    }
                    shortestPath.Pop();
                    previouslyVisited.Add(_gameMaze[character.x][character.y + 1]);
                }
                else
                {
                    if (!previouslyVisited.Contains(_gameMaze[character.x][character.y]))
                    {
                        currentScore -= 3;
                    }
                    shortestPath.Push(_gameMaze[character.x][character.y + 1]);
                    previouslyVisited.Add(_gameMaze[character.x][character.y + 1]);
                }
            }
        }
        private void toggleBreadCrumbs()
        {
            showBreadcrumbs= !showBreadcrumbs;
        }
        private void toggleHint()
        {
            showHint = !showHint;
        }
        private void toggleShortestPath()
        {
            showShortestPath = !showShortestPath;
        }
        private void toggleHighscore()
        {
            showHighscore = !showHighscore;
        }
        private void toggleCredits()
        {
            showCredits = !showCredits;
        }
        #endregion
    }
}