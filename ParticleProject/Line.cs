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
    class Line : Emitter
    {
        private const float SPEED = 2f;
        private const int DIR = -1;
        private GraphicsDevice gd;
        private GameLine line;
        private Vector2 pos1;
        private Vector2 pos2;
        private Vector2 launchPos;
        private KeyboardState kb;
        public Line(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade, int lineLength, GraphicsDevice gd, int lineWidth)
                    : base(img, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                          speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade)
        {
            pos1 = pos;
            pos2.X = pos.X + lineLength;
            pos2.Y = pos.Y;
            line = new GameLine(gd, pos1, pos2, lineWidth);
        }

        private Vector2 GetLaunchPos()
        {
            int xPos = GetRandInt((int)pos1.X, (int)pos2.X);
            int yPos = GetRandInt((int)pos1.Y, (int)pos2.Y);
            return new Vector2(xPos, yPos);
        }

        //Questionable
        //public override void SetPos(float x, float y)
        //{
        //    emitterX = x;
        //    emitterY = y;
        //}

        private void TranslateLine()
        {
            kb = Keyboard.GetState();
            //Move the line emitter
            if (kb.IsKeyDown(Keys.D))
            {
                line.Translate(SPEED, 0f);
                pos1.X += SPEED;
                pos2.X += SPEED;
            }
            else if (kb.IsKeyDown(Keys.A))
            {
                line.Translate(SPEED * DIR, 0f);
                pos1.X -= SPEED;
                pos2.X -= SPEED;
            }
            else if (kb.IsKeyDown(Keys.W))
            {
                line.Translate(0f, SPEED * DIR);
                pos1.Y -= SPEED;
                pos2.Y -= SPEED;
            }
            else if (kb.IsKeyDown(Keys.S))
            {
                line.Translate(0f, SPEED);
                pos1.Y += SPEED;
                pos2.Y += SPEED;
            }
        }
        protected override void Launch()
        {
            if (!launchTimer.IsActive())
            {
                particles.Add(CreateParticles());
                launchPos = GetLaunchPos();
                if (particles.Count > 0)
                {
                    SetPos(launchPos.X, launchPos.Y);
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

                TranslateLine();

                if (!explode)
                {
                    Launch();
                }

                if (explode)
                {
                    LaunchAll();
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
                line.Draw(spriteBatch, Color.Black);

                for (int i = 0; i < particles.Count; i++)
                {
                    particles[i].Draw(spriteBatch);
                }

                //spriteBatch.Draw(img, GetRectangle()/*launchPos*/, Color.White);
            }

        }
    }
}
