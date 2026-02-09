# Skill: Workflow Management

This skill provides instructions on how to maintain, debug, and expand the agentic workflows for the `NhemDangFugBixs.Tooling` project.

## Troubleshooting "Slash Command Not Found"

If a workflow (e.g., `/plan`, `/build-and-test`) does not appear in the command list:

1. **Check Frontmatter**: Ensure the first 3 lines are `---`, followed by `description: ...`, and then `---`.
2. **Add H1 Heading**: Ensure there is a `# Heading` on the line immediately following the frontmatter.
3. **Verify Directory**: The file must reside in `.agent/workflows/` and have a `.md` extension.
4. **Gitignore Issues**: If the folder is ignored in `.gitignore`, the IDE might skip indexing it. Temporarily unignore it or open the file manually to trigger a re-index.
5. **Index Reference**: Add the command to `.agent/workflows/workflow.md`.

## Creating New Workflows

When creating a new workflow:

1. Use the template:
   ```markdown
   ---
   description: [Description for the slash command]
   ---
   # [Workflow Title]
   [Steps...]
   ```
2. Annotate executable commands with `// turbo` or `// turbo-all` for automation.
