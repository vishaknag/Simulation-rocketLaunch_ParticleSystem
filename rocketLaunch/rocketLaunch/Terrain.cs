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
    public class Terrain
    {
        // Terrain variables
        VertexPositionColor[] verts;
        int[] indices;

        private int width = 4;
        private int height = 3;
        private float[,] terrainHeightData;

        public Vector3 terrainPosition = new Vector3(-1350, -900, 400);

        Effect TerrainEffect;

        public void InitTerrain(Texture2D terrainHeightMap, Effect terrainEffect)
        {
            TerrainEffect = terrainEffect;
            SetHeightData(terrainHeightMap);
            SetVerts();
            SetIndices();
        }

        public void SetHeightData(Texture2D terrainHeightMap)
        {
            width = terrainHeightMap.Width;
            height = terrainHeightMap.Height;

            Color[] heightMapColors = new Color[width * height];
            terrainHeightMap.GetData(heightMapColors);

            terrainHeightData = new float[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    terrainHeightData[x, y] = heightMapColors[x + y * width].R / 5.0f;
        }

        public void SetVerts()
        {
            float minHeight = float.MaxValue;
            float maxHeight = float.MinValue;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (terrainHeightData[x, y] < minHeight)
                        minHeight = terrainHeightData[x, y];
                    if (terrainHeightData[x, y] > maxHeight)
                        maxHeight = terrainHeightData[x, y];
                }
            }

            verts = new VertexPositionColor[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    verts[x + y * width].Position = new Vector3(x, terrainHeightData[x, y], -y);

                    if (terrainHeightData[x, y] < minHeight + (maxHeight - minHeight) / 4)
                        verts[x + y * width].Color = Color.Cyan;
                    else if (terrainHeightData[x, y] < minHeight + (maxHeight - minHeight) * 2 / 4)
                        verts[x + y * width].Color = Color.ForestGreen;
                    else if (terrainHeightData[x, y] < minHeight + (maxHeight - minHeight) * 3 / 4)
                        verts[x + y * width].Color = Color.Honeydew;
                    else
                        verts[x + y * width].Color = Color.White;
                }
            }
        }

        public void SetIndices()
        {
            indices = new int[(width - 1) * (height - 1) * 6];
            int counter = 0;
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    int lowerLeft = x + y * width;
                    int lowerRight = (x + 1) + y * width;
                    int topLeft = x + (y + 1) * width;
                    int topRight = (x + 1) + (y + 1) * width;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
        }

        public void RenderTerrain(GameTime gameTime, Matrix vMatrix, Matrix pMatrix, GraphicsDevice gDevice)
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            gDevice.RasterizerState = rs;

            Matrix wMatrix = Matrix.CreateScale(20f) * Matrix.CreateTranslation(terrainPosition);

            TerrainEffect.CurrentTechnique = TerrainEffect.Techniques["ColoredNoShading"];
            TerrainEffect.Parameters["xView"].SetValue(vMatrix);
            TerrainEffect.Parameters["xProjection"].SetValue(pMatrix);
            TerrainEffect.Parameters["xWorld"].SetValue(wMatrix);

            foreach (EffectPass pass in TerrainEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                gDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verts, 0, verts.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
            }
        }
    }
}
