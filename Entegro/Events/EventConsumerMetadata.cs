using Entegro.Engine.Modularity;

namespace Entegro.Events
{
    public class EventConsumerMetadata
    {
        public Type ContainerType { get; set; }
        public IModuleDescriptor ModuleDescriptor { get; set; }
    }
}
