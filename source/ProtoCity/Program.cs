using SharpRay.Core;
using static SharpRay.Core.Application;

namespace ProtoCity
{
    public static class Program
    {
        private static Zone newZone = new();

        public static void Main(string[] args)
        {
            AddEntity(newZone);

            Run(new Config { WindowWidth = 1080, WindowHeight = 720 });
        }
    }
}
