using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using HarmonyLib;

namespace TerraSocket
{
    public static class WebSocketServerHelper
    {
        public static WebSocketServer wssv { get; set; } = new WebSocketServer("ws://127.0.0.1");
        public static void CloseServer()
        {
            wssv.Stop();
        }
        public static WebSocketServer InitializeServer()
        {
            wssv = new WebSocketServer("ws://127.0.0.1");
            wssv.AddWebSocketService<Startup>("/");
            wssv.Start();
            return wssv;
        }
        public static void SendWSMessage(WebSocketMessageModel msg)
        {
            string jsonMessage = JsonConvert.SerializeObject(msg);
            if (wssv != null)
                wssv.WebSocketServices.Broadcast(jsonMessage);
        }
    }
    public class Startup : WebSocketBehavior
    {
        protected override void OnClose(CloseEventArgs e)
        {
            Send("Closing WebSocket Server.");
            base.OnClose(e);
        }
        protected override void OnOpen()
        {
            TerraSocket._logger.Info($"Client joined. ID:{ID}");
            base.OnOpen();
        }
    }
}
