using System.ComponentModel;

namespace Entegro.Engine
{
    /// <summary>
    /// Provides access to the singleton instance of the Entegro engine.
    /// </summary>
    public class EngineContext
    {
        private static IEngine _instance;

        /// <summary>
        /// Gets the singleton Entegro engine used to access Entegro services.
        /// </summary>
        public static IEngine Current
        {
            get => _instance;
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void Replace(IEngine engine)
        {
            Interlocked.Exchange(ref _instance, engine);
        }
    }
}
