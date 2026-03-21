---
name: Performance Regression
about: Report a performance issue or build time regression
title: '[PERF] '
labels: performance
assignees: ''
---

## ⚡ Performance Issue

A clear description of the performance problem.

## 📊 Performance Metrics

### Before (Previous Version)
- **Build Time**: [e.g., 5 seconds]
- **Analyzer Time**: [e.g., 1 second]
- **Package Version**: [e.g., 5.0.0]

### After (Current Version)
- **Build Time**: [e.g., 30 seconds]
- **Analyzer Time**: [e.g., 10 seconds]
- **Package Version**: [e.g., 6.0.0]

### Regression
- **Build Time Increase**: [e.g., 6x slower]
- **Analyzer Time Increase**: [e.g., 10x slower]

## 🔧 Project Information

- **Number of Services**: [e.g., ~200]
- **Number of Scopes**: [e.g., 5]
- **Number of Assemblies**: [e.g., 3]
- **Project Size**: [e.g., Small/Medium/Large]
- **Unity Version**: [e.g., 2021.3.0f1]
- **.NET SDK**: [e.g., 10.0.100]
- **OS**: [e.g., Windows 11]

## 📝 Reproduction Steps

1. Create project with X services
2. Build with command: `dotnet build`
3. Measure time with: `Measure-Command { dotnet build }`

## 📈 Profile Data

If you've profiled the build, please share:
- Which phase is slow? (Generator/Analyzer/Build)
- Any specific diagnostics causing slowdown?
- Profiler output or screenshots

## 🔍 Additional Context

- Does this happen on every build or just the first build?
- Does clean build vs incremental build make a difference?
- Any specific code patterns that trigger slowdown?

## 💾 Sample Project

If possible, provide:
- Link to a minimal reproduction repository
- Or anonymized project structure showing scale
