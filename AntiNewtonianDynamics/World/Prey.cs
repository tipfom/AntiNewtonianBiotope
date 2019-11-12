using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AntiNewtonianDynamics.World
{
    public class Prey : Body
    {
        public Prey(Vector2 position, Vector2 velocity, ParameterSet parameter) : base(position, velocity, Color.Gray, parameter)
        { }

        public Prey(Vector2 position, Vector2 velocity, ParameterSet parameters, Force predatorPreyForce, Force preyPreyForce, Force dissipativeForce) : base(position, velocity, Color.Gray, parameters, predatorPreyForce, preyPreyForce, dissipativeForce)
        { }

        public override void UpdateVelocity(IEnumerable<Body> bodies, float dt)
        {
            foreach (Body body in bodies)
            {
                Vector2 distance = Position - body.Position;
                if (body.GetType() == typeof(Predator))
                {
                    // Predator Prey Interaction
                    Velocity += dt * PredatorPreyForce(Parameters, Velocity.Length(), distance.Length()) * distance / distance.Length();
                }
                else if (body != this)
                {
                    // Prey Prey Grouping
                    Velocity += -dt * PreyPreyForce(Parameters, Velocity.Length(), distance.Length()) * distance / distance.Length();
                }
            }
            if (Velocity.Length() > 0) Velocity -= dt * DissipativeForce(Parameters, Velocity.Length(), 0) * Velocity / Velocity.Length();
        }
    }
}
