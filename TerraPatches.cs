using HarmonyLib;
using Mono.Cecil;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerraSocket
{
    [HarmonyPatch]
    class TerraPatches
    {

        /*
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), nameof(Player.Hurt))]
        static void PlayerHurtPostfix(Player __instance, PlayerDeathReason damageSource, int Damage, int hitDirection, bool pvp = false, bool quiet = false, bool Crit = false, int cooldownCounter = -1)
        {
            string player = Main.player[__instance.whoAmI].name;
            if (TerraSocket.TryGetCausingEntity(damageSource, out string source))
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerDamaged", true, new WebSocketMessageModel.ContextInfo(player, new WebSocketMessageModel.ContextInfo.ContextPlayerDamage(Damage, Crit, pvp, quiet, hitDirection, source))));
            }
            else
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerDamaged", true));
            }
        }
        */

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitByNPC))]
        static void PlayerDamaged(ModPlayer __instance, NPC npc, int damage, bool crit)
        {
            string player = __instance.Name;
            string npcName = npc.FullName;
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerDamaged", true, new WebSocketMessageModel.ContextInfo(player, new WebSocketMessageModel.ContextInfo.ContextPlayerDamage(damage, crit, false, false, 0, npcName))));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitNPC))]
        static void NPCKilledPostfix(Item item, NPC target, int damage, float knockback, bool crit, Player __instance)
        {
            if(target.life < 0)
            {
                string playerName = __instance.name;
                string itemNameWithPrefix = item.HoverName;
                string itemName = item.Name;
                string npcName = target.FullName;
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("NPCKilled", true, new WebSocketMessageModel.ContextInfo(playerName, null, new WebSocketMessageModel.ContextInfo.ContextNpcKilled(npcName, itemNameWithPrefix, itemName,target.life+damage, damage, target.life*-1))));
            }
            else
            {
                //TODO: Send message on damage.
            }
        }
    }

}
