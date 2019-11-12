using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AntiNewtonianDynamics.World
{
    public class Predator : Body
    {
        public Predator(Vector2 position, Vector2 velocity, ParameterSet parameter) : base(position, velocity, Color.Red, parameter)
        { }

        public Predator(Vector2 position, Vector2 velocity, ParameterSet parameters, Force predatorPreyForce, Force preyPreyForce, Force dissipativeForce) : base(position, velocity, Color.Red, parameters, predatorPreyForce, preyPreyForce, dissipativeForce)
        { }

        public override void UpdateVelocity(IEnumerable<Body> bodies, float dt)
        {
            foreach (Body body in bodies)
            {
                if (body.GetType() == typeof(Prey))
                {
                    Vector2 distance = body.Position - Position;
                    Velocity += dt * PredatorPreyForce(Parameters, Velocity.Length(), distance.Length()) * distance / distance.Length();
                }
            }
            if (Velocity.Length() > 0) Velocity -= dt * DissipativeForce(Parameters, Velocity.Length(), 0) * Velocity / Velocity.Length();
        }
    }
}
