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
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AGXNASK
{
    public class Treasure : Model3D
    {
        private NavNode position;       //NavNode with the position of the treasure
        private Boolean captured;       //Whether the treasure is tagged or not
        public Boolean Captured
        {
            get { return captured; }
            set { captured = value; }
        }

        public NavNode Position
        {
            get { return position; } 
        }

        //Vector3 position of the treasure
        public Vector3 VectorPosition
        {
            get { return position.Translation; }
            set { position.Translation = value; }
        }
      
        private Model closed;       //closed treasure chest model
        private Model open;         //open treasure chest model
        private const string opendModel = "Models/treasure_chest";

        public Treasure(Stage stage, string name, string file)
            : base(stage, name, file)
        {
            open = stage.Content.Load<Model>(opendModel);
            isCollidable = false;
            closed = model;
            captured = false;
            position = new NavNode(Vector3.Zero, NavNode.NavNodeEnum.VERTEX);
        }

        public override void Update(GameTime gameTime)
        {
            
            //Used to test if the treasure chest would switch from close to open
            //KeyboardState k = Keyboard.GetState();
            //if (k.IsKeyDown(Keys.Z))
            //{
            //    captured = !captured;
            //}

            //Switch model to open chest if treasure is tagged
            if (captured)
                model = open;
            base.Update(gameTime);
        }

        public override string ToString()
        {
            return name;
        }
    }
}
