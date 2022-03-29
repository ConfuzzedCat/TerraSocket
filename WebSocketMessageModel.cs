﻿namespace TerraSocket
{
    public class WebSocketMessageModel
    {
        public WebSocketMessageModel(string _event, bool status, ContextInfo contextInfo = null)
        {
            SetWebSocketServerVersion();
            this.Event = _event;
            if (status)
            {
                this.Status = "Succes";
            }
            else
            {
                this.Status = "Error";
            }
            this.ExtraInfo = contextInfo;
        }

        public SemVersion webSocketServerVersion { get; private set; }
        private void SetWebSocketServerVersion()
        {
            webSocketServerVersion = new SemVersion(0,2,2);
        }
        public string Event { get; set; }
        public string Status { get; set; }
        public ContextInfo ExtraInfo { get; set; }

        public class SemVersion
        {
            public SemVersion(int major, int minor, int patch)
            {
                this.MajorVersion = major;
                this.MinorVersion = minor;
                this.PatchVersion = patch;
            }
            public int MajorVersion { get; set; }
            public int MinorVersion { get; set; }
            public int PatchVersion { get; set; }
        }

        public class ContextInfo
        {
            public ContextInfo(string player = null, ContextPlayerDamage contextPlayerDamage = null, ContextNpcKilled npcKilled = null)
            {
                this.player = player;
                this.PlayerDamage = contextPlayerDamage;
                this.npcKilled = npcKilled;
            }
            public string player { get; set; }
            public ContextPlayerDamage PlayerDamage { get; set; }
            public ContextNpcDamage NpcDamage { get; set; }
            public ContextNpcKilled npcKilled { get; set; }
            public class ContextPlayerDamage
            {
                public ContextPlayerDamage(double damage = 0, bool crit = false, bool pvp = false, bool quiet = false, int hitDirection = 0, string source = null)
                {
                    this.damage = damage;
                    this.crit = crit;
                    this.pvp = pvp;
                    this.quiet = quiet;
                    this.hitDirection = hitDirection;
                    this.source = source;
                }
                public bool pvp { get; set; }
                public bool quiet { get; set; }
                public double damage { get; set; }
                public int hitDirection { get; set; }
                public bool crit { get; set; }
                public string source { get; set; }
            }

            public class ContextNpcDamage
            {
                //TODO
            }

            public class ContextNpcKilled
            {
                public ContextNpcKilled(string npcname = null, string sourceWithPrefix = null, string source = null, int npcLifePreHit = 0, int damageDealt = 0, int overflowDamage = 0)
                {
                    this.NPCname = npcname;
                    this.SourceWithPrefix = sourceWithPrefix;
                    this.Source = source;
                    this.NPCLifePreHit = npcLifePreHit;
                    this.DamageDealt = damageDealt;
                    this.OverflowDamage = overflowDamage;
                }
                public string NPCname { get; set; }
                public string SourceWithPrefix { get; set; }
                public string Source { get; set; }
                public int NPCLifePreHit { get; set; }
                public int DamageDealt { get; set; }
                public int OverflowDamage { get; set; }
            }
        }
    }
}