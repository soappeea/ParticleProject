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
        protected int INFINITE = -1;
        protected int MAX_PARTICLES = 5000;
        private const int INACTIVE = 0;
        private const int ACTIVE = 1;
        private const int DEAD = 2;
        private const int DONE = 3;
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
        protected Timer launchTimer;
        protected Color colour;
        protected bool envCollisions;
        protected bool fade;
        protected bool explode;
        protected bool running;
        protected bool showLauncher;

        protected List <Particle> particles = new List<Particle>();

        public Emitter(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg, 
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax, 
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade)
        {
            this.img = img;
            this.scale = scale;
            this.rec = new Rectangle(0, 0, (int)(img.Width * scale), (int)(img.Height * scale));
            this.pos = pos;

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

            this.forces = forces;
            this.reboundScaler = reboundScaler;
            this.colour = colour;
            this.envCollisions = envCollisions;
            this.fade = fade;

            this.state = INACTIVE;

        }

        //private int GetRandNum(int min, int max)
        //{

        //}
        //private float GetRandFloat(int min, int max)
        //{

        //}
        //public int SetState(int state)
        //{

        //}
        //public int SetPos(Vector2 pos)
        //{

        //}

        //public Particle CreateParticles()
        //{
        //    particles.Add(new Particle(partImg, float partScale, lifeSpan, int angle, float speed, Vector2 forces, bool fade, float reboundScaler, Color colour, bool envCollisions));
        //}
        public virtual void EmitParticles()
        {

        }
        public virtual void EmitAllParticles()
        {

        }
        public virtual void Update(GameTime gameTIme)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }



    }
}
