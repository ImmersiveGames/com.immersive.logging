# Logging Boundary

`com.immersive.logging` is the generic logging package used by Immersive Framework modules.

## Current Contract

The package provides:

- pure runtime logging records, fields, formatters, policies, sinks and local loggers;
- single-line readable console output for smoke evidence;
- configurable policy by default level, namespace prefix, owner type and optional owner attributes;
- optional Unity adapter with console sink, formatter, `LoggingConfigAsset` and same-frame dedupe;
- no mandatory singleton or hidden global configuration.

## Runtime Assembly Rule

`Immersive.Logging.Runtime` has `noEngineReferences: true` and must not reference `UnityEngine`.

Allowed runtime concepts:

- `LogLevel`
- `LogLevelAttribute`
- `LogRecord`
- `LogField`
- `Logger`
- `ScopedLogger`
- `ILogSink`
- `ILogFormatter`
- `ILogPolicy`
- generic policies and formatters

## Unity Adapter Rule

Unity-specific behavior stays under `Runtime/Unity` in `Immersive.Logging.Unity`.

Allowed Unity concerns:

- `UnityConsoleLogSink`
- `UnityConsoleLogFormatter`
- `LoggingConfigAsset`
- `UnityFrameDedupeLogPolicy`
- Unity console rich text
- Unity frame-based dedupe

## Formatting Rule

Human-readable console logs should keep the complete evidence on the first line:

```txt
[INFO][Immersive.Framework][FrameworkBootstrap] Boot succeeded. app='Game Application' route='Startup Route' scene='StartupScene'.
```

Timestamps are optional and should not be forced into the default Unity Console view.

## Policy Precedence

When using `ConfigurableLogPolicy`, the resolution order is:

1. type rule;
2. namespace/category longest-prefix rule;
3. `LogLevelAttribute` on owner type, when enabled;
4. default minimum level.

## Explicitly Out of Scope

- old `DebugUtility` monolith;
- `HardFailFastH1`;
- framework bootstrap;
- service locator;
- mandatory singleton;
- hidden global config;
- module-specific tags for Session, Route, Activity, Actor, Input, Camera, Save or Pooling;
- degraded mode policy;
- framework-specific diagnostics.
