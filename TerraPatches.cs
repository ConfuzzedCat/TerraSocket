using HarmonyLib;
using Terraria;
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
        static void PlayerHitWithProj(Projectile proj, int damage, bool crit, ModPlayer __instance)
        {
            string player = __instance.Name;
            string projName = proj.Name;
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerDamaged", true, new WebSocketMessageModel.ContextInfo(player, new WebSocketMessageModel.ContextInfo.ContextPlayerDamage(damage, crit, false, false, 0, "PROJECTILE", projName))));
        }

        /// <summary>
        /// This sends a WebSocket message when a npc killed. TODO: send one on damage too.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitNPC))]
        static void NPCHitPostfix(Item item, NPC target, int damage, float knockback, bool crit, ModPlayer __instance)
        {
            string playerName = __instance.Name;
            string itemNameWithPrefix = item.HoverName;
            string itemName = item.Name;
            string npcName = target.FullName;
            if (target.life < 0)
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("NPCKilled", true, new WebSocketMessageModel.ContextInfo(playerName, null, null, new WebSocketMessageModel.ContextInfo.ContextNpcKilled(npcName, itemNameWithPrefix, "MEELEE_ITEM", itemName, playerName, target.life + damage, damage, target.life * -1))));
            }
            else
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("NPCHit", true, new WebSocketMessageModel.ContextInfo(playerName, null, new WebSocketMessageModel.ContextInfo.ContextNpcDamage(npcName, "MEELEE_ITEM", itemName, playerName, target.life +damage, damage), null)));
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitNPCWithProj))]
        static void NPCHitWithProjPostfix(Projectile proj, NPC target, int damage, float knockback, bool crit, ModPlayer __instance)
        {
            string playerName = __instance.Name;
            string projName = proj.Name;
            string npcName = target.FullName;
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("NPCHit", true, new WebSocketMessageModel.ContextInfo(playerName, null, new WebSocketMessageModel.ContextInfo.ContextNpcDamage(npcName, "PROJECTILE", projName, playerName, target.life + damage, damage), null)));

        }
    }
}
