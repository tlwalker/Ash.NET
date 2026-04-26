# AGENTS.md

## Developer Commands
- Build: `dotnet build Ash.NET.sln`
- Test: `dotnet test Ash.NET.sln`

## Architecture & Structure
- **Project:** C# port of the Ash entity framework.
- **Target Framework:** `net10.0`
- **Language Version:** C# 14
- **Source:** `Ash/`
- **Tests:** `AshTests/` (uses NUnit 3)
- **Solution:** Use `Ash.NET.sln` as the primary entry point for build/test.

## Tooling Quirks
- **NCrunch:** The repository includes NCrunch configuration (`.ncrunchsolution`, `.ncrunchproject`), used for continuous testing.
