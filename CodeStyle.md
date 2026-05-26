# C# Code Style Guidelines

This document outlines the standard coding style guidelines for the Beskar.Memory project.

## Code Clarity & Design
* **Precise & Self-Explaining**: Code should be expressive, readable, and clean. Variable, method, and class names should clearly declare their intent.
* **Sharp XML Comments**: When documenting APIs, keep the XML comments sharp, concise, and focused. Avoid verbose descriptions.

## Indentation & Spacing
* **3-Space Indentation**: Always use exactly **3 spaces** for indentation. Do not use tabs or 4 spaces.
* **Logical Blocks inside Methods**: Group code lines into logical blocks (such as Arrange/Act, Assert, and Cleanup) separated by a single blank line.

## C# Language Features
* **Implicit Typing (`var`)**: Always use `var` instead of explicit types whenever possible for local variable declarations.
* **Collection Expressions**: Prefer C# collection expressions (`[1, 2, 3]`) over array creation expressions (`new int[] { 1, 2, 3 }` or `new[] { 1, 2, 3 }`).
  * *Incorrect*: `var source = new int[] { 1, 2, 3 };`
  * *Correct*: `int[] source = [1, 2, 3];`

## Testing Guidelines
* **Test Naming without Underscores**: Test method names must **not** contain underscores (`_`). Use CamelCase/PascalCase to group parts of a test name.
  * *Incorrect*: `Empty_ReturnsDefaultState`
  * *Correct*: `EmptyReturnsDefaultState`
* **Zero Comments in Tests**: Test files must not contain any inline, block, or XML comments. The test code must speak for itself.
