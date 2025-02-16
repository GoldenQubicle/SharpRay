namespace SharpRay.Core
{
    /// <summary>
    /// Configuration for the SharpRay application, passed in <seealso cref="Application.Initialize(SharpRayConfig)"/>
    /// </summary>
    public record SharpRayConfig
    {
        /// <summary>
        /// Used internally for rendering. By convention all times are in milliseconds * TickMultiplier
        /// </summary>
        public static readonly double TickMultiplier = 10000d;
        /// <summary>
        /// How many times should fixed update be performed
        /// </summary>
        public static readonly double FixedUpdate = 60d; // times per second
        /// <summary>
        /// Name of the SharpRay application as shown in window bar.
        /// </summary>
        public string Name { get; init; } = Assembly.GetEntryAssembly().GetName().Name;
        /// <summary>
        /// Width of the application window.
        /// </summary>
        public int WindowWidth { get; init; }
        /// <summary>
        /// Height of the application window.
        /// </summary>
        public int WindowHeight { get; init; }
        /// <summary>
        /// When true will output event logging to the RayLib console. 
        /// </summary>
        public bool DoEventLogging { get; init; }
        /// <summary>
        /// When true will show the current Frames per Second in the upper left corner of the window. 
        /// </summary>
        public bool ShowFPS { get; init; }
        /// <summary>
        /// The background color set every frame in the rendering loop. Grey by default. 
        /// </summary>
        public Color BackGroundColor { get; init; } = Gray;
    }
}