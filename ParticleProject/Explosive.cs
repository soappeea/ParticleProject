//Author: Sophia Lin
//File Name: Explosive.cs
//Project Name: Particle Project
//Creation Date: October 20, 2023
//Modified Date: October 30, 2023
//Description: Handle explosive emitter's actions
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
    class Explosive : Emitter
    {
        //Store emitter and particle data of explosive emitter
        private const float EMIT_SCALE = 0f;
        private const float SCALE_MIN = 0.1f;
        private const float SCALE_MAX = 0.3f;
        private const int LIFE_MIN = 1500;
        private const int LIFE_MAX = 3500;
        private const int ANGLE_MIN = 0;
        private const int ANGLE_MAX = 360;
        private const int SPEED_MIN = 0;
        private const int SPEED_MAX = 850;
        private const int NO_TIME = -1;
        private const bool COLLISION = true;
        private const bool FADE = true;

        /// <summary>
        /// Create an instance of the explosive emitter
        /// </summary>
        /// <param name="img">Particle image</param>
        /// <param name="pos">Position that particle is to be launched from</param>
        /// <param name="numParticles">Number of particles to be launched</param>
        public Explosive(Texture2D img, Vector2 pos, int numParticles) : base(null, EMIT_SCALE, pos, numParticles, NO_TIME, NO_TIME, img, SCALE_MIN, SCALE_MAX,
                        LIFE_MIN, LIFE_MAX, ANGLE_MIN, ANGLE_MAX, SPEED_MIN, SPEED_MAX, Game1.gravity, Particle.BOWLING_BALL, Color.White, COLLISION, FADE)
        {
        }

        /// <summary>
        /// Launch all the number of particles at once
        /// </summary>
        protected override void LaunchAll()
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

                    //Explosion is finished
                    isFinished = true;

                    //All particles have been launched, emitter is dead
                    state = DEAD;
                }
            }
        }

        /// <summary>
        /// Update the explosive emitter
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="platforms">List of platforms for collision detection</param>
        public override void Update(GameTime gameTime, List<Platform> platforms)
        {
            //Launch particles if the emitter is activated
            if (state == ACTIVE)
            {
                //Update launch timer
                launchTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                //Launch all particles 
                LaunchAll();
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

            //Set state of emitter to dead when no more particles are present
            if (particles.Count == NO_PARTICLES)
            {
                state = DONE;
            }
        }

        /// <summary>
        /// The particles are drawn
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw particles
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(spriteBatch);
            }
        }
    }
}
