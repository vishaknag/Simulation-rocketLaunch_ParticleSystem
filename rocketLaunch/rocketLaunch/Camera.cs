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
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera matrices  
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        // Camera vectors  
        public Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraUp;

        // Speed  
        float speed = 10;

        // Mouse stuff  
        MouseState prevMouseState;  

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up) : base(game)  
        {  
            // Build camera view matrix  
            cameraPosition = pos;  
            cameraDirection = target - pos;  
            cameraDirection.Normalize();  
            cameraUp = up;  
            CreateLookAt();  
  
            projection = Matrix.CreatePerspectiveFieldOfView(  
                MathHelper.PiOver4,  
                (float)Game.Window.ClientBounds.Width /  
                (float)Game.Window.ClientBounds.Height,  
                1, 3000);  
        }

        public override void Initialize()
        {
            // Set mouse position and do initial get state  
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState(); 

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Move forward/backward  
            MouseState mState = Mouse.GetState();
            KeyboardState kstate = Keyboard.GetState();

            if (mState.ScrollWheelValue > prevMouseState.ScrollWheelValue &&
                cameraPosition.Y > 100)
            {
                cameraPosition += cameraDirection * speed * 7;
            }
            if (mState.ScrollWheelValue < prevMouseState.ScrollWheelValue &&
                cameraPosition.Y < 400)
            {
                cameraPosition -= cameraDirection * speed * 7;
            }

            //Move Forward and Back  
            if (kstate.IsKeyDown(Keys.W))
                cameraPosition += new Vector3(speed * cameraDirection.X, 0, speed * cameraDirection.Z);
            if (kstate.IsKeyDown(Keys.S))
                cameraPosition += new Vector3(-(speed * cameraDirection.X), 0, -(speed * cameraDirection.Z));
            // Move side to side  
            if (kstate.IsKeyDown(Keys.A))
                cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
            if (kstate.IsKeyDown(Keys.D))
                cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed;
            //Rotate  
            if (kstate.IsKeyDown(Keys.Q))
                cameraDirection = Vector3.TransformNormal(cameraDirection, Matrix.CreateRotationY(MathHelper.Pi / 60));
            if (kstate.IsKeyDown(Keys.E))
                cameraDirection = Vector3.TransformNormal(cameraDirection, Matrix.CreateRotationY(-MathHelper.Pi / 60));

            // Reset prevMouseState  
            prevMouseState = mState;

            // Recreate the camera view matrix  
            CreateLookAt();  

            base.Update(gameTime);
        }  

        public void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition,
                cameraPosition + cameraDirection, cameraUp);
        } 

    }
}
