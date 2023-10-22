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
        //for rng max restraint
        private const int INCREMENT = 1;

        //Used to define and Emitter to never stop launching
        public const int INFINITE = -1;

        //Used when defining an Emitter to enforce an upper limit on the # of particles, also used when trying to emit all particles at once 
        public const int MAX_PARTICLES = 5000;

        public const int INACTIVE = 0;
        public const int ACTIVE = 1;
        public const int DEAD = 2;
        public const int DONE = 3;
        protected int state;

        protected Texture2D img;
        protected Rectangle rec;
        protected float scale = 1f;
        protected Vector2 pos;
        protected int numParticles;
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

        //public for debugging purpose
        protected Timer launchTimer;

        protected Color colour;
        protected bool envCollisions;
        protected bool fade;
        protected bool drawn;
        protected bool explode;
        protected bool running;
        protected bool showLauncher;


        protected float emitterX;
        protected float emitterY;
        protected int imgWidth;
        protected int imgHeight;
        //private SpriteFont hudFont;

        //protected double timeSinceLastLaunch;
        //protected double launchInterval;
        //protected int partCount = -1;

        protected List<Particle> particles = new List<Particle>();

        public Emitter(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade)
        {
            this.img = img;
            this.scale = scale;
            if (img != null)
            {
                this.imgWidth = (int)(img.Width * scale);
                this.imgHeight = (int)(img.Height * scale);
                this.emitterX = pos.X;
                this.emitterY = pos.Y;
                this.state = INACTIVE;
                this.running = false;
                
            }
            else
            {
                //Mouse emitter specific
                this.emitterX = pos.X;
                this.emitterY = pos.Y;
                this.state = ACTIVE;
                this.running = true;

            }
            //this.pos = pos;

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
            this.reboundScaler = reboundScaler;
            this.forces = forces;
            this.colour = colour;
            this.envCollisions = envCollisions;
            this.fade = fade;

            if (launchTimeMin == -1 || launchTimeMax == -1)
            {
                explode = true;
                this.launchTimer = new Timer(1, true);
            }
            else
            {
                this.launchTimer = new Timer(GetRandInt(launchTimeMin, launchTimeMax), false);
            }

            this.drawn = (img != null && scale > 0);
        }

        protected int GetRandInt(int min, int max)
        {
            int randNum = Game1.rng.Next(min, max + INCREMENT);
            return randNum;
        }

        protected float GetRandFloat(int min, int max)
        {
            float randFloat = (float)(Game1.rng.Next(min, max + INCREMENT)) / 100;
            return randFloat;
        }

        public virtual void ChangeState()
        {
            running = !running;

            if (running)
            {
                state = ACTIVE;
            }
            else
            {
                state = INACTIVE;
            }
        }
        public int GetState()
        {
            return state;
        }

        private Rectangle GetRectangle()
        {
            return new Rectangle((int)(this.emitterX - this.imgWidth / 2),
                                           (int)(this.emitterY - this.imgHeight / 2), imgWidth, imgHeight);
        }

        protected Vector2 GetPosition()
        {
            return new Vector2(emitterX, emitterY);
        }
        public virtual void SetPos(float x, float y)
        {
            emitterX = x;
            emitterY = y;
        }

        protected virtual void Launch()
        {
            if (!launchTimer.IsActive())
            {
                particles.Add(CreateParticles());

                if (particles.Count > 0)
                    particles[particles.Count - 1].Launch(new Vector2(emitterX, emitterY));

                launchTimer.ResetTimer(true, GetRandInt(launchTimeMin, launchTimeMax));
            }
        }
        protected virtual void LaunchAll()
        {
            //Create all the numPurticles of particlesm then launch ALL 
            if (numParticles != INFINITE)
            {
                for (int i = 0; i < numParticles; i++)
                {
                    particles.Add(CreateParticles());
                }
            }
        }
        protected Particle CreateParticles()
        {
            float partScale = GetRandFloat((int)(scaleMin * 100), (int)(scaleMax * 100));
            int lifeSpan = GetRandInt(lifeMin, lifeMax);
            int angle = GetRandInt(angleMin, angleMax);
            float speed = GetRandFloat((int)(speedMin * 100), (int)(speedMax * 100));


            Particle newParticle = new Particle(partImg, partScale, lifeSpan, angle,
                                speed, forces, fade, reboundScaler, colour, envCollisions);

            return newParticle;
        }

        public virtual void Update(GameTime gameTime, List<Platform> platforms)
        {
            launchTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            if (img != null && scale > 0)
            {
                drawn = true;
            }

            if (numParticles == INFINITE && !explode)
            {
                Launch();
            }

            if (explode)
            {
                LaunchAll();
            }

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
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (drawn)
                spriteBatch.Draw(img, GetRectangle(), colour);

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
