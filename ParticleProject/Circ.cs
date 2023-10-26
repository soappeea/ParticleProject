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

        private int radius;
        private GraphicsDevice gd;
        private GameCircle circle;
        private Vector2 launchPos;
        private Vector2 launchDimensions;

        public Circ(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade, int radius, GraphicsDevice gd)
                    : base(img, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                          speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade)
        {
            this.radius = radius;
            this.gd = gd;
            this.pos.X = pos.X;
            this.pos.Y = pos.Y;
            //this.centeredPos.X = (int)pos.X - width / 2;
            //this.centeredPos.Y = (int)pos.Y - height / 2;
            circle = new GameCircle(gd, pos, radius);
            SetLaunchArea((int)pos.X, (int)pos.Y);
            this.state = INACTIVE;
            this.running = false;
        }

        public void SetLaunchArea(int x, int y)
        {
            int angle = GetRandInt(angleMin, angleMax + INCREMENT);
            launchDimensions.X = Math.Abs(radius * (float)Math.Cos(angle));
            launchDimensions.Y = Math.Abs(radius * (float)Math.Sin(angle));
        }
        private Vector2 GetLaunchPos(int x, int y)
        {
            int xPos = 0;
            int yPos = 0;

            xPos = GetRandInt(x - (int)launchDimensions.X, x + (int)launchDimensions.X);
            yPos = GetRandInt(y - (int)launchDimensions.Y, y + (int)launchDimensions.Y);

            return new Vector2(xPos, yPos);
        }

        //Questionable
        //public override void SetPos(float x, float y)
        //{
        //    emitterX = x;
        //    emitterY = y; 
        //    SetLaunchArea((int)emitterX, (int)emitterY);
        //}
        protected override void Launch()
        {
            if (!launchTimer.IsActive())
            {
                particles.Add(CreateParticles());
                launchPos = GetLaunchPos((int)pos.X, (int)pos.Y);

                if (particles.Count > 0)
                {
                    SetPos(launchPos.X, launchPos.Y);
                    numLaunched++;
                    particles[particles.Count - 1].Launch(launchPos);
                }
                launchTimer.ResetTimer(true, GetRandInt(launchTimeMin, launchTimeMax));
            }
        }
        public override void Update(GameTime gameTime, List<Platform> platforms)
        {
            if (state == ACTIVE)
            {
                launchTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                if (!explode)
                {
                    Launch();
                }

                if (explode)
                {
                    LaunchAll();
                }

                if (numLaunched > numParticles)
                {
                    state = DEAD;
                }
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
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (showLauncher)
            {
                //KEYNOTE: THE RECTANGLE IS CENTRED AT EMITTER POSITION, AND THE TOP LEFT IS LOCATED AT THE EMITTER POSITION MINUS HALF THE WIDTH AND HEIGHT OF THE RECTNAGLE 
                circle.Draw(spriteBatch, Color.Red * transparency);

                for (int i = 0; i < particles.Count; i++)
                {
                    particles[i].Draw(spriteBatch);
                }

                spriteBatch.Draw(img, GetRectangle()/*launchPos*/, Color.White);
            }

        }

    }
}
