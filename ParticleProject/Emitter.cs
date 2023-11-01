//Author: Sophia Lin
//File Name: Emitter.cs
//Project Name: Particle Project
//Creation Date: October 18, 2023
//Modified Date: October 30, 2023
//Description: Handle emitter's actions 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using GameUtility;

namespace ParticleProject
{
    class Emitter
    {
        //Store the increment value for when calculating RNG values, to overcome the exclusion on the maximum
        protected int INCREMENT = 1;

        //Used to define and Emitter to never stop launching
        public const int INFINITE = -1;

        //Used when defining an Emitter to enforce an upper limit on the # of particles, also used when trying to emit all particles at once 
        public const int MAX_PARTICLES = 5000;

        //Store value of no particlese
        public const int NO_PARTICLES = 0;

        //Store states of the emitter
        public const int INACTIVE = 0;
        public const int ACTIVE = 1;
        public const int DEAD = 2;
        public const int DONE = 3;
        protected int state;

        //Store the emitter image
        protected Texture2D img;
        protected float scale = 1f;
        protected int imgWidth;
        protected int imgHeight;

        //Store particle-related information
        protected Vector2 pos;
        protected int numParticles;
        protected int numLaunched;
        protected int launchTimeMin;
        protected int launchTimeMax;
        protected Texture2D partImg;
        protected float scaleMin;
        protected float scaleMax;
        protected int lifeMin;
        protected int lifeMax;
        protected int angleMin;
        protected int angleMax;
        protected int speedMin;
        protected int speedMax;
        protected Vector2 forces;
        protected float reboundScaler;
        protected Color colour;
        protected bool envCollisions;
        protected bool fade;
        protected bool isFinished = false;

        //Store the launch timer to know when to launch particles (intermission)
        protected Timer launchTimer;

        //Store emitter-specific properties
        protected bool drawn;
        protected bool explode;
        protected bool running;
        protected bool showLauncher;

        //Store emitter's location
        protected float emitterX;
        protected float emitterY;

        //Store special/Level 4 emitter's transparency
        protected const float transparency = 0.7f;

        //Store list of particles
        protected List<Particle> particles = new List<Particle>();

        /// <summary>
        /// Create an instance of Emitter
        /// </summary>
        /// <param name="img">Emitter image</param>
        /// <param name="scale">Scale for the emitter image</param>
        /// <param name="pos">Position of the emitter</param>
        /// <param name="numParticles">Number of particles in the emitter</param>
        /// <param name="launchTimeMin">Minimum launch time</param>
        /// <param name="launchTimeMax">Maximum launch time</param>
        /// <param name="partImg">Particle image</param>
        /// <param name="scaleMin">Minimum scale of the particle image</param>
        /// <param name="scaleMax">Maximum scale of the particle image</param>
        /// <param name="lifeMin">Minimum lifespan of the particle</param>
        /// <param name="lifeMax">Maximum lifespan of the particle</param>
        /// <param name="angleMin">Minimum angle of the particle</param>
        /// <param name="angleMax">Maximum angle of the particle</param>
        /// <param name="speedMin">Minimum speed of the particle</param>
        /// <param name="speedMax">Maximum speed of the particle</param>
        /// <param name="forces">Forces that affect the particle</param>
        /// <param name="reboundScaler">Scale of how much the particle rebounds</param>
        /// <param name="colour">Particle's colour</param>
        /// <param name="envCollisions">Track if the particle collides with the environment</param>
        /// <param name="fade">Track if the particle fades</param>
        public Emitter(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade)
        {
            //Store emitter image
            this.img = img;
            this.scale = scale;

            //Set state based on whether there is an image or not
            if (img != null)
            {
                //Store image dimensions
                this.imgWidth = (int)(img.Width * scale);
                this.imgHeight = (int)(img.Height * scale);

                //Set states to inactive
                this.state = INACTIVE;
                this.running = false;

            }
            else if (img == null)
            {
                //Set states to active
                this.state = ACTIVE;
                this.running = true;

            }

            //Set emitter location
            this.emitterX = pos.X;
            this.emitterY = pos.Y;

            //Set particle-related information
            this.numParticles = numParticles;
            this.launchTimeMin = launchTimeMin;
            this.launchTimeMax = launchTimeMax;
            this.partImg = partImg;
            this.scaleMin = scaleMin;
            this.scaleMax = scaleMax;
            this.lifeMin = lifeMin;
            this.lifeMax = lifeMax;
            this.angleMin = angleMin;
            this.angleMax = angleMax;
            this.speedMin = speedMin;
            this.speedMax = speedMax;

            //Set extra features of particle
            this.reboundScaler = reboundScaler;
            this.forces = forces;
            this.colour = colour;
            this.envCollisions = envCollisions;
            this.fade = fade;

            //Set launch timer and whether an explosion should occur or not
            if (launchTimeMin == -1 || launchTimeMax == -1)
            {
                explode = true;
                this.launchTimer = new Timer(1, true);
            }
            else
            {
                this.launchTimer = new Timer(GetRandInt(launchTimeMin, launchTimeMax), false);
            }

            //Set if the emitter should be drawn or not
            this.drawn = (img != null && scale > NO_PARTICLES);
        }

