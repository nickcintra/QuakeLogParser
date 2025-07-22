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

    private readonly string _filePath;

    public LogReader(string filePath)
    {
      _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public IEnumerable<string> ReadLines(string filePath)
    {
      if (string.IsNullOrEmpty(_filePath))
        throw new InvalidOperationException("File path must be provided.");

      if (!File.Exists(_filePath))
        throw new FileNotFoundException("The log file was not found.", _filePath);

      return File.ReadLines(_filePath);
    }
  }
}
