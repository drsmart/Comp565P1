/*  
    Copyright (C) 2012 G. Michael Barnes
 
    The file Stage.cs is part of AGXNASKv4.

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

/* Ian Graham Sam Huffman
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
    /// A non-playing character that moves.  Override the inherited Update(GameTime)
    /// to implement a movement (strategy?) algorithm.
    /// Distribution NPAgent moves along an "exploration" path that is created by
    /// method makePath().  The exploration path is traversed in a reverse path loop.
    /// Paths can also be specified in text files of Vector3 values.  
    /// In this case create the Path with a string argument for the file's address.
    /// 
    /// 2/14/2012 last changed
    /// </summary>

    public class NPAgent : Agent
    {
        private NavNode nextGoal;
        private NavNode previousGoal;
        private Path path;
        private int snapDistance = 20;
        private int turnCount = 0;
        private Boolean treasureHunting;
        private KeyboardState oldKeyboardState;
        private Treasure nearest;

        private const double TREASURE_DETECTION_RADIUS = 4000;
        private const double TAG_DISTANCE = 500;
        private const float MAX_DIST = 300;

        private Sensor sensors;
        private Boolean onDetour;
        private Boolean positionSaved;
        private Vector3 collisionPosition;
        private float detourDistance;
        public Boolean OnDetour
        {
            get { return onDetour; }
            set { onDetour = value; }
        }

        /// <summary>
        /// Create a NPC. 
        /// AGXNASK distribution has npAgent move following a Path.
        /// </summary>
        /// <param name="theStage"> the world</param>
        /// <param name="label"> name of </param>
        /// <param name="pos"> initial position </param>
        /// <param name="orientAxis"> initial rotation axis</param>
        /// <param name="radians"> initial rotation</param>
        /// <param name="meshFile"> Direct X *.x Model in Contents directory </param>

        public NPAgent(Stage theStage, string label, Vector3 pos, Vector3 orientAxis, float radians, string meshFile)
            : base(theStage, label, pos, orientAxis, radians, meshFile)
        {
            // change names for on-screen display of current camera
            first.Name = "npFirst";
            follow.Name = "npFollow";
            above.Name = "npAbove";
            IsCollidable = true;  // have NPAgent test collisions

            // path is built to work on specific terrain
            path = new Path(stage, makePath(), Path.PathType.REVERSE); // continuous search path
            stage.Components.Add(path);
            nextGoal = path.NextNode;  // get first path goal
            agentObject.turnToFace(nextGoal.Translation);  // orient towards the first path goal
            treasureHunting = false;
            positionSaved = false;
            detourDistance = 0;
            sensors = new Sensor(stage, "Sensors", "Models/sensor", this);
        }

        /// <summary>
        /// Procedurally make a path for NPAgent to traverse
        /// </summary>
        /// <returns></returns>
        private List<NavNode> makePath()
        {
            List<NavNode> aPath = new List<NavNode>();
            int spacing = stage.Spacing;
            // make a simple path, show how to set the type of the NavNode outside of construction.

            aPath.Add(new NavNode(new Vector3(316 * spacing, stage.Terrain.surfaceHeight(316, 451), 451 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));

            NavNode n;
            n = new NavNode(new Vector3(430 * spacing, stage.Terrain.surfaceHeight(430, 400), 400 * spacing));
            n.Navigatable = NavNode.NavNodeEnum.PATH;
            aPath.Add(n);

            n = new NavNode(new Vector3(430 * spacing, stage.Terrain.surfaceHeight(430, 320), 320 * spacing));
            n.Navigatable = NavNode.NavNodeEnum.VERTEX;
            aPath.Add(n);

            aPath.Add(new NavNode(new Vector3(334 * spacing, stage.Terrain.surfaceHeight(334*spacing, 369*spacing), 369 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));



            aPath.Add(new NavNode(new Vector3(390 * spacing, stage.Terrain.surfaceHeight(390, 470), 470 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));
            Random rand = new Random();

            //if (rand.Next(2) == 1)
            //    aPath.Reverse();

            return (aPath);
        }

        /// <summary>
        /// A very simple limited random walk.  Repeatedly moves skipSteps forward then
        /// randomly decides how to turn (left, right, or not to turn).  Does not move
        /// very well -- its just an example...
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            stage.setInfo(15,
               string.Format("npAvatar:  Location ({0:f0},{1:f0},{2:f0})  Looking at ({3:f2},{4:f2},{5:f2})",
                  agentObject.Translation.X, agentObject.Translation.Y, agentObject.Translation.Z,
                  agentObject.Forward.X, agentObject.Forward.Y, agentObject.Forward.Z));
            stage.setInfo(16,
               string.Format("nextGoal:  ({0:f0},{1:f0},{2:f0})", nextGoal.Translation.X, nextGoal.Translation.Y, nextGoal.Translation.Z));
            if (!treasureHunting)
                ScanForTreasures();
            // See if at or close to nextGoal, distance measured in the flat XZ plane
            float distance = Vector3.Distance(new Vector3(nextGoal.Translation.X, 0, nextGoal.Translation.Z), new Vector3(agentObject.Translation.X, 0, agentObject.Translation.Z));

            if (OnDetour)
            {
                if (positionSaved)
                {
                    detourDistance = Vector3.Distance(AgentObject.Translation, collisionPosition);

                    if (detourDistance >= MAX_DIST)
                    {
                        OnDetour = false;
                        positionSaved = false;
                        detourDistance = 0;
                        collisionPosition = Vector3.Zero;
                        agentObject.turnSlightly(nextGoal.Translation);
                    }
                }
                else
                {
                    collisionPosition = AgentObject.Translation;
                    positionSaved = true;
                    detourDistance = 0;
                }
            }

            if (keyboardState.IsKeyDown(Keys.N) && !oldKeyboardState.IsKeyDown(Keys.N))
            {
                GoToTreasure();
            }

            if (distance <= snapDistance || (distance <= TAG_DISTANCE && treasureHunting))
            {
                stage.setInfo(17, string.Format("distance to goal = {0,5:f2}", distance));
                // snap to nextGoal and orient toward the new nextGoal 
                if (treasureHunting)
                {
                    nextGoal = previousGoal;
                    treasureHunting = false;
                    if (!nearest.Captured)
                    {
                        nearest.Captured = true;
                        TreasureCount++;
                        stage.setInfo(18, "Found " + nearest.ToString());
                    }
                }
                else
                {
                    nextGoal = path.NextNode;
                }
                agentObject.turnToFace(nextGoal.Translation);
                if (path.Done)
                    stage.setInfo(18, "path traversal is done");
                else
                {
                    turnCount++;
                    stage.setInfo(18, string.Format("turnToFace count = {0}", turnCount));
                }
            }

            sensors.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            sensors.Draw(gameTime);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Gets the nearest untagged treasure from the list of treasures
        /// </summary>
        /// <param name="treasures">The list of treasure on the stage</param>
        /// <returns>The nearest untagged Treasure</returns>
        private Treasure getNearest(List<Treasure> treasures)
        {
            Treasure nearest = treasures.First<Treasure>();
            Vector3 position = new Vector3(AgentObject.Translation.X, AgentObject.Translation.Y, AgentObject.Translation.Z);
            float minDistance = Vector3.Distance(position, treasures.First<Treasure>().VectorPosition);

            foreach (Treasure t in treasures)
            {
                float distance = Vector3.Distance(position, t.VectorPosition);
                if (distance < minDistance && !t.Captured)
                {
                    minDistance = distance;
                    nearest = t;
                }
            }

            if (nearest.Captured)
                return null;
            else
                return nearest;
        }

        /// <summary>
        /// Sets the NPAgents nextgoal to the nearest treasure if one exists
        /// </summary>
        public void GoToTreasure()
        {
            nearest = getNearest(treasures);
            if (nearest != null && !treasureHunting)
            {
                treasureHunting = true;
                previousGoal = nextGoal;
                nextGoal = nearest.Position;
                AgentObject.turnToFace(nextGoal.Translation);
            }
        }

        /// <summary>
        /// Scans for treasures within the treasure detection radius
        /// </summary>
        private void ScanForTreasures()
        {
            Vector3 position = new Vector3(AgentObject.Translation.X, AgentObject.Translation.Y, AgentObject.Translation.Z);

            foreach (Treasure t in treasures)
            {
                double distance = Vector3.Distance(t.VectorPosition, position);
                if (!t.Captured && distance <= TREASURE_DETECTION_RADIUS)
                {
                    treasureHunting = true;
                    nearest = t;
                    break;
                }
            }

            if (treasureHunting && nearest != null)
            {
                previousGoal = nextGoal;
                nextGoal = nearest.Position;
                AgentObject.turnToFace(nextGoal.Translation);
            }
        }
    }
}
