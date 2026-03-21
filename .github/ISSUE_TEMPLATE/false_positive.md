---
name: Diagnostic False Positive
about: Report an incorrect diagnostic warning/error
title: '[FALSE-POSITIVE] NDXXX: '
labels: analyzer, false-positive
assignees: ''
---

## 🚨 Diagnostic Information

- **Diagnostic ID**: [e.g., ND005]
- **Diagnostic Title**: [e.g., "Registration Conflict Detected"]
- **Severity**: [Error/Warning]
- **Package Version**: [e.g., 6.0.0]

## ❌ False Positive Description

A clear description of why this diagnostic is incorrectly triggered.

## 📝 Code That Triggers False Positive

```csharp
// Your code that incorrectly triggers the diagnostic
[AutoRegisterIn(typeof(GameScope))]
public class MyService : IMyService {
    public MyService(IDependency dep) { }
}
```

## ✅ Why This Should Be Valid

Explain why this code is actually correct and should not trigger a diagnostic:
- This follows VContainer best practices
- The dependency is properly registered
- etc.

## 🔧 Environment

- **Unity Version**: [e.g., 2021.3.0f1]
- **IDE**: [e.g., Rider 2024.1]
- **OS**: [e.g., Windows 11]

## 📸 Screenshots

If applicable, show the diagnostic in your IDE.

## 🔍 Expected Behavior

What should happen instead:
- [ ] No diagnostic should be shown
- [ ] Different diagnostic should be shown
- [ ] Message should be different

## 🛠️ Workaround

If you've found a workaround, please share:
```csharp
// Workaround code
```

## 📚 Additional Context

- Does this happen in specific scenarios only?
- Any related issues or discussions?
- Have you tested with different package versions?

## 🧪 Minimal Reproduction

If possible, provide a minimal project or code sample that reproduces the issue:
- Repository link
- Or complete minimal code sample
