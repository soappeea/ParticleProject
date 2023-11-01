//Author: Sophia Lin
//File Name: Game1.cs
//Project Name: Particle Project
//Creation Date: October 18, 2023
//Modified Date: October 30, 2023
//Description: Run whole game 
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using GameUtility;

namespace ParticleProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Public RNG to be used by all classes. 
        public static Random rng = new Random();

        //Spritefonts for various forms of output
        private SpriteFont hudFont;
        private SpriteFont popUpFont;

        //Store the background image
        private Texture2D bgImg;

        //Store the emitter image
        private Texture2D emitterImg;
        private Texture2D cloudImg;
        private Texture2D heartImg;

        //Store the particle images
        private Texture2D[] smokeImgs = new Texture2D[2];
        private Texture2D blankPartImg;
        private Texture2D bluePartImg;
        private Texture2D redPartImg;
        private Texture2D starImg;
        private Texture2D raindropImg;

        //Store the explosion and platform images
        private Texture2D explodeImg;
        private Texture2D brickImg;

        //Store the background rectangle
        private Rectangle bgRec;

        //Store input states
        private KeyboardState kb;
        private KeyboardState prevKb;
        private MouseState mouse;
        private MouseState prevMouse;

        //Store the screen dimensions
        private int screenWidth;
        private int screenHeight;

        //Store the game's paused state
        private bool paused = false;

        //forces Vectors
        public static Vector2 gravity = new Vector2(0, 9.8f);
        public static Vector2 wind = new Vector2(2f, 0);
        public static Vector2 lift = new Vector2(0.5f, -1.5f);

        //Store the platforms
        private List<Platform> platforms = new List<Platform>();

        //Store list of all kinds of Emitters 
        private List<Emitter> emitters = new List<Emitter>();
        private List<Emitter> explosiveEmitters = new List<Emitter>();
        private List<Cloud> cloudEmitters = new List<Cloud>();
        private Emitter lineEmitter;

        //Track index of the separation indicator of which the emitters differ
        private const int NORMAL_EMIT_END = 4;
        private const int MOUSE_EMITTER = 7;
        private const int SPECIAL_EMIT_BEGIN = 5;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            IsMouseVisible = true;

            //Setup the screen dimensions
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 740;
            graphics.ApplyChanges();
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

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

            //Load fonts
            hudFont = Content.Load<SpriteFont>("Fonts/HUDFont");
            popUpFont = Content.Load<SpriteFont>("Fonts/PopUpFont");

            //Load the background, emitter, platform and particle images
            bgImg = Content.Load<Texture2D>("Images/Backgrounds/GrassBG");
            emitterImg = Content.Load<Texture2D>("Images/Sprites/Box");
            smokeImgs[0] = Content.Load<Texture2D>("Images/Sprites/Smoke");
            smokeImgs[1] = Content.Load<Texture2D>("Images/Sprites/Smoke2");
            blankPartImg = Content.Load<Texture2D>("Images/Sprites/WhiteBall");
            bluePartImg = Content.Load<Texture2D>("Images/Sprites/BlueBall");
            redPartImg = Content.Load<Texture2D>("Images/Sprites/RedBall");
            starImg = Content.Load<Texture2D>("Images/Sprites/Star");
            explodeImg = Content.Load<Texture2D>("Images/Sprites/Explode");
            brickImg = Content.Load<Texture2D>("Images/Sprites/Brick");
            cloudImg = Content.Load<Texture2D>("Images/Sprites/Cloud");
            raindropImg = Content.Load<Texture2D>("Images/Sprites/Raindrop");
            heartImg = Content.Load<Texture2D>("Images/Sprites/Heart");

            //Store the background rectangle
            bgRec = new Rectangle(0, 0, screenWidth, screenHeight);

            //Setup platforms for walls and floating platforms
            float brickScale = 0.25f;
            //left wall
            platforms.Add(new Platform(brickImg, 26, brickScale, 0, 0, false));
            //right wall
            platforms.Add(new Platform(brickImg, 26, brickScale, screenWidth - (int)(brickImg.Width * brickScale), 0, false));
            //floor
            platforms.Add(new Platform(brickImg, 10, 1f, 0, screenHeight - brickImg.Height, true));

            //left platform
            platforms.Add(new Platform(brickImg, 4, 0.5f, 150, 500, true));
            platforms.Add(new Platform(brickImg, 2, 0.5f, 200, 400, false));

            //Hill
            platforms.Add(new Platform(brickImg, 1, 0.5f, 700, 590, false));
            platforms.Add(new Platform(brickImg, 2, 0.5f, 750, 540, false));
            platforms.Add(new Platform(brickImg, 3, 0.5f, 800, 490, false));
            platforms.Add(new Platform(brickImg, 2, 0.5f, 850, 540, false));
            platforms.Add(new Platform(brickImg, 1, 0.5f, 900, 590, false));

            //Water platform
            platforms.Add(new Platform(brickImg, 10, 0.25f, 25, 108, true));

            //Add permanent emitters
            //Water Emitter
            float emitterScaleWater = 0.25f;
            //top left
            emitters.Add(new Emitter(emitterImg, emitterScaleWater, new Vector2(38, 96), Emitter.INFINITE, 0, 10, bluePartImg, 0.1f,
                                    0.25f, 3000, 4000, 330, 390, 250, 500, gravity, Particle.SPLAT_BALL, Color.White, true, true));
            //top
            emitters.Add(new Emitter(emitterImg, emitterScaleWater, new Vector2(screenWidth / 2, 12), Emitter.INFINITE, 0, 10, bluePartImg, 0.1f,
                                    0.25f, 3000, 4000, 240, 300, 250, 500, gravity, Particle.SPLAT_BALL, Color.White, true, true));
            //right
            emitters.Add(new Emitter(emitterImg, emitterScaleWater, new Vector2(963, 100), Emitter.INFINITE, 0, 10, bluePartImg, 0.1f,
                                    0.25f, 3000, 4000, 150, 210, 250, 500, gravity, Particle.SPLAT_BALL, Color.White, true, true));
            //On hill
            emitters.Add(new Emitter(emitterImg, emitterScaleWater, new Vector2(825, 480), Emitter.INFINITE, 0, 10, bluePartImg, 0.1f,
                                    0.25f, 3000, 4000, 60, 120, 250, 500, gravity, Particle.SPLAT_BALL, Color.White, true, true));

            //Smoke Emitter
            float emitterScaleSmoke = 0.25f;
            emitters.Add(new Emitter(emitterImg, emitterScaleSmoke, new Vector2(225, 388), Emitter.INFINITE, 0, 100, smokeImgs[0], 0.3f,
                                    0.5f, 2500, 3500, 75, 105, 100, 100, lift, Particle.SPLAT_BALL, Color.White, true, true));


            //Rectangle Emitter
            float emitterScaleSpecial = 0.05f;
            emitters.Add(new Rect(heartImg, emitterScaleSpecial, new Vector2(platforms[0].GetBoundingBox().Width + 38, 500), 50, 2000, 4000,
                                  blankPartImg, 0.2f, 0.3f, 2000, 3000, 90, 270, 200, 400, gravity, Particle.SPLAT_BALL, Color.Yellow, true, true, 75, 200, GraphicsDevice));

            //Circle Emitter
            emitters.Add(new Circ(heartImg, emitterScaleSpecial, new Vector2(500, 400), 75, 500, 700, blankPartImg, 0.2f, 0.3f, 2000, 3000,
                                  0, 360, 200, 400, gravity, Particle.SPLAT_BALL, Color.HotPink, true, true, 70, GraphicsDevice, true));

            //Line emitter 
            lineEmitter = new Line(emitterImg, emitterScaleSpecial, new Vector2(200, 200), Emitter.INFINITE, 1, 100, blankPartImg,
                                   0.05f, 0.1f, 2000, 3000, 90, 100, 200, 400, gravity, Particle.RUBBER_BALL, Color.Red, true, true, 100, GraphicsDevice, 3);

            //Mouse Emitter 
            float emitterScaleMouse = 0f;
            emitters.Add(new Emitter(null, emitterScaleMouse, new Vector2(0, 0), Emitter.INFINITE, 50, 100, blankPartImg, 0.1f, 0.2f, 1000, 2000,
                            0, 360, 0, 100, gravity, Particle.BOWLING_BALL, Color.Green, false, true));

            //Cloud Emitter
            float emitterScaleCloud = 0.05f;
            cloudEmitters.Add(new Cloud(cloudImg, emitterScaleCloud, new Vector2(95, 50), Emitter.INFINITE, 200, 250, raindropImg, 0.05f, 0.075f, 2000, 3000,
                                260, 280, 300, 500, gravity, Particle.SPLAT_BALL, Color.White, true, true, 30, GraphicsDevice, 1, 2, emitterImg, false));
            cloudEmitters.Add(new Cloud(cloudImg, emitterScaleCloud, new Vector2(490, 50), Emitter.INFINITE, 200, 250, raindropImg, 0.05f, 0.075f, 2000, 3000,
                                260, 280, 300, 500, gravity, Particle.SPLAT_BALL, Color.White, true, true, 30, GraphicsDevice, 1, 2, emitterImg, false));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Nothing here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Exit on the escape key
            if (kb.IsKeyDown(Keys.Escape)) Exit();

            //Update input devices
            prevKb = kb;
            prevMouse = mouse;
            kb = Keyboard.GetState();
            mouse = Mouse.GetState();

            //Toggle pause
            if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            {
                paused = !paused;
            }

            //Only update and get input when simulation is not paused
            if (!paused)
            {
                //Ignite Bomb at mouse click location
                if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                {
                    bool noCollision = true;
                    //Verify no platforms collide with the mouse's click location
                    for (int i = 0; i < platforms.Count; i++)
                    {
                        if (platforms[i].GetBoundingBox().Contains(mouse.Position))
                        {
                            noCollision = false;
                            break;
                        }
                    }

                    //No mouse collision with platforms was found, explosion is allowed
                    if (noCollision)
                    {
                        //Store number of particles to be in explosion
                        int numParticles = MathHelper.Clamp(1000, 1, Emitter.MAX_PARTICLES);

                        //Add an explosion of particles to list of explosive emitters
                        explosiveEmitters.Add(new Explosive(starImg, new Vector2(mouse.X, mouse.Y), numParticles));
                    }
                }

                //Toggle water spouts
                //Left spout
                if (kb.IsKeyDown(Keys.D1) && !prevKb.IsKeyDown(Keys.D1))
                {
                    //Turn on/off left water spout and change state to active
                    emitters[0].ToggleOnOff();
                    emitters[0].Activate();
                }
                //Top spout
                if (kb.IsKeyDown(Keys.D2) && !prevKb.IsKeyDown(Keys.D2))
                {
                    //Turn on/off top water spout and change state to active
                    emitters[1].ToggleOnOff();
                    emitters[1].Activate();
                }
                //Right
                if (kb.IsKeyDown(Keys.D3) && !prevKb.IsKeyDown(Keys.D3))
                {
                    //Turn on/off right water spout and change state to active
                    emitters[2].ToggleOnOff();
                    emitters[2].Activate();
                }
                //Bottom 
                if (kb.IsKeyDown(Keys.D4) && !prevKb.IsKeyDown(Keys.D4))
                {
                    //Turn on/off bottom water spout and change state to active
                    emitters[3].ToggleOnOff();
                    emitters[3].Activate();
                }
                //Toggle smoke emitter
                if (kb.IsKeyDown(Keys.D5) && !prevKb.IsKeyDown(Keys.D5))
                {
                    //Turn on/off smoke emitter and change state to active
                    emitters[4].ToggleOnOff();
                    emitters[4].Activate();
                }

                //Toggle special/level 4 emitters if they are shown on screen
                for (int i = 5; i < 6; i++)
                {
                    if (emitters[i].GetShowLaunch() == true)
                    {
                        //Toggle rectangular launcher
                        if (kb.IsKeyDown(Keys.D6) && !prevKb.IsKeyDown(Keys.D6))
                        {
                            //Turn on/off rectangular emitter and change state to active
                            emitters[5].ToggleOnOff();
                            emitters[5].Activate();
                        }

                        //Toggle circular launcher
                        if (kb.IsKeyDown(Keys.D7) && !prevKb.IsKeyDown(Keys.D7))
                        {
                            //Turn on/off circular emitter and change state to active
                            emitters[6].ToggleOnOff();
                            emitters[6].Activate();
                        }

                        //Toggle cloud launchers
                        if (kb.IsKeyDown(Keys.D8) && !prevKb.IsKeyDown(Keys.D8))
                        {
                            //Turn on/off cloud emitters and change state to active
                            cloudEmitters[0].ToggleOnOff();
                            cloudEmitters[0].Activate();
                            cloudEmitters[0].ToggleCloudCirc();
                            cloudEmitters[1].ToggleOnOff();
                            cloudEmitters[1].Activate();
                            cloudEmitters[1].ToggleCloudCirc();
                        }

                        //Toggle line launcher
                        if (kb.IsKeyDown(Keys.D9) && !prevKb.IsKeyDown(Keys.D9))
                        {
                            //Turn on/off line emitter and change state to active
                            lineEmitter.ToggleOnOff();
                            lineEmitter.Activate();
                        }
                    }
                }

                //Toggle showing the special/level 4 emitters
                if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))
                {
                    for (int i = 5; i < emitters.Count; i++)
                    {
                        emitters[i].ToggleLauncherVisibility();
                    }

                    for (int i = 0; i < cloudEmitters.Count; i++)
                    {
                        cloudEmitters[i].ToggleLauncherVisibility();
                    }

                    lineEmitter.ToggleLauncherVisibility();
                }

                //Update mouse Emitter's location
                emitters[emitters.Count - 1].SetPos(mouse.X, mouse.Y);

                //Update normal emitters
                for (int i = 0; i < emitters.Count; i++)
                {
                    //Update if the emitter is running
                    if (emitters[i].GetRunState() == true)
                    {
                        if (i <= NORMAL_EMIT_END || i == MOUSE_EMITTER)
                        {
                            emitters[i].Update(gameTime, platforms);
                        }
                    }

                    //Remove the emitter when its state is done
                    if (emitters[i].GetState() == Emitter.DONE)
                    {
                        emitters.RemoveAt(i);
                    }
                }

                //Update special emitters
                for (int i = SPECIAL_EMIT_BEGIN; i < MOUSE_EMITTER; i++)
                {
                    //Update if the emitter is running
                    if (emitters[i].GetRunState() == true)
                    {
                        //Update if emitter is shown on the screen
                        if (emitters[i].GetShowLaunch() == true)
                        {
                            emitters[i].Update(gameTime, platforms);
                        }
                    }

                    //Remove the emitter when its state is done
                    if (emitters[i].GetState() == Emitter.DONE)
                    {
                        emitters.RemoveAt(i);
                    }
                }

                //Update cloud emitters
                for (int i = 0; i < cloudEmitters.Count; i++)
                {
                    //Update if the emitter is running
                    if (cloudEmitters[i].GetRunState() == true)
                    {
                        //Update if emitter is shown on the screen
                        if (cloudEmitters[i].GetShowLaunch() == true)
                        {
                            cloudEmitters[i].Update(gameTime, platforms);
                        }
                    }
                }

                //Update explosive emitters
                for (int i = 0; i < explosiveEmitters.Count; i++)
                {
                    //Update if the emitter is running
                    if (explosiveEmitters[i].GetRunState() == true)
                    {
                        explosiveEmitters[i].Update(gameTime, platforms);
                    }

                    //Remove the emitter when its state is done
                    if (explosiveEmitters[i].GetState() == Emitter.DONE)
                    {
                        explosiveEmitters.RemoveAt(i);
                    }
                }

                //Update line emitter if it is running
                if (lineEmitter.GetRunState() == true)
                {
                    //Update if emitter is shown on the screen
                    if (lineEmitter.GetShowLaunch() == true)
                    {
                        lineEmitter.Update(gameTime, platforms);
                    }
                }

                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //Draw the background
            spriteBatch.Draw(bgImg, bgRec, Color.White);

            //Draw all platforms
            for (int i = 0; i < platforms.Count; i++)
            {
                platforms[i].Draw(spriteBatch);
            }

            //Display emitters
            for (int i = 0; i < emitters.Count; i++)
            {
                emitters[i].Draw(spriteBatch);
            }

            //Display explosive emitter
            for (int i = 0; i < explosiveEmitters.Count; i++)
            {
                explosiveEmitters[i].Draw(spriteBatch);
            }

            //Display cloud emitters
            for (int i = 0; i < cloudEmitters.Count; i++)
            {
                cloudEmitters[i].Draw(spriteBatch);
            }

            //Display line emitter
            lineEmitter.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
