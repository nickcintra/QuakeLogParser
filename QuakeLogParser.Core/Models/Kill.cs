using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeLogParser.Core.Models
{
  public class Kill
  {
    public string Attacker { get; set; }
    public string Victim { get; set; }
    public string Weapon { get; set; }
  }
}
