using HarmonyLib;
using Terraria;
using Terraria.Achievements;
using Terraria.GameContent.Events;
using Terraria.ModLoader;

namespace TerraSocket
{

    /// <summary>
    /// This class contains all the patches this mods will patch.
    /// </summary>
    [HarmonyPatch]
    class TerraPatches
    {
        private static bool EventIsPartyUp { get; set; } = false;
        private static bool EventIsSandstormThere { get; set; } = false;
        private static bool EventIsDD2There { get; set; } = false;
        private static bool EventIsSnowMoonThere { get; set; } = false;
        private static bool EventIsPumpkinMoonThere { get; set; } = false;
        private static bool EventIsBloodMoonThere { get; set; } = false;


        #region PlayerHitPatches
        /// <summary>
        /// This sends a WebSocket message when the player takes damage from a npc.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitByNPC))]
        static void PlayerDamaged(NPC npc, int damage, bool crit, ModPlayer __instance)
        {
            string playerName = __instance.player.name;
            string npcName = npc.FullName;
            if (__instance.player.statLife <= 0)
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerKilled", true, new WebSocketMessageModel.ContextInfo(playerName, null, null, null, null, new WebSocketMessageModel.ContextInfo.ContextPlayerKilled(playerName, "NPC", npcName, __instance.player.statLife + damage, damage, __instance.player.statLife * -1))));
            }
            else
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerHit", true, new WebSocketMessageModel.ContextInfo(playerName, null, new WebSocketMessageModel.ContextInfo.ContextPlayerDamage(playerName, damage, crit, false, false, 0, "NPC", npcName))));
            }
        }

        /// <summary>
        /// This sends a WebSocket message when the player takes damage from a projectile.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitByProjectile))]
        static void PlayerHitWithProj(Projectile proj, int damage, bool crit, ModPlayer __instance)
        {
            string playerName = __instance.player.name;
            string projName = proj.Name;
            if (__instance.player.statLife <= 0)
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerKilled", true, new WebSocketMessageModel.ContextInfo(playerName, null, null, null, null, new WebSocketMessageModel.ContextInfo.ContextPlayerKilled(playerName, "PROJECTILE", projName, __instance.player.statLife + damage, damage, __instance.player.statLife * -1))));
            }
            else
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerHit", true, new WebSocketMessageModel.ContextInfo(playerName, null, new WebSocketMessageModel.ContextInfo.ContextPlayerDamage(playerName, damage, crit, false, false, 0, "PROJECTILE", projName))));

            }

        }
        #endregion

        #region NPCHitPatches
        /// <summary>
        /// This sends a WebSocket message when a npc is either hit or killed. 
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitNPC))]
        static void NPCHitPostfix(Item item, NPC target, int damage, float knockback, bool crit, ModPlayer __instance)
        {
            string playerName = __instance.player.name;
            string itemNameWithPrefix = item.HoverName;
            string itemName = item.Name;
            string npcName = target.FullName;
            if (target.life <= 0)
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("NPCKilled", true, new WebSocketMessageModel.ContextInfo(playerName, null, null, null, new WebSocketMessageModel.ContextInfo.ContextNPCKilled(npcName, itemNameWithPrefix, "MEELEE_ITEM", itemName, playerName, target.life + damage, damage, target.life * -1))));
            }
            else
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("NPCHit", true, new WebSocketMessageModel.ContextInfo(playerName, null, null, new WebSocketMessageModel.ContextInfo.ContextNPCDamage(npcName, "MEELEE_ITEM", itemName, playerName, target.life + damage, damage), null)));
            }
        }

        /// <summary>
        /// This sends a WebSocket message when a npc is either hit by a Projectile or killed by one.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitNPCWithProj))]
        static void NPCHitWithProjPostfix(Projectile proj, NPC target, int damage, float knockback, bool crit, ModPlayer __instance)
        {
            string playerName = __instance.player.name;
            string projName = proj.Name;
            string npcName = target.FullName;
            if (target.life < 0)
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("NPCKilled", true, new WebSocketMessageModel.ContextInfo(playerName, null, null, null, new WebSocketMessageModel.ContextInfo.ContextNPCKilled(npcName, null, "PROJECTILE", projName, playerName, target.life + damage, damage, target.life * -1))));
            }
            else
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("NPCHit", true, new WebSocketMessageModel.ContextInfo(playerName, null, null, new WebSocketMessageModel.ContextInfo.ContextNPCDamage(npcName, "PROJECTILE", projName, playerName, target.life + damage, damage), null)));
            }
        }
        #endregion

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitPvp))]
        static void OnHitPvpPostfix(Item item, Player target, int damage, bool crit, ModPlayer __instance)
        {
            string playerName = __instance.player.name;
            if (target.statLife <= 0)
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerPVPKill", true, new WebSocketMessageModel.ContextInfo(playerName, null, null, null, null, new WebSocketMessageModel.ContextInfo.ContextPlayerKilled(target.name, "MEELEE_ITEM", item.Name, target.statLife + damage, damage, target.statLife * -1))));
            }
            else
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerPVPHit", true, new WebSocketMessageModel.ContextInfo(playerName, null, new WebSocketMessageModel.ContextInfo.ContextPlayerDamage(target.name, damage, crit, true, false, 0, "MEELEE_ITEM", item.Name))));
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnHitPvpWithProj))]
        static void OnHitPvpWithProjPostfix(Projectile proj, Player target, int damage, bool crit, ModPlayer __instance)
        {
            string playerName = __instance.player.name;
            if (target.statLife <= 0)
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerPVPKill", true, new WebSocketMessageModel.ContextInfo(playerName, null, null, null, null, new WebSocketMessageModel.ContextInfo.ContextPlayerKilled(target.name, "PROJECTILE", proj.Name, target.statLife + damage, damage, target.statLife * -1))));
            }
            else
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerPVPHit", true, new WebSocketMessageModel.ContextInfo(playerName, null, new WebSocketMessageModel.ContextInfo.ContextPlayerDamage(target.name, damage, crit, true, false, 0, "PROJECTILE", proj.Name))));
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnEnterWorld))]
        static void OnEnterWorldPostfix(Player player)
        {
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("OnEnterWorld", true, new WebSocketMessageModel.ContextInfo(player.name)));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.OnRespawn))]
        static void OnRespawnPostfix(Player player)
        {
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("OnRespawn", true, new WebSocketMessageModel.ContextInfo(player.name)));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.PlayerConnect))]
        static void PlayerConnectPostfix(Player player)
        {
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerConnect", true, new WebSocketMessageModel.ContextInfo(player.name)));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModPlayer), nameof(ModPlayer.PlayerDisconnect))]
        static void PlayerDisconnectPostfix(Player player)
        {
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PlayerDisconnect", true, new WebSocketMessageModel.ContextInfo(player.name)));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Mod), nameof(Mod.PostUpdateEverything))]
        static void EventsPostfix()
        {
            if (BirthdayParty.PartyIsUp)
            {
                if (!EventIsPartyUp)
                {
                    EventIsPartyUp = true;
                    WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PartyEvent", true));
                }
            }
            else
            {
                EventIsPartyUp = false;
            }
            if (Sandstorm.Happening)
            {
                if (!EventIsSandstormThere)
                {
                    EventIsSandstormThere = true;
                    WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("SandstormEvent", true));
                }
            }
            else
            {
                EventIsSandstormThere = false;
            }
            if (DD2Event.Ongoing)
            {
                if (!EventIsDD2There)
                {
                    WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("DD2Event", true));
                }
            }
            else
            {
                EventIsDD2There = false;
            }
            if (Main.pumpkinMoon)
            {
                if (!EventIsPumpkinMoonThere)
                {
                    EventIsPumpkinMoonThere = true;
                    WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("PumpkinMoonEvent", true));
                }
            }
            else
            {
                EventIsPumpkinMoonThere = false;
            }
            if (Main.snowMoon)
            {
                if (!EventIsSnowMoonThere)
                {
                    EventIsSnowMoonThere = true;
                    WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("SnowMoonEvent", true));
                }
            }
            else
            {
                EventIsSnowMoonThere = false;
            }
            if (Main.bloodMoon)
            {
                if (!EventIsBloodMoonThere)
                {
                    EventIsBloodMoonThere = true;
                    WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("BloodMoonEvent", true));
                }
            }
            else
            {
                EventIsBloodMoonThere = false;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NPC), nameof(NPC.NewNPC))]
        static void NPCSpawnPostfix(int __result, int X, int Y, int Type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int Target = 255)
        {
            NPC npc = Main.npc[__result];
            string npcName = npc.FullName;
            int npcLife = npc.life;
            if (npc.boss)
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("BossSpawn", true, new WebSocketMessageModel.ContextInfo(null, null, null, null, null, null, new WebSocketMessageModel.ContextInfo.ContextBossSpawn(npcName, npcLife))));
            }

            switch (__result)
            {
                case 437:
                    WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("CultistRitualStarted", true));
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WorldGen), nameof(WorldGen.meteor))]
        static void MeteorPostfix(int i, int j, ref bool __result)
        {
            if (__result)
            {
                WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("MeteorLanded", true));
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(WorldGen), nameof(WorldGen.TriggerLunarApocalypse))]
        static void LunarApocalypsePostfix()
        {
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("LunarApocalypseStarted", true));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Main),nameof(Main.StartSlimeRain))]
        static void RainSlimeEventPostfix(bool announce = true)
        {
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("SlimeRainEvent", true));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Main), nameof(Main.AnglerQuestSwap))]
        static void NewAnglerQuestPostfix()
        {
            WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("AnglerQuestReset", true));
        }


        // tModLoader, doesn't have Achievements, to use in TerrariaInjector
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(AchievementCondition), nameof(AchievementCondition.Complete))]
        //static void OnAchievementCompletePrefix(AchievementCondition __instance)
        //{
        //    if (__instance.IsCompleted)
        //    {
        //        WebSocketServerHelper.SendWSMessage(new WebSocketMessageModel("AchievementComplete", true, new WebSocketMessageModel.ContextInfo(null, __instance.Name)));
        //    }
        //}
    }
}
