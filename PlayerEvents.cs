using Terraria;
using Terraria.ModLoader;

namespace TerraSocket
{
    public class PlayerEvents : ModPlayer
    {
        public override void OnRespawn(Player player)
        {
            base.OnRespawn(player);
        }
        public override void OnEnterWorld(Player player)
        {
            base.OnEnterWorld(player);
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPC(item, target, damage, knockback, crit);
        }
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            base.OnHitByNPC(npc, damage, crit);
        }
        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            base.OnHitByProjectile(proj, damage, crit);
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
        }

    }
}
