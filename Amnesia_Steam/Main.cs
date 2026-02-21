using AmnesiaTools.Core;
using AmnesiaTools.Core.LoggerHelper;
using Life;
using Life.DB;
using Life.Network;
using Mirror;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Amnesia_Steam
{
    public class Main : AmnesiaTools.AmnesiaSystem
    {
        public Main(IGameAPI aPI) : base(aPI) { new PluginInformations("AmnesiaSteam", "V.1.0.0", "Zerox"); }
        public HttpClient client = new HttpClient();
        public override async void OnPlayerSpawnCharacter(Player player, NetworkConnection conn, Characters character)
        {
            base.OnPlayerSpawnCharacter(player, conn, character); ;
            var response = await client.GetAsync($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=9BDCB7258E81FEDC8BB8818811C5982C&steamids={player.steamId}");
            if(response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                JObject jobject = JObject.Parse(content);
                JArray responseArray = (JArray)jobject["response"]["players"];
                if(responseArray.Count == 0)
                {
                    Logger.LogError("[Amnesia_Steam] None player was returned by the steam api");
                }
                long timestamp = (long)responseArray[0]["timecreated"];
                if(Nova.UnixTimeNow() - timestamp < 2678400)
                {
                    player.account.banReason = "[AmnesiaSteam] Compte steam trop récent";
;                   player.account.banTimestamp = -1L;
                    player.Disconnect();
                }
                Console.WriteLine(content);            
            }
            else
            {
                Logger.LogError($"[Amnesia_Steam] Error with the request of an account steam (steam id : {player.steamId}");
            }
        }
    }
}
