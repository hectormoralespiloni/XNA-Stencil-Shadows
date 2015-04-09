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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace StencilShadows
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
		  SpriteFont spriteFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
				graphics.PreferredDepthStencilFormat = SelectStencilMode();
        }

		  private static DepthFormat SelectStencilMode()
		  {
				// Check stencil formats
				GraphicsAdapter adapter = GraphicsAdapter.DefaultAdapter;
				SurfaceFormat format = adapter.CurrentDisplayMode.Format;

				if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth24Stencil8))
					 return DepthFormat.Depth24Stencil8;
				else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth24Stencil8Single))
					 return DepthFormat.Depth24Stencil8Single;
				else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth24Stencil4))
					 return DepthFormat.Depth24Stencil4;
				else if (adapter.CheckDepthStencilMatch(DeviceType.Hardware, format, format, DepthFormat.Depth15Stencil1))
					 return DepthFormat.Depth15Stencil1;
				else
					 throw new InvalidOperationException("Could Not Find Stencil Buffer for Default Adapter");
		  }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
				//load the fonts
				spriteFont = Content.Load<SpriteFont>("Kootenay");

				//Get the camera
				Camera camera = new Camera(this);
				camera.Initialize();
				camera.CameraPosition = new Vector3(0, 150, 300);
				Components.Insert(0, camera);

				//Load the scene
				Scene scene = new Scene(this);
				scene.content = this.Content;
				scene.camera = camera;
				scene.Initialize();
				Components.Insert(1, scene);
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

				Camera camera = (Camera)this.Components[0];
				Scene scene = (Scene)this.Components[1];
				scene.camera = camera;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkOliveGreen);

            base.Draw(gameTime);
				spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
				string msg = "Shadows Demo:\n" +
					 "F1 - Starts light rotation\n" +
					 "F2 - Stops light rotation\n"+
					 "F3 - Move the light farther\n" +
					 "F4 - Move the light closer\n";
				spriteBatch.DrawString(spriteFont, msg, new Vector2(1, 1), Color.Silver);
				spriteBatch.End();
        }
    }
}
