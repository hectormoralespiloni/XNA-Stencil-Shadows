using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StencilShadows
{
	 public class Scene : DrawableGameComponent
	 {
		  public Camera camera { get; set; }
		  public ContentManager content { get; set; }

		  Model ant;
		  Model ground;
		  Plane groundPlane;
		  Matrix shadow;
		  Vector3 ambient;
		  Vector3 diffuse;
		  Vector3 emissive;
		  Vector3 lightPos;
		  float lightRot;

		  bool isLightMoving;

		  public Scene(Game game)
				: base(game)
		  {
				
		  }

		  public override void Initialize()
		  {
				//Load the models
				ant = content.Load<Model>("ant");
				ground = content.Load<Model>("ground");

				groundPlane = new Plane(Vector3.Up, 0);

				lightPos = new Vector3(-200, 100, 100);
				lightRot = 0.0f;

				isLightMoving = false;
				ambient = diffuse = emissive = -Vector3.One;

				base.Initialize();
		  }

		  public override void Update(GameTime gameTime)
		  {
				KeyboardState keyState = Keyboard.GetState();

				if (keyState.IsKeyDown(Keys.F1))
					 isLightMoving = true;
				if (keyState.IsKeyDown(Keys.F2))
					 isLightMoving = false;

				if (keyState.IsKeyDown(Keys.F3))
				{
					 if (lightPos.Y-- <= 50)
						  lightPos.Y = 50;
				}
				if (keyState.IsKeyDown(Keys.F4))
				{
					 if (lightPos.Y++ >= 200)
						  lightPos.Y = 200;
				}

				if (isLightMoving)
				{
					 lightRot += (float)gameTime.ElapsedGameTime.TotalSeconds;
					 lightPos = Vector3.Right * -200.0f * (float)Math.Cos(lightRot) +
						  Vector3.Up * lightPos.Y +
						  Vector3.Forward * 100.0f * (float)Math.Sin(lightRot);
				}
				base.Update(gameTime);
		  }

		  public override void Draw(GameTime gameTime)
		  {
				//get original depth bias of ground
				float depthBias = GraphicsDevice.RenderState.DepthBias;

				Matrix[] transforms = new Matrix[ant.Bones.Count];
				ground.CopyAbsoluteBoneTransformsTo(transforms);

				//draw the ground
				foreach (ModelMesh mesh in ground.Meshes)
				{
					 foreach (BasicEffect effect in mesh.Effects)
					 {
						  effect.World = transforms[mesh.ParentBone.Index] * camera.WorldMatrix * Matrix.CreateScale(2.0f);
						  effect.View = camera.ViewMatrix;
						  effect.Projection = camera.ProjectionMatrix;
					 }
					 mesh.Draw();
				}

				//set new depth bias to avoid z-fighting
				GraphicsDevice.RenderState.DepthBias = -0.0002f;

				//draw the ant
				ant.CopyAbsoluteBoneTransformsTo(transforms);
				foreach (ModelMesh mesh in ant.Meshes)
				{
					 foreach (BasicEffect effect in mesh.Effects)
					 {
						  //restore original colors (overwritten by the shadow)
						  if (ambient == -Vector3.One)
						  {
								ambient = effect.AmbientLightColor;
								diffuse = effect.DiffuseColor;
								emissive = effect.EmissiveColor;
						  }
						  effect.AmbientLightColor = ambient;
						  effect.DiffuseColor = diffuse;
						  effect.EmissiveColor = emissive;
						  effect.PreferPerPixelLighting = true;
						  effect.LightingEnabled = true;
						  effect.DirectionalLight0.Direction = -lightPos;
						  effect.DirectionalLight0.Enabled = true;
						  effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();
						  effect.SpecularColor = Color.White.ToVector3();
						  effect.SpecularPower = 64;
						  effect.World = transforms[mesh.ParentBone.Index] * camera.WorldMatrix;
						  effect.View = camera.ViewMatrix;
						  effect.Projection = camera.ProjectionMatrix;
					 }
					 mesh.Draw();
				}

				//Enable stencil operations
				GraphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0, 0);
				GraphicsDevice.RenderState.StencilEnable = true;
				GraphicsDevice.RenderState.ReferenceStencil = 0;
				GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
				GraphicsDevice.RenderState.StencilPass = StencilOperation.Increment;

				//Enable alpha blending
				GraphicsDevice.RenderState.AlphaBlendEnable = true;
				GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
				GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

				GraphicsDevice.RenderState.DepthBias = -0.0001f;
				//Draw the shadows without lighting
				shadow = Matrix.CreateShadow(lightPos, groundPlane);
				foreach (ModelMesh mesh in ant.Meshes)
				{
					 foreach (BasicEffect effect in mesh.Effects)
					 {
						  effect.Alpha = 0.5f;
						  effect.AmbientLightColor = Color.Black.ToVector3();
						  effect.DiffuseColor = Color.Black.ToVector3();
						  effect.EmissiveColor = Color.Black.ToVector3();
						  effect.View = camera.ViewMatrix;
						  effect.Projection = camera.ProjectionMatrix;
						  effect.World = transforms[mesh.ParentBone.Index] * shadow;
					 }
					 mesh.Draw();
				}

				GraphicsDevice.RenderState.StencilEnable = false;
				GraphicsDevice.RenderState.AlphaBlendEnable = false;
				GraphicsDevice.RenderState.DepthBias = depthBias;

				base.Draw(gameTime);
		  }
	 }
}