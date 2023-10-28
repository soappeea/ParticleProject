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
        public const int INACTIVE = 0;
        public const int ACTIVE = 1;
        public const int DEAD = 2;

        public const float RUBBER_BALL = 0.8f;
        public const float BOWLING_BALL = 0.3f;
        public const float SPLAT_BALL = 0.1f;         //90% of velocity taken away when hit the wall
        private const int REBOUND = -1;

        private const int TOP = 1;
        private const int BOTTOM = 2;
        private const int LEFT = 3;
        private const int RIGHT = 4;

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

        private float speedTolerance = 0.005f;

        public Particle(Texture2D img, float scale, int lifeSpan, int angle, float speed, Vector2 forces, bool fade, float reboundScaler, Color colour, bool envCollisions)
        {
            this.img = img;
            this.scale = scale;
            this.rec = new Rectangle(0, 0, (int)(img.Width * scale), (int)(img.Height * scale));
            this.lifeSpan = lifeSpan;
            this.lifeTimer = new Timer(lifeSpan, false);
            this.angle = MathHelper.ToRadians(angle);
            this.vel = new Vector2((float)(speed * Math.Cos(this.angle)), -(float)(speed * Math.Sin(this.angle)));
            this.forces = forces;
            this.fade = fade;

            this.reboundScaler = reboundScaler;
            this.colour = colour;
            this.envCollisions = envCollisions;

            this.state = INACTIVE;
            this.opacity = 1f;
        }

        public void Update(GameTime gameTime, List<Platform> platforms)
        {
            if (state == ACTIVE)
            {
                lifeTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                if (lifeTimer.IsActive())
                {
                    if (fade) opacity = lifeTimer.GetTimeRemainingInt() / (float)lifeSpan;
                    vel += forces;
                    Translate(vel * (float)gameTime.ElapsedGameTime.TotalSeconds);

                    if (envCollisions && platforms != null)
                    {
                        //TODO: Handle Platform Collisions here.  Check the particle's rec against EACH platform
                        //If a collision is found, determine the side of the particle that made contact, by checking
                        //collision with the midpoint of each wall of the particle's bounding box.  Then modify
                        //velocity as needed and adjust the particle's position to eliminate overlap

                        //Top
                        CollisionDetection(rec.X + rec.Width / 2, rec.Y, platforms, TOP);

                        //Bottom
                        CollisionDetection(rec.X + rec.Width / 2, rec.Y + rec.Height, platforms, BOTTOM);

                        //Left
                        CollisionDetection(rec.X, rec.Y + rec.Height / 2, platforms, LEFT);

                        //Right
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
                    state = DEAD;
                }
            }
        }

        private void CollisionDetection(float x, float y, List<Platform> platforms, int side)
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                if (platforms[i].GetBoundingBox().Contains(x, y))
                {
                    switch (side)
                    {
                        case TOP:
                            pos.Y = platforms[i].GetBoundingBox().Y + platforms[i].GetBoundingBox().Height + rec.Height / 2;
                            SetPosition(pos);
                            vel.Y *= REBOUND * reboundScaler;
                            break;
                        case BOTTOM:
                            pos.Y = platforms[i].GetBoundingBox().Y - rec.Height / 2;
                            SetPosition(pos);
                            vel.Y *= REBOUND * reboundScaler;
                            break;
                        case LEFT:
                            pos.X = platforms[i].GetBoundingBox().X + platforms[i].GetBoundingBox().Width + rec.Width / 2;
                            SetPosition(pos);
                            vel.X *= REBOUND * reboundScaler;
                            break;
                        case RIGHT:
                            pos.X = platforms[i].GetBoundingBox().X - platforms[i].GetBoundingBox().Width / 2;
                            SetPosition(pos);
                            vel.X *= REBOUND * reboundScaler;
                            break;
                    }

                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (state == ACTIVE && opacity > 0)
            {
                spriteBatch.Draw(img, rec, colour * opacity);
            }
        }

        private void Translate(Vector2 deltaVel)
        {
            pos += deltaVel;
            rec.X = (int)pos.X - rec.Width / 2;
            rec.Y = (int)pos.Y - rec.Height / 2;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(this.pos.X - rec.Width / 2, this.pos.Y - rec.Height / 2);
            //this.pos = pos;
            //rec.X = (int)this.pos.X - rec.Width / 2;
            //rec.Y = (int)this.pos.Y - rec.Height / 2;
        }
        private void SetPosition(Vector2 pos)
        {
            this.pos = pos;
            rec.X = (int)this.pos.X - rec.Width / 2;
            rec.Y = (int)this.pos.Y - rec.Height / 2;
        }

        public void Launch(Vector2 startPos)
        {
            if (state == INACTIVE)
            {
                state = ACTIVE;
                SetPosition(startPos);
                lifeTimer.Activate();
            }
        }

        public int GetState()
        {
            return state;
        }
    }
}
