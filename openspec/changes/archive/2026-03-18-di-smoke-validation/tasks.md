## 1. Validator Command

- [x] 1.1 Add a DI smoke validation command entry point to the tooling workflow.
- [x] 1.2 Define the validator contract for loading generated assemblies and collecting DI graph findings.

## 2. Reflection Validation Core

- [x] 2.1 Implement reflection-based inspection of generated injector output and registration metadata.
- [x] 2.2 Detect missing registrations, duplicate bindings, and scope mismatches in the generated graph.
- [x] 2.3 Produce a machine-readable and human-readable failure summary for validator output.

## 3. Build Integration

- [x] 3.1 Hook the validator into the build/test flow so it can run before Unity Play mode.
- [x] 3.2 Ensure the command can run in CI using build artifacts only.

## 4. Verification

- [x] 4.1 Add passing validator tests for a complete DI graph.
- [x] 4.2 Add failing validator tests for missing bindings, duplicate registrations, and invalid scope wiring.
