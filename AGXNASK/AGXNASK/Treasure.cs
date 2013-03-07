using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AGXNASK
{
    class Treasure : Model3D
    {
        private NavNode position;
        private Boolean captured;

        public Boolean Captured
        {
            get { return captured; }
            set { captured = value; }
        }

        public Vector3 Position
        {
            get { return position.Translation; }
            set { position.Translation = value; }
        }
        private Model closed;
        private Model open;
        private const string opendModel = "treasure_chest";

        public Treasure(Stage stage, string name, string file)
            : base(stage, name, file)
        {
            open = stage.Content.Load<Model>(opendModel);
            closed = model;
            captured = false;
            position = new NavNode(Vector3.Zero, NavNode.NavNodeEnum.VERTEX);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState k = Keyboard.GetState();

            if (k.IsKeyDown(Keys.Z))
            {
                captured = !captured;
            }
            if (captured)
                model = open;
            base.Update(gameTime);
        }
    }
}
