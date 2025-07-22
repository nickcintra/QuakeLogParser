using QuakeLogParser.Core.Interfaces;
using QuakeLogParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeLogParser.Core.Services
{
  public class KillUpdater : IKillUpdater
  {
    public void UpdateKills(string attacker, string victim, Game game)
    {
      if(attacker == "<world>")
      {
        game.TotalKills++;

        if (!game.KillsByPlayer.ContainsKey(victim))
          game.KillsByPlayer[victim] = 0;

        game.KillsByPlayer[victim]--;
      }
      else
      {
        if (!game.KillsByPlayer.ContainsKey(attacker))
          game.KillsByPlayer[attacker] = 0;

        game.KillsByPlayer[attacker]++;
        game.TotalKills++;
      }

    }
  }
}
