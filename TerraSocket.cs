using Terraria.ModLoader;
using HarmonyLib;
using log4net;
using Terraria;
using Terraria.DataStructures;
using System.IO;
using Newtonsoft.Json;
using System;

namespace TerraSocket
{
    public class TerraSocket : Mod
    {
        public static WebSocketServerHelper Server { get; set; }
        public static ILog _logger { get; set; }
        public override void Load()
        {
            _logger = Logger;
            PatchAll();
            base.Load();
            string ipPath = Path.Combine(Directory.GetCurrentDirectory(), "wsipconfig.json");
            ConfigModel config;
            if (File.Exists(ipPath))
            {
                string ipcontent = File.ReadAllText(ipPath);
                try
                {
                    config = JsonConvert.DeserializeObject<ConfigModel>(ipcontent);
                }
                catch (Exception e)
                {
                    Logger.Error("Invalid content in wsipconfig.json", e);
                    config = DefaultIp();
                    File.WriteAllText(ipPath, JsonConvert.SerializeObject(config));
                }
            }
            else
            {
                config = DefaultIp();
                File.WriteAllText(ipPath, JsonConvert.SerializeObject(config));
            }

            Server = new WebSocketServerHelper(config.Host, config.Port);
            Logger.Info($"WebSocket has started at {WebSocketServerHelper.wssv.Address}:{WebSocketServerHelper.wssv.Port}/");
            TerraPatches._server = Server;
        }

        private static ConfigModel DefaultIp()
        {
            return new ConfigModel() { Host = "127.0.0.1", Port = 7394 };
        }

        public override void Unload()
        {
            Logger.Info("Unloading...");
            Server.CloseServer();
            base.Unload();
        }

        private static void PatchAll()
        {
            _logger.Info("Initializing patching...");
            Harmony harmony = new Harmony("com.github.confuzzedcat.terraria.terrasocket");
            harmony.PatchAll();
            foreach(var method in harmony.GetPatchedMethods())
            {
                _logger.Debug($"Successfully patched: { method.Name}");
            }
        }
        public static bool TryGetCausingEntity(PlayerDeathReason _damageSource, out string entity)
        {
            entity = null;
            if (Main.npc.IndexInRange(_damageSource.SourceNPCIndex))
            {
                entity = Main.npc[_damageSource.SourceNPCIndex].FullName;
                return true;
            }
            if (Main.projectile.IndexInRange(_damageSource.SourceProjectileIndex))
            {
                entity = Main.projectile[_damageSource.SourceProjectileIndex].Name;
                return true;
            }
            if (Main.player.IndexInRange(_damageSource.SourcePlayerIndex))
            {
                entity = Main.player[_damageSource.SourcePlayerIndex].name;
                return true;
            }
            return false;
        }


        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
        }
    }
}