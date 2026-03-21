---
name: Bug Report
about: Report a bug or unexpected behavior
title: '[BUG] '
labels: bug
assignees: ''
---

## 🐛 Bug Description

A clear and concise description of what the bug is.

## 📋 Steps to Reproduce

1. Go to '...'
2. Create a class with '...'
3. Add attribute '...'
4. See error

## ✅ Expected Behavior

What you expected to happen.

## ❌ Actual Behavior

What actually happened.

## 🔧 Environment

- **Package Version**: [e.g., 5.1.0]
- **Unity Version**: [e.g., 2021.3.0f1]
- **.NET SDK Version**: [e.g., 10.0.100]
- **IDE**: [e.g., Rider 2024.1, Visual Studio 2022]
- **OS**: [e.g., Windows 11, macOS 14]

## 📝 Code Sample

```csharp
// Minimal code that reproduces the issue
[AutoRegisterIn(typeof(GameScope))]
public class MyService {
    // ...
}
```

## 📸 Screenshots/Output

If applicable, add screenshots or error messages.

```
Error output here
```

## 🔍 Additional Context

Any other information that might be helpful:
- Does this only happen in Unity or also in standalone builds?
- Did this work in a previous version?
- Any workarounds you've found?

## 📊 Diagnostic Information

If you're seeing a diagnostic error (ND001-ND113), please include:
- **Diagnostic ID**: [e.g., ND005]
- **Full error message**
- **Which file/line triggered it**
