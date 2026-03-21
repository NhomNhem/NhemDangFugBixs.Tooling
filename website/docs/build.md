# Building and Running

## Development Workflow
1. Open `Source~\NhemDangFugBixs.Tooling.sln` in your IDE
2. Build the solution to compile DLLs
3. Deployment:
   - `Directory.Build.props` manages auto-deployment to Unity
   - Set `NHEM_UNITY_PROJECT_ROOT` env variable or create `Source~\LocalBuild.props` to specify Unity project path
   - Alternatively, place a `.nhem-deploy-target` file in your Unity project root

## Usage in Unity
1. Ensure VContainer is installed
2. Import analyzer DLLs and mark as Roslyn Analyzers in Unity Inspector
3. Reference `NhemDangFugBixs.Runtime.dll` in your asmdef files
4. Generator produces `VContainerRegistration.g.cs` with registration methods
