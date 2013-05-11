/*  
    Copyright (C) 2012 G. Michael Barnes
 
    The file Pack.cs is part of AGXNASKv4.

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
    /// Pack represents a "flock" of MovableObject3D's Object3Ds.
    /// Usually the "player" is the leader and is set in the Stage's LoadContent().
    /// With no leader, determine a "virtual leader" from the flock's members.
    /// Model3D's inherited List<Object3D> instance holds all members of the pack.
    /// 
    /// 1/25/2012 last changed
    /// </summary>
    public class Pack : MovableModel3D
    {
        Object3D leader;
        Random random;
        Stage currentStage;
        /// <summary>
        /// Construct a leaderless pack.
        /// </summary>
        /// <param name="theStage"> the scene</param>
        /// <param name="label"> name of pack</param>
        /// <param name="meshFile"> model of pack instance</param>
        public Pack(Stage theStage, string label, string meshFile)
            : base(theStage, label, meshFile)
        {
            isCollidable = true;
            leader = null;
            random = new Random();
        }

        /// <summary>
        /// Construct a pack with an Object3D leader
        /// </summary>
        /// <param name="theStage"> the scene </param>
        /// <param name="label"> name of pack</param>
        /// <param name="meshFile"> model of a pack instance</param>
        /// <param name="aLeader"> Object3D alignment and pack center </param>
        public Pack(Stage theStage, string label, string meshFile, Object3D aLeader)
            : base(theStage, label, meshFile)
        {
            isCollidable = true;
            leader = aLeader;
            random = new Random();
            currentStage = theStage;
        }

        /// <summary>
        /// Each pack member's orientation matrix will be updated.
        /// Distribution has pack of dogs moving randomly.  
        /// Supports leaderless and leader based "flocking" 
        /// </summary>      
        public override void Update(GameTime gameTime)
        {
            // if (leader == null) need to determine "virtual leader from members"
            float angle = 0.3f;

            foreach (Object3D obj in instance)
            {
                obj.Yaw = 0.0f;
                // change direction 4 time a second  0.07 = 4/60

                float distance = Vector3.Distance(
                new Vector3(obj.Translation.X, 0, obj.Translation.Z),
                new Vector3(leader.Translation.X, 0, leader.Translation.Z));
                if (random.NextDouble() < 0.07)
                {
                    if (random.NextDouble() > currentStage.FlockingOdds)
                    {

                        if (random.NextDouble() < 0.5) obj.Yaw -= angle; // turn left
                        else obj.Yaw += angle; // turn right

                    }
                    else if (distance >= 1000)
                    {
                        Vector3 axis, toTarget, toObj, target = leader.Translation;
                        double radian, aCosDot;
                        // put both vector on the XZ plane of Y == 0
                        toObj = new Vector3(obj.Translation.X, 0, obj.Translation.Z);
                        target = new Vector3(target.X, 0, target.Z);
                        toTarget = toObj - target; // new
                        // normalize
                        toObj.Normalize();
                        toTarget.Normalize();
                        // make sure vectors are not co-linear by a little nudge in X and Z
                        if (toTarget == toObj || Vector3.Negate(toTarget) == toObj)
                        {
                            toTarget.X += 0.05f;
                            toTarget.Z += 0.05f;
                            toTarget.Normalize();
                        }
                        // determine axis for rotation
                        axis = Vector3.Cross(toTarget, obj.Orientation.Backward);   // order of arguments mater
                        axis.Normalize();
                        // get cosine of rotation
                        aCosDot = Math.Acos(Vector3.Dot(toTarget, obj.Orientation.Backward));  //Backward
                        // test and adjust direction of rotation into radians
                        if (aCosDot == 0) radian = Math.PI * 2;
                        else if (aCosDot == Math.PI) radian = Math.PI;
                        else if (axis.X + axis.Y + axis.Z >= 0) radian = (float)(2 * Math.PI - aCosDot);
                        else radian = -aCosDot;
                        if (radian < 0 && radian < (-1 * angle))
                            obj.Yaw += angle;
                        else if (radian > 0 && radian > angle)
                            obj.Yaw -= angle;
                        else obj.turnToFace(leader.Translation);
                    }
                    else if (distance < 1000)
                    {
                        GiveMeSpace(obj);
                    }
                }
                obj.updateMovableObject();
                stage.setSurfaceHeight(obj);
            }
            base.Update(gameTime);  // MovableMesh's Update();
        }

        /// <summary>
        /// This is for making sure that if the dogs get too close they turn away
        /// and ultimately try to orient themselves in the direction of the player
        /// </summary>

        public void GiveMeSpace(Object3D obj)
        {
            float distance = Vector3.Distance(
                new Vector3(obj.Translation.X, 0, obj.Translation.Z),
                new Vector3(leader.Translation.X, 0, leader.Translation.Z));
            if (distance < 1000 && distance > 500)
            {
                float angle = Vector3.Dot(obj.Orientation.Left, leader.Orientation.Forward);
                if (angle < 0)
                {
                    obj.Yaw += 0.3f;
                }
                else if (angle > 0)
                {
                    obj.Yaw -= 0.3f;
                }
            }
            else if (distance < 500)
            {
                Vector3 axis, toTarget, toObj, target = leader.Translation;
                double radian, aCosDot;
                // put both vector on the XZ plane of Y == 0
                toObj = new Vector3(obj.Translation.X, 0, obj.Translation.Z);
                target = new Vector3(target.X, 0, target.Z);
                toTarget = toObj - target; // new
                // normalize
                toObj.Normalize();
                toTarget.Normalize();
                // make sure vectors are not co-linear by a little nudge in X and Z
                if (toTarget == toObj || Vector3.Negate(toTarget) == toObj)
                {
                    toTarget.X += 0.05f;
                    toTarget.Z += 0.05f;
                    toTarget.Normalize();
                }
                // determine axis for rotation
                axis = Vector3.Cross(toTarget, obj.Orientation.Backward);   // order of arguments mater
                axis.Normalize();
                // get cosine of rotation
                aCosDot = Math.Acos(Vector3.Dot(toTarget, obj.Orientation.Backward));  //Backward
                // test and adjust direction of rotation into radians
                if (aCosDot == 0) radian = Math.PI * 2;
                else if (aCosDot == Math.PI) radian = Math.PI;
                else if (axis.X + axis.Y + axis.Z >= 0) radian = (float)(2 * Math.PI - aCosDot);
                else radian = -aCosDot;
                if (radian < 0)
                    obj.Yaw -= 0.3f;
                else
                    obj.Yaw += 0.3f;
            }
        }

        public Object3D Leader
        {
            get { return leader; }
            set { leader = value; }
        }

    }
}
