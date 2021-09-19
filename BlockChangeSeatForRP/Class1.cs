using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Rocket.Core.Utils;
using System.Timers;
using Rocket.Unturned.Chat;
using System.Net;
using System.Text.RegularExpressions;
using Logger = Rocket.Core.Logging.Logger;

namespace BlockChangeSeatForRP
{
    public class Class1 : RocketPlugin<Config>
    {
        public Class1 Instance;
        public List<CSteamID> EnCooldown { get; set; }
        public Timer Reloj { get; set; }
        protected override void Load()
        {
            Instance = this;


            if (!Configuration.Instance.Lock_to_change_seats)
            {
                Reloj = new Timer(1 * 1000); Reloj.Elapsed += Reloj_Elapsed;
                Reloj.Start(); Reloj.AutoReset = true; Reloj.Enabled = true;
                Logger.Log("Se habilito el reloj");
            }

            
            VehicleManager.onSwapSeatRequested += asiento;

            EnCooldown = new List<CSteamID>();
            Logger.Log("==================", ConsoleColor.Cyan);
            Logger.Log("Creado Por Margarita#8172", ConsoleColor.Cyan);
            Logger.Log("Para EnvyHosting", ConsoleColor.Cyan);
            Logger.Log("==================", ConsoleColor.Cyan);
        }



        private void Reloj_Elapsed(object sender, ElapsedEventArgs e)
        {
            TaskDispatcher.QueueOnMainThread(() =>
            {

                Provider.clients.ForEach(delegate (SteamPlayer client)
                {

                    UnturnedPlayer player; player = UnturnedPlayer.FromSteamPlayer(client);

                    bool owo = EnCooldown.Contains(player.CSteamID);

                    if (owo)
                    {
                        Dato valor = player.GetComponent<Dato>();
                        if(valor.tiempo <= 0)
                        {
                            EnCooldown.Remove(player.CSteamID);
                            return;
                        }
                        else
                        {
                            valor.tiempo--;
                        }
                    }
                });

            });
        }



        private void asiento(Player player, InteractableVehicle vehicle, ref bool shouldAllow, byte fromSeatIndex, ref byte toSeatIndex)
        {
            if (Configuration.Instance.Lock_to_change_seats)
            {
                shouldAllow = false;
            }

            UnturnedPlayer jugador = UnturnedPlayer.FromPlayer(player);


            bool owo = EnCooldown.Contains(jugador.CSteamID);

            if (owo)
            {
                Dato datovalor = jugador.GetComponent<Dato>();
                UnturnedChat.Say(jugador, Translate("in_cooldown", datovalor.tiempo.ToString()));
                shouldAllow = false;
            }
            else
            {
                Dato datovalor = jugador.GetComponent<Dato>();
                shouldAllow = true;
                datovalor.tiempo = Configuration.Instance.cooldown;
                EnCooldown.Add(jugador.CSteamID);
            }

            
            




        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "in_cooldown", "¡Tienes Que Esperar {0} Para Poder Cambiar De Asiento!"}
                };
            }
        }



        protected override void Unload()
        {
            EnCooldown = null;
            Reloj = null;
            VehicleManager.onSwapSeatRequested -= asiento;
        }
    }
}
