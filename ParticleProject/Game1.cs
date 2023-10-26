//Author: Sophia Lin
//File Name: Game1.cs
//Project Name: Particle Project
//Creation Date: 
//Modified Date: 
//Description: 
//TODO:
//cloud
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

        //public RNG to be used by all classes. e.g. Game1.rng.Next(1,11)
        public static Random rng = new Random();

        //Spritefonts for various forms of output
        private SpriteFont hudFont;
        private SpriteFont popUpFont;

        //Store the background image
        private Texture2D bgImg;

        //Store the emitter image
        private Texture2D emitterImg;
        private Texture2D cloudImg;

        //Store the particle images
        private Texture2D[] smokeImgs = new Texture2D[2];
        private Texture2D blankPartImg;
        private Texture2D bluePartImg;
        private Texture2D redPartImg;
        private Texture2D starImg;

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

        //forces Vectors, they can be directly added to combine them if needed
        public static Vector2 gravity = new Vector2(0, 9.8f);
        public static Vector2 wind = new Vector2(2f, 0);
        public static Vector2 lift = new Vector2(0.5f, -1.5f);

        //Store the platforms
        private List<Platform> platforms = new List<Platform>();

        //Define List of all Emitters here (Thank you polymorphism!)
        private List<Emitter> emitters = new List<Emitter>();
        private List<Emitter> explosiveEmitters = new List<Emitter>();
        private List<Emitter> cloudEmitters = new List<Emitter>();

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

            //Store the background rectangle
            bgRec = new Rectangle(0, 0, screenWidth, screenHeight);

            //Setup platforms for walls and floating platforms
            float brickScale = 0.25f;
            platforms.Add(new Platform(brickImg, 26, brickScale, 0, 0, false));                                                 //left wall
            platforms.Add(new Platform(brickImg, 26, brickScale, screenWidth - (int)(brickImg.Width * brickScale), 0, false));  //right wall
            platforms.Add(new Platform(brickImg, 10, 1f, 0, screenHeight - brickImg.Height, true));                             //floor

            platforms.Add(new Platform(brickImg, 4, 0.5f, 150, 500, true));                                                     //left platform
            platforms.Add(new Platform(brickImg, 2, 0.5f, 200, 400, false));

            platforms.Add(new Platform(brickImg, 1, 0.5f, 700, 590, false));                                                    //Hill
            platforms.Add(new Platform(brickImg, 2, 0.5f, 750, 540, false));
            platforms.Add(new Platform(brickImg, 3, 0.5f, 800, 490, false));
            platforms.Add(new Platform(brickImg, 2, 0.5f, 850, 540, false));
            platforms.Add(new Platform(brickImg, 1, 0.5f, 900, 590, false));

            platforms.Add(new Platform(brickImg, 10, 0.25f, 25, 108, true));                                                    //Water platform

            //TODO: Add permanent emitters here

            //Water Emitter
            float emitterScaleWater = 0.25f;
            emitters.Add(new Emitter(emitterImg, emitterScaleWater, new Vector2(38, 96), Emitter.INFINITE, 0, 10, bluePartImg, 0.1f,             //top left
                 0.25f, 3000, 4000, 330, 390, 250, 500, gravity, Particle.SPLAT_BALL, Color.White, true, true));
            emitters.Add(new Emitter(emitterImg, emitterScaleWater, new Vector2(screenWidth / 2, 12), Emitter.INFINITE, 0, 10, bluePartImg, 0.1f,          //top
                   0.25f, 3000, 4000, 240, 300, 250, 500, gravity, Particle.SPLAT_BALL, Color.White, true, true));
            emitters.Add(new Emitter(emitterImg, emitterScaleWater, new Vector2(963, 100), Emitter.INFINITE, 0, 10, bluePartImg, 0.1f,          //right
                   0.25f, 3000, 4000, 150, 210, 250, 500, gravity, Particle.SPLAT_BALL, Color.White, true, true));
            emitters.Add(new Emitter(emitterImg, emitterScaleWater, new Vector2(825, 480), Emitter.INFINITE, 0, 10, bluePartImg, 0.1f,          //On hill
                   0.25f, 3000, 4000, 60, 120, 250, 500, gravity, Particle.SPLAT_BALL, Color.White, true, true));

            //Smoke Emitter
            float emitterScaleSmoke = 0.25f;
            emitters.Add(new Emitter(emitterImg, emitterScaleSmoke, new Vector2(225, 388), Emitter.INFINITE, 0, 100, smokeImgs[0], 0.3f,
                   0.5f, 2500, 3500, 75, 105, 100, 100, lift, Particle.SPLAT_BALL, Color.White, true, true));

            //Rectangle Emitter
            emitters.Add(new Rect(emitterImg, emitterScaleWater, new Vector2(platforms[0].GetBoundingBox().Width + 38, 400 + 100) /*since rectnalge is centred at emitter position*/,
                /*lalalaalalalal*/50, /*temp*/2000, /*temp*/4000, blankPartImg, /*temp*/0.2f, /*temp*/0.3f,  /*temp*/2000,  /*temp*/3000,  /*temp*/90,  /*temp*/270,
                /*temp*/200,  /*temp*/400, gravity,/*temp*/Particle.SPLAT_BALL, Color.Yellow, true, true, /*temp*/75,/*temp*/ 200, GraphicsDevice));

            //Circle Emitter
            emitters.Add(new Circ(emitterImg, emitterScaleWater, /*temporary loc*/new Vector2(500, 400), 75, /*temp*/500, /*temp*/700, blankPartImg,
                /*temp*/0.2f, /*temp*/0.3f,  /*temp*/2000,  /*temp*/3000,  /*temp*/0,  /*temp*/360,  /*temp*/200,  /*temp*/400, gravity,
                /*temp*/Particle.SPLAT_BALL, Color.HotPink, true, true, /*temp*/70, GraphicsDevice));

            //Mouse Emitter (set as last emitter)
            float emitterScaleMouse = 0f;
            emitters.Add(new Emitter(null, emitterScaleMouse, new Vector2(0, 0), Emitter.INFINITE, 50, 100, blankPartImg, 0.1f,
                 0.2f, 1000, 2000, 0, 360, 0, 100, gravity, Particle.BOWLING_BALL, Color.Green, false, true));

            //Cloud Emitter
            float emitterScaleCloud = 0.04f;
            cloudEmitters.Add(new Cloud(cloudImg, emitterScaleCloud, new Vector2(70, 35), Emitter.INFINITE, 500, 1000, blankPartImg, 0.1f, 0.2f, 2000, 3000,
                                0, 360, 300, 500, gravity, Particle.SPLAT_BALL, Color.DarkBlue, true, true, 70, GraphicsDevice, 10, 20));
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
                        //TODO: Add an explosion of particles here to List of explosive emitters
                        //ASK ABOUT CLAMP NUMBER OF PARTICLES, needs to be enforced at a number between 1 and max particles\

                        int numParticles = MathHelper.Clamp(1000, 1, Emitter.MAX_PARTICLES);
                        explosiveEmitters.Add(new Explosive(starImg, new Vector2(mouse.X, mouse.Y), numParticles));
                    }
                }

                //Toggle water spouts
                //Left spout
                if (kb.IsKeyDown(Keys.D1) && !prevKb.IsKeyDown(Keys.D1))
                {
                    emitters[0].ToggleOnOff();
                    emitters[0].Activate();
                }
                if (kb.IsKeyDown(Keys.D2) && !prevKb.IsKeyDown(Keys.D2))    //Top spout
                {
                    emitters[1].ToggleOnOff();
                    emitters[1].Activate();
                }
                if (kb.IsKeyDown(Keys.D3) && !prevKb.IsKeyDown(Keys.D3))    //Right
                {
                    emitters[2].ToggleOnOff();
                    emitters[2].Activate();
                }
                if (kb.IsKeyDown(Keys.D4) && !prevKb.IsKeyDown(Keys.D4))    //Bottom 
                {
                    emitters[3].ToggleOnOff();
                    emitters[3].Activate();
                }

                //Toggle Smoker
                if (kb.IsKeyDown(Keys.D5) && !prevKb.IsKeyDown(Keys.D5))    //Smoke emitter
                {
                    emitters[4].ToggleOnOff();
                    emitters[4].Activate();
                }

                for (int i = 5; i < 6; i++)
                {
                    if (emitters[i].GetShowLaunch() == true)
                    {
                        //Toggle the Circular, Rectangular and Line Emitters
                        if (kb.IsKeyDown(Keys.D6) && !prevKb.IsKeyDown(Keys.D6))    //Rectangular Launcher
                        {
                            //TODO: Turn on/off bottom wall spout
                            emitters[5].ToggleOnOff();
                            emitters[5].Activate();
                        }
                        if (kb.IsKeyDown(Keys.D7) && !prevKb.IsKeyDown(Keys.D7))    //Circular Launcher
                        {
                            //TODO: Turn on/off bottom wall spout
                            emitters[6].ToggleOnOff();
                            emitters[6].Activate();

                        }
                        if (kb.IsKeyDown(Keys.D8) && !prevKb.IsKeyDown(Keys.D8))    //Line Launcher
                        {
                            //TODO: Turn on/off bottom wall spout
                            
                        }
                    }
                }

                if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))    //Show Launch indicators
                {
                    for (int i = 5; i < emitters.Count; i++)
                    {
                        emitters[i].ToggleLauncherVisibility();
                    }
                    //emitters[5].ToggleLauncherVisibility();
                    //emitters[6].ToggleLauncherVisibility();
                    for (int i = 0; i < cloudEmitters.Count; i++)
                    {
                        cloudEmitters[i].ToggleLauncherVisibility();
                    }
                }

                //Update mouse Emitter's location
                emitters[emitters.Count - 1].SetPos(mouse.X, mouse.Y);

                //TODO: Update all emitters, removing them when completed
                for (int i = 0; i < emitters.Count; i++)
                {
                    if (emitters[i].GetRunState() == true)
                    {
                        emitters[i].Update(gameTime, platforms);
                    }

                    if (emitters[i].GetState() == Emitter.DEAD)
                    {
                        emitters.RemoveAt(i);
                    }

                }


                for (int i = 0; i < explosiveEmitters.Count; i++)
                {
                    if (explosiveEmitters[i].GetRunState() == true)
                    {
                        explosiveEmitters[i].Update(gameTime, platforms);
                    }

                    if (explosiveEmitters[i].GetState() == Emitter.DEAD)
                    {
                        explosiveEmitters.RemoveAt(i);
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

            for (int i = 0; i < cloudEmitters.Count; i++)
            {
                cloudEmitters[i].Draw(spriteBatch);
            }

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
