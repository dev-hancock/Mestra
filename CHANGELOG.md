# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## \[1.1.0] - 2025-06-08

### Added

* `Mestra.Abstractions` package introduced:
    * Added `IPipelineFactory` and `IMessageHandlerFactory` abstractions
* `Mestra.Extensions.Microsoft.DependencyInjection` package introduced:
    * Moved `AddMestra()` registration extension to dedicated DI extension package
    * Moved `MestraBuilder` as fluent registration API
    * Added `PipelineFactory` and `MessageHandlerFactory` implementations
* `Mestra.FluentValidation` package introduced
* Improved `PublishDispatcher` to use `IMessageHandlerFactory` instead of direct `IServiceProvider`
* Improved `Mediator` to use `IPipelineFactory` instead of direct `IServiceProvider`

### Changed

* Simplified `Mestra` core package structure
* Improved naming and structure of builder and factory classes
* Improved unit test structure across solution
* Aligned test naming conventions

---

## \[1.0.1] - 2025-06-07

### Added

* xdoc summaries on interfaces and classes

---

## \[1.0.0] - 2025-06-07

### Added

* Initial release of Mestra

