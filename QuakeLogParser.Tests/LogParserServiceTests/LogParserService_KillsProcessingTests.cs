using Moq;
using QuakeLogParser.Core.Interfaces;
using QuakeLogParser.Core.Services;
using QuakeLogParser.Core.Models;
using System.Collections.Generic;
using Xunit;

namespace QuakeLogParser.Tests.LogParserServiceTests
{
  public class LogParserService_KillsByPlayerProcessingTests
  {
    [Fact]
    public void ShouldProcessKillAndAddPlayers()
    {
      // Arrange
      var realPlayerManager = new PlayerManager();
      var realKillUpdater = new KillUpdater();
      var mockLogReader = new Mock<ILogReader>();

      var processors = new List<ILogProcessor>
      {
          new KillEventProcessor(realPlayerManager, realKillUpdater)
      };

      mockLogReader.Setup(reader => reader.ReadLines(It.IsAny<string>()))
                   .Returns(new List<string>
                   {
                     @"0:00 InitGame: \sv_floodProtect\1\sv_maxPing\0\sv_minPing\0\sv_maxRate\10000",
                     "21:42 Kill: 1022 2 22: <world> killed Isgalamido by MOD_TRIGGER_HURT",
                     "22:00 Kill: 1022 2 23: Isgalamido killed Dono da Bola by MOD_RAILGUN",
                     "23:15 Kill: 2 3 20: Zeh killed Isgalamido by MOD_ROCKET",
                     "23:50 Kill: 3 2 12: Dono da Bola killed Zeh by MOD_RAILGUN"
                   });

      var parserService = new LogParserService(mockLogReader.Object, processors);

      // Act
      var games = parserService.ParseLog("path_to_log");

      // Assert
      Assert.Single(games);
      Assert.Equal(4, games[0].TotalKills);

      Assert.Equal(0, games[0].KillsByPlayer["Isgalamido"]); // Isgalamido matou 1
      Assert.Equal(1, games[0].KillsByPlayer["Dono da Bola"]); // Dono da Bola matou 1
      Assert.Equal(1, games[0].KillsByPlayer["Zeh"]); // Zeh matou 1
      Assert.DoesNotContain("<world>", games[0].KillsByPlayer.Keys);

      Assert.Contains("Isgalamido", games[0].Players);
      Assert.Contains("Dono da Bola", games[0].Players);
      Assert.Contains("Zeh", games[0].Players);
    }

    [Fact]
    public void ShouldCountKillsByPlayerCorrectlyAndIncludeWorldKillsByPlayer()
    {
      // Arrange
      var realPlayerManager = new PlayerManager();
      var realKillUpdater = new KillUpdater();
      var mockLogReader = new Mock<ILogReader>();

      var processors = new List<ILogProcessor>
      {
          new KillEventProcessor(realPlayerManager, realKillUpdater)
      };

      mockLogReader.Setup(reader => reader.ReadLines(It.IsAny<string>()))
                   .Returns(new List<string>
                   {
                 @"0:00 InitGame: \sv_floodProtect\1\sv_maxPing\0\sv_minPing\0\sv_maxRate\10000",
                 "21:42 Kill: 1022 2 22: <world> killed Isgalamido by MOD_TRIGGER_HURT",
                 "22:00 Kill: 1022 2 23: Isgalamido killed Dono da Bola by MOD_RAILGUN",
                 "23:15 Kill: 2 3 20: Zeh killed Isgalamido by MOD_ROCKET",
                 "23:50 Kill: 3 2 12: Dono da Bola killed Zeh by MOD_RAILGUN"
                   });

      var parserService = new LogParserService(mockLogReader.Object, processors);

      // Act
      var games = parserService.ParseLog("path_to_log");

      // Assert
      Assert.Single(games); // Deve conter apenas um jogo

      // Verificar o total de kills
      Assert.Equal(4, games[0].TotalKills); // O total de kills deve ser 4 (incluindo o <world>)

      // Verificar os KillsByPlayer por jogador
      Assert.Equal(0, games[0].KillsByPlayer["Isgalamido"]); // Isgalamido matou 1, morreu para <world>
      Assert.Equal(1, games[0].KillsByPlayer["Dono da Bola"]); // Dono da Bola matou 1
      Assert.Equal(1, games[0].KillsByPlayer["Zeh"]); // Zeh matou 1
      Assert.DoesNotContain("<world>", games[0].KillsByPlayer.Keys); // <world> não deve aparecer no dicionário de KillsByPlayer

      // Verificar a lista de jogadores
      Assert.Contains("Isgalamido", games[0].Players);
      Assert.Contains("Dono da Bola", games[0].Players);
      Assert.Contains("Zeh", games[0].Players);
    }

    [Fact]
    public void ShouldNotIncludeWorldInKillsByPlayerDictionary()
    {
      // Arrange
      var realPlayerManager = new PlayerManager();
      var realKillUpdater = new KillUpdater();
      var mockLogReader = new Mock<ILogReader>();

      var processors = new List<ILogProcessor>
    {
        new KillEventProcessor(realPlayerManager, realKillUpdater)
    };

      mockLogReader.Setup(reader => reader.ReadLines(It.IsAny<string>()))
                   .Returns(new List<string>
                   {
                     "0:00 InitGame: \\sv_floodProtect\\1\\sv_maxPing\\0\\sv_minPing\\0\\sv_maxRate\\10000",
                     "21:42 Kill: 1022 2 22: <world> killed Isgalamido by MOD_TRIGGER_HURT"
                   });

      var parserService = new LogParserService(mockLogReader.Object, processors);

      // Act
      var games = parserService.ParseLog("path_to_log");

      // Assert
      Assert.DoesNotContain("<world>", games[0].KillsByPlayer.Keys); // <world> não deve estar no dicionário de KillsByPlayer
      Assert.DoesNotContain("<world>", games[0].Players); // <world> não deve estar na lista de players
      Assert.Contains("Isgalamido", games[0].KillsByPlayer.Keys); // A vítima deve estar no dicionário
      Assert.Equal(-1, games[0].KillsByPlayer["Isgalamido"]); // A vítima perde 1 kill
    }
  }
}
