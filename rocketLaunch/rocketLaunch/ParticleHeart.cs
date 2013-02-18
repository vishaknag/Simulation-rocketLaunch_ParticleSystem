using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace rocketLaunch
{
    class ParticleHeart
    {
        public ParticleSprite mySprite;
        public Vector3 myPlace { get; set; }
        public Vector3 mySpeed { get; set; }
        public bool heartBeating { get; set; }
        public float HeartLifeTime {get; set;}
        public float TimeSinceSpawn {get; set;}
        private Random select = new Random();
        public int myEmitterIndex;
 
        // Constructor for the Heart of the particle
        public ParticleHeart(GraphicsDevice gDevice, Texture2D spriteTexture, int heartLifeTime, int emitterIndex)
        {
            mySprite = new ParticleSprite(gDevice, spriteTexture.Width, spriteTexture.Height, spriteTexture);
            HeartLifeTime = heartLifeTime;
            myEmitterIndex = emitterIndex;
        }

        public void ProgressParticlePos(double timeElapsed)
        {
            // Add my speed to the place where the particle is to make it move in every frame

            if (myEmitterIndex == 4)
            {
                // If time left is "factor %" of the entire lifetime then add gravity
                // factor increases -> gravity is added earlier in lifetime
                // factor decreases -> gravity is added later in the lifetime
                if ((timeElapsed - TimeSinceSpawn) > (HeartLifeTime - (0.6 * HeartLifeTime)))
                {
                    if (mySpeed.Y > 0)
                        mySpeed = mySpeed * new Vector3(1.0F, -1.0F, 1.0F);
                }
            }

            myPlace += mySpeed;

            if (HeartLifeTime < (timeElapsed - TimeSinceSpawn))
                heartBeating = false;
        }

        public void RenderParticleHeart(GraphicsDevice gDevice, Matrix vMatrix, Matrix pMatrix)
        {
            mySprite.RenderSprite(Matrix.CreateTranslation(myPlace), vMatrix, pMatrix);
        }

        public float Ting
        {
            get { return mySprite.Ting; }
            set { mySprite.Ting = value; }
        }
    }
}
