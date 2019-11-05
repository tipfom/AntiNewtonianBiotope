using AntiNewtonianDynamics.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace AntiNewtonianDynamics
{
    public class SimulationGame : Game
    {
        private float scalingFactor = 100f;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D circleTexture;

        private bool leftClickHandled, rightClickHandled;

        private Vector2 offset = Vector2.Zero;

        private List<Body> bodies = new List<Body>();

        private int lockIndex = -1;
        private float overheadDt = 0;
        public SimulationGame() : base()
        {
            graphics = new GraphicsDeviceManager(this) { SynchronizeWithVerticalRetrace = true };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            using (FileStream stream = File.OpenRead("./Content/circle.png"))
                circleTexture = Texture2D.FromStream(graphics.GraphicsDevice, stream);
        }


        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            CheckKeyboard(dt);
            CheckMouse();

            if (lockIndex > -1 && lockIndex < bodies.Count) offset = bodies[lockIndex].Position - new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height) / scalingFactor / 2f;

            overheadDt += 8 * dt;
            while ((overheadDt -= 0.001f) > 0)
                foreach (Body body in bodies) body.Move(bodies, 0.001f);

            base.Update(gameTime);
        }

        private void CheckKeyboard(float dt)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (lockIndex > -1)
            {
                if (keyboardState.IsKeyDown(Keys.W)) offset.Y -= 2f * dt;
                if (keyboardState.IsKeyDown(Keys.S)) offset.Y += 2f * dt;
                if (keyboardState.IsKeyDown(Keys.A)) offset.X -= 2f * dt;
                if (keyboardState.IsKeyDown(Keys.D)) offset.X += 2f * dt;
            }

            if (keyboardState.IsKeyDown(Keys.D0)) lockIndex = -1;

            if (keyboardState.IsKeyDown(Keys.D1)) lockIndex = 0;
            if (keyboardState.IsKeyDown(Keys.D2)) lockIndex = 1;
            if (keyboardState.IsKeyDown(Keys.D3)) lockIndex = 2;
            if (keyboardState.IsKeyDown(Keys.D4)) lockIndex = 3;
            if (keyboardState.IsKeyDown(Keys.D5)) lockIndex = 4;
            if (keyboardState.IsKeyDown(Keys.D6)) lockIndex = 5;
            if (keyboardState.IsKeyDown(Keys.D7)) lockIndex = 6;
            if (keyboardState.IsKeyDown(Keys.D8)) lockIndex = 7;
            if (keyboardState.IsKeyDown(Keys.D9)) lockIndex = 8;

            if (keyboardState.IsKeyDown(Keys.Y) && bodies.Count == 0)
            {
                // quasiperiodic trajectory no dissipation
                bodies.Add(new Prey(new Vector2(1, 0), new Vector2(0, 1), new Body.ParameterSet(2, 0)));
                bodies.Add(new Predator(new Vector2(2, 0), new Vector2(0, 2), new Body.ParameterSet(1, 0)));
            }
            if (keyboardState.IsKeyDown(Keys.X) && bodies.Count == 0)
            {
                // quasiperiodic trajectory with dissipation
                bodies.Add(new Prey(new Vector2(1, 1), new Vector2(-0.1f, 0.9f), new Body.ParameterSet(1, 1)));
                bodies.Add(new Predator(new Vector2(9, 3), new Vector2(0, 0.4f), new Body.ParameterSet(2, 0.1f)));
            }
            if (keyboardState.IsKeyDown(Keys.C) && bodies.Count == 0)
            {
                // quasiperiodic trajectory with dissipation
                bodies.Add(new Prey(new Vector2(0.3f, 24), new Vector2(0f, 0f), new Body.ParameterSet(0.5f, 2)));
                bodies.Add(new Predator(new Vector2(-0.2f, 47f), new Vector2(0, 0f), new Body.ParameterSet(1, 1f)));
            }
            if (keyboardState.IsKeyDown(Keys.V) && bodies.Count == 0)
            {
                // quasiperiodic trajectory with dissipation
                bodies.Add(new Prey(new Vector2(0.5f, 1), new Vector2(0f, 0f), new Body.ParameterSet(1f, 3)));
                bodies.Add(new Predator(new Vector2(1f, 0.1f), new Vector2(-0.4f, -0.4f), new Body.ParameterSet(2, 1f)));
                bodies.Add(new Predator(new Vector2(0f, 2f), new Vector2(0.4f, 0f), new Body.ParameterSet(2, 1f)));
            }

            if (keyboardState.IsKeyDown(Keys.Q)) bodies.Clear();
        }

        private void CheckMouse()
        {
            MouseState mouseState = Mouse.GetState();

            scalingFactor = 100f * (float)Math.Pow(0.95f, mouseState.ScrollWheelValue / 100f);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!leftClickHandled) bodies.Add(new Predator(new Vector2(mouseState.Position.X / scalingFactor + offset.X, mouseState.Position.Y / scalingFactor + offset.Y), Vector2.Zero, new Body.ParameterSet(2, 1)));
                leftClickHandled = true;
            }
            else leftClickHandled = false;

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (!rightClickHandled) bodies.Add(new Prey(new Vector2(mouseState.Position.X / scalingFactor + offset.X, mouseState.Position.Y / scalingFactor + offset.Y), Vector2.Zero, new Body.ParameterSet(2, 1)));
                rightClickHandled = true;
            }
            else rightClickHandled = false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGreen);

            spriteBatch.Begin();

            foreach (Body body in bodies)
            {
                foreach (Vector2 trace in body.Trace)
                {
                    spriteBatch.Draw(circleTexture, new Rectangle(((trace - offset) * scalingFactor - new Vector2(5, 5)).ToPoint(), new Point(10, 10)), new Rectangle(0, 0, 80, 80), body.TraceColor);
                }
            }
            foreach (Body body in bodies)
                spriteBatch.Draw(circleTexture, new Rectangle(((body.Position - offset) * scalingFactor - new Vector2(20, 20)).ToPoint(), new Point(40, 40)), new Rectangle(0, 0, 80, 80), body.BodyColor);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
