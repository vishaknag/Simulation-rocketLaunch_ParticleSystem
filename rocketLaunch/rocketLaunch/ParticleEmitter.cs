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
    class ParticleEmitter : Game1
    {
        private ParticleHeart[] MyGroup;
        private Queue<ParticleHeart> DeadMates;
        private Random select = new Random();
        public int HeartLifeTime;
        public int Quantity;
        public float Speed;
        public float initSpeed;
        public int myIndex;
        public int genAngleFactor = 0;

        public ParticleEmitter(int howMany, int heartLifeTime, int quantity, float speed, int index)
        {
            HeartLifeTime = heartLifeTime;
            Quantity = quantity;
            Speed = speed;
            initSpeed = speed;
            myIndex = index;
            genAngleFactor = 0;

            // Queue to add new particles and delete the expired ones
            DeadMates = new Queue<ParticleHeart>(howMany);

            // Array of the entire particles in the simulation
            MyGroup = new ParticleHeart[howMany];
        }

        public void SetupGroup(GraphicsDevice gDevice, Texture2D spriteTextures)
        {
            // Instantiate the particles and add them to the queue because they are still not born
            for (int particleI = 0; particleI < MyGroup.Length; particleI++)
            {
                MyGroup[particleI] = new ParticleHeart(gDevice, spriteTextures, HeartLifeTime, myIndex);
                DeadMates.Enqueue(MyGroup[particleI]);
            }
        }


        /* heartLifeTime : New amount to be increased or decreased
         * IncDec : Flag to indicate Increase/Decrease
         */
        public void ResetLifeTime(int heartLifeTime, int IncDec)
        {
            if(IncDec == 1)
                this.HeartLifeTime += heartLifeTime;
            else if (IncDec == 2)
                this.HeartLifeTime -= heartLifeTime;
                
            foreach (ParticleHeart particle in MyGroup)
            {
                if (IncDec == 1)
                    particle.HeartLifeTime += heartLifeTime;
                else if (IncDec == 2)
                    particle.HeartLifeTime -= heartLifeTime;
            }
        }


        public void Generate(Vector3 spawnPosition, GameTime gameTime)
        {
            float TimeSinceSpawn = (float)gameTime.TotalGameTime.TotalMilliseconds;

            // Give life only Quantity number of particles from the dead particles queue
            for (int particleI = 0; particleI < Quantity && DeadMates.Count > 0; particleI++)
            {
                // Take a new dead particle in the queue and give life to it
                ParticleHeart particle = DeadMates.Dequeue();
                particle.heartBeating = true;
                particle.myPlace = spawnPosition;
                particle.TimeSinceSpawn = TimeSinceSpawn;

                Vector3 speed = new Vector3(Speed * (float)select.NextDouble(), 0, 0);

                if (myIndex == 1)   // If center main exhaust
                {
                    speed = Vector3.Transform(speed, Matrix.CreateRotationZ(MathHelper.ToRadians(select.Next(225 - genAngleFactor, 315 + genAngleFactor))));
                }
                else if (myIndex == 2)     // If left exhaust
                {
                    speed = Vector3.Transform(speed, Matrix.CreateRotationZ(MathHelper.ToRadians(select.Next(180 - genAngleFactor, 225 + genAngleFactor))));
                }
                else if (myIndex == 3)     // If right exhaust
                {
                    speed = Vector3.Transform(speed, Matrix.CreateRotationZ(MathHelper.ToRadians(select.Next(315 - genAngleFactor, 360 + genAngleFactor))));
                }
                else if (myIndex == 4)     // If water fountain
                {
                    speed = Vector3.Transform(speed, Matrix.CreateRotationZ(MathHelper.ToRadians(select.Next(80 - genAngleFactor, 100 + genAngleFactor))));
                }

                if (myIndex == 4)
                {
                    particle.mySpeed = speed;
                }
                else
                {
                    if ((particle.myPlace + speed).Y < 3.0F)
                    {
                        speed = new Vector3(Speed * (float)select.NextDouble(), 0, 0);

                        if (myIndex == 1)   // If center main exhaust
                        {
                            particle.mySpeed = 2 * Vector3.Transform(speed, Matrix.CreateRotationY(MathHelper.ToRadians(select.Next(360))));
                        }
                        else if (myIndex == 2)     // If left exhaust
                        {
                            particle.mySpeed = 2 * Vector3.Transform(speed, Matrix.CreateRotationY(MathHelper.ToRadians(select.Next(135, 225))));
                        }
                        else if (myIndex == 3)     // If right exhaust
                        {
                            particle.mySpeed = 2 * Vector3.Transform(speed, Matrix.CreateRotationY(MathHelper.ToRadians(select.Next(315, 405))));
                        }
                    }
                    else
                    {
                        particle.mySpeed = speed;
                    }
                }
            }
        }

        public void ProgressGroup(GameTime gameTime, int style)
        {
            foreach (ParticleHeart particle in MyGroup)
            {
                if (particle.heartBeating)
                {
                    // Compute what amount of time is spent since last position progression
                    float fractionLifeTime = (float)(gameTime.TotalGameTime.TotalMilliseconds - particle.TimeSinceSpawn) / particle.HeartLifeTime;

                    if (style == 1)
                    {
                        // Linearly interpolate the particle time spent between 0 and 1
                        particle.Ting = MathHelper.Lerp(1, 0, fractionLifeTime);
                    }

                    if (style == 2)
                    {
                        // Add a new style in future
                    }

                    // Add the speed of the particle to its current position to get the new position
                    particle.ProgressParticlePos(gameTime.TotalGameTime.TotalMilliseconds);

                    // After updating, if the particle heart is found to be
                    // stopped then add it to the dead particle queue. 
                    // These dead particles are again spawned later
                    if (!particle.heartBeating)
                        DeadMates.Enqueue(particle);
                }
            }
        }

        public void RenderGroup(GraphicsDevice gDevice, Matrix vMatrix, Matrix pMatrix)
        {
            // Graphics configuration settings
            gDevice.BlendState = BlendState.Additive;
            gDevice.DepthStencilState = DepthStencilState.None;
           
            // Render every livin particle (heart beating) in the current state
            foreach(ParticleHeart heart in MyGroup)
            {
                if (heart.heartBeating)
                    heart.RenderParticleHeart(gDevice, vMatrix, pMatrix);
            }

            // Graphics settings resetting
            gDevice.BlendState = BlendState.Opaque;
            gDevice.DepthStencilState = DepthStencilState.Default;
        }
    }
}
