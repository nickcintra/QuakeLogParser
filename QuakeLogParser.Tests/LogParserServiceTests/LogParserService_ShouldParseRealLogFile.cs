using QuakeLogParser.Core.Interfaces;
using QuakeLogParser.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeLogParser.Tests.LogParserServiceTests
{
  public class LogParserService_ShouldParseRealLogFile
  {
    [Fact]
    public void ShouldParseRealLogFile()
    {

      var logFilePath = Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\games.log");
      logFilePath = Path.GetFullPath(logFilePath);

      var realPlayerManager = new PlayerManager();
      var realKillUpdater = new KillUpdater();
      var processors = new List<ILogProcessor>
    {
        new KillEventProcessor(realPlayerManager, realKillUpdater)
    };
      var logReader = new LogReader(); // Implementação real que lê do disco
      var parserService = new LogParserService(logReader, processors);

      var games = parserService.ParseLog(logFilePath);

      Assert.NotEmpty(games); // Ou outros asserts conforme esperado
    }
  }
}
