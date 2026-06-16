# Xperience by Kentico: Form Clone

[![CI: Build and Test](https://github.com/BiQ-Bluesoft/xperience-community-form-clone/actions/workflows/ci.yml/badge.svg)](https://github.com/BiQ-Bluesoft/xperience-community-form-clone/actions/workflows/ci.yml)

## Description

A Xperience by Kentico admin extension that adds a **Clone form** action to the Forms list in the administration UI. With a single click, editors can duplicate any existing BizForm — including its field definitions and settings — under a new display name, without having to recreate the form from scratch.

The clone dialog pre-fills the new form's display name as `{original name} (copy)` and requires the **Create** permission. The underlying database table for the cloned form is generated automatically with a unique name.

## Requirements

### Library Version Matrix

| Xperience Version | Library Version |
| ----------------- | --------------- |
| >= 31.5.4         | 2.0.0           |
| >= 31.2.0         | 1.0.0           |

### Dependencies

- [ASP.NET Core 8.0+](https://dotnet.microsoft.com/en-us/download)
- [Xperience by Kentico >= 31.5.4](https://docs.kentico.com)

## Package Installation

Install from NuGet:

```powershell
dotnet add package XperienceCommunity.FormClone
```

## Quick Start

1. Install the NuGet package.
2. Register the services in `Program.cs`:

```csharp
builder.Services.AddXperienceCommunityFormClone();
```

3. Open the Xperience administration, navigate to **Digital Marketing > Forms**, and use the new **Clone form** action in the row actions of any form.

## Full Instructions

View the [Usage Guide](https://github.com/BiQ-Bluesoft/xperience-community-form-clone/blob/main/docs/Usage-Guide.md) for more detailed instructions.

## Contributing

Instructions and technical details for contributing to this project can be found in [Contributing Setup](https://github.com/BiQ-Bluesoft/xperience-community-form-clone/blob/main/docs/Contributing-Setup.md).

## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.
