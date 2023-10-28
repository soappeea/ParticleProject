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
        //for debugging, draw circle and draw it on top of cloud

        //CHECK FOR COLLISION OF CLOUD TO RIGHT OR LEFT, THEN REVERSE DIRECTION
        private Circ cloudCirc;
        private const int WALL_LEFT = 0;
        private const int WALL_RIGHT = 1;
        private const int DIR = -1;
        private int cloudSpeedMin;
        private int cloudSpeedMax;
        private int cloudSpeed;

        public Cloud(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade, int radius, GraphicsDevice gd,
                        int cloudSpeedMin, int cloudSpeedMax, Texture2D emitterImg, bool showCirc)
                     : base(img, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                                  speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade)
        {
            this.cloudSpeedMin = cloudSpeedMin;
            this.cloudSpeedMax = cloudSpeedMax;

            cloudCirc = new Circ(emitterImg, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                                  speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade, radius, gd, showCirc);
            cloudSpeed = GetRandInt(cloudSpeedMin, cloudSpeedMax);
        }

        public void ToggleCloudCirc()
        {
            cloudCirc.ToggleOnOff();
            cloudCirc.Activate();

            if (!cloudCirc.GetShowLaunch())
            {
                cloudCirc.ToggleLauncherVisibility();
            }
        }

        private void TranslateCloud(List<Platform> platforms)
        {
            if (pos.X + imgWidth / 2 >= platforms[WALL_RIGHT].GetBoundingBox().X || pos.X - imgWidth / 2 <= platforms[WALL_LEFT].GetBoundingBox().X + platforms[WALL_LEFT].GetBoundingBox().Width)
            {
                cloudSpeed *= DIR;
            }
            cloudCirc.SetCirclePos(cloudSpeed);
            pos = cloudCirc.GetPosition();
            emitterX = pos.X;
            emitterY = pos.Y;
        }
        public override void Update(GameTime gameTime, List<Platform> platforms)
        {
            TranslateCloud(platforms);
            cloudCirc.Update(gameTime, platforms);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (showLauncher)
            {
                cloudCirc.Draw(spriteBatch);

                spriteBatch.Draw(img, GetRectangle(), Color.White);
            }



        }


    }
}
