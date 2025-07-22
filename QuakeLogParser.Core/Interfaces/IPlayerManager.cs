using QuakeLogParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeLogParser.Core.Interfaces
{
  public interface IPlayerManager
  {
    void AddPlayerIfNecessary(string player, Game game);
  }
}
