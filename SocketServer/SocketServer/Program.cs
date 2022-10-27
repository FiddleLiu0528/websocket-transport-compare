using SocketServer.Models;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);


//加入WebSocket處理服務
builder.Services.AddSingleton<WebSocketJsonHandler>();
builder.Services.AddSingleton<WebSocketJsonCompressHandler>();
builder.Services.AddSingleton<WebSocketProtobufHandler>();

var app = builder.Build();

//加入 WebSocket 功能
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30),
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws-json")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using (WebSocket ws = await context.WebSockets.AcceptWebSocketAsync())
            {
                var wsHandler = context.RequestServices.GetRequiredService<WebSocketJsonHandler>();
                await wsHandler.ProcessWebSocket(ws);
            }
        }
        else
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
    }
    else if (context.Request.Path == "/ws-compress-json")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using (WebSocket ws = await context.WebSockets.AcceptWebSocketAsync())
            {
                var wsHandler = context.RequestServices.GetRequiredService<WebSocketJsonCompressHandler>();
                await wsHandler.ProcessWebSocket(ws);
            }
        }
        else
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
    }
    else if (context.Request.Path == "/ws-protobuf")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using (WebSocket ws = await context.WebSockets.AcceptWebSocketAsync())
            {
                var wsHandler = context.RequestServices.GetRequiredService<WebSocketProtobufHandler>();
                await wsHandler.ProcessWebSocket(ws);
            }
        }
        else
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
    }
    else await next();
});

app.Run();
