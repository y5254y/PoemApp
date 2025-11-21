# 启动 Blazor WASM 调试环境
# 1. 启动带调试端口的 Chrome
Start-Process "chrome.exe" "--remote-debugging-port=9222 --user-data-dir=`"$env:TEMP\blazor-chrome-debug`" --no-first-run"

# 等待一下让浏览器启动
Start-Sleep -Seconds 2

# 2. 启动 Web 项目
dotnet run --project PoemApp.Web --launch-profile "PoemApp.Web"
