using System;
using System.IO;
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
    /// An OrbitCamera is a camera that orbits about a given target node where its
    /// yaw and pitch parameters pivot the camera about a sphere defined by a given 
    /// radius instead of pivoting the camera itself.
    /// </summary>
    public class OrbitCamera : StaticCamera
    {
        /// <summary>
        /// The target node to observe.
        /// </summary>
        public ANode OrbitTarget;

        /// <summary>
        /// The internally stored orbit distance.
        /// </summary>
        private float InternalOrbitDistance;

        /// <summary>
        /// The orbit distance is the distance at which the camera will orbit its target.
        /// </summary>
        public float OrbitDistance
        {
            get
            {
                return InternalOrbitDistance;
            }
            set
            {
                InternalOrbitDistance = value;
                InternalOrbitDistance = MathHelper.Clamp(InternalOrbitDistance, MinimumOrbitDistance, MaximumOrbitDistance);
            }
        }

        private float InternalOrbitYaw;
        public override float Yaw
        {
            set
            {
                InternalOrbitYaw = value;
                InternalOrbitYaw %= MathHelper.TwoPi;
            }
            get
            {
                return InternalOrbitYaw;
            }
        }

        private float InternalOrbitPitch;
        public override float Pitch
        {
            set
            {
                InternalOrbitPitch = value;
                InternalOrbitPitch %= MathHelper.TwoPi;

                InternalOrbitPitch = InternalOrbitPitch < (float)Math.PI / 12 ? (float)Math.PI / 12 : InternalOrbitPitch;
                InternalOrbitPitch = InternalOrbitPitch >= MathHelper.PiOver2 ? MathHelper.PiOver2 - 0.01f : InternalOrbitPitch;
            }
            get
            {
                return InternalOrbitPitch;
            }
        }

        private float InternalMinimumOrbitDistance;
        public float MinimumOrbitDistance
        {
            get
            {
                return InternalMinimumOrbitDistance;
            }
            set
            {
                InternalMinimumOrbitDistance = value;

                InternalMinimumOrbitDistance = MathHelper.Clamp(InternalMinimumOrbitDistance, 1, InternalMaximumOrbitDistance);
            }
        }

        private float InternalMaximumOrbitDistance;
        public float MaximumOrbitDistance
        {
            get
            {
                return InternalMaximumOrbitDistance;
            }
            set
            {
                InternalMaximumOrbitDistance = value;

                InternalMaximumOrbitDistance = MathHelper.Clamp(InternalMaximumOrbitDistance, InternalMinimumOrbitDistance, 500);
            }
        }

        public OrbitCamera(Game game, ANode observe) : base(game, observe.Position, observe.Position, Vector3.Up)
        {
            OrbitTarget = observe;

            Pitch = (float)Math.PI / 4;

            MaximumOrbitDistance = 100;
            MinimumOrbitDistance = 20;
            OrbitDistance = 60.0f;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            // Calculate the Yaw Pos
            Vector3 direction = new Vector3((float)Math.Cos(Yaw), 0, (float)Math.Sin(Yaw));

            direction = Vector3.Transform(direction,
                 Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, direction),
                -Pitch));

            InternalPosition = OrbitTarget.Position + (direction * OrbitDistance);
 
            View = Matrix.CreateLookAt(InternalPosition, OrbitTarget.Position, Vector3.Up);
        }

    }
}
