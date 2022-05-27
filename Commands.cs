using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.Localization;
using Terraria.Net;

namespace TerraSocket
{
    public static class Commands
    {
        public static void CommandHandler(string response)
        {
            try
            {
                CommandModel cm = JsonConvert.DeserializeObject<CommandModel>(response);
                switch (cm.Command.ToLower())
                {
                    case "giveitem":
                        GiveItem(cm.UserName, cm.ItemID);
                        break;
                    case "killplayer":
                        KillPlayer(cm.UserName);
                        break;
                    case "healplayer":
                        HealPlayer(cm.UserName, cm.HealAmount);
                        break;
                    default:
                        throw new Exception($"Unknown command. {cm.Command}.");
                }
            }
            catch (Exception e)
            {
                TerraSocket._logger.Error("Error while parsing command", e);
            }
        }

        public static void GiveItem(string sourceUser, int id)
        {
            try
            {
                Player player = Main.player[Main.myPlayer];
                Item item = new Item();
                item.SetDefaults(id);
                player.GetItem(player.whoAmI, item);
                NetworkText text = NetworkText.FromLiteral($"{sourceUser} has given you a {item.Name}.");
                SendChatMessage(text, Main.myPlayer, Color.Cyan);
                //ChatHelper.DisplayMessageOnClient(text, Color.Cyan, Main.myPlayer);
            }
            catch (Exception e)
            {
                TerraSocket._logger.Error("Error Killing player", e);
            }
        }
        public static void KillPlayer(string sourceUser)
        {
            try
            {

                Player player = Main.player[Main.myPlayer];
                Helper.SpawnKillNpc(player.Center.X, player.Center.Y, 68);
                NetworkText text = NetworkText.FromLiteral($"{sourceUser} has sent a Dungeon Guardian after you!");
                SendChatMessage(text, Main.myPlayer, Color.Red);
            }
            catch (Exception e)
            {

                TerraSocket._logger.Error("Error Killing player", e);
            }
        }

        public static void HealPlayer(string sourceUser, int amount)
        {
            amount = Math.Abs(amount);
            Player player = Main.player[Main.myPlayer];
            player.statLife += amount;
            NetworkText text = NetworkText.FromLiteral($"{sourceUser} has healed you {amount} hp.");
            SendChatMessage(text, Main.myPlayer, Color.Green);
        }

        private static void SendChatMessage(NetworkText text, int playerid, Color messageColor)
        {
            NetPacket packet = NetTextModule.SerializeServerMessage(text, messageColor, (byte)playerid);
            NetManager.Instance.Broadcast(packet, -1);
        }
    }
}
