#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using System.Text;
using NhemDangFugBixs.Attributes;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace NhemDangFugBixs.Editor
{
    public static class NhemToolingDiagnostics
    {
        [MenuItem("Tools/Nhem/Tooling Diagnostics/Print Diagnostics")]
        public static void PrintDiagnostics()
        {
            var report = BuildReport();
            Debug.Log(report);
        }

        private static string BuildReport()
        {
            var sb = new StringBuilder();

            sb.AppendLine("NhemDangFugBixs Tooling Diagnostics");
            sb.AppendLine();

            // Package info
            var packageInfo = GetPackageInfo();
            sb.AppendLine("Package:");
            sb.AppendLine($"- package.json version: {packageInfo.Version ?? "unknown"}");
            sb.AppendLine($"- package path: {packageInfo.ResolvedPath ?? "unknown"}");
            sb.AppendLine();

            // Assembly info
            var attributesAssembly = typeof(AutoRegisterInAttribute).Assembly;
            var attributesVersion = attributesAssembly.GetName().Version?.ToString() ?? "unknown";

            sb.AppendLine("Assemblies:");
            sb.AppendLine($"- NhemDangFugBixs.Attributes: {attributesVersion}");
            
            // Runtime assembly if available
            var runtimeAssembly = GetRuntimeAssembly();
            if (runtimeAssembly != null)
            {
                var runtimeVersion = runtimeAssembly.GetName().Version?.ToString() ?? "unknown";
                sb.AppendLine($"- NhemDangFugBixs.Runtime: {runtimeVersion}");
            }
            sb.AppendLine();

            // Attribute checks
            sb.AppendLine("Attributes:");
            AppendTypeCheck(sb, attributesAssembly, "NhemDangFugBixs.Attributes.AsAttribute");
            AppendTypeCheck(sb, attributesAssembly, "NhemDangFugBixs.Attributes.EntryPointAttribute");
            AppendTypeCheck(sb, attributesAssembly, "NhemDangFugBixs.Attributes.RegisterComponentInHierarchyAttribute");
            sb.AppendLine();

            // Analyzer DLL checks
            sb.AppendLine("Analyzer:");
            var packagePath = packageInfo.ResolvedPath;
            if (packagePath != null)
            {
                var generatorsPath = Path.Combine(packagePath, "Analyzers", "NhemDangFugBixs.Generators.dll");
                var analyzersPath = Path.Combine(packagePath, "Analyzers", "NhemDangFugBixs.Analyzers.dll");
                sb.AppendLine($"- NhemDangFugBixs.Generators.dll: {(File.Exists(generatorsPath) ? "found" : "missing")}");
                sb.AppendLine($"- NhemDangFugBixs.Analyzers.dll: {(File.Exists(analyzersPath) ? "found" : "missing")}");
            }
            else
            {
                sb.AppendLine("- Unable to check analyzer DLLs (package path unknown)");
            }
            sb.AppendLine();

            // VContainer dependency
            sb.AppendLine("VContainer:");
            var vcontainerVersion = GetVContainerVersion(packagePath);
            if (vcontainerVersion != null)
            {
                sb.AppendLine($"- package dependency declared: jp.hadashikick.vcontainer {vcontainerVersion}");
            }
            else
            {
                sb.AppendLine("- Unable to determine VContainer dependency version");
            }
            sb.AppendLine();

            // Status summary
            var status = DetermineStatus(packageInfo.Version, attributesVersion);
            sb.AppendLine($"Status: {status}");

            if (status == "WARNING")
            {
                sb.AppendLine();
                sb.AppendLine("WARNING:");
                sb.AppendLine($"Package version is {packageInfo.Version} but loaded Attributes assembly is {attributesVersion}.");
                sb.AppendLine("Likely stale Unity PackageCache or packages-lock.json.");
                sb.AppendLine("Suggested fix:");
                sb.AppendLine("1. Close Unity");
                sb.AppendLine("2. Delete Library/PackageCache/com.nhemdangfugbixs.tooling*");
                sb.AppendLine("3. Check Packages/packages-lock.json");
                sb.AppendLine("4. Reopen Unity");
                sb.AppendLine("5. Regenerate project files");
            }

            return sb.ToString();
        }

        private static void AppendTypeCheck(StringBuilder sb, Assembly assembly, string typeName)
        {
            var found = assembly.GetType(typeName, throwOnError: false) != null;
            sb.AppendLine($"- {typeName}: {(found ? "found" : "missing")}");
        }

        private static (string Version, string ResolvedPath) GetPackageInfo()
        {
            try
            {
                var packageInfo = PackageInfo.FindForAssembly(typeof(AutoRegisterInAttribute).Assembly);
                if (packageInfo != null)
                {
                    return (packageInfo.version, packageInfo.resolvedPath);
                }
            }
            catch
            {
                // Fall back to manual package.json reading
            }

            // Fallback: read package.json directly
            var packagePath = Path.Combine(Application.dataPath, "..", "Packages", "com.nhemdangfugbixs.tooling", "package.json");
            if (File.Exists(packagePath))
            {
                var content = File.ReadAllText(packagePath);
                var versionMatch = System.Text.RegularExpressions.Regex.Match(content, @"""version""\s*:\s*""([^""]+)""");
                if (versionMatch.Success)
                {
                    return (versionMatch.Groups[1].Value, Path.GetDirectoryName(packagePath));
                }
            }

            return (null, null);
        }

        private static Assembly GetRuntimeAssembly()
        {
            try
            {
                return Assembly.Load("NhemDangFugBixs.Runtime");
            }
            catch
            {
                return null;
            }
        }

        private static string GetVContainerVersion(string packagePath)
        {
            if (packagePath == null) return null;
            
            try
            {
                var packageJsonPath = Path.Combine(packagePath, "package.json");
                if (File.Exists(packageJsonPath))
                {
                    var content = File.ReadAllText(packageJsonPath);
                    var match = System.Text.RegularExpressions.Regex.Match(content, @"""jp\.hadashikick\.vcontainer""\s*:\s*""([^""]+)""");
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }
            }
            catch
            {
                // Ignore errors
            }

            return null;
        }

        private static string DetermineStatus(string packageVersion, string assemblyVersion)
        {
            if (packageVersion == null || assemblyVersion == null)
            {
                return "WARNING";
            }

            // Simple version comparison - check if major version matches
            var packageMajor = packageVersion.Split('.')[0];
            var assemblyMajor = assemblyVersion.Split('.')[0];

            if (packageMajor != assemblyMajor)
            {
                return "WARNING";
            }

            return "PASS";
        }
    }
}
#endif
