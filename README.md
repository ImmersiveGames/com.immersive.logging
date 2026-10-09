# Immersive Logging

Structured and configurable logging package for Immersive Framework modules.

## Installation

Use the package source already configured by the consuming project. Resolve the installed version from `Packages/manifest.json` and `packages-lock.json`; this README does not pin a version for discovery.

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

## Debug Logs

Use `Debug` for lifecycle details, local callback probes and verbose diagnostics that should not appear in normal smoke evidence. Keep regular smoke output at `Info`, then raise only the namespace you need to inspect.

Framework projects should configure this through `Project Settings > Immersive Framework > Logging Config` by assigning a `LoggingConfigAsset`. A typical setup is:

```txt
Default Minimum Level: Info

Namespace Rules:
- Immersive.Framework.Diagnostics = Debug
```

This keeps high-level outcomes visible:

```txt
[INFO][Immersive.Framework][FrameworkQaCanvas] QA Smoke completed. name='Standard Smoke'.
```

And shows detailed probes only when the namespace rule allows `Debug`:

```txt
[DEBUG][Immersive.Framework][RouteContentLifecycleSmokeProbe] Route Content Smoke Probe callback. phase='Entered' probe='Canonical Route Probe' route='QA Canonical Route' scene='StartupScene' object='QA_RouteContent_Canonical' localCount='1'
```

Do not create a hidden `Resources` fallback or a global singleton for this. The framework-owned configuration path is the assigned logging config in Project Settings.

## Compose logging in a Unity component

Create a `LoggingConfigAsset` from the menu above. Configure the default minimum level and enabled namespace/type rules on the asset; `LoggingConfigAsset.CreatePolicy()` builds the configured policy (and optional same-frame deduplication), while `CreateFormatter()` builds the Unity formatter. Pass the asset to `UnityLoggingFactory.CreateLogger` to apply both to the logger and its Unity console sink.

```csharp
using Immersive.Logging.Loggers;
using Immersive.Logging.Unity;
using UnityEngine;

public sealed class MatchPresenter : MonoBehaviour
{
    [SerializeField] private LoggingConfigAsset loggingConfig;

    private ScopedLogger _log;

    private void Awake()
    {
        Logger logger = UnityLoggingFactory.CreateLogger(loggingConfig);
        _log = logger.For<MatchPresenter>("Game.Match");
    }

    private void Start()
    {
        _log.Info("Match presenter ready.");
    }

    private void OnDestroy()
    {
        // Logger and ScopedLogger own no disposable resources or subscriptions.
        _log = null;
    }
}
```

The logger has no cleanup API: it retains its sink and policy but owns no disposable subscription. A null config is supported and uses the default Unity formatter and an enabled `Info` minimum-level policy. A supplied config controls enabled state, level rules, formatter options, stack-trace behavior, and optional deduplication. A missing assigned asset therefore selects documented defaults; it does not load a hidden asset. Verify by logging at `Info` and `Debug`, then adjust the asset's default level or `Game.Match` namespace rule and confirm the Console filtering/format. Duplicate rules produce Inspector warnings from `LoggingConfigAsset.OnValidate`.

## Capability navigation

| Intent | Procedure/API | Validation |
|---|---|---|
| Log from a Unity component | [Unity composition example](#compose-logging-in-a-unity-component), `UnityLoggingFactory`, `Logger`, `ScopedLogger` | Unity Console with configured level and namespace rule |
| Add structured fields | [Scoped logger example](#scoped-logger-example), `LogFields` | Inspect formatted Console record |
| Filter by level, namespace, or type | [Policy example](#policy-example), `LoggingConfigAsset` | Test allowed and filtered levels; inspect duplicate-rule warnings |
| Understand assembly ownership and exclusions | [Logging Boundary](Documentation~/Logging-Boundary.md) | Static assembly/API review |


## Boundary

This package contains generic logging primitives plus a Unity adapter. It does not provide game/framework lifecycle, global configuration, singleton bootstrap, pooling, or automatic project-wide setup. See [Logging Boundary](Documentation~/Logging-Boundary.md) for assembly boundaries and exclusions.

## License

Licensed under the [MIT License](LICENSE.md).
