using QuakeLogParser.Core.Interfaces;
using QuakeLogParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuakeLogParser.Core.Services
{
  public class KillEventProcessor : ILogProcessor
  {

    private readonly IPlayerManager _playerManager;
    private readonly IKillUpdater _killUpdater;

    public KillEventProcessor(IPlayerManager playerManager, IKillUpdater killUpdater)
    {
      _playerManager = playerManager;
      _killUpdater = killUpdater;
    }

    public void Processor(string logLine, Game game)
    {
      if (!logLine.Contains("killed"))
        return;

      var killPattern = new Regex(@"Kill:.*?: (.+?) killed (.+?) by (\S+)");
      var match = killPattern.Match(logLine);

      if (!match.Success)
        return;

      var attacker = match.Groups[1].Value; // Jogador que fez o kill
      var victim = match.Groups[2].Value;   // Jogador que foi morto
      var weapon = match.Groups[3].Value;   // Arma usada

      _playerManager.AddPlayerIfNecessary(attacker, game);
      _playerManager.AddPlayerIfNecessary(victim, game);

      _killUpdater.UpdateKills(attacker, victim, game);
    }
  }
}
