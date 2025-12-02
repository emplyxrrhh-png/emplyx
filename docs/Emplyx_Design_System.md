# Emplyx Design System & Documentation Guidelines

This document outlines the design standards and structure for the Emplyx project documentation. The goal is to maintain a consistent, professional, and easy-to-navigate documentation set, inspired by DevOps best practices and the visual structure of Azure DevOps project overviews.

## 1. Documentation Structure

The documentation is organized hierarchically to mimic a standard DevOps project structure, ensuring information is easy to find and logically grouped.

### 1.1 Folder Hierarchy
All documentation resides in the `docs/` directory.

- **`docs/`**: Root documentation folder.
  - **`Architecture/`**: High-level architectural decisions, system diagrams, and data models.
  - **`Guides/`**: User manuals, developer onboarding guides, and "How-to" articles (e.g., `Emplyx_Guia.md`).
  - **`ReleaseNotes/`**: Changelogs, version history, and migration guides (e.g., `Migracion Emplyx 4.0.md`).
  - **`Inventory/`**: Asset lists, resource inventories, and environment details (e.g., `Inventario 11.1 Emplyx.md`).
  - **`DevOps/`**: CI/CD pipelines, infrastructure as code (IaC), and deployment strategies.
  - **`Design/`**: UI/UX guidelines, design systems, and style guides (this document belongs here).

## 2. Page Layout Standard

Each markdown file should follow this standard layout to ensure consistency across the project.

### 2.1 Header
Every page must start with a Level 1 Header (`#`) followed by a brief summary block.

```markdown
# Page Title

> **Summary**: A brief one-sentence description of what this page covers.
> **Author**: [Optional]
> **Last Updated**: [Date]
```

### 2.2 Sections
Use Level 2 (`##`) and Level 3 (`###`) headers to organize content. Avoid going deeper than Level 4 (`####`) to maintain readability.

### 2.3 Table of Contents
For long documents, include a Table of Contents (TOC) after the summary to aid navigation.

## 3. Visual Components

To achieve a clean "DevOps-like" look, use the following components effectively.

### 3.1 Status Badges & Alerts
Use blockquotes to highlight important information, mimicking the "Info" and "Warning" banners in Azure DevOps.

> **ℹ️ Note**: Useful information that users should know but isn't critical.

> **⚠️ Warning**: Critical information that could cause issues if ignored.

> **✅ Tip**: Best practices or shortcuts.

### 3.2 Code Blocks
Always specify the language for syntax highlighting. This is crucial for technical documentation.

```csharp
// Example C# Code
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
}
```

### 3.3 Tables
Use tables for structured data, inventories, or property lists. Ensure columns are aligned.

| Feature | Status | Owner | Description |
| :--- | :---: | :--- | :--- |
| Authentication | ✅ Ready | Team A | Identity server integration |
| Reporting | 🚧 In Progress | Team B | Monthly PDF generation |

### 3.4 Checklists
Use task lists to track progress or define requirements.

- [x] Requirement gathered
- [ ] Design approved
- [ ] Implementation complete

## 4. Writing Style

- **Clarity**: Be concise and direct. Avoid fluff.
- **Active Voice**: Use "The system sends an email" instead of "An email is sent by the system".
- **Consistency**: Use the same terminology across all documents (e.g., always use "Emplyx" not "the app").
- **Links**: Use relative links to other documents in the `docs/` folder to ensure they work in source control and local previews.

## 5. Maintenance

- Update the `docs/` folder whenever a new feature is released.
- Review this Design System document quarterly to ensure it remains relevant.
- Ensure all new documents are linked from a central index or the main `README.md`.
