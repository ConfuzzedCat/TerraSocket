using Terraria.ModLoader;
using WebSocketSharp.Server;
using HarmonyLib;
using log4net;
using Terraria;
using Terraria.DataStructures;

namespace TerraSocket
{
    public class TerraSocket : Mod
    {
        public static WebSocketServer wssv { get; set; }
        public static ILog _logger { get; set; }
        public override void Load()
        {
            _logger = Logger;
            PatchAll();
            base.Load();
            wssv = WebSocketServerHelper.InitializeServer();
            Logger.Info($"WebSocket has started at {wssv.Address}:{wssv.Port}/");
        }
        public override void Unload()
        {
            Logger.Info("Unloading...");
            WebSocketServerHelper.CloseServer();
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
    }
}