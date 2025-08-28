using Microsoft.Extensions.Hosting;
using VidHub.Platform.Generic.Interfaces;

namespace VidHub.Platform.Generic
{
    public class Context
    {
        public static IWindowContext MainWindow { get => window; set => window = value; }
        private static IWindowContext window = null;

        public static IHost MainHost { get => host; set => host = value; }
        private static IHost host = null;
    }
}
