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

        public Body(Vector2 position, Vector2 velocity, Color color, ParameterSet parameters)
        {
            Position = position;
            Velocity = velocity;
            BodyColor = color;
            TraceColor = new Color(color, 0.3f);
            Parameters = parameters;
        }

        public virtual void Move(IEnumerable<Body> bodies, float dt)
        {
            timeSinceLastTrace += dt;
            if (timeSinceLastTrace > 0.2f)
            {
                timeSinceLastTrace -= 0.2f;
                Trace.Enqueue(Position);
                if (Trace.Count > 500) Trace.Dequeue();
            }
        }

        #region Examples of Forces
        private static float ZeroForce(ParameterSet self, float velocity, float distance)
        {
            return 0;
        }

        private static float GeneralGammaForceConservative(float gamma, ParameterSet self, float velocity, float distance)
        {
            return (float)Math.Pow(distance, gamma) / self.Mass;
        }

        private static float InverseSquareForceConservative(ParameterSet self, float velocity, float distance)
        {
            return GeneralGammaForceConservative(-2, self, velocity, distance);
        }

        private static float InverseForceConservative(ParameterSet self, float velocity, float distance)
        {
            return GeneralGammaForceConservative(-1, self, velocity, distance);
        }

        private static float GeneralFrictionForce(float alpha, ParameterSet self, float velocity, float distance)
        {
            return self.DragCoefficient * (float)Math.Pow(velocity, alpha) / self.Mass;
        }

        private static float NewtonianFiction(ParameterSet self, float velocity, float distance)
        {
            return GeneralFrictionForce(2, self, velocity, distance);
        }
        #endregion
    }
}
