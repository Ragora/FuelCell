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

namespace FuelCell
{
    /// <summary>
    /// A static camera is a fixed camera in the game space represented by a point, its field of view, near plane, far plane,
    /// and facing direction.
    /// </summary>
    public class StaticCamera : ANode
    {
        /// <summary>
        /// A matrix representing the position, look position and up vector.
        /// </summary>
        public Matrix View;

        /// <summary>
        /// The current yaw angle.
        /// </summary>
        public virtual float Yaw { set; get; }

        /// <summary>
        /// The current pitch angle.
        /// </summary>
        public virtual float Pitch { set; get; }

        public virtual void SetYaw(float yaw)
        {
            Direction = Vector3.Transform(Direction,
                Matrix.CreateFromAxisAngle(Vector3.Up, yaw));
        }

        public virtual void SetPitch(float pitch)
        {
            Direction = Vector3.Transform(Direction,
             Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, Direction),
            pitch));

            Up = Vector3.Transform(Vector3.Up,
                 Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, Direction),
                 pitch));
        }

        /// <summary>
        /// A matrix representing the field of view, aspect ration, near plane and far plane.
        /// </summary>
        public Matrix Projection; // FoV, aspectRatio, nearPlane, farPlane

        private Vector3 InternalUp;
        public Vector3 Up
        {
            set
            {
                View = Matrix.CreateLookAt(InternalPosition, InternalPosition + Direction, value);
                InternalUp = value;
            }
            get
            {
                return InternalUp;
            }
        }

        private Vector3 InternalTarget;
        public Vector3 Target
        {
            set
            {
                View = Matrix.CreateLookAt(InternalPosition, InternalPosition + Direction, InternalUp);
                InternalTarget = value;
            }
            get
            {
                return InternalTarget;
            }
        }

        protected Vector3 InternalPosition;
        public new Vector3 Position
        {
            set
            {
                View = Matrix.CreateLookAt(value, value + Direction, InternalUp);
                InternalPosition = value;
            }
            get
            {
                return InternalPosition;
            }
        }

        private Vector3 InternalDirection;
        public Vector3 Direction
        {
            set
            {
                InternalDirection = value;
                InternalDirection.Normalize();

                View = Matrix.CreateLookAt(InternalPosition, InternalPosition + value, InternalUp);
            }
            get
            {
                return InternalDirection;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position">Position in game world.</param>
        /// <param name="target">Position to be looking up.</param>
        /// <param name="up">Orientation.</param>
        public StaticCamera(Game game, Vector3 position, Vector3 target, Vector3 up) : base(game)
        {
            InternalPosition = position;
            InternalTarget = target;
            InternalUp = up;
            InternalDirection = target - position;
            InternalDirection.Normalize();

            View = Matrix.CreateLookAt(position, target + Direction, up);

            Projection = Matrix.CreatePerspectiveFieldOfView((float)game.Window.ClientBounds.Width / (float)game.Window.ClientBounds.Height,
                (float)(3.0f * Math.PI) / 4.0f,
                1,
                1000);
        }
    }
}
