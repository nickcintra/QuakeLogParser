using QuakeLogParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeLogParser.Core.Interfaces
{
  public interface ILogProcessor
  {
    void Processor(string logLine, Game game);
  }

  public interface ILogReader
  {
    IEnumerable<string> ReadLines(string filePath);
  }
}
