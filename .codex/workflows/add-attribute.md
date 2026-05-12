# Codex Workflow — Add Public Attribute

Use this when adding an attribute to Runtime/Attributes.

## Steps

1. Confirm the attribute is necessary.
2. Keep constructor/property API minimal.
3. Avoid dependency on VContainer if possible.
4. Add generator support.
5. Add analyzer support if misuse is possible.
6. Add tests.
7. Add docs.
8. Add sample if user-facing.

## Checklist

- [ ] Attribute works across asmdef boundaries.
- [ ] Attribute does not force one architecture.
- [ ] Attribute can be discovered by generator.
- [ ] Attribute has clear generated intent.
- [ ] Misuse produces helpful diagnostics.
