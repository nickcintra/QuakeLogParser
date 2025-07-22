using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeLogParser.Core.Models
{
  public class Game
  {
    public string GameId { get; set; }
    public int TotalKills { get; set; }
    public List<string> Players { get; set; } = new List<string>();
    public Dictionary<string, int> KillsByPlayer { get; set; } = new Dictionary<string, int>();
  }
}
