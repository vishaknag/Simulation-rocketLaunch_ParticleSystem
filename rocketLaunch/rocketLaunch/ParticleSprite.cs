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
    class ParticleSprite
    {
        protected Vector2 spriteCorner1 = new Vector2(0, 0);
        protected Vector2 spriteCorner2 = new Vector2(1, 0);
        protected Vector2 spriteCorner3 = new Vector2(0, 1);
        protected Vector2 spriteCorner4 = new Vector2(1, 1);

        // Array : store the 4 vertices of the sprite particle
        protected VertexBuffer spriteVertices;

        // Effect to register the sprite texture effect
        protected BasicEffect spriteEffect;

        // Constructor for the ParticleSprite
        public ParticleSprite(GraphicsDevice gDevice, int spriteBreadth, int spriteLength, Texture2D spriteTexture)
        {
            // Array: store vertex positions and the corresponding texture co-ordinates
            VertexPositionTexture[] spritePosTex = new VertexPositionTexture[4];
            spriteVertices = new VertexBuffer(gDevice, typeof(VertexPositionTexture), spritePosTex.Length, BufferUsage.None);
            float breadthCenter = spriteBreadth / 2;
            float lengthCenter = spriteLength / 2;

            // Set the vertices with the positions and their corresponding texture co-ordinates
            spritePosTex[0] = new VertexPositionTexture(new Vector3(-breadthCenter, lengthCenter, 0), spriteCorner1);
            spritePosTex[1] = new VertexPositionTexture(new Vector3(breadthCenter, lengthCenter, 0), spriteCorner2);
            spritePosTex[2] = new VertexPositionTexture(new Vector3(-breadthCenter, -lengthCenter, 0), spriteCorner3);
            spritePosTex[3] = new VertexPositionTexture(new Vector3(breadthCenter, -lengthCenter, 0), spriteCorner4);

            // Store the above vertex information in the vertex buffer
            spriteVertices.SetData(spritePosTex);

            // Set the texture effect to the sprite
            spriteEffect = new BasicEffect(gDevice)
            {
                Texture = spriteTexture,
                TextureEnabled = true
            };
        }

        public void RenderSprite(Matrix wMatrix, Matrix vMatrix, Matrix pMatrix)
        {
            // Set the vertices data to the sprite effect
            spriteEffect.GraphicsDevice.SetVertexBuffer(spriteVertices);

            spriteEffect.World = wMatrix;
            spriteEffect.View = vMatrix;
            spriteEffect.Projection = pMatrix;

            foreach(EffectPass pass in spriteEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                spriteEffect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 10);
            }
        }

        public float Ting
        {
            get { return spriteEffect.Alpha; }
            set { spriteEffect.Alpha = value; }
        }
    }
}
