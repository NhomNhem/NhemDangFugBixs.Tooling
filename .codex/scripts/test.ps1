$ErrorActionPreference = "Stop"

$solution = "Source~/NhemDangFugBixs.Tooling.sln"
$filter = $args[0]

if ([string]::IsNullOrWhiteSpace($filter)) {
    dotnet test $solution -c Release
} else {
    dotnet test $solution -c Release --filter $filter
}
