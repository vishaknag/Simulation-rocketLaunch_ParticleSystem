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
using Nuclex.Audio.Formats;
using Nuclex.UserInterface;
using Nuclex.Windows;
using Nuclex.Input;
using Microsoft.Xna.Framework.Design;

namespace rocketLaunch
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private ParticleEmitter particleEmitter1, particleEmitter2, particleEmitter3, particleEmitter4;//, particleEmitter5;
        Model rocketModel, launchpadModel;
        
        Matrix viewMatrix;
        Matrix projectionMatrix;

        public static Vector3 launchpadPosition = new Vector3(20, -500, -500);
        public static Vector3 rocketPosition = new Vector3(launchpadPosition.X - 100f, launchpadPosition.Y + 40f, launchpadPosition.Z - 350f);

        static Vector3 xShift = new Vector3(10, 60, -10);
        static Vector3 emissionPosition1 = new Vector3(rocketPosition.X - xShift.X, rocketPosition.Y - 135f, rocketPosition.Z);
        static Vector3 emissionPosition2 = new Vector3(rocketPosition.X - xShift.Y, rocketPosition.Y - 135f, rocketPosition.Z);
        static Vector3 emissionPosition3 = new Vector3(rocketPosition.X - xShift.Z, rocketPosition.Y - 135f, rocketPosition.Z);

        static Vector3 emissionPosition4 = new Vector3(-390, -195, -1900);

        // Camera variables
        Vector3 camPosition = new Vector3(0, 200, 1000);
        Vector3 camDirection = Vector3.Zero;
        Vector3 camUp = Vector3.Up;

        MouseState PreviousMouseState;

        // Terrain
        public Texture2D terrainHeightMap;
        public Terrain terrain;

        // Speed  
        float speed = 10;

        // User controls
        int launch = 0;
        int land = 0;
        int whichEmitter = 1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 700;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            /*
             * howMany - particles
             * heartLifeTime - how long
             * quantity - how many at a time
             * speed - how fast
             */

            particleEmitter1 = new ParticleEmitter(500, 4500, 1, 3f, 1);
            particleEmitter2 = new ParticleEmitter(500, 6500, 1, 2f, 2);
            particleEmitter3 = new ParticleEmitter(500, 6500, 1, 2f, 3);

            particleEmitter4 = new ParticleEmitter(500, 6500, 1, 2f, 4);

            Window.Title = "Rocket Launch Particle System";
            camDirection = Vector3.Zero - camPosition;
            camDirection.Normalize();
            terrain = new Terrain();

            base.Initialize();            
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            PreviousMouseState = Mouse.GetState();

            rocketModel = LoadModel("Assets/Rocket/rocket");
            launchpadModel = LoadModel("Assets/Rocket/launchpad");

            Texture2D spriteMainExhaust = Content.Load<Texture2D>("Assets/fire4thin");
            Texture2D spriteSideExhaust = Content.Load<Texture2D>("Assets/fire5thin");
            Texture2D spriteFountain = Content.Load<Texture2D>("Assets/BlueFire2");

            // Initialize terrain
            terrainHeightMap = Content.Load<Texture2D>("Assets/terrainMap");
            Effect terrainEffect = Content.Load<Effect>("effects");
            terrain.InitTerrain(terrainHeightMap, terrainEffect);

            particleEmitter1.SetupGroup(GraphicsDevice, spriteMainExhaust);
            particleEmitter2.SetupGroup(GraphicsDevice, spriteSideExhaust);
            particleEmitter3.SetupGroup(GraphicsDevice, spriteSideExhaust);


            particleEmitter4.SetupGroup(GraphicsDevice, spriteFountain);

            SetUpCamera();
        }

        private Model LoadModel(string modelName)
        {
            Model model = Content.Load<Model>(modelName);
            return model;
        }

        private void SetUpCamera()
        {
            CreateLookAt();

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 50000);
        }

        private void CreateLookAt()
        {
            viewMatrix = Matrix.CreateLookAt(camPosition, camPosition + camDirection, camUp);
        }

        private void ResetCamera()
        {
            camPosition = new Vector3(0, 200, 1000);
            camDirection = Vector3.Zero;
            camUp = Vector3.Up;

            camDirection = Vector3.Zero - camPosition;
            camDirection.Normalize();

            SetUpCamera();
        }

        private void UserInput()
        {
            // Move forward/backward  
            MouseState mState = Mouse.GetState();
            KeyboardState kstate = Keyboard.GetState();

            if (mState.ScrollWheelValue > PreviousMouseState.ScrollWheelValue &&
                camPosition.Y > 100)
            {
                camPosition += camDirection * speed * 7;
                CreateLookAt(); 
            }

            if (mState.ScrollWheelValue < PreviousMouseState.ScrollWheelValue &&
                camPosition.Y < 400)
            {
                camPosition -= camDirection * speed * 7;
                CreateLookAt(); 
            }

            //Move Forward and Back  
            if (kstate.IsKeyDown(Keys.W))
            {
                Vector3 velocity = speed * viewMatrix.Backward;
                viewMatrix *= Matrix.CreateTranslation(velocity);
            }

            if (kstate.IsKeyDown(Keys.S))
            {
                Vector3 velocity = speed * viewMatrix.Forward;
                viewMatrix *= Matrix.CreateTranslation(velocity);
            }

            // Move side to side  
            if (kstate.IsKeyDown(Keys.A))
            {
                Vector3 velocity = speed * viewMatrix.Right;
                viewMatrix *= Matrix.CreateTranslation(velocity);
            }

            if (kstate.IsKeyDown(Keys.D))
            {
                Vector3 velocity = speed * viewMatrix.Left;
                viewMatrix *= Matrix.CreateTranslation(velocity);
            }

            // Up and Down 
            if (kstate.IsKeyDown(Keys.Q))
            {
                Vector3 velocity = speed * viewMatrix.Up;
                viewMatrix *= Matrix.CreateTranslation(velocity);
            }

            if (kstate.IsKeyDown(Keys.E))
            {
                Vector3 velocity = speed * viewMatrix.Down;
                viewMatrix *= Matrix.CreateTranslation(velocity);
            }

            // Reset prevMouseState  
            PreviousMouseState = mState;
            
            // Rotation
            if (kstate.IsKeyDown(Keys.Up))
                viewMatrix *= Matrix.CreateFromAxisAngle(viewMatrix.Right, MathHelper.ToRadians(1));

            if (kstate.IsKeyDown(Keys.Down))
                viewMatrix *= Matrix.CreateFromAxisAngle(viewMatrix.Right, MathHelper.ToRadians(-1));

            if (kstate.IsKeyDown(Keys.Left))
                viewMatrix *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(1));

            if (kstate.IsKeyDown(Keys.Right))
                viewMatrix *= Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(-1));

            if (kstate.IsKeyDown(Keys.Space))
            {
                launch = 1 - launch;
                land = 0;
            }
            if (kstate.IsKeyDown(Keys.Back))
            {
                land = 1 - land;
                launch = 0;
            }

            // select the emitter for controls
            if (kstate.IsKeyDown(Keys.D1))
                whichEmitter = 1;
            if (kstate.IsKeyDown(Keys.D2))
                whichEmitter = 2;
            if (kstate.IsKeyDown(Keys.D3))
                whichEmitter = 3;
            if (kstate.IsKeyDown(Keys.D4))
                whichEmitter = 4;

            // Life Time controls
            if (kstate.IsKeyDown(Keys.PageUp))
            {
                // Increase - 1
                if(whichEmitter == 1)
                    particleEmitter1.ResetLifeTime(100, 1);
                else if (whichEmitter == 2)
                    particleEmitter2.ResetLifeTime(100, 1);
                else if (whichEmitter == 3)
                    particleEmitter3.ResetLifeTime(100, 1);
                else if (whichEmitter == 4)
                    particleEmitter4.ResetLifeTime(100, 1);
            }
            if (kstate.IsKeyDown(Keys.PageDown))
            {
                // Decrease - 2
                if (whichEmitter == 1)
                    particleEmitter1.ResetLifeTime(100, 2);
                else if (whichEmitter == 2)
                    particleEmitter2.ResetLifeTime(100, 2);
                else if (whichEmitter == 3)
                    particleEmitter3.ResetLifeTime(100, 2);
                else if (whichEmitter == 4)
                    particleEmitter4.ResetLifeTime(100, 2);
            }


            // Speed controls
            if (kstate.IsKeyDown(Keys.Tab))
            {
                if (whichEmitter == 1)
                    particleEmitter1.Speed += 1;
                else if (whichEmitter == 2)
                    particleEmitter2.Speed += 1;
                else if (whichEmitter == 3)
                    particleEmitter3.Speed += 1;
                else if (whichEmitter == 4)
                    particleEmitter4.Speed += 1;
            }

            if (kstate.IsKeyDown(Keys.CapsLock))
            {
                if (whichEmitter == 1)
                {
                    particleEmitter1.Speed -= 1;
                    if (particleEmitter1.Speed < particleEmitter1.initSpeed)
                        particleEmitter1.Speed += 1;
                }
                else if (whichEmitter == 2)
                {
                    particleEmitter2.Speed -= 1;
                    if (particleEmitter2.Speed < particleEmitter2.initSpeed)
                        particleEmitter2.Speed += 1;
                }
                else if (whichEmitter == 3)
                {
                    particleEmitter3.Speed -= 1;
                    if (particleEmitter3.Speed < particleEmitter3.initSpeed)
                        particleEmitter3.Speed += 1;
                }
                else if (whichEmitter == 4)
                {
                    particleEmitter4.Speed -= 1;
                    if (particleEmitter4.Speed < particleEmitter4.initSpeed)
                        particleEmitter4.Speed += 1;
                }
            }

            // Generate angle for emitters
            if (kstate.IsKeyDown(Keys.Home))
            {
                if (whichEmitter == 1)
                {
                    particleEmitter1.genAngleFactor += 2;
                    if(particleEmitter1.genAngleFactor > 40)
                        particleEmitter1.genAngleFactor -= 2;
                }
                else if (whichEmitter == 2)
                {
                    particleEmitter2.genAngleFactor += 2;
                    if (particleEmitter2.genAngleFactor > 40)
                        particleEmitter2.genAngleFactor -= 2;
                }
                else if (whichEmitter == 3)
                {
                    particleEmitter3.genAngleFactor += 2;
                    if (particleEmitter3.genAngleFactor > 40)
                        particleEmitter3.genAngleFactor -= 2;    
                }
                else if (whichEmitter == 4)
                {
                    particleEmitter4.genAngleFactor += 2;
                    if (particleEmitter4.genAngleFactor > 40)
                        particleEmitter4.genAngleFactor -= 2;
                }
            }

            if (kstate.IsKeyDown(Keys.End))
            {
                if (whichEmitter == 1)
                {
                    particleEmitter1.genAngleFactor -= 2;
                    if (particleEmitter1.genAngleFactor < 0)
                        particleEmitter1.genAngleFactor = 0;
                }
                else if (whichEmitter == 2)
                {
                    particleEmitter2.genAngleFactor -= 2;
                    if (particleEmitter2.genAngleFactor < 0)
                        particleEmitter2.genAngleFactor = 0;
                }
                else if (whichEmitter == 3)
                {
                    particleEmitter3.genAngleFactor -= 2;
                    if (particleEmitter3.genAngleFactor < 0)
                        particleEmitter3.genAngleFactor = 0;
                }
                else if (whichEmitter == 4)
                {
                    particleEmitter4.genAngleFactor -= 2;
                    if (particleEmitter4.genAngleFactor < 0)
                        particleEmitter4.genAngleFactor = 0;
                }
            }

            // Reset camera
            if (kstate.IsKeyDown(Keys.R))
            {
                ResetCamera();
            }
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            UserInput();

            if (launch == 1)
                Launching();
            else if (land == 1)
                Landing();

            particleEmitter1.Generate(emissionPosition1, gameTime);
            particleEmitter1.ProgressGroup(gameTime, 1);

            particleEmitter2.Generate(emissionPosition2, gameTime);
            particleEmitter2.ProgressGroup(gameTime, 1);

            particleEmitter3.Generate(emissionPosition3, gameTime);
            particleEmitter3.ProgressGroup(gameTime, 1);


            particleEmitter4.Generate(emissionPosition4, gameTime);
            particleEmitter4.ProgressGroup(gameTime, 1);

            base.Update(gameTime);
        }

        public void Launching()
        {
            rocketPosition.Y += 2;

            emissionPosition1 = new Vector3(rocketPosition.X - xShift.X, rocketPosition.Y - 135f, rocketPosition.Z);
            emissionPosition2 = new Vector3(rocketPosition.X - xShift.Y, rocketPosition.Y - 135f, rocketPosition.Z - 30f);
            emissionPosition3 = new Vector3(rocketPosition.X - xShift.Z, rocketPosition.Y - 135f, rocketPosition.Z - 30f);
        }

        public void Landing()
        {
            rocketPosition.Y -= 2;

            emissionPosition1 = new Vector3(rocketPosition.X - xShift.X, rocketPosition.Y - 135f, rocketPosition.Z);
            emissionPosition2 = new Vector3(rocketPosition.X - xShift.Y, rocketPosition.Y - 135f, rocketPosition.Z - 30f);
            emissionPosition3 = new Vector3(rocketPosition.X - xShift.Z, rocketPosition.Y - 135f, rocketPosition.Z - 30f);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // RENDER ROCKET
            Matrix rocketModelMatrix = Matrix.Identity;
            rocketModelMatrix = Matrix.CreateFromAxisAngle(rocketModelMatrix.Up, 90) * Matrix.CreateTranslation(rocketPosition);

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[rocketModel.Bones.Count];
            rocketModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in rocketModel.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * rocketModelMatrix;//Matrix.CreateTranslation(Vector3.Zero);
                    effect.View = viewMatrix;//Matrix.CreateLookAt(camPosition, Vector3.Zero, Vector3.Up);
                    effect.Projection = projectionMatrix;//Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), GraphicsDevice.Viewport.AspectRatio, 1.0f, 50000.0f);
                }

                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            // RENDER LAUNCH PAD
            Matrix launchpadModelMatrix = Matrix.Identity;
            launchpadModelMatrix = Matrix.CreateScale(3.5f) * Matrix.CreateRotationY(110f) * Matrix.CreateFromAxisAngle(launchpadModelMatrix.Up, 0) * Matrix.CreateTranslation(launchpadPosition);

            // Copy any parent transforms.
            Matrix[] transforms2 = new Matrix[launchpadModel.Bones.Count];
            launchpadModel.CopyAbsoluteBoneTransformsTo(transforms2);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in launchpadModel.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms2[mesh.ParentBone.Index] * launchpadModelMatrix;//Matrix.CreateTranslation(Vector3.Zero);
                    effect.View = viewMatrix;//Matrix.CreateLookAt(camPosition, Vector3.Zero, Vector3.Up);
                    effect.Projection = projectionMatrix;//Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), GraphicsDevice.Viewport.AspectRatio, 1.0f, 50000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            // RENDER TERRAIN
            terrain.RenderTerrain(gameTime, viewMatrix, projectionMatrix, GraphicsDevice);

            // RENDER PARTICLE SYSTEM
            particleEmitter1.RenderGroup(GraphicsDevice, viewMatrix, projectionMatrix);
            particleEmitter2.RenderGroup(GraphicsDevice, viewMatrix, projectionMatrix);
            particleEmitter3.RenderGroup(GraphicsDevice, viewMatrix, projectionMatrix);

            particleEmitter4.RenderGroup(GraphicsDevice, viewMatrix, projectionMatrix);

            base.Draw(gameTime);
        }
    }
}


