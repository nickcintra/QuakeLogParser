using QuakeLogParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeLogParser.Core.Interfaces
{
  public interface IKillUpdater
  {
    void UpdateKills(string attacker, string victim, Game game);
  }
}
