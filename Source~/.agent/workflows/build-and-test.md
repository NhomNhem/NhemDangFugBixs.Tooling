---
description: Build project and run tests
---

# Build and Test

1. Restore dependencies:
// turbo
dotnet restore

2. Build the solution:
// turbo
dotnet build --no-restore

3. Run unit tests:
// turbo
dotnet test --no-build --logger "console;verbosity=normal"
