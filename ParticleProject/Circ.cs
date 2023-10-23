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
        public Circ(Texture2D img, float scale, Vector2 pos, int numParticles, int launchTimeMin, int launchTimeMax, Texture2D partImg,
                        float scaleMin, float scaleMax, int lifeMin, int lifeMax, int angleMin, int angleMax, int speedMin, int speedMax,
                        Vector2 forces, float reboundScaler, Color colour, bool envCollisions, bool fade, int radius, GraphicsDevice gd)
                    : base(img, scale, pos, numParticles, launchTimeMin, launchTimeMax, partImg, scaleMin, scaleMax, lifeMin, lifeMax, angleMin, angleMax,
                          speedMin, speedMax, forces, reboundScaler, colour, envCollisions, fade)
        {

        }
    }
}
