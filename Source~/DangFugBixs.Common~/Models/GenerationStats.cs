using System.Collections.Generic;

namespace NhemDangFugBixs.Common.Models;

internal class GenerationStats {
    public string Version { get; set; } = "v3.4.0";
    public string Timestamp { get; set; } = "";
    public int ServiceCount { get; set; }
    public List<string> Warnings { get; } = new List<string>();
}
