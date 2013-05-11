/*  
    Copyright (C) 2012 G. Michael Barnes
 
    The file Agent.cs is part of AGXNASKv4.

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

/* Ian Graham 
 * Sam Huffman
 * Devon Smart
 * Comp 565
 * AGNXNASK 2
 * ian.graham.534@my.csun.edu
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
    /// A model that moves.  
    /// Has three Cameras:  first, follow, above.
    /// Camera agentCamera references the currently used camera {first, follow, above}
    /// Follow camera shows the MovableMesh from behind and up.
    /// Above camera looks down on avatar.
    /// The agentCamera (active camera) is updated by the avatar's Update().
    /// 
    /// 1/25/2012 last changed
    /// </summary>
    public abstract class Agent : MovableModel3D
    {
        protected Object3D agentObject = null;
        protected Camera agentCamera, first, follow, above;
        public enum CameraCase { FirstCamera, FollowCamera, AboveCamera }
        public Vector3 Position;
        protected int treasureCount;            //Count of the number of treasures an agent has tagged
        protected List<Treasure> treasures;     //List of treasures on the stage

        /// <summary>
        /// Create an Agent.
        /// All Agents are collidable and have a single instance Object3D named agentObject.
        /// Set StepSize, create first, follow and above cameras.
        /// Set first as agentCamera
        /// <param name="stage"></param>
        /// <param name="label"></param>
        /// <param name="position"></param>
        /// <param name="orientAxis"></param>
        /// <param name="radians"></param>
        /// <param name="meshFile"></param>
        /// </summary>
        public Agent(Stage stage, string label, Vector3 position, Vector3 orientAxis, float radians, string meshFile)
            : base(stage, label, meshFile)
        {
            Position = position;
            addObject(position, orientAxis, radians);
            agentObject = instance.First<Object3D>();
            first = new Camera(stage, agentObject, Camera.CameraEnum.FirstCamera);
            follow = new Camera(stage, agentObject, Camera.CameraEnum.FollowCamera);
            above = new Camera(stage, agentObject, Camera.CameraEnum.AboveCamera);
            stage.addCamera(first);
            stage.addCamera(follow);
            stage.addCamera(above);
            agentCamera = first;
            treasureCount = 0;
        }

        // Properties  

        public Object3D AgentObject
        {
            get { return agentObject; }
        }

        public Camera AvatarCamera
        {
            get { return agentCamera; }
            set { agentCamera = value; }
        }

        public Camera First
        {
            get { return first; }
        }

        public Camera Follow
        {
            get { return follow; }
        }

        public Camera Above
        {
            get { return above; }
        }

        public int TreasureCount
        {
            get { return treasureCount; }
            set { treasureCount = value; }
        }

        public List<Treasure> Treasures
        {
            set { treasures = value; }
        }

        // Methods

        public override string ToString()
        {
            return agentObject.Name;
        }

        public void updateCamera()
        {
            agentCamera.updateViewMatrix();
        }

        public override void Update(GameTime gameTime)
        {
            agentObject.updateMovableObject();
            base.Update(gameTime);
            // Agent is in correct (X,Z) position on the terrain 
            // set height to be on terrain -- this is a crude "first approximation" solution.
            stage.setSurfaceHeight(agentObject);
        }

    }
}
