# 🔥 BlazorForge.Components

[![NuGet](https://img.shields.io/nuget/v/BlazorForge.Components.svg)](https://www.nuget.org/packages/BlazorForge.Components)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazorForge.Components.svg)](https://www.nuget.org/packages/BlazorForge.Components)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-blueviolet)](https://dotnet.microsoft.com)

A modern, lightweight Blazor component library for rapid UI development — supporting **Blazor WebAssembly** and **Blazor Server** out of the box.

---

## ✨ Features

- ⚡ Ready-to-use Razor components
- 🎨 Easily customizable via parameters and CSS variables
- 🌐 Supports Blazor WebAssembly & Blazor Server
- 📦 Multi-target: `.NET 8`, `.NET 9`, `.NET 10`
- 🔍 Full IntelliSense support with XML documentation
- 🪶 Lightweight with minimal dependencies

---

## 📦 Installation

Install via **NuGet Package Manager**:

```bash
dotnet add package SandevBlazorComponent
```

Or via the NuGet Package Manager Console in Visual Studio:

```powershell
Install-Package SandevBlazorComponent
```

---

## 🚀 Getting Started

### 1. Register the services

In your `Program.cs`:

```csharp
using SandevBlazorComponent;

// Add BlazorForge services (if any DI registration is needed)
builder.Services.AddSandevBlazor();
```

### 2. Add the namespace globally

In your `_Imports.razor`:

```razor
@using SandevBlazorComponent
@using SandevBlazorComponent.Infrastructure.EnumClass
```

In .NET8+ your head tag and body tag `App.razor`:

```razor
<link href="_content/SandevBlazorComponent/css/sandev-blazor.css" rel="stylesheet" />
<script src="_content/SandevBlazorComponent/js/sandev-blazor.js"></script>
```

### 3. Use the components

```razor
<BlazorForgeButton Variant="primary" OnClick="HandleClick">
    Click Me!
</BlazorForgeButton>

@code {
    private void HandleClick()
    {
        Console.WriteLine("Button clicked!");
    }
}
```

---

## 🧩 Available Components

| Component | Description |
|---|---|
| `<BlazorForgeButton>` | Customizable button with variants |
| `<BlazorForgeAlert>` | Alert/notification banners |
| `<BlazorForgeCard>` | Content card container |
| `<BlazorForgeModal>` | Accessible modal/dialog |
| `<BlazorForgeBadge>` | Status badge indicator |
| *(more coming soon...)* | |

---

## ⚙️ Component Parameters Example

### `<BlazorForgeButton>`

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Variant` | `string` | `"primary"` | Visual style: `primary`, `secondary`, `danger`, `ghost` |
| `Disabled` | `bool` | `false` | Disables the button |
| `OnClick` | `EventCallback` | — | Click event handler |
| `ChildContent` | `RenderFragment` | — | Button label/content |

```razor
<BlazorForgeButton Variant="danger" Disabled="false" OnClick="OnDelete">
    Delete Item
</BlazorForgeButton>
```

---

## 🎨 Customization

BlazorForge components use CSS custom properties for easy theming. Override them in your `app.css`:

```css
:root {
    --blazorforge-primary: #6366f1;
    --blazorforge-primary-hover: #4f46e5;
    --blazorforge-border-radius: 8px;
    --blazorforge-font-family: 'Inter', sans-serif;
}
```

---

## 🛠️ Requirements

| Target | Minimum Version |
|---|---|
| .NET | 8.0 / 9.0 / 10.0 |
| Blazor | WebAssembly or Server |
| Browser | Modern (Chrome, Firefox, Edge, Safari) |

- Blazor Server:
  app.UseStaticFiles();

- No need to add script manually

---

## 📁 Project Structure

```
BlazorForge.Components/
├── Components/
│   ├── Button/
│   │   ├── BlazorForgeButton.razor
│   │   └── BlazorForgeButton.razor.css
│   ├── Alert/
│   │   └── BlazorForgeAlert.razor
│   └── ...
├── wwwroot/
│   └── blazorforge.css
├── BlazorForge.Components.csproj
└── README.md
```

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-new-component`
3. Commit your changes: `git commit -m 'Add: MyNewComponent'`
4. Push to the branch: `git push origin feature/my-new-component`
5. Open a Pull Request

Please make sure all components include XML documentation and a usage example.

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## 🙋 Support

- 🐛 Found a bug? [Open an issue](https://github.com/IrsanIskandar/SandevBlazorForgeComponent/issues)
- 💡 Have an idea? [Start a discussion](https://github.com/YOUR_USERNAME/BlazorForge.Components/discussions)
- ⭐ Like the project? Give it a star on GitHub!

---

<p align="center">Made with ❤️ for the Blazor community</p>
