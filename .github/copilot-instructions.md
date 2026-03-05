# Copilot Instructions

## General Guidelines
- Use clear and concise language in your code comments.
- Follow best practices for code organization and structure.

## Code Style
- Adhere to consistent naming conventions throughout your project.
- Use specific formatting rules for readability.

## Project-Specific Rules
- Develop a Blazor project (Robolink.API) with a hierarchical navigation structure: Projects ? ProjectManagement (phases) ? PhaseTaskManagement (tasks).
- Implement step-by-step navigation flows to enhance user experience.
- Understand component communication and routing in Blazor to facilitate effective data sharing and navigation between components.
- Design enterprise-grade Blazor component architecture with strict separation of concerns. All components (.razor.cs files) must be split into separate files: 
  - `State.cs` (data container)
  - `Handlers.cs` (event handlers)
  - `DataLoading.cs` (API calls)
  - `Validation.cs` (form validation)
  - core `.razor.cs` (lifecycle only)
  - `.razor` (pure markup)
- Ensure each file has a single responsibility, with core files limited to ~30 lines and others between 50-150 lines. This pattern serves as a template for copy-pasting to new components.