# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.2] - 2026-02-25

### Added

- **`[AutoRegisterScene]` attribute**: Automated registration of Scene-based MonoBehaviours.
- **Improved discovery**: Source Generator now intelligently finds and registers components existing in the Scene hierarchy.

### Fixed

- **Ambiguous reference (CS0104)**: Resolved conflict between `UnityEngine.Object` and `System.Object` in generated code.
- **Non-Unity build support**: Added preprocessor guards to allow generator and sandbox projects to compile outside of Unity environment.

## [1.0.1] - 2026-02-09

### Added

- **GitHub Issue Templates**: Structured forms for Bug Reports and Feature Requests.
- **AI Agent Workflows**: Standardized slash commands for `/release`, `/build-and-test`, and `/design-plan`.

### Changed

- **Infrastructure Consolidation**: Merged technical laws and AI rules into a single `.agent` directory.
- **Git Metadata Support**: Fixed `.gitignore` to correctly track `.meta` files for UPM distribution.

## [1.0.0] - 2026-02-09

### Added

- **VContainer Auto-Registration**: Source Generator for automatic dependency injection mapping.
- **Optimized Stat System**: High-performance character stat calculation (HP, Attack, etc.) with Zero-GC logic.
- **Automatic Interface Mapping**: Intelligent detection and registration of primary interfaces.
- **Infrastructure**: Isolated source code in `Source~` to prevent Unity domain reload issues.
- **Professional Packaging**: Standard UPM structure for seamless Git import.
- **Roslyn Analyzers**: Real-time validation rules for VContainer best practices.
- **Documentation**: Comprehensive README, Release Guide, and Technical Laws.

### Support

Developed by NhomNhem
