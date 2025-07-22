using QuakeLogParser.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeLogParser.Core.Services
{
  public class LogReader : ILogReader
  {

    public IEnumerable<string> ReadLines(string filePath)
    {
      if (string.IsNullOrEmpty(filePath))
        throw new ArgumentNullException(nameof(filePath));

      if (!File.Exists(filePath))
        throw new FileNotFoundException("The log file was not found.", filePath);

      return File.ReadLines(filePath);
    }
  }
}
