using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TerraSocket
{
    public class WebSocketServerHelper
    {
        public WebSocketServerHelper(string ip = "127.0.0.1", ushort port = 7394)
        {
            wssv = InitializeServer(ip, port);
        }

        public static WebSocketServer wssv { get; set; }
        public void CloseServer()
        {
            wssv.Stop();
        }
        private WebSocketServer InitializeServer(string ip, ushort port)
        {
            string addr = $"ws://{ip}:{port}";
            wssv = new WebSocketServer(addr);
            wssv.AddWebSocketService<Startup>("/");
            wssv.Start();
            TerraSocket._logger.Info($"WebSocket server started at \"{ip + ':' + port}\"");
            return wssv;
        }
        public void SendWSMessage(WebSocketMessageModel msg)
        {
            string jsonMessage = JsonConvert.SerializeObject(msg);
            if (!(wssv is null))
            {
                wssv.WebSocketServices.Broadcast(jsonMessage);
                TerraSocket._logger.Info($"\"{msg.Event}\" sent to clients.");
            }
            else
            {
                TerraSocket._logger.Warn("WebSocket Server not found.");
            }

        }
    }
    public class Startup : WebSocketBehavior
    {
        protected override void OnClose(CloseEventArgs e)
        {
            TerraSocket._logger.Info($"Client Disconnected. ID:{ID}");
            base.OnClose(e);
        }
        protected override void OnOpen()
        {
            TerraSocket._logger.Info($"Client joined. ID:{ID}");
            base.OnOpen();
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            TerraSocket._logger.Debug($"Message received: {e.Data}");
            //Commands.KillPlayer("ConfuzzedCat");
            Commands.CommandHandler(e.Data);
            base.OnMessage(e);
        }
        protected override void OnError(ErrorEventArgs e)
        {
            TerraSocket._logger.Error("WebSocket Error", e.Exception);
            base.OnError(e);
        }
    }
}
