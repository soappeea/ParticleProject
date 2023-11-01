//Author: Sophia Lin
//File Name: Rect.cs
//Project Name: Particle Project
//Creation Date: October 21, 2023
//Modified Date: October 30, 2023
//Description: Handle rectangle launcher
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
    class Rect : Emitter
    {
        //Set rectangle launcher's information
        private int width;
        private int height;

        //Store GraphicsDevice variable
        private GraphicsDevice gd;

        //Store GameRectangle 
        private GameRectangle rectangle;

        //Store rectangle's launch position, centered position, and launch dimensions
        private Vector2 launchPos;
        private Vector2 centeredPos;
        private Vector2 launchDimensions;

        /// <summary>
        /// Create an instance of the Rectangle Launcher
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
        /// <param name="width">Width of rectangle launcher</param>
        /// <param name="height">Height of rectangle launcher</param>
        /// <param name="gd">GraphicsDevice variable to create GameRectangle</param>
        public Rect(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade, int width, int height, GraphicsDevice gd)
                    : base(img, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                          speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade)
        {
            //Set Rectangle information
            this.width = width;
            this.height = height;
            this.gd = gd;
            this.pos.X = pos.X;
            this.pos.Y = pos.Y;
            this.centeredPos.X = (int)pos.X - width / 2;
            this.centeredPos.Y = (int)pos.Y - height / 2;
            rectangle = new GameRectangle(gd, (int)centeredPos.X, (int)centeredPos.Y, width, height);
            SetLaunchDimensions((int)centeredPos.X, (int)centeredPos.Y);
            this.state = INACTIVE;
            this.running = false;
        }

        /// <summary>
        /// Set the dimensions of where the particles can be launched from
        /// </summary>
        /// <param name="x">x-coord of the rectangle launcher</param>
        /// <param name="y">y-coord of the rectangle launcher</param>
        public void SetLaunchDimensions(int x, int y)
        {
            //Width of launch dimension
            launchDimensions.X = x + rectangle.Width;

            //Height of launch dimension
            launchDimensions.Y = y + rectangle.Height;
        }

        /// <summary>
        /// Set the position of the emitter inside the rectangle launcher
        /// </summary>
        /// <param name="x">x-coord of the location the particle is emitting from</param>
        /// <param name="y">y-coord of the location the particle is emitting from</param>
        public override void SetPos(float x, float y)
        {
            //Set the internal emitter position to where the particle is launching from
            emitterX = x;
            emitterY = y;
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
                launchPos = GetLaunchPos((int)centeredPos.X, (int)centeredPos.Y);

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
        /// Update the rectangle launcher
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
                if (numLaunched >= numParticles)
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
            if (particles.Count == NO_PARTICLES && state == DEAD)
            {
                state = DONE;
            }
        }

        /// <summary>
        /// Draw rectangle launcher, emitter, and its particles
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw if launcher is visible
            if (showLauncher)
            {
                //Draw rectangle launcher
                rectangle.Draw(spriteBatch, Color.Blue * transparency, true);

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
        /// <param name="x">x-coord of the rectangle launcher</param>
        /// <param name="y">y-coord of the rectangle launcher</param>
        /// <returns>Vector2 of the random launch position the particle will be launched at</returns>
        private Vector2 GetLaunchPos(int x, int y)
        {
            //Generate random (x,y) coordinates in the rectangle launcher for the particle to launch at
            int xPos = GetRandInt(x, (int)launchDimensions.X);
            int yPos = GetRandInt(y, (int)launchDimensions.Y);
            return new Vector2(xPos, yPos);
        }
    }
}
