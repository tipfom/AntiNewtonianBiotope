using System;

namespace AntiNewtonianDynamics
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SimulationGame()) game.Run();
        }
    }
}
