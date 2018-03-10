﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HexGame {
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        private const float HexSize = 0.5f;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont _font;

        // Camera
        private Camera Camera { get; set; }

        private Input Input { get; set; }

        private BasicEffect BasicEffect { get; set; }

        private HexMap Map { get; set; }

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();

            Input = new Input();

            var bindings = new Dictionary<string, List<Keys>> {
                [Commands.CameraStrafeLeft] = new List<Keys>{Keys.Left},
                [Commands.CameraStrafeRight] = new List<Keys> { Keys.Right},
                [Commands.CameraForward] = new List<Keys> { Keys.Up},
                [Commands.CameraBackward] = new List<Keys> { Keys.Down},
                [Commands.CameraZoomIn] = new List<Keys> { Keys.OemPlus, Keys.Add},
                [Commands.CameraZoomOut] = new List<Keys> { Keys.OemMinus, Keys.Subtract},
                [Commands.CameraOrbitRight] = new List<Keys>{Keys.OemPeriod},
                [Commands.CameraOrbitLeft] = new List<Keys>{Keys.OemComma},
                [Commands.CameraOrbitDown] = new List<Keys>{Keys.OemQuestion},
                [Commands.CameraOrbitUp] = new List<Keys>{Keys.OemQuotes},

                [Commands.GameExit] = new List<Keys> { Keys.Escape}
            };

            Input.AddBindings(bindings);


            Camera = new Camera( Input);
            Camera.SetLens(MathHelper.ToRadians(45), GraphicsDevice.DisplayMode.AspectRatio, .01f, 1000f);
            Camera.LookAt(new Vector3(0, 10, 1), Vector3.Zero, Vector3.Up );

            BasicEffect = new BasicEffect(GraphicsDevice) {
                VertexColorEnabled = true,
            };

            Map = new HexMap(GraphicsDevice, 15, 10);

        }
        


        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("default");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            Input.Update();

            if (Input.IsPressed(Commands.GameExit)) {
                Exit();
            }
            
            Camera.Update(gameTime);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {

            BasicEffect.Projection = Camera.ProjectionMatrix;
            BasicEffect.View = Camera.ViewMatrix;
            //BasicEffect.World = Camera.WorldMatrix;

            GraphicsDevice.Clear(Color.Black);
           
            GraphicsDevice.SetVertexBuffer(Map.VertexBuffer);
            GraphicsDevice.Indices = Map.IndexBuffer;

            //GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (var pass in BasicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Map.Triangles);
            }
            DrawHexCoords();

            base.Draw(gameTime);
        }

        private void DrawHexCoords() {
            spriteBatch.Begin();
            foreach (var row in Map.Hexes) {
                foreach (var hex in row) {
                    var projected = GraphicsDevice.Viewport.Project(hex.Position, Camera.ProjectionMatrix, Camera.ViewMatrix, Camera.WorldMatrix);
                    var screen = new Vector2(projected.X, projected.Y);
                    var pos = $"{hex.MapPos.X}, {hex.MapPos.Y}";
                    var m = _font.MeasureString(pos);
                    spriteBatch.DrawString(_font, pos, screen - m / 2, Color.White);
                }
            }
            spriteBatch.End();
        }
    }
}
