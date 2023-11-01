//Author: Sophia Lin
//File Name: Circ.cs
//Project Name: Particle Project
//Creation Date: October 22, 2023
//Modified Date: October 30, 2023
//Description: Handle circle launcher
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
    class Circ : Emitter
    {
        //Store circle launcher's information
        private int radius;
        private bool showCirc;

        //Store GraphicsDevice variable
        private GraphicsDevice gd;

        //Store Gamecircle 
        private GameCircle circle;


        //Store rectangle's launch position and launch dimensions
        private Vector2 launchPos;
        private Vector2 launchDimensions;

        /// <summary>
        /// Create an instance of the Circle Launcher
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
        /// <param name="radius">Radius of circle launcher</param>
        /// <param name="gd">GraphicsDevice variable to create the GameCircle</param>
        /// <param name="showCirc">Tracks whether to show circle or not</param>
        public Circ(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade, int radius, GraphicsDevice gd, bool showCirc)
                    : base(img, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                          speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade)
        {
            //Set circle information
            this.radius = radius;
            this.gd = gd;
            this.pos.X = pos.X;
            this.pos.Y = pos.Y;
            circle = new GameCircle(gd, pos, radius);
            SetLaunchDimensions();
            this.showCirc = showCirc;
        }

        /// <summary>
        /// Set the dimensions of where the particles can be launched from
        /// </summary>
        public void SetLaunchDimensions(/*int x, int y*/)
        {
            //Generate a random angle to determine a random diameter(diff directions) of the circle that the particle can be launched from
            int angle = GetRandInt(angleMin, angleMax + INCREMENT);
            launchDimensions.X = Math.Abs(radius * (float)Math.Cos(angle));
            launchDimensions.Y = Math.Abs(radius * (float)Math.Sin(angle));
        }

        /// <summary>
        /// Retrieve the position of the circle's center
        /// </summary>
        /// <returns>Vector2 of the circle's center coordinates</returns>
        public Vector2 GetPosition()
        {
            return new Vector2(pos.X, pos.Y);
        }

        /// <summary>
        /// Set the circle's updated position
        /// </summary>
        /// <param name="speed">Speed at which the circle translates</param>
        public void SetCirclePos(int speed)
        {
            //Translate the horizontal position
            pos.X += speed;
        }

        /// <summary>
        /// Launch the particles
        /// </summary>
        protected override void Launch()
        {
            //Add and launch the particles when the launch timer is inactive
            if (!launchTimer.IsActive())
            {
                //Add a particle to the emitter
                particles.Add(CreateParticles());

                //Set launch position
                launchPos = GetLaunchPos((int)pos.X, (int)pos.Y);

                //Launch the particle
                if (particles.Count > NO_PARTICLES)
                {
                    //Set the internal emitter position to where the launch position is
                    SetPos(launchPos.X, launchPos.Y);

                    //Increment the amount of particles launched
                    numLaunched++;

                    //Launch the particle
                    particles[particles.Count - 1].Launch(launchPos);
                }

                //Reset launch timer once the particle is launched so the next particle can launch once it is over
                launchTimer.ResetTimer(true, GetRandInt(launchTimeMin, launchTimeMax));
            }
        }

        /// <summary>
        /// Update the circular launcher
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="platforms">List of platforms for collision detection</param>
        public override void Update(GameTime gameTime, List<Platform> platforms)
        {
            //Launch particles if the launcher is activated
            if (state == ACTIVE)
            {
                //Update launch timer
                launchTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                //Launch the particles one by one infinitely
                if (!explode)
                {
                    Launch();
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
            if (numLaunched > numParticles && numParticles != INFINITE)
            {
                state = DONE;
            }
        }

        /// <summary>
        /// Draw the circle launcher, the particles, and its emitter
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw if launcher is visible
            if (showLauncher)
            {
                //Draw circle launcher if it is visible
                if (showCirc)
                {
                    circle.Draw(spriteBatch, Color.Red * transparency);
                }

                //Draw particles
                for (int i = 0; i < particles.Count; i++)
                {
                    particles[i].Draw(spriteBatch);
                }

                //Draw emitter
                spriteBatch.Draw(img, GetRectangle(), Color.White);
            }

        }

        /// <summary>
        /// Retrieve the launch position of the particle
        /// </summary>
        /// <param name="x">x-coord of the circle launcher</param>
        /// <param name="y">y-coord of the circle launcher</param>
        /// <returns>Vector2 of the random launch position the particle will be launched at</returns>
        private Vector2 GetLaunchPos(int x, int y)
        {
            //Initialize x and y position
            int xPos = 0;
            int yPos = 0;

            //Generate random (x,y) coordinates in the circle
            xPos = GetRandInt(x - (int)launchDimensions.X, x + (int)launchDimensions.X);
            yPos = GetRandInt(y - (int)launchDimensions.Y, y + (int)launchDimensions.Y);

            return new Vector2(xPos, yPos);
        }
    }
}
