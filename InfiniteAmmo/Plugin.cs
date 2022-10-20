using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace InfiniteAmmo
{
    public class Plugin : Plugin<Config>
    {
        public override string Prefix => "InfiniteAmmo";
        public override string Name => "InfiniteAmmo";
        public override string Author => "Mariki";
        public static Plugin plugin;
        public bool Known = false;
        public override Version Version { get; } = new Version(1, 0, 0);
        public List<Player> Gplayers = new List<Player>();
        bool going = false;
        public override void OnEnabled()
        {
            plugin = this;

            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            Exiled.Events.Handlers.Server.RestartingRound += OnRoundEnd;

            Exiled.Events.Handlers.Player.UsingRadioBattery += OnRadioUse;
            Exiled.Events.Handlers.Player.Died += OnPlayerDie;
            Exiled.Events.Handlers.Player.ChangingRole += OnPlayerForceclass;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeave;

            base.OnEnabled();
        }



        void infam()
        {
            if (going)
            {
                foreach (Pickup pickup in Map.Pickups)
                {
                    if (pickup.Type.ToString().ToLower().StartsWith("ammo"))
                    {
                        pickup.Destroy();
                    }
                }
                foreach(Player player in Player.List)
                {
                    foreach (AmmoType ammo in new List<AmmoType>() { AmmoType.Nato556, AmmoType.Nato762, AmmoType.Nato9, AmmoType.Ammo44Cal, AmmoType.Ammo12Gauge })
                    {
                        if (!player.IsCuffed)
                        player.SetAmmo(ammo, 100);
                    }
                }
                Timing.CallDelayed(0f, () => infam());
            }
        }
        
        bool CanSpawn = true;
        private void OnAmmoDropped(DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        private void OnRadioUse(UsingRadioBatteryEventArgs ev)
        {
            ev.IsAllowed = false;
            ev.Radio.Range = Exiled.API.Enums.RadioRange.Ultra;
            ev.Drain = 0;
        }
        public void SpawnPlayer(Player player)
        {
            
            Gplayers.Add(player);

        }
       


        private void OnRoundEnd()
        {
            Gplayers.Clear();
            going = false;
        }

        private void OnRoundStart()
        {
            Gplayers.Clear();
            going = true;
            infam();        
        }
        private void OnPlayerLeave(LeftEventArgs ev)
        {
            Gplayers.Remove(ev.Player);
        }
        private void OnPlayerForceclass(ChangingRoleEventArgs ev)
        {
            Gplayers.Remove(ev.Player);
            ev.Player.CustomInfo = "";
        }

        private void OnPlayerDie(DiedEventArgs ev)
        {
            Gplayers.Remove(ev.Target);
            ev.Target.CustomInfo = "";
        }
        public override void OnDisabled()
        {
            plugin = null;

            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
            Exiled.Events.Handlers.Server.RestartingRound -= OnRoundEnd;

            Exiled.Events.Handlers.Player.UsingRadioBattery -= OnRadioUse;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDie;
            Exiled.Events.Handlers.Player.ChangingRole -= OnPlayerForceclass;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeave;
            base.OnDisabled();
        }
    }
}
