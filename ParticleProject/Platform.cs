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
    class Platform
    {
        private Texture2D img;
        private Rectangle rec;
        private Rectangle[] blockRecs;

        public Platform(Texture2D blockImg, int numBricks, float scale, int x, int y, bool isHorizontal)
        {
            img = blockImg;
            blockRecs = new Rectangle[Math.Max(1,numBricks)];

            int width = (int)(img.Width * scale);
            int height = (int)(img.Height * scale);

            for (int i = 0; i < numBricks; i++)
            {
                blockRecs[i] = new Rectangle(x + (width * i * (isHorizontal ? 1 : 0)),  //Add on to x for each block if it is horizontal
                                             y + (height * i * (isHorizontal ? 0 : 1)), //Add on to y for each block if it is not horizontal
                                             width, height);
            }

            rec = new Rectangle(x, y, 
                                blockRecs[numBricks - 1].Right - blockRecs[0].Left,   //width = Right side of last block - Left side of first block
                                blockRecs[numBricks - 1].Bottom - blockRecs[0].Top);  //height = Bottom side of last block - Top side of first block
        }

        public Rectangle GetBoundingBox()
        {
            return rec;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < blockRecs.Length; i++)
            {
                spriteBatch.Draw(img, blockRecs[i], Color.White);
            }
        }
    }
}
