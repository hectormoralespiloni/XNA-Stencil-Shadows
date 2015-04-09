using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StencilShadows
{
    public class Camera : GameComponent
    {
        MouseState originalMouseState;

        public Matrix ProjectionMatrix { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix WorldMatrix { get; set; }

        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraForward { get; set; }
        public Vector3 CameraUpVector { get; set; }
        public Vector3 CameraRotation { get; set; }
        public Vector3 CameraWalkForward { get; set; }
        public Vector3 CameraWalkRight { get; set; }

        public Camera(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            int width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Mouse.SetPosition(width/2, height/2);
            originalMouseState = Mouse.GetState();

            CameraPosition = new Vector3(0, 0, 10);
            CameraForward = Vector3.Forward;
            CameraUpVector = Vector3.Up;
            CameraRotation = Vector3.Zero;

            //get aspect ratio from default adapter
            float aspect = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio;

            //create a PROJECTION matrix
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.0001f, 1000.0f);

            //create a VIEW matrix
            ViewMatrix = Matrix.CreateLookAt(CameraPosition, CameraForward, CameraUpVector);

            //create a WORLD matrix
            WorldMatrix = Matrix.Identity;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();
            int width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            float rotSpeed = 0.05f;

            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
            {
                //walk forward
                Vector3 translation = Vector3.Multiply(CameraWalkForward, 2);
                CameraPosition = Vector3.Add(CameraPosition, translation);
            }
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
            {
                //walk backwards
                Vector3 translation = Vector3.Multiply(CameraWalkForward, -2);
                CameraPosition = Vector3.Add(CameraPosition, translation);
            }
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
            {
                //strafe left
                Vector3 translation = Vector3.Multiply(CameraWalkRight, -2);
                CameraPosition = Vector3.Add(CameraPosition, translation);
            }
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
            {
                //strafe right
                Vector3 translation = Vector3.Multiply(CameraWalkRight, 2);
                CameraPosition = Vector3.Add(CameraPosition, translation);
            }

            //camera yaw angle
            float yawAngle = (currentMouseState.X - originalMouseState.X) * rotSpeed;
            //camera pitch angle
            float pitchAngle = (currentMouseState.Y - originalMouseState.Y) * rotSpeed;

            CameraRotation = new Vector3(CameraRotation.X - pitchAngle, CameraRotation.Y - yawAngle, 0);
            Mouse.SetPosition(width/2, height/2);

            //create a rotation matrix
            Matrix rotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(CameraRotation.X)) *
                                    Matrix.CreateRotationY(MathHelper.ToRadians(CameraRotation.Y));

            //keep track of the orientation of the camera
            CameraWalkForward = Vector3.Transform(Vector3.Forward, rotationMatrix);
            CameraWalkRight = Vector3.Transform(Vector3.Right, rotationMatrix);

            Vector3 rotatedFw = Vector3.Transform(CameraForward, rotationMatrix);
            Vector3 rotatedUp = Vector3.Transform(CameraUpVector, rotationMatrix);
            rotatedFw += CameraPosition;

            ViewMatrix = Matrix.CreateLookAt(CameraPosition, rotatedFw, rotatedUp);

            base.Update(gameTime);
        }
    }
}