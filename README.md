# Immersive Logging

Structured and configurable logging package for Immersive Framework modules.

## Current Shape

`Immersive.Logging.Runtime` stays pure and does not reference `UnityEngine`.

Runtime API:

- `LogLevel`
- `LogLevelAttribute`
- `LogField` / `LogFields`
- `LogRecord`
- `Logger`
- `ScopedLogger`
- `ILogSink`
- `ILogFormatter`
- `ILogPolicy`
- `HumanReadableLogFormatter`
- `PlainTextLogFormatter`
- `AllowAllLogPolicy`
- `RejectAllLogPolicy`
- `MinimumLogLevelPolicy`
- `CompositeLogPolicy`
- `ConfigurableLogPolicy`
- `NamespaceLogRule`
- `TypeLogRule`

Unity adapter API:

- `UnityConsoleLogSink`
- `UnityConsoleLogFormatter`
- `LoggingConfigAsset`
- `UnityFrameDedupeLogPolicy`
- `UnityLoggingFactory`

## Console Format

The default human-readable formatter is single-line first. This keeps Unity Console and smoke logs readable:

```txt
[INFO][Immersive.Framework][FrameworkBootstrap] Boot succeeded. app='Game Application' route='Startup Route' scene='StartupScene' validation='Standard'.
```

Exceptions keep a compact summary on the first line and can include full details on the following lines.

## Scoped Logger Example

```csharp
using Immersive.Logging.Loggers;
using Immersive.Logging.Records;

public sealed class FrameworkBootstrap
{
    private readonly ScopedLogger _log;

    public FrameworkBootstrap(Logger logger)
    {
        _log = logger.For<FrameworkBootstrap>("Immersive.Framework");
    }

    public void Boot(string app, string route, string scene)
    {
        _log.Info(
            "Boot succeeded.",
            LogFields.Field("app", app),
            LogFields.Field("route", route),
            LogFields.Field("scene", scene));
    }
}
```

## Policy Example

```csharp
var policy = new ConfigurableLogPolicy(
    enabled: true,
    defaultMinimumLevel: LogLevel.Info,
    namespaceRules: new[]
    {
        new NamespaceLogRule("framework_debug", "Immersive.Framework", LogLevel.Debug),
        new NamespaceLogRule("pooling_warning", "Immersive.Pooling", LogLevel.Warning)
    });
```

Rules use longest-prefix matching. Type rules have higher precedence than namespace rules. `LogLevelAttribute` is honored when enabled and no explicit type/namespace rule matches.

## Unity Configuration

Create an optional asset through:

`Assets > Create > Immersive > Logging > Logging Config`

The asset can configure:

- global enabled;
- default minimum level;
- namespace rules;
- type rules;
- rich text;
- timestamp visibility;
- exception detail visibility;
- suppression of stack traces for regular `Log` entries;
- optional suppression of stack traces for warnings;
- same-frame dedupe and repeated-call warnings.

The package does not create a singleton, service locator, hidden bootstrap or mandatory project-wide config.

Framework projects can assign this asset through `Project Settings > Immersive Framework > Logging Config`. If no asset is assigned, the Unity logger uses `Info` as the minimum level and suppresses stack traces for regular `Log` entries.

## Boundary

This package contains only generic logging concerns. It must not include `Session`, `Route`, `Activity`, `Actor`, `Input`, `Camera`, `Save`, `Pooling`, framework bootstrap, degraded mode, hard-fail policy, or module-specific tags.
