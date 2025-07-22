using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using QuakeLogParser.Core.Interfaces;
using QuakeLogParser.Core.Services;

namespace QuakeLogParser.Tests.LogParserServiceTests
{
  public class LogParserService_InitializationTests
  {
    [Fact]
    public void ShouldStartNewGameWhenInitGameLineIsParsed()
    {
      // Arrange
      var mockLogReader = new Mock<ILogReader>();
      var mockEventProcessors = new List<ILogProcessor>(); // Sem event processors para este teste

      mockLogReader.Setup(reader => reader.ReadLines(It.IsAny<string>()))
                   .Returns(new List<string> { "21:42 InitGame: sv_floodProtect 1" });

      var parserService = new LogParserService(mockLogReader.Object, mockEventProcessors);

      // Act
      var games = parserService.ParseLog("path_to_log");

      // Assert
      Assert.Single(games); // Deve conter apenas um jogo
      Assert.Equal("game_1", games[0].GameId); // O jogo deve ser chamado game_1
    }
  }
}
