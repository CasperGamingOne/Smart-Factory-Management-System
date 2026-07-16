# Automated PR Summary Generation Guidelines (Jules)

When asked to act like "Sourcery AI" or review a PR on this repository, you should act autonomously and follow these steps to generate a detailed summary comment:

1. **Identify the Target Branch:** 
   Determine which branch is being PR'd. Typically, it will be a feature branch (e.g., `day_4-luca`) merging into `new-tui-frontend` or `main`.
2. **Analyze Changes:** 
   Use `git diff <target-branch>..<feature-branch> --stat` and `git diff` to fully analyze all added, modified, and removed files.
3. **Review & Refactor (Minor):** 
   Review the code for immediate, obvious minor bugs or performance issues. If any are found, fix them in the branch and ensure tests/builds still pass using `dotnet build` and `dotnet test`. (Avoid major architectural changes unless explicitly requested).
4. **Generate the Summary:** 
   Draft a comprehensive PR summary formatted exactly like the following template. Categorize the changes based on what you found in the diffs.

### Formatting Template:

```markdown
## Summary by Jules

[1-2 sentences summarizing the overall goal of the pull request]

New Features:
- [Detail any new services, classes, or significant functional additions]
- ...

Bug Fixes:
- [Detail any bugs that were resolved or edge cases that were handled]
- ...

Enhancements:
- [Detail any improvements to existing code, logic, or performance optimizations]
- ...

Refactoring:
- [Detail any structural changes, decoupling, or code cleanups]
- ...
```

5. **Provide Output:** 
   Print the finalized Markdown summary directly in the chat output for the user to copy/paste into their GitHub Pull Request.
