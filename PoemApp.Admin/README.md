PoemApp.Admin - Blazor Server 管理后台 (minimal scaffold)

包含：
- MudBlazor UI
- `Poems` 管理页面示例 (调用 `PoemApp.API` 的 `api/poems`)
- `PoemApiClient` 简单封装 HttpClient

要运行：
1. 在 `PoemApp.Admin` 项目中设置启动配置，确保 `Api:BaseUrl` 指向你的 API。
2. 参考 Program.cs 启动。