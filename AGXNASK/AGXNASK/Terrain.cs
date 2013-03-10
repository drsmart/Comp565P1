/*  
    Copyright (C) 2012 G. Michael Barnes
 
    The file Terrain.cs is part of AGXNASKv4.

    AGXNASKv4 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AGXNASK
{

    /// <summary>
    /// Terrain represents a ground.
    /// The vertices have position and color.  Terrain width = height.  
    /// Reads two textures to set terrain height and color values.
    /// You might want to pre-compute and store heights of surfaces to be 
    /// returned by the surfaceHeight(x, z) method.
    /// 
    /// 1/25/2012 last changed
    /// </summary>
    /// 
    public class Terrain : IndexVertexBuffers
    {
        protected VertexPositionColor[] vertex;  // stage vertices    
        private int height, width, multiplier = 20, spacing;
        private int[,] terrainHeight;
        private BasicEffect effect;
        private GraphicsDevice display;
        private Texture2D heightTexture, colorTexture;
        private Microsoft.Xna.Framework.Color[] heightMap, colorMap;


        public Terrain(Stage theStage, string label, string heightFile, string colorFile) : base(theStage, label)
        {
            int i;
            range = stage.Range;
            width = height = range;
            nVertices = width * height;
            terrainHeight = new int[width, height];
            vertex = new VertexPositionColor[nVertices];
            nIndices = (width - 1) * (height - 1) * 6;
            indices = new int[nIndices];  // there are 6 indices 2 faces / 4 vertices 
            spacing = stage.Spacing;
            // set display information 
            display = stage.Display;
            effect = stage.SceneEffect;
            heightTexture = stage.Content.Load<Texture2D>("Textures/" + heightFile);
            heightMap = new Microsoft.Xna.Framework.Color[width * height];
            heightTexture.GetData<Microsoft.Xna.Framework.Color>(heightMap);
            // create colorMap values from colorTexture
            colorTexture = stage.Content.Load<Texture2D>("Textures/" + colorFile);
            colorMap = new Microsoft.Xna.Framework.Color[width * height];
            colorTexture.GetData<Microsoft.Xna.Framework.Color>(colorMap);
            // create  vertices for terrain
            Vector4 vector4;
            int vertexHeight;
            i = 0;
            for (int z = 0; z < height; z++)
                for (int x = 0; x < width; x++)
                {
                    vector4 = heightMap[i].ToVector4();       // convert packed Rgba32 values to floats
                    vertexHeight = (int)(vector4.X * 255);   // scale vertexHeight 0..255
                    vertexHeight *= multiplier;               // multiply height
                    terrainHeight[x, z] = vertexHeight;       // save height for navigation
                    vertex[i] = new VertexPositionColor( new Vector3(x * spacing, vertexHeight, z * spacing), new Color(colorMap[i].ToVector4()));
                    i++;
                }
            // free up unneeded maps
            colorMap = null;
            heightMap = null;
            // set indices clockwise from point of view
            i = 0;
            for (int z = 0; z < height - 1; z++)
                for (int x = 0; x < width - 1; x++)
                {
                    indices[i++] = z * width + x;
                    indices[i++] = z * width + x + 1;
                    indices[i++] = (z + 1) * width + x;
                    indices[i++] = (z + 1) * width + x;
                    indices[i++] = z * width + x + 1;
                    indices[i++] = (z + 1) * width + x + 1;
                }

            // create VertexBuffer and store on GPU
            vb = new VertexBuffer(display, typeof(VertexPositionColor), vertex.Length, BufferUsage.WriteOnly);
            vb.SetData<VertexPositionColor>(vertex); // , 0, vertex.Length);
            // create IndexBuffer and store on GPU
            ib = new IndexBuffer(display, typeof(int), indices.Length, BufferUsage.WriteOnly);
            IB.SetData<int>(indices);
        }

        // Properties

        public int Spacing
        {
            get { return stage.Spacing; }
        }

        // Methods

        ///<summary>
        /// Height of  surface containing position (x,z) terrain coordinates.
        /// This method is a "stub" for the correct get code.
        /// How would you determine a surface's height at (x,?,z) ? 
        /// </summary>
        /// <param name="x"> left -- right terrain position </param>
        /// <param name="z"> forward -- backward terrain position</param>
        /// <returns> vertical height of surface containing position (x,z)</returns>
        public float surfaceHeight(float x, float z)
        {
            int xPos = (int)(x / spacing);
            int zPos = (int)(z / spacing);
            if (xPos < 0 || xPos > 511 || zPos < 0 || zPos > 511) 
                return 0.0f;  // index valid ?
            float height = 0.0f;

            int xPlusOne = xPos + 1;
            int zPlusOne = zPos + 1;

            float triZ0 = (this.terrainHeight[xPos, zPos]);
            float triZ1 = (this.terrainHeight[xPlusOne, zPos]);
            float triZ2 = (this.terrainHeight[xPos, zPlusOne]);
            float triZ3 = (this.terrainHeight[xPlusOne, zPlusOne]);

            float sqX = (x / spacing) - xPos;
            float sqZ = (z / spacing) - zPos;

            if ((sqX + sqZ) < 1)
            {
                height = triZ0;
                height += (triZ1 - triZ0) * sqX;
                height += (triZ2 - triZ0) * sqZ;
            }
            else
            {
                height = triZ3;
                height += (triZ1 - triZ3) * (1.0f - sqZ);
                height += (triZ2 - triZ3) * (1.0f - sqX);
            }

            return height;
        }

        public override void Draw(GameTime gameTime)
        {
            effect.VertexColorEnabled = true;
            if (stage.Fog)
            {
                effect.FogColor = Color.CornflowerBlue.ToVector3();
                effect.FogStart = stage.FogStart;
                effect.FogEnd = stage.FogEnd;
                effect.FogEnabled = true;
            }
            else effect.FogEnabled = false;
            effect.DirectionalLight0.DiffuseColor = stage.DiffuseLight;
            effect.AmbientLightColor = stage.AmbientLight;
            effect.DirectionalLight0.Direction = stage.LightDirection;
            effect.DirectionalLight0.Enabled = true;
            effect.View = stage.View;
            effect.Projection = stage.Projection;
            effect.World = Matrix.Identity;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                display.SetVertexBuffer(vb);
                display.Indices = ib;
                display.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, nVertices, 0, nIndices / 3);
            }
        }
    }
}
