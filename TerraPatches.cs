using HarmonyLib;
using Mono.Cecil;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TerraSocket
{

    /// <summary>
    /// This class contains 
    /// </summary>
    [HarmonyPatch]
    class TerraPatches
    {
        /// <summary>
        /// This sends a WebSocket message when the player takes damage from a npc.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitByNPC))]
        static void PlayerDamaged(ModPlayer __instance, NPC npc, int damage, bool crit)
        {
            string player = __instance.Name;
            string npcName = npc.FullName;
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerDamaged", true, new WebSocketMessageModel.ContextInfo(player, new WebSocketMessageModel.ContextInfo.ContextPlayerDamage(damage, crit, false, false, 0, "NPC", npcName))));
        }

        /// <summary>
        /// This sends a WebSocket message when the player takes damage from a projectile.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitByProjectile))]
        static void PlayerHitWithProj(Projectile proj, int damage, bool crit, Player __instance)
        {
            string player = __instance.name;
            string projName = proj.Name;
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerDamaged", true, new WebSocketMessageModel.ContextInfo(player, new WebSocketMessageModel.ContextInfo.ContextPlayerDamage(damage, crit, false, false, 0, "Projectile".ToUpper(), projName))));
        }

        /// <summary>
        /// This sends a WebSocket message when a npc killed. TODO: send one on damage too.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitNPC))]
        static void NPCHitPostfix(Item item, NPC target, int damage, float knockback, bool crit, Player __instance)
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