        /// <summary>
        /// Retrieve the state of the emitter
        /// </summary>
        /// <returns>Current state of emitter as an int</returns>
        public int GetState()
        {
            return state;
        }

        /// <summary>
        /// Retrieve the running state of the emitter
        /// </summary>
        /// <returns>Current running state of emitter as a bool</returns>
        public bool GetRunState()
        {
            return running;
        }

        /// <summary>
        /// Retrieve whether the emitter is visible or not
        /// </summary>
        /// <returns>Emitter visiblity as a bool</returns>
        public bool GetShowLaunch()
        {
            return showLauncher;
        }

        /// <summary>
        /// Retrieve the rectangle of the emitter where it's (x,y) is centered
        /// </summary>
        /// <returns>Emitter's centered rectangle</returns>
        protected virtual Rectangle GetRectangle()
        {
            return new Rectangle((int)(this.emitterX - this.imgWidth / 2),
                                           (int)(this.emitterY - this.imgHeight / 2), imgWidth, imgHeight);
        }

        /// <summary>
        /// Set the position of the emitter
        /// </summary>
        /// <param name="x">The x-coordinate of the emitter</param>
        /// <param name="y">The y-coordinate of the emitter</param>
        public virtual void SetPos(float x, float y)
        {
            emitterX = x;
            emitterY = y;
        }

        /// <summary>
        /// Handle the creation and launch of the particles in the emitter
        /// </summary>
        protected virtual void Launch()
        {
            //Add and launch the particles when the launch timer is inactive
            if (!launchTimer.IsActive())
            {
                //Add a particle to the emitter
                particles.Add(CreateParticles());

                //Launch a particle as long as there is a particle present
                if (particles.Count > NO_PARTICLES)
                {
                    //Increment the amount of particles launched
                    numLaunched++;

                    //Launch the particle
                    particles[particles.Count - 1].Launch(new Vector2(emitterX, emitterY));
                }

                //Reset the launch timer once the particle is launched so the next particle can launch once it is over
                launchTimer.ResetTimer(true, GetRandInt(launchTimeMin, launchTimeMax));
            }
        }

        /// <summary>
        /// Launch all the number of particles at once
        /// </summary>
        protected virtual void LaunchAll()
        {
            //Create all the number of particles then launch ALL as long as the number of particles are not infinite
            if (numParticles != INFINITE)
            {
                //Create and launch the particles as long as the explosion is not complete
                if (!isFinished)
                {
                    //Create and add the number of particles to the emitter
                    for (int i = 0; i < numParticles; i++)
                    {
                        particles.Add(CreateParticles());
                    }

                    //Launch all the particles when the launch timer is over
                    if (launchTimer.IsFinished())
                    {
                        //Launch the particles as long as there is a particle present
                        if (particles.Count > NO_PARTICLES)
                        {
                            //Launch all the particles
                            for (int i = 0; i < numParticles; i++)
                            {
                                //Launch only the number of particles intended, not going over it
                                if (numLaunched < numParticles)
                                {
                                    //Increment the amount of particles launched
                                    numLaunched++;

                                    //Launch the particle
                                    particles[i].Launch(new Vector2(emitterX, emitterY));
                                }
                            }
                        }

                        //All is launched 
                        isFinished = true;
                        //All particles have been launched, emitter is dead
                        state = DEAD;

                    }
                }
            }
        }

