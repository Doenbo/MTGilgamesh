using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MTG.Core.Helper;

public class LogManager
{
    public static ILoggerFactory Factory { get; set; } = NullLoggerFactory.Instance;

    public static ILogger<T> GetLogger<T>() => Factory.CreateLogger<T>();
}
