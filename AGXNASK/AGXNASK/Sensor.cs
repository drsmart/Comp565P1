using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AGXNASK
{
    class Sensor : MovableModel3D
    {
        Object3D frontLeftSensor;
        Object3D frontRightSensor;
        Object3D rightSensor;
        Object3D leftSensor;
        private static Matrix FRONT_LEFT_TRANSLATION = Matrix.CreateTranslation(new Vector3(-100, 0, -150));
        private static Matrix FRONT_RIGHT_TRANSLATION = Matrix.CreateTranslation(new Vector3(100, 0, -150));
        private static Matrix LEFT_TRANSLATION = Matrix.CreateTranslation(new Vector3(-150, 0, -50));
        private static Matrix RIGHT_TRANSLATION = Matrix.CreateTranslation(new Vector3(150, 0, -50));

        private Boolean rightFront;
        private Boolean leftFront;
        private Boolean right;
        private Boolean left;

        private Boolean haveCollided;

        public Boolean RightWall
        {
            get { return rightFront; }
            set { rightFront = value; }
        }

        public Boolean LeftWall
        {
            get { return leftFront; }
            set { leftFront = value; }
        }

        public Boolean HaveCollided
        {
            get { return haveCollided; }
            set { haveCollided = value; }
        }

        NPAgent agent;

        public Sensor(Stage theStage, string label, string meshFile, NPAgent agent)
            : base(theStage, label, meshFile)
        {
            this.agent = agent;
            isCollidable = false;
            for (int i = 0; i < 4; i++)
                addObject(agent.AgentObject.Translation, Vector3.UnitY, 0);

            frontLeftSensor = instance.ElementAt<Object3D>(0);
            frontRightSensor = instance.ElementAt<Object3D>(1);
            rightSensor = instance.ElementAt<Object3D>(2);
            leftSensor = instance.ElementAt<Object3D>(3);

            RightWall = false;
            LeftWall = false;
            HaveCollided = false;
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
            frontLeftSensor.Orientation = FRONT_LEFT_TRANSLATION;
            frontLeftSensor.Orientation *= agent.AgentObject.Orientation;
            frontRightSensor.Orientation = FRONT_RIGHT_TRANSLATION;
            frontRightSensor.Orientation *= agent.AgentObject.Orientation;
            leftSensor.Orientation = LEFT_TRANSLATION;
            leftSensor.Orientation *= agent.AgentObject.Orientation;
            rightSensor.Orientation = RIGHT_TRANSLATION;
            rightSensor.Orientation *= agent.AgentObject.Orientation;

            CheckCollision();
            base.Update(gameTime);
        }

        public void CheckCollision()
        {
            leftFront = frontLeftSensor.collision(frontLeftSensor.Translation);
            rightFront = frontRightSensor.collision(frontRightSensor.Translation);
            left = leftSensor.collision(leftSensor.Translation);
            right = rightSensor.collision(rightSensor.Translation);
            HaveCollided = (left || right || leftFront || rightFront || HaveCollided) ? true: false;
           
            if (rightFront && leftFront)
            {
                if (left && right)
                    agent.AgentObject.Yaw = (float)Math.PI / 4;
                else if (left)
                    agent.AgentObject.Yaw = -(float)Math.PI / 50;
                else if (right)
                    agent.AgentObject.Yaw = (float)Math.PI / 50;
                else
                    agent.AgentObject.Yaw = -(float)Math.PI / 50;
            }
            else if (right && rightFront)
            {
                agent.AgentObject.Yaw = (float)Math.PI / 50;
            }
            else if (rightFront)
            {
                agent.AgentObject.Yaw = (float)Math.PI / 50;
            }
            else if (left && leftFront)
            {
                agent.AgentObject.Yaw = -(float)Math.PI / 50;
            }
            else if (leftFront)
            {
                agent.AgentObject.Yaw = -(float)Math.PI / 50;
            }
            else if (left)
            {
                agent.AgentObject.Yaw = -(float)Math.PI / 100;
            }
            else if (right)
            {
                agent.AgentObject.Yaw = (float)Math.PI / 100;
            }
            else
            {
                if (HaveCollided)
                {
                    agent.OnDetour = true;
                    agent.AgentObject.Yaw = 0;
                }
            }         
        }
    }
}
