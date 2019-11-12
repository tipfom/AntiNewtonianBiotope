using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace AntiNewtonianDynamics.World
{
    public abstract class Body
    {
        public struct ParameterSet
        {
            public float Mass;
            public float DragCoefficient;

            public ParameterSet(float mass, float dragCoefficient)
            {
                Mass = mass;
                DragCoefficient = dragCoefficient;
            }
        }

        public delegate float Force(ParameterSet self, float velocity, float distance);

        protected Force PredatorPreyForce = InverseForceConservative;
        // protected Force PredatorPredatorForce = InverseSquareForceConservative;
        protected Force PreyPreyForce = (self, velocity, distance) => InverseForceConservative(self, velocity, distance) - InverseSquareForceConservative(self, velocity, distance);
        protected Force DissipativeForce = NewtonianFiction;

        public Vector2 Position;
        public Vector2 Velocity;
        public Color BodyColor;
        public Color TraceColor;
        public ParameterSet Parameters;
        public Queue<Vector2> Trace = new Queue<Vector2>();

        private float timeSinceLastTrace = 0;

        public Body(Vector2 position, Vector2 velocity, Color color, ParameterSet parameters) : this(position, velocity, color, parameters, InverseForceConservative, (self, v, d) => InverseForceConservative(self, v, d) - InverseSquareForceConservative(self, v, d), LinearFriction)
        { }

        public Body(Vector2 position, Vector2 velocity, Color color, ParameterSet parameters, Force predatorPreyForce, Force preyPreyForce, Force dissipativeForce)
        {
            Position = position;
            Velocity = velocity;
            BodyColor = color;
            TraceColor = new Color(color, 0.3f);
            Parameters = parameters;
            PredatorPreyForce = predatorPreyForce;
            PreyPreyForce = preyPreyForce;
            DissipativeForce = dissipativeForce;
        }

        public abstract void UpdateVelocity(IEnumerable<Body> bodies, float dt);

        public void Move(float dt)
        {
            timeSinceLastTrace += dt;
            if (timeSinceLastTrace > 0.2f)
            {
                timeSinceLastTrace -= 0.2f;
                Trace.Enqueue(Position);
                if (Trace.Count > 5000) Trace.Dequeue();
            }
            Position += dt * Velocity;
        }

        #region Examples of Forces
        public static float ZeroForce(ParameterSet self, float velocity, float distance)
        {
            return 0;
        }

        public static float GeneralGammaForceConservative(float gamma, ParameterSet self, float velocity, float distance)
        {
            return (float)Math.Pow(distance, gamma) / self.Mass;
        }

        public static float InverseSquareForceConservative(ParameterSet self, float velocity, float distance)
        {
            return GeneralGammaForceConservative(-2, self, velocity, distance);
        }

        public static float InverseForceConservative(ParameterSet self, float velocity, float distance)
        {
            return GeneralGammaForceConservative(-1, self, velocity, distance);
        }

        public static float GeneralFrictionForce(float alpha, ParameterSet self, float velocity, float distance)
        {
            return self.DragCoefficient * (float)Math.Pow(velocity, alpha) / self.Mass;
        }

        public static float NewtonianFiction(ParameterSet self, float velocity, float distance)
        {
            return GeneralFrictionForce(2, self, velocity, distance);
        }

        public static float LinearFriction(ParameterSet self, float velocity, float distance)
        {
            return GeneralFrictionForce(1, self, velocity, distance);
        }
        #endregion
    }
}
