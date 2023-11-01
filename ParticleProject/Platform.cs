//Author: Sophia Lin
//File Name: Game1.cs
//Project Name: Particle Project
//Creation Date: October 18, 2023
//Modified Date: October 30, 2023
//Description: Handle platform
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
        //Store platform information
        private Texture2D img;
        private Rectangle rec;
        private Rectangle[] blockRecs;

        /// <summary>
        /// Create an instance of platform
        /// </summary>
        /// <param name="blockImg">Platform's block image</param>
        /// <param name="numBricks">Number of bricks of how long platform is</param>
        /// <param name="scale">Scale for platform</param>
        /// <param name="x">x-coordinate of platform</param>
        /// <param name="y">y-coordinate of platform</param>
        /// <param name="isHorizontal">Track if the platform is horizontal or vertical</param>
        public Platform(Texture2D blockImg, int numBricks, float scale, int x, int y, bool isHorizontal)
        {
            //Set platform image
            img = blockImg;

            //Set how many rectangles of the block image there will be in a platform
            blockRecs = new Rectangle[Math.Max(1, numBricks)];

            //Set dimensions of platform's block image
            int width = (int)(img.Width * scale);
            int height = (int)(img.Height * scale);

            //Set the rectangles that make up the platform
            for (int i = 0; i < numBricks; i++)
            {
                blockRecs[i] = new Rectangle(x + (width * i * (isHorizontal ? 1 : 0)),  //Add on to x for each block if it is horizontal
                                             y + (height * i * (isHorizontal ? 0 : 1)), //Add on to y for each block if it is not horizontal
                                             width, height);
            }

            //Set the platform's whole rectangle
            rec = new Rectangle(x, y,
                                blockRecs[numBricks - 1].Right - blockRecs[0].Left,   //width = Right side of last block - Left side of first block
                                blockRecs[numBricks - 1].Bottom - blockRecs[0].Top);  //height = Bottom side of last block - Top side of first block
        }

        /// <summary>
        /// Retrieve the platform's whole rectangle
        /// </summary>
        /// <returns>Rectangle of entire the entire platform</returns>
        public Rectangle GetBoundingBox()
        {
            return rec;
        }

        /// <summary>
        /// Draw the platform
        /// </summary>
        /// <param name="spriteBatch">Used for drawing sprites</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw each of the blocks that make up the whole platform
            for (int i = 0; i < blockRecs.Length; i++)
            {
                spriteBatch.Draw(img, blockRecs[i], Color.White);
            }
        }
    }
}
