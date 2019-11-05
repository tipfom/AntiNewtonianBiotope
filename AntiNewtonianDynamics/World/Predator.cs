using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AntiNewtonianDynamics.World
{
    public class Predator : Body
    {
        public Predator(Vector2 position, Vector2 velocity, ParameterSet parameter) : base(position, velocity, Color.Red, parameter)
        {
        }

        public override void Move(IEnumerable<Body> bodies, float dt)
        {
            base.Move(bodies, dt);
            
            foreach (Body body in bodies)
            {
                if (body.GetType() == typeof(Prey))
                {
                    Vector2 distance = Position - body.Position;
                    Velocity += dt * PredatorPreyForce(Parameters, Velocity.Length(), -distance.Length()) * distance / distance.Length();
                }
            }
            if (Velocity.Length() > 0) Velocity -= dt * DissipativeForce(Parameters, Velocity.Length(), 0) * Velocity / Velocity.Length();
            Position += dt * Velocity;
        }
    }
}
