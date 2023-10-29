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
        private const float ANGLE_INCREMENT = 0.25f;
        private const int ANGLE_CONVERSION = 180;
        private GraphicsDevice gd;
        private GameLine line;
        private Vector2 pos1;
        private Vector2 pos2;
        private Vector2 launchPos;
        private Vector2 pos2Update;
        private KeyboardState kb;
        private float lineAngle;
        private int lineLength;
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
            this.lineLength = lineLength;
        }

        private Vector2 GetLaunchPos()
        {
            int xPos;
            int yPos;
            float slope;

            slope = (pos2.Y - pos1.Y) / (pos2.X - pos1.X);

            if (pos2.X == pos1.X)
            {
                slope = 0;
            }
            //if pos1x > pos2x then do that otherwise do pos2.x, pos1.x (same with posy)
            if (pos1.X < pos2.X)
            {
                xPos = GetRandInt((int)pos1.X, (int)pos2.X);
            }
            else
            {
                xPos = GetRandInt((int)pos2.X, (int)pos1.X);
            }

            if (pos1.Y < pos2.Y)
            {
                //yPos = Convert.ToInt32(pos1.Y + slope * xPos);
                yPos = GetRandInt((int)pos1.Y, (int)pos2.Y);
            }
            else
            {
                //yPos = Convert.ToInt32(pos1.Y - slope * xPos);
                yPos = GetRandInt((int)pos2.Y, (int)pos1.Y);
            }
            
            
            return new Vector2(xPos, yPos);
        }

        //Questionable
        //public override void SetPos(float x, float y)
        //{
        //    pos2.X = x;
        //    pos2.Y = y;
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

            if (kb.IsKeyDown(Keys.L))
            {
                //lineAngle += ANGLE_INCREMENT;
                lineAngle++;
                ChangeLineRotation();
                line.SetPt2(pos2Update);
                //SetPos(pos2Update.X, pos2Update.Y);
                pos2.X = pos2Update.X;
                pos2.Y = pos2Update.Y;

            }
            else if (kb.IsKeyDown(Keys.J))
            {
                //lineAngle -= ANGLE_INCREMENT;
                lineAngle--;
                ChangeLineRotation();
                line.SetPt2(pos2Update);
                //SetPos(pos2Update.X, pos2Update.Y);
                pos2.X = pos2Update.X;
                pos2.Y = pos2Update.Y;
            }
            
        }

        private void ChangeLineRotation()
        {
            if (lineAngle <= 90 && lineAngle >= 0)
            {
                pos2Update.X = pos1.X + Math.Abs(lineLength * (float)Math.Cos(Math.PI * lineAngle / ANGLE_CONVERSION));
                pos2Update.Y = pos1.Y + Math.Abs(lineLength * (float)Math.Sin(Math.PI * lineAngle / ANGLE_CONVERSION));
            }
            else if (lineAngle > 90 && lineAngle <= 180)
            {
                pos2Update.X = pos1.X - Math.Abs(lineLength * (float)Math.Cos(Math.PI * lineAngle / ANGLE_CONVERSION));
                pos2Update.Y = pos1.Y + Math.Abs(lineLength * (float)Math.Sin(Math.PI * lineAngle / ANGLE_CONVERSION));
            }
            else if (lineAngle > 180 && lineAngle <= 270)
            {
                pos2Update.X = pos1.X - Math.Abs(lineLength * (float)Math.Cos(Math.PI * lineAngle / ANGLE_CONVERSION));
                pos2Update.Y = pos1.Y - Math.Abs(lineLength * (float)Math.Sin(Math.PI * lineAngle / ANGLE_CONVERSION));
            }
            else if (lineAngle > 270 && lineAngle <= 360)
            {
                pos2Update.X = pos1.X + Math.Abs(lineLength * (float)Math.Cos(Math.PI * lineAngle / ANGLE_CONVERSION));
                pos2Update.Y = pos1.Y - Math.Abs(lineLength * (float)Math.Sin(Math.PI * lineAngle / ANGLE_CONVERSION));
            }

            if (lineAngle > 360)
            {
                lineAngle = 0;
            }
            else if (lineAngle < 0)
            {
                lineAngle = 360;
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
