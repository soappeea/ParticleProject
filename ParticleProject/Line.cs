//Author: Sophia Lin
//File Name: Line.cs
//Project Name: Particle Project
//Creation Date: October 27, 2023
//Modified Date: October 30, 2023
//Description: Handle line emitter
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
        //Store misc information
        private const int DIR = -1;
        private const int ANGLE_CONVERSION = 180;

        //Store line emitter information
        private const float SPEED = 2f;
        private float lineAngle;
        private int lineLength;

        //Store GameLine
        private GameLine line;

        //Store position of each point on line
        private Vector2 pos1;
        private Vector2 pos2;

        //Store launch position of particle on line and updated position of a point on the line
        private Vector2 launchPos;
        private Vector2 pos2Update;

        //Store input state
        private KeyboardState kb;

        /// <summary>
        /// Create an instance of the line emitter
        /// </summary>
        /// <param name="img">Line emitter image</param>
        /// <param name="scale">Scale for line emitter image</param>
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
        /// <param name="lineLength">Length of the line</param>
        /// <param name="gd">GraphicsDevice variable to create the GameLine</param>
        /// <param name="lineWidth">Width of the line</param>
        public Line(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade, int lineLength, GraphicsDevice gd, int lineWidth)
                    : base(img, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                          speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade)
        {
            //Set line information
            pos1 = pos;
            pos2.X = pos.X + lineLength;
            pos2.Y = pos.Y;
            line = new GameLine(gd, pos1, pos2, lineWidth);
            this.lineLength = lineLength;
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
                launchPos = GetLaunchPos();

                //Launch the particle
                if (particles.Count > NO_PARTICLES)
                {
                    //Launch the particle
                    particles[particles.Count - 1].Launch(launchPos);
                }

                //Reset the launch timer once the particle is launched so the next particle can launch once it is over
                launchTimer.ResetTimer(true, GetRandInt(launchTimeMin, launchTimeMax));
            }
        }

        /// <summary>
        ///  Update the line emitter
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

                //Translate the line emitter
                TranslateLine();

                //Launch the particles one by one infinitely
                if (!explode)
                {
                    Launch();
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
        }

        /// <summary>
        /// Draw line emitter and its particles
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw if launcher is visible
            if (showLauncher)
            {
                //Draw line emitter
                line.Draw(spriteBatch, Color.Black);

                //Draw particles
                for (int i = 0; i < particles.Count; i++)
                {
                    particles[i].Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Retrieve launch position of particle 
        /// </summary>
        /// <returns>Vector2 of position particles will launch from</returns>
        private Vector2 GetLaunchPos()
        {
            //Store location
            int xPos;
            int yPos;

            //Generate random x position
            if (pos1.X < pos2.X)
            {
                xPos = GetRandInt((int)pos1.X, (int)pos2.X);
            }
            else
            {
                xPos = GetRandInt((int)pos2.X, (int)pos1.X);
            }

            //Generate random y position
            if (pos1.Y < pos2.Y)
            {
                yPos = GetRandInt((int)pos1.Y, (int)pos2.Y);
            }
            else
            {
                yPos = GetRandInt((int)pos2.Y, (int)pos1.Y);
            }


            return new Vector2(xPos, yPos);
        }

        /// <summary>
        /// Translate the line
        /// </summary>
        private void TranslateLine()
        {
            //Update input device
            kb = Keyboard.GetState();

            float noChange = 0f;

            //Move the line emitter left or right using D OR A keys
            if (kb.IsKeyDown(Keys.D))
            {
                line.Translate(SPEED, noChange);
                pos1.X += SPEED;
                pos2.X += SPEED;
            }
            else if (kb.IsKeyDown(Keys.A))
            {
                line.Translate(SPEED * DIR, noChange);
                pos1.X -= SPEED;
                pos2.X -= SPEED;
            }

            //Move the line emitter up or down using W OR S keys
            if (kb.IsKeyDown(Keys.W))
            {
                line.Translate(noChange, SPEED * DIR);
                pos1.Y -= SPEED;
                pos2.Y -= SPEED;
            }
            else if (kb.IsKeyDown(Keys.S))
            {
                line.Translate(noChange, SPEED);
                pos1.Y += SPEED;
                pos2.Y += SPEED;
            }

            //Rotate line clockwise or counter-clockwise using L or J keys
            if (kb.IsKeyDown(Keys.L))
            {
                //Increment line angle
                lineAngle++;

                //Rotate line
                ChangeLineRotation();

                //Update one point on line
                line.SetPt2(pos2Update);
                pos2.X = pos2Update.X;
                pos2.Y = pos2Update.Y;

            }
            else if (kb.IsKeyDown(Keys.J))
            {
                //Decrement line angle
                lineAngle--;

                //Rotate line
                ChangeLineRotation();

                //Update one point on line
                line.SetPt2(pos2Update);
                pos2.X = pos2Update.X;
                pos2.Y = pos2Update.Y;
            }
        }

        /// <summary>
        /// Rotate the line according to its line angle
        /// </summary>
        private void ChangeLineRotation()
        {
            //Store degree in each quadrant
            int normal = 0;
            int quad1 = 90;
            int quad2 = 180;
            int quad3 = 270;
            int quad4 = 360;

            //Update the position point 2 of the line according to the line angle
            if (lineAngle <= quad1 && lineAngle >= normal)
            {
                pos2Update.X = pos1.X + Math.Abs(lineLength * (float)Math.Cos(Math.PI * lineAngle / ANGLE_CONVERSION));
                pos2Update.Y = pos1.Y + Math.Abs(lineLength * (float)Math.Sin(Math.PI * lineAngle / ANGLE_CONVERSION));
            }
            else if (lineAngle > quad1 && lineAngle <= quad2)
            {
                pos2Update.X = pos1.X - Math.Abs(lineLength * (float)Math.Cos(Math.PI * lineAngle / ANGLE_CONVERSION));
                pos2Update.Y = pos1.Y + Math.Abs(lineLength * (float)Math.Sin(Math.PI * lineAngle / ANGLE_CONVERSION));
            }
            else if (lineAngle > quad2 && lineAngle <= quad3)
            {
                pos2Update.X = pos1.X - Math.Abs(lineLength * (float)Math.Cos(Math.PI * lineAngle / ANGLE_CONVERSION));
                pos2Update.Y = pos1.Y - Math.Abs(lineLength * (float)Math.Sin(Math.PI * lineAngle / ANGLE_CONVERSION));
            }
            else if (lineAngle > quad3 && lineAngle <= quad4)
            {
                pos2Update.X = pos1.X + Math.Abs(lineLength * (float)Math.Cos(Math.PI * lineAngle / ANGLE_CONVERSION));
                pos2Update.Y = pos1.Y - Math.Abs(lineLength * (float)Math.Sin(Math.PI * lineAngle / ANGLE_CONVERSION));
            }

            //Account for if the line angle exceeds 360 or is smaller than 0
            if (lineAngle > quad4)
            {
                lineAngle = normal;
            }
            else if (lineAngle < normal)
            {
                lineAngle = quad4;
            }
        }
    }
}
