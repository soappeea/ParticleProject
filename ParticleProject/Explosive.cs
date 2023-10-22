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
        private const float EMIT_SCALE = 0f;
        private const float SCALE_MIN = 0.1f;
        private const float SCALE_MAX = 0.3f;
        private const int LIFE_MIN = 1500;
        private const int LIFE_MAX = 3500;
        private const int ANGLE_MIN = 0;
        private const int ANGLE_MAX = 360;
        private const int SPEED_MIN = 0;
        private const int SPEED_MAX = 850;
        private const int NO_TIME = -1; //used launchTimes
        private const bool COLLISION = true;
        private const bool FADE = true;
        private int numLaunched;
        private bool reachedNumParticles = false;

        public Explosive(Texture2D img, Vector2 pos, int numParticles) : base(null, EMIT_SCALE, pos, numParticles, NO_TIME, NO_TIME, img, SCALE_MIN, SCALE_MAX, 
                        LIFE_MIN, LIFE_MAX, ANGLE_MIN, ANGLE_MAX, SPEED_MIN, SPEED_MAX, Game1.gravity, Particle.BOWLING_BALL, Color.White, COLLISION, FADE)
        {
        }

        protected override void LaunchAll()
        {
            if (!reachedNumParticles)
            {
                for (int i = 0; i < numParticles; i++)
                {
                    particles.Add(CreateParticles());
                }

                if (launchTimer.IsFinished())
                {
                    if (particles.Count > 0)
                    {
                        for (int i = 0; i < numParticles; i++)
                        {
                            if (numLaunched < numParticles)
                            {
                                numLaunched++;
                                particles[i].Launch(new Vector2(emitterX, emitterY));
                            }
                        }
                    }

                    reachedNumParticles = true;
                }
            }
        }

        public override void Update(GameTime gameTime, List<Platform> platforms)
        {
            launchTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            if (state == ACTIVE)
            {
                LaunchAll();

                for (int i = 0; i < particles.Count; i++)
                {
                    if (particles[i].GetState() == Particle.ACTIVE)
                    {
                        particles[i].Update(gameTime, platforms);
                    }
                    else if (particles[i].GetState() == Particle.DEAD)
                    {
                        particles.RemoveAt(i);
                    }
                }

                if (particles.Count == 0)
                {
                    state = DEAD;
                    //running = false;
                }
            }
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (state != INACTIVE)
            {
                for (int i = 0; i < particles.Count; i++)
                {
                    particles[i].Draw(spriteBatch);
                }
            }
        }
    }
}