        /// <summary>
        /// Generate a random integer
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>Random int</returns>
        protected int GetRandInt(int min, int max)
        {
            //Store the randomly generated integer
            int randNum = Game1.rng.Next(min, max + INCREMENT);
            return randNum;
        }

        /// <summary>
        /// Generate a random float
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns>Random float</returns>
        protected float GetRandFloat(int min, int max)
        {
            //Conversion 
            int deciConvert = 100;

            //Store the randomly generated float
            float randFloat = (float)(Game1.rng.Next(min, max + INCREMENT)) / deciConvert;
            return randFloat;
        }

        /// <summary>
        /// Create the particle
        /// </summary>
        /// <returns>The particle object</returns>
        protected Particle CreateParticles()
        {
            //Conversion 
            int deciConvert = 100;

            //Randomize the scale, lifespan, angle, and speed of the particle
            float partScale = GetRandFloat((int)(scaleMin * deciConvert), (int)(scaleMax * deciConvert));
            int lifeSpan = GetRandInt(lifeMin, lifeMax);
            int angle = GetRandInt(angleMin, angleMax);
            float speed = GetRandFloat((int)(speedMin * deciConvert), (int)(speedMax * deciConvert));

            //Create an instance of particle
            Particle newParticle = new Particle(partImg, partScale, lifeSpan, angle,
                                speed, forces, fade, reboundScaler, colour, envCollisions);

            return newParticle;
        }

        /// <summary>
        /// Update the emitter
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="platforms">List of platforms for collision detection</param>
        public virtual void Update(GameTime gameTime, List<Platform> platforms)
        {
            //Launch particles if the emitter is activated
            if (state == ACTIVE)
            {
                //Update launch timer
                launchTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                //Launch the particles one by one infinitely
                if (numParticles == INFINITE && !explode)
                {
                    Launch();
                }

                //Launch all particles 
                if (explode)
                {
                    LaunchAll();
                }

                //Set state of emitter to dead if the emitter has a limited amount of particles and it has launched all 
                if (numLaunched >= numParticles && numParticles != INFINITE)
                {
                    state = DEAD;
                }
            }

            //Perform actions to particles based on their state
            for (int i = 0; i < particles.Count; i++)
            {
                //Update particle or remove based on state
                if (particles[i].GetState() == Particle.ACTIVE)
                {
                    //Update particle
                    particles[i].Update(gameTime, platforms);
                }
                else if (particles[i].GetState() == Particle.DEAD)
                {
                    //Remove particle
                    particles.RemoveAt(i);
                }
            }

            //Set state of emitter to done when no more particles are present
            if (particles.Count == NO_PARTICLES)
            {
                state = DONE;
            }
        }

        /// <summary>
        /// The emitter and particles are drawn
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //Draw particles
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(spriteBatch);
            }

            //Draw emitters 
            if (drawn)
            {
                spriteBatch.Draw(img, GetRectangle(), colour);
            }
        }

        /// <summary>
        /// Toggle the running state of the emitter
        /// </summary>
        public virtual void ToggleOnOff()
        {
            running = !running;
        }

        /// <summary>
        /// Activate the emitter
        /// </summary>
        public virtual void Activate()
        {
            state = ACTIVE;
        }

        /// <summary>
        /// Toggle the emitter's visiblity
        /// </summary>
        public void ToggleLauncherVisibility()
        {
            showLauncher = !showLauncher;
        }
    }
}
