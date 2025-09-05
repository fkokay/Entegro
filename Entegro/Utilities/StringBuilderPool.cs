using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Entegro.Utilities
{
    public static class StringBuilderPool
    {
        public static ObjectPool<StringBuilder> Instance { get; } = new DefaultObjectPoolProvider().CreateStringBuilderPool();
    }
}