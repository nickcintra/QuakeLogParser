using Moq;
using QuakeLogParser.Core.Interfaces;
using QuakeLogParser.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeLogParser.Tests.LogParserServiceTests
{
  public class LogParserService_PlayerManagementTests
  {
    [Fact]
    public void ShouldAddPlayersWhenKillIsProcessed()
    {
      // Arrange
      var mockPlayerManager = new Mock<IPlayerManager>();
      var mockLogReader = new Mock<ILogReader>();
      var mockEventProcessors = new List<ILogProcessor> { new KillEventProcessor(mockPlayerManager.Object, new Mock<IKillUpdater>().Object) };

      mockLogReader.Setup(reader => reader.ReadLines(It.IsAny<string>()))
                   .Returns(new List<string> {
                     @"0:00 InitGame: \sv_floodProtect\1\sv_maxPing\0\sv_minPing\0\sv_maxRate\10000",
                     "21:42 Kill: 1022 2 22: <world> killed Isgalamido by MOD_TRIGGER_HURT" 
                   });

      var parserService = new LogParserService(mockLogReader.Object, mockEventProcessors);

      // Act
      var games = parserService.ParseLog("path_to_log");

      // Assert
      mockPlayerManager.Verify(manager => manager.AddPlayerIfNecessary("Isgalamido", games[0]), Times.Once); // "Isgalamido" deve ser adicionado
    }
  }
}
