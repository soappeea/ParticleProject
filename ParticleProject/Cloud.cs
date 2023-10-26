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
        private int cloudSpeedMin;
        private int cloudSpeedMax;


        public Cloud(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade, int radius, GraphicsDevice gd, int cloudSpeedMin, int cloudSpeedMax)
                     : base(img, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                                  speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade)
        {
            this.cloudSpeedMin = cloudSpeedMin;
            this.cloudSpeedMax = cloudSpeedMax;
            cloudCirc = new Circ(img, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                                  speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade, radius, gd);

        }

        public override void Update(GameTime gameTime, List<Platform> platforms)
        {
            cloudCirc.Update(gameTime, platforms);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (showLauncher)
            {
                //KEYNOTE: THE RECTANGLE IS CENTRED AT EMITTER POSITION, AND THE TOP LEFT IS LOCATED AT THE EMITTER POSITION MINUS HALF THE WIDTH AND HEIGHT OF THE RECTNAGLE 
                cloudCirc.Draw(spriteBatch);
                //for (int i = 0; i < particles.Count; i++)
                //{
                //    particles[i].Draw(spriteBatch);
                //}

                spriteBatch.Draw(img, GetRectangle()/*launchPos*/, Color.White);
            }



        }


    }
}
