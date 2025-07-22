using QuakeLogParser.Core.Interfaces;
using QuakeLogParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeLogParser.Core.Services
{
  public class PlayerManager : IPlayerManager
  {
    public void AddPlayerIfNecessary(string player, Game game)
    {
      if(!game.Players.Contains(player) && player != "<world>")
      {
        game.Players.Add(player);
      }
    }
  }
}
