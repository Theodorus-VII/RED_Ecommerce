// Configuration for sentry.io intergration.
// Provides realtime performance and error tracking

namespace Ecommerce.Configuration;

public class SentryConfiguration
{
    public string Dsn { get; set; } = null!;
    public bool SendDefaultPii { get; set; }
    public bool IsGlobalModeEnabled { get; set; }
    public string MaxRequestBodySize { get; set; } = null!;
    public string MinimumBreadcrumbLevel { get; set; } = null!;
    public string MinimumEventLevel { get; set; } = null!;
    public bool AttachStackTrace { get; set; }
    public bool Debug { get; set; }
    public string DiagnosticLevel { get; set; } = null!;
    public bool EnableTracing { get; set; }
    public float TracesSampleRate { get; set; }
    public float ProfilesSampleRate { get; set; }

}