//Author: Sophia Lin
//File Name: Particle.cs
//Project Name: Particle Project
//Creation Date: October 18, 2023
//Modified Date: October 30, 2023
//Description: Handle particle actions
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
    class Particle
    {
        //Store the state of the particle
        public const int INACTIVE = 0;
        public const int ACTIVE = 1;
        public const int DEAD = 2;

        //Store the rebound rate of the particle
        public const float RUBBER_BALL = 0.8f;
        public const float BOWLING_BALL = 0.3f;
        public const float SPLAT_BALL = 0.1f;
        private const int REBOUND = -1;

        //Store the side of the particle
        private const int TOP = 1;
        private const int BOTTOM = 2;
        private const int LEFT = 3;
        private const int RIGHT = 4;

        //Store the particle's information
        private Texture2D img;
        private Rectangle rec;
        private Vector2 pos;
        private float scale = 1f;
        private int lifeSpan;
        private Timer lifeTimer;
        private float angle;
        private Vector2 vel;
        private Vector2 forces;
        private float reboundScaler;
        private Color colour;
        private bool envCollisions;
        private int state;
        private float opacity;
        private bool fade;

        //Store the particle's speed tolerance
        private float speedTolerance = 0.005f;

        /// <summary>
        /// Create an instance of particle
        /// </summary>
        /// <param name="img">Particle image</param>
        /// <param name="scale">Particle scale</param>
        /// <param name="lifeSpan">Particle's lifespan</param>
        /// <param name="angle">Angle particle is launched at</param>
        /// <param name="speed">Speed particle travels at</param>
        /// <param name="forces">Forces particle undergo</param>
        /// <param name="fade">Track if the particle fades</param>
        /// <param name="reboundScaler">Scale of how much particle rebounds</param>
        /// <param name="colour">Colour of particle</param>
        /// <param name="envCollisions">Track if the particle collides with the environment</param>
        public Particle(Texture2D img, float scale, int lifeSpan, int angle, float speed, Vector2 forces, bool fade, float reboundScaler, Color colour, bool envCollisions)
        {
            //Set particle information
            this.img = img;
            this.scale = scale;
            this.rec = new Rectangle(0, 0, (int)(img.Width * scale), (int)(img.Height * scale));
            this.lifeSpan = lifeSpan;
            this.lifeTimer = new Timer(lifeSpan, false);
            this.angle = MathHelper.ToRadians(angle);
            this.vel = new Vector2((float)(speed * Math.Cos(this.angle)), -(float)(speed * Math.Sin(this.angle)));
            this.forces = forces;
            this.fade = fade;

            //Set extra features of particle
            this.reboundScaler = reboundScaler;
            this.colour = colour;
            this.envCollisions = envCollisions;

            //Set particle state and appearance
            this.state = INACTIVE;
            this.opacity = 1f;
        }

        /// <summary>
        /// Retrieve the state of the particle
        /// </summary>
        /// <returns>The state of the particle as an int</returns>
        public int GetState()
        {
            return state;
        }

        /// <summary>
        /// Set the position 
        /// </summary>
        /// <param name="pos"></param>
        private void SetPosition(Vector2 pos)
        {
            //Set position
            this.pos = pos;

            //Centre the particle's rectangle
            rec.X = (int)this.pos.X - rec.Width / 2;
            rec.Y = (int)this.pos.Y - rec.Height / 2;
        }

        /// <summary>
        /// Update the particle
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="platforms">List of platforms for collision detection</param>
        public void Update(GameTime gameTime, List<Platform> platforms)
        {
            //Update the particle if it is in an active state
            if (state == ACTIVE)
            {
                //Update the life timer
                lifeTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                //Handle the particle's actions according to its timer's state
                if (lifeTimer.IsActive())
                {
                    //Particle fades as time passes
                    if (fade) opacity = lifeTimer.GetTimeRemainingInt() / (float)lifeSpan;

                    //Affect the particle's velocity with forces 
                    vel += forces;

                    //Translate particle
                    Translate(vel * (float)gameTime.ElapsedGameTime.TotalSeconds);

                    //Handle collision detection between particles and platforms
                    if (envCollisions && platforms != null)
                    {
                        //Detect collision between top of particle and platforms
                        CollisionDetection(rec.X + rec.Width / 2, rec.Y, platforms, TOP);

                        //Detect collision between bottom of particle and platforms
                        CollisionDetection(rec.X + rec.Width / 2, rec.Y + rec.Height, platforms, BOTTOM);

                        //Detect collision between left of particle and platforms
                        CollisionDetection(rec.X, rec.Y + rec.Height / 2, platforms, LEFT);

                        //Detect collision between right of particle and platforms
                        CollisionDetection(rec.X + rec.Width, rec.Y + rec.Height / 2, platforms, RIGHT);
                    }

                    //Stop the particle when it's velocity goes below the tolerance
                    if (Math.Abs(vel.X) < speedTolerance && Math.Abs(vel.Y) < speedTolerance)
                    {
                        vel = Vector2.Zero;
                        forces = Vector2.Zero;
                    }
                }
                else if (lifeTimer.IsFinished())
                {
                    //Set particle's state to dead when it's timer is complete
                    state = DEAD;
                }
            }
        }

        /// <summary>
        /// Draw the particle
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the particle if its lifespan is active
            if (state == ACTIVE && opacity > 0)
            {
                spriteBatch.Draw(img, rec, colour * opacity);
            }
        }

        /// <summary>
        /// Launch the particle
        /// </summary>
        /// <param name="startPos">The position the particle launches from</param>
        public void Launch(Vector2 startPos)
        {
            //Set the particle state to active and launch
            if (state == INACTIVE)
            {
                state = ACTIVE;
                SetPosition(startPos);
                lifeTimer.Activate();
            }
        }

        /// <summary>
        /// Detect collision between particle and platforms
        /// </summary>
        /// <param name="x">The x-coordinate of the particle's midpoint</param>
        /// <param name="y">The y-coordinate of the particle's midpoint</param>
        /// <param name="platforms">The list of platforms</param>
        /// <param name="side">The side that the particle</param>
        private void CollisionDetection(float x, float y, List<Platform> platforms, int side)
        {
            //Check for collision between every platform
            for (int i = 0; i < platforms.Count; i++)
            {
                //Collision occurred
                if (platforms[i].GetBoundingBox().Contains(x, y))
                {
                    //Perform adjustment and rebound the particle based on which side collided with
                    switch (side)
                    {
                        case TOP:
                            //Reposition the particle and adjust velocity
                            pos.Y = platforms[i].GetBoundingBox().Y + platforms[i].GetBoundingBox().Height + rec.Height / 2;
                            SetPosition(pos);
                            vel.Y *= REBOUND * reboundScaler;
                            break;
                        case BOTTOM:
                            //Reposition the particle and adjust velocity
                            pos.Y = platforms[i].GetBoundingBox().Y - rec.Height / 2;
                            SetPosition(pos);
                            vel.Y *= REBOUND * reboundScaler;
                            break;
                        case LEFT:
                            //Reposition the particle and adjust velocity
                            pos.X = platforms[i].GetBoundingBox().X + platforms[i].GetBoundingBox().Width + rec.Width / 2;
                            SetPosition(pos);
                            vel.X *= REBOUND * reboundScaler;
                            break;
                        case RIGHT:
                            //Reposition the particle and adjust velocity
                            pos.X = platforms[i].GetBoundingBox().X - platforms[i].GetBoundingBox().Width / 2;
                            SetPosition(pos);
                            vel.X *= REBOUND * reboundScaler;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Translate the location 
        /// </summary>
        /// <param name="deltaVel">Pixels per update</param>
        private void Translate(Vector2 deltaVel)
        {
            //Change the particle's position
            pos += deltaVel;

            //Centre the particle's rectangle
            rec.X = (int)pos.X - rec.Width / 2;
            rec.Y = (int)pos.Y - rec.Height / 2;
        }

    }
}
