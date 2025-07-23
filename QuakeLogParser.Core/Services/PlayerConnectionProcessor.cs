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
  public class PlayerConnectionProcessor : ILogProcessor
  {
    private readonly IPlayerManager _playerManager;

    public PlayerConnectionProcessor(IPlayerManager playerManager)
    {
      _playerManager = playerManager;
    }

    public void Processor(string logLine, Game game) {

      var match = Regex.Match(logLine, @"n\\([^\\]+)\\");
      if (match.Success) { 
        var playerName = match.Groups[1].Value;
        _playerManager.AddPlayerIfNecessary(playerName, game);
      }
    }
  }
}
