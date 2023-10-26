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
        private int width;
        private int height;
        private GraphicsDevice gd;
        private GameRectangle rectangle;
        private Vector2 launchPos;
        private Vector2 centeredPos;
        private Vector2 launchDimensions;


        public Rect(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade, int width, int height, GraphicsDevice gd)
                    : base(img, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                          speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade)
        {
            this.width = width;
            this.height = height;
            this.gd = gd;
            this.pos.X = pos.X;
            this.pos.Y = pos.Y;
            this.centeredPos.X = (int)pos.X - width / 2;
            this.centeredPos.Y = (int)pos.Y - height / 2;
            rectangle = new GameRectangle(gd, (int)centeredPos.X, (int)centeredPos.Y, width, height);
            SetLaunchArea((int)centeredPos.X, (int)centeredPos.Y);
            this.state = INACTIVE;
            this.running = false;
        }

        public void SetLaunchArea(int x, int y)
        {
            launchDimensions.X = x + rectangle.Width;
            launchDimensions.Y = y + rectangle.Height;
        }
        private Vector2 GetLaunchPos(int x, int y)
        {
            int xPos = GetRandInt(x, (int)launchDimensions.X);
            int yPos = GetRandInt(y, (int)launchDimensions.Y);
            return new Vector2(xPos, yPos);
        }

        //Questionable
        public override void SetPos(float x, float y)
        {
            emitterX = x;
            emitterY = y;
            //SetLaunchArea((int)emitterX, (int)emitterY);
        }
        protected override void Launch()
        {
            if (!launchTimer.IsActive())
            {
                particles.Add(CreateParticles());
                launchPos = GetLaunchPos((int)centeredPos.X, (int)centeredPos.Y);
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
                rectangle.Draw(spriteBatch, Color.Blue * transparency, true);

                for (int i = 0; i < particles.Count; i++)
                {
                    particles[i].Draw(spriteBatch);
                }

                spriteBatch.Draw(img, GetRectangle()/*launchPos*/, Color.White);
            }

        }

    }
}
