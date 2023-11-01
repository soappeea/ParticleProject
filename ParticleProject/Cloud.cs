//Author: Sophia Lin
//File Name: Cloud.cs
//Project Name: Particle Project
//Creation Date: October 24, 2023
//Modified Date: October 30, 2023
//Description: Handle cloud emitter
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
    class Cloud : Emitter
    {
        //Store environment information
        private const int WALL_LEFT = 0;
        private const int WALL_RIGHT = 1;
        private const int DIR = -1;

        //Store cloud emitter's information
        private Circ cloudCirc;
        private int cloudSpeedMin;
        private int cloudSpeedMax;
        private int cloudSpeed;

        /// <summary>
        /// Create an instance of the cloud emitter
        /// </summary>
        /// <param name="img">Cloud Emitter image</param>
        /// <param name="scale">Scale for the emitter image</param>
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
        /// <param name="radius">Radius of circle launcher</param>
        /// <param name="gd">GraphicsDevice variable to create the GameCircle</param>
        ///<param name="cloudSpeedMin">Minimum speed of cloud emitter</param>
        /// <param name="cloudSpeedMax">Maximum speed of cloud emitter</param>
        /// <param name="emitterImg">Temporary internal emitter image</param>
        /// <param name="showCirc">Tracks whether to show circle or not</param>
        public Cloud(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade, int radius, GraphicsDevice gd,
                        int cloudSpeedMin, int cloudSpeedMax, Texture2D emitterImg, bool showCirc)
                     : base(img, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                                  speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade)
        {
            //Set cloud information
            this.cloudSpeedMin = cloudSpeedMin;
            this.cloudSpeedMax = cloudSpeedMax;
            cloudSpeed = GetRandInt(cloudSpeedMin, cloudSpeedMax);

            //Create a circle to emit particles from
            cloudCirc = new Circ(emitterImg, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                                  speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade, radius, gd, showCirc);
        }

        /// <summary>
        /// Toggle the cloud's circle
        /// </summary>
        public void ToggleCloudCirc()
        {
            //Toggle cloud circle
            cloudCirc.ToggleOnOff();
            cloudCirc.Activate();

            //Toggle visiblity of cloud
            if (!cloudCirc.GetShowLaunch())
            {
                cloudCirc.ToggleLauncherVisibility();
            }
        }

        /// <summary>
        /// Update the cloud launcher
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="platforms">List of platforms for collision detection</param>
        public override void Update(GameTime gameTime, List<Platform> platforms)
        {
            //Translate the cloud
            TranslateCloud(platforms);

            //Update the cloud's circle
            cloudCirc.Update(gameTime, platforms);
        }

        /// <summary>
        /// Draw the cloud and its emitter
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw if launcher is visible
            if (showLauncher)
            {
                //Draw cloud's circle
                cloudCirc.Draw(spriteBatch);

                //Draw cloud
                spriteBatch.Draw(img, GetRectangle(), Color.White);
            }
        }

        /// <summary>
        /// Translate the cloud
        /// </summary>
        /// <param name="platforms"></param>
        private void TranslateCloud(List<Platform> platforms)
        {
            //Alter direction if the cloud hits either the left or right platform wall
            if (pos.X + imgWidth / 2 >= platforms[WALL_RIGHT].GetBoundingBox().X || pos.X - imgWidth / 2 <= platforms[WALL_LEFT].GetBoundingBox().X + platforms[WALL_LEFT].GetBoundingBox().Width)
            {
                cloudSpeed *= DIR;
            }

            //Set the new position of the cloud circle
            cloudCirc.SetCirclePos(cloudSpeed);

            //Set the position of the cloud
            pos = cloudCirc.GetPosition();
            emitterX = pos.X;
            emitterY = pos.Y;
        }
    }
}
