# Logging Boundary

`com.immersive.logging` is a reusable logging package with a pure runtime and an optional Unity adapter. The [README](../README.md) is the canonical consumer guide for APIs, policy authoring, composition, and validation.

## Assembly ownership

- `Immersive.Logging.Runtime` (`noEngineReferences: true`) owns records, fields, loggers, formatter/policy/sink contracts, and engine-independent implementations.
- `Immersive.Logging.Unity` owns Unity Console output, `LoggingConfigAsset`, Unity formatting, stack-trace options, frame deduplication, and `UnityLoggingFactory`.
- Editor-only validation/authoring behavior remains outside the pure runtime assembly.

## Composition and lifetime

The consumer creates a `Logger` or calls `UnityLoggingFactory.CreateLogger(config)`, then scopes it with `Logger.For<T>()` or `For(Type)`. The logger retains its sink and policy; `ScopedLogger` retains its logger and owner metadata. Neither exposes cleanup/disposal because these objects own no external subscriptions or disposable resources. Configuration is explicit; there is no global registry, singleton, mandatory asset, or hidden bootstrap.

## Policy boundary

The consumer-provided/config-authored policy decides whether a `LogRecord` reaches its sink. Namespace rules use longest-prefix matching; type rules take precedence over namespace rules; `LogLevelAttribute` is considered when enabled and no explicit matching type/namespace rule applies. Unity Console behavior belongs to the adapter, not the pure runtime.

Framework lifecycle concepts and module-specific policy/ownership are not package responsibilities. Changes to the public API require package-level compatibility and changelog review.
