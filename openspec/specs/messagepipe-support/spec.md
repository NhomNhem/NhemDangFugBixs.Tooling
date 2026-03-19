# Capability: messagepipe-support

## Requirement

- **Goal**: Discover MessagePipe-related registrations and emit scope-aware VContainer wiring for them.
- **Registration**:
  - Support MessagePipe broker metadata and emit broker registration in the matching scope registration method.
  - Preserve registration when a type targets multiple scopes.
  - Preserve broker metadata when services are remapped into type-safe scopes.
- **Symbol Safety**:
  - Resolve supported APIs by symbol, not by method name alone.
  - Ignore unrelated APIs that happen to share MessagePipe-like names.
- **Discovery Consistency**:
  - Attribute targets must match actual discovery support.
  - If `struct` is allowed by the attribute, the discovery pipeline must scan structs too; otherwise the attribute should not advertise `struct`.

## Verification

- **Broker Registration**:
  - Given a broker-marked type in a scope.
  - Expect generated code to emit MessagePipe registration in that scope method.
- **Multi-Scope Attribute**:
  - Given a type annotated for more than one MessagePipe target scope.
  - Expect generated registration in every declared target scope.
- **False Positive Guard**:
  - Given a non-MessagePipe API with the same method name as a supported registration method.
  - Expect no MessagePipe-specific behavior.
