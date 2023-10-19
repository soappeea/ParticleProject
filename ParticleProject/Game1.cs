//Author: 
//File Name: 
//Project Name: 
//Creation Date: 
//Modified Date: 
//Description: 
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

        //Store the platforms
        private List<Platform> platforms = new List<Platform>();

        //Define List of all Emitters here (Thank you polymorphism!)
        private List<Emitter> emitters = new List<Emitter>();

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
                        
                    }
                }

                //Toggle water spouts
                if (kb.IsKeyDown(Keys.D1) && !prevKb.IsKeyDown(Keys.D1))    //Rightward spout with platform
                {
                    //TODO: Turn on/off left wall spout
                    
                }
                if (kb.IsKeyDown(Keys.D2) && !prevKb.IsKeyDown(Keys.D2))    //Downward spout
                {
                    //TODO: Turn on/off top wall spout
                    
                }
                if (kb.IsKeyDown(Keys.D3) && !prevKb.IsKeyDown(Keys.D3))    //Leftward spout
                {
                    //TODO: Turn on/off right wall spout
                    
                }
                if (kb.IsKeyDown(Keys.D4) && !prevKb.IsKeyDown(Keys.D4))    //Upward Spout
                {
                    //TODO: Turn on/off bottom wall spout
                    
                }

                //Toggle Smoker
                if (kb.IsKeyDown(Keys.D5) && !prevKb.IsKeyDown(Keys.D5))    //Smoke emitter
                {
                    //TODO: Turn on/off smoker
                    
                }

                //Toggle the Circular, Rectangular and Line Emitters
                if (kb.IsKeyDown(Keys.D6) && !prevKb.IsKeyDown(Keys.D6))    //Circular Launcher
                {
                    //TODO: Turn on/off bottom wall spout
                    
                }
                if (kb.IsKeyDown(Keys.D7) && !prevKb.IsKeyDown(Keys.D7))    //Rectangular Launcher
                {
                    //TODO: Turn on/off bottom wall spout
                    
                }
                if (kb.IsKeyDown(Keys.D8) && !prevKb.IsKeyDown(Keys.D8))    //Line Launcher
                {
                    //TODO: Turn on/off bottom wall spout
                    
                }

                if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))    //Show Launch indicators
                {
                    //TODO: Turn on/off bottom wall spout
                    
                }

                
                //TODO: Update mouse Emitter's location
                

                //TODO: Update all emitters, removing them when completed

            }

            base.Update(gameTime);
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

            //if (paused)
            //{
            //    spriteBatch.DrawString(hudFont, "PAUSED", new Vector2(400, 300), Color.Blue);
            //}

            //TODO: Draw all emitters

            spriteBatch.End();
            

            base.Draw(gameTime);
        }
    }
}
