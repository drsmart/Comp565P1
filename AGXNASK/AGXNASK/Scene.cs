/*  
    Copyright (C) 2012 G. Michael Barnes
 
    The file Scene.cs is part of AGXNASKv4.

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

/* Sam Huffman
 * Devon Smart
 * Comp 565
 * AGNXNASK 1
 * sam.huffman.11@my.csun.edu
 * devon.smart.962@my.csun.edu
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
    /// Scene.cs  the "main class" for AGXNASK
    /// 
    /// AGXNASK is a starter kit for Comp 565 assignments using XNA Game Studio 4.0
    /// and Visual Studio 2010.
    /// 
    /// Scene declares and initializes program specific entities and user interaction.
    /// 
    /// See AGXNASKv4-doc.pdf file for class diagram and usage information. 
    /// 
    /// 1/25/2012 last changed
    /// 
    /// </summary>
    public class Scene : Stage
    {
        public Scene() { }
        
        // Overridden Game class methods. 

        /// <summary>
        /// Set GraphicDevice display and rendering BasicEffect effect.  
        /// Create SpriteBatch, font, and font positions.
        /// Creates the traceViewport to display information and the sceneViewport
        /// to render the environment.
        /// Create and add all DrawableGameComponents and Cameras.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();  // create the Scene entities -- Inspector.

            // create a castle
            Model3D m3d = new Model3D(this, "castle", "Models/castle");
            m3d.IsCollidable = false;
            m3d.addObject(new Vector3(430 * spacing, terrain.surfaceHeight(430, 300), 300 * spacing), new Vector3(0, 1, 0), 0.0f);
            Components.Add(m3d);

            // create walls for obstacle avoidance or path finding algorithms
            Wall wall = new Wall(this, "wall", "Models/100x100x100Brick");
            Components.Add(wall);

            // create a Pack of dogs
            Pack pack = new Pack(this, "dog", "Models/dogV3");
            Components.Add(pack);
            Random random = new Random();
            for (int x = -9; x < 10; x += 6)
                for (int z = -3; z < 4; z += 6)
                {
                    float scale = (float)(0.5 + random.NextDouble());
                    float xPos = (384 + x) * spacing;
                    float zPos = (384 + z) * spacing;
                    pack.addObject(
                       new Vector3(xPos, terrain.surfaceHeight((int)xPos / spacing, (int)zPos / spacing), zPos),
                       new Vector3(0, 1, 0), 0.0f,
                       new Vector3(scale, scale, scale));
                }
            // create some clouds
            Cloud cloud = new Cloud(this, "cloud", "Models/cloudV3");
            Components.Add(cloud);
            // add 9 cloud instances
            for (int x = range / 4; x < range; x += (range / 4))
                for (int z = range / 4; z < range; z += (range / 4))
                    cloud.addObject(
                       new Vector3(x * spacing, terrain.surfaceHeight(x, z) + 7000, z * spacing),
                       new Vector3(0, 1, 0), 0.0f,
                       new Vector3(random.Next(3) + 1, random.Next(3) + 1, random.Next(3) + 1));
            // Set initial camera and projection matrix
            nextCamera();  // select the first camera
        }

        /// <summary>
        /// Uses an Inspector to display update and display information to player.
        /// All user input that affects rendering of the stage is processed either
        /// from the gamepad or keyboard.
        /// See Player.Update(...) for handling of user events that affect the player.
        /// The current camera's place is updated after all other GameComponents have 
        /// been updated.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Scene stage = new Scene()) { stage.Run(); }
        }
    }
}
