Fix: Duplicate attribute definitions (AutoRegister*, AutoInjectScene*)

Problem
- Attribute types such as AutoRegisterInAttribute and AutoInjectSceneAttribute are defined in both:
  - DangFugBixs.Runtime (correct location)
  - DangFugBixs.Generators (incorrect duplicate)

Cause
- Generators project contains a copy of the attribute source files, so built Generators.dll and Runtime.dll each contain the same public types. When both DLLs are distributed to Unity/IDE, the editor loads both types and shows duplicate-type suggestions.

Goal
- Make the Runtime assembly the single source-of-truth for attribute types and ensure Generators/Analyzers reference Runtime instead of re-defining attributes.

Implementation steps (apply in source repo projects)
1. Edit DangFugBixs.Generators project file (DangFugBixs.Generators.csproj):
   - Remove any <Compile Include="**\\Attributes\\*.cs" /> or individual attribute files from the Generators project.
   - Add a ProjectReference to the Runtime project:

     <ItemGroup>
       <ProjectReference Include="..\\DangFugBixs.Runtime\\DangFugBixs.Runtime.csproj" />
     </ItemGroup>

2. Do the same for DangFugBixs.Analyzers.csproj if it also contains attribute sources.

3. Search the repository for duplicate attribute files (AutoRegister*.cs, AutoInjectScene*.cs) and remove them from Generators/Analyzers source folders (keep only in Runtime).

4. Rebuild solution:
   dotnet clean
   dotnet build NhemDangFugBixs.Tooling.sln -c Release

5. Run tests and analyzer validation:
   dotnet test
   dotnet run --project DangFugBixs.Tools\\DangFugBixs.DiSmokeValidation (if applicable)

6. Bump version and publish new release (v6.0.3):
   - Update package metadata if needed
   - Tag release and attach analyzer zip

7. Verify in Unity by installing com.nhemdangfugbixs.tooling@https://github.com/NhomNhem/NhemDangFugBixs.Tooling.git#deploy and confirm duplicate suggestions are gone.

Notes
- This change is source-level and requires access to the generator/analyzer projects (not just the compiled DLLs). If you want, I can create a PR with the project file edits and remove files if the source projects live in this repo. If not, apply the patch in the canonical source repository that contains the Generators/Analyzers projects.

Tracking
- Follow-up GitHub issue created: #4
- After applying these changes, create a release v6.0.3 with rebuilt analyzer zip.
