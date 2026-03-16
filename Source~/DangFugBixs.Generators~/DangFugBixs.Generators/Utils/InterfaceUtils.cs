using System;
using System.Collections.Generic;
using System.Linq;

namespace NhemDangFugBixs.Generators.Utils;

internal static class InterfaceUtils {
    private static readonly HashSet<string> EntryPointInterfaces = new() {
        "IInitializable",
        "IPostInitializable",
        "IStartable",
        "IPostStartable",
        "IFixedTickable",
        "IPostFixedTickable",
        "ITickable",
        "IPostTickable",
        "ILateTickable",
        "IPostLateTickable",
        "IAsyncStartable",
        "IDisposable"
    };

    /// <summary>
    /// Checks if a type name (simple or fully qualified) represents a VContainer entry point interface.
    /// </summary>
    public static bool IsVContainerEntryPoint(string typeName) {
        if (string.IsNullOrEmpty(typeName)) return false;

        // 1. Exact match against known names (simple or qualified)
        if (EntryPointInterfaces.Contains(typeName)) return true;

        // 2. Suffix match to handle qualified names (e.g., VContainer.Unity.IInitializable)
        foreach (var entryPoint in EntryPointInterfaces) {
            if (typeName.EndsWith("." + entryPoint, StringComparison.Ordinal) || typeName == entryPoint) {
                return true;
            }
        }

        return false;
    }
}
