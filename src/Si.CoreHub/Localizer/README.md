# 模块化JSON本地化组件

本组件基于ASP.NET Core的IStringLocalizer接口实现，提供了使用JSON文件作为本地化资源的功能，并且与模块化开发系统无缝集成。

## 功能特点

- 基于ASP.NET Core的IStringLocalizer接口实现，兼容标准本地化API
- 使用JSON文件作为本地化资源，易于编辑和维护
- 支持模块化，每个模块都有自己的翻译文件
- 支持资源文件热更新（可选）
- 支持文化回退机制，当特定文化找不到翻译时回退到父文化
- 内置丰富的扩展方法，方便快速获取特定模块的本地化资源

## 使用方法

### 1. 注册服务

在应用程序启动时注册本地化服务：

```csharp
// 在 Program.cs 或 Startup.cs 中
// 添加模块化JSON本地化服务
builder.Services.AddModuleJsonLocalization(watchResourceFiles: true);

// 在配置中间件时
app.UseModuleJsonLocalization(watchResourceFiles: true);
```

### 2. 创建模块的本地化资源文件

在每个模块的目录下创建 `Resources` 文件夹，然后添加JSON格式的本地化资源文件：

```
ModuleName/
  └── Resources/
      ├── Messages.zh-CN.json
      ├── Messages.en-US.json
      ├── Errors.zh-CN.json
      └── Errors.en-US.json
```

JSON文件格式示例（Messages.zh-CN.json）：

```json
{
  "Welcome": "欢迎使用我们的系统",
  "Hello": "你好，{0}",
  "Goodbye": "再见"
}
```

### 3. 在控制器或服务中使用

```csharp
using Microsoft.Extensions.Localization;
using Si.CoreHub.Localizor;

public class ExampleController : Controller
{
    private readonly IStringLocalizer _localizer;
    
    public ExampleController(IStringLocalizerFactory factory)
    {
        // 获取特定模块和资源的本地化器
        _localizer = factory.ForModule("ModuleName", "Messages");
    }
    
    public IActionResult Index()
    {
        // 使用本地化字符串
        var welcome = _localizer["Welcome"];
        var hello = _localizer["Hello", "用户"];
        
        return View();
    }
}
```

### 4. 在Razor视图中使用

```html
@using Microsoft.Extensions.Localization
@inject IStringLocalizer<Messages> Localizer

<h1>@Localizer["Welcome"]</h1>
<p>@Localizer["Hello", ViewData["UserName"]]</p>
```

## 高级用法

### 通过泛型类获取本地化器

```csharp
public class ExampleService
{
    private readonly IStringLocalizer<Messages> _localizer;
    
    public ExampleService(IStringLocalizer<Messages> localizer)
    {
        _localizer = localizer;
    }
    
    public void DoSomething()
    {
        Console.WriteLine(_localizer["SomeMessage"]);
    }
}
```

### 使用扩展方法快速获取模块本地化器

```csharp
public class ExampleService
{
    private readonly IStringLocalizer _localizer;
    
    public ExampleService(IStringLocalizerFactory factory)
    {
        // 获取当前模块的Messages资源
        _localizer = factory.ForCurrentModule<Messages>();
    }
}
```

## 注意事项

1. 确保每个模块都有一个 `Resources` 文件夹用于存放本地化资源
2. JSON文件的命名格式必须为：`{资源名}.{文化名}.json`
3. 如果启用了资源文件监控，修改JSON文件后会自动重新加载，无需重启应用程序 