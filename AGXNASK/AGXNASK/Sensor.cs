using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AGXNASK
{
    class Sensor : MovableModel3D
    {
        Object3D leftSensor;
        Object3D rightSensor;
        private static Matrix LEFT_TRANSLATION =  Matrix.CreateTranslation(new Vector3(-100, 0, -200));
        private static Matrix RIGHT_TRANSLATION = Matrix.CreateTranslation(new Vector3(100, 0, -200));
        Agent agent;

        public Sensor(Stage theStage, string label, string meshFile, Agent agent)
            : base(theStage, label, meshFile)
        {
            this.agent = agent;
            isCollidable = false;
            addObject(agent.AgentObject.Translation, Vector3.UnitY, 0);
            addObject(agent.AgentObject.Translation, Vector3.UnitY, 0);
            leftSensor = instance.ElementAt<Object3D>(0);
            rightSensor = instance.ElementAt<Object3D>(1);
            isCollidable = true;
        }

        public void UpdatePosition(Vector3 position)
        {
            foreach (Object3D obj in instance)
            {
                obj.Translation = position;
            }
        }

        public override void Update(GameTime gameTime)
        {
            leftSensor.Orientation = LEFT_TRANSLATION;
            leftSensor.Orientation *= agent.AgentObject.Orientation;
            rightSensor.Orientation = RIGHT_TRANSLATION;
            rightSensor.Orientation *= agent.AgentObject.Orientation;
            CheckCollision();
            base.Update(gameTime);
        }

        public void CheckCollision()
        {
            if (leftSensor.collision(leftSensor.Translation))
            {
                stage.setInfo(19, "Left Collision");
            }
            if (rightSensor.collision(rightSensor.Translation))
            {
                stage.setInfo(20, "Right Collision");
            }
        }
    }
}
