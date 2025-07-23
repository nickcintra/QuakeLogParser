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

    [Fact]
    public void DeveProcessarLogCompletoComVariosEventos()
    {
      // Arrange
      var realPlayerManager = new PlayerManager();
      var realKillUpdater = new KillUpdater();
      var mockLogReader = new Mock<ILogReader>();

      var processors = new List<ILogProcessor>
      {
          new KillEventProcessor(realPlayerManager, realKillUpdater),
          new PlayerConnectionProcessor(realPlayerManager)
      };

      var logLines = new List<string>
      {
        "20:37 InitGame: \\sv_floodProtect\\1\\sv_maxPing\\0\\sv_minPing\\0\\sv_maxRate\\10000\\sv_minRate\\0\\sv_hostname\\Code Miner Server\\g_gametype\\0\\sv_privateClients\\2\\sv_maxclients\\16\\sv_allowDownload\\0\\bot_minplayers\\0\\dmflags\\0\\fraglimit\\20\\timelimit\\15\\g_maxGameClients\\0\\capturelimit\\8\\version\\ioq3 1.36 linux-x86_64 Apr 12 2009\\protocol\\68\\mapname\\q3dm17\\gamename\\baseq3\\g_needpass\\0",
        "20:38 ClientConnect: 2",
        "20:38 ClientUserinfoChanged: 2 n\\Isgalamido\\t\\0\\model\\uriel/zael\\hmodel\\uriel/zael\\g_redteam\\\\g_blueteam\\\\c1\\5\\c2\\5\\hc\\100\\w\\0\\l\\0\\tt\\0\\tl\\0",
        "20:38 ClientBegin: 2",
        "20:40 Item: 2 weapon_rocketlauncher",
        "20:40 Item: 2 ammo_rockets",
        "20:42 Item: 2 item_armor_body",
        "20:54 Kill: 1022 2 22: <world> killed Isgalamido by MOD_TRIGGER_HURT",
        "20:59 Item: 2 weapon_rocketlauncher",
        "21:04 Item: 2 ammo_shells",
        "21:07 Kill: 1022 2 22: <world> killed Isgalamido by MOD_TRIGGER_HURT",
        "21:10 ClientDisconnect: 2",
        "21:15 ClientConnect: 2",
        "21:15 ClientUserinfoChanged: 2 n\\Isgalamido\\t\\0\\model\\uriel/zael\\hmodel\\uriel/zael\\g_redteam\\\\g_blueteam\\\\c1\\5\\c2\\5\\hc\\100\\w\\0\\l\\0\\tt\\0\\tl\\0",
        "21:17 ClientUserinfoChanged: 2 n\\Isgalamido\\t\\0\\model\\uriel/zael\\hmodel\\uriel/zael\\g_redteam\\\\g_blueteam\\\\c1\\5\\c2\\5\\hc\\100\\w\\0\\l\\0\\tt\\0\\tl\\0",
        "21:17 ClientBegin: 2",
        "21:18 Item: 2 weapon_rocketlauncher",
        "21:21 Item: 2 item_armor_body",
        "21:32 Item: 2 item_health_large",
        "21:33 Item: 2 weapon_rocketlauncher",
        "21:34 Item: 2 ammo_rockets",
        "21:42 Kill: 1022 2 22: <world> killed Isgalamido by MOD_TRIGGER_HURT",
        "21:49 Item: 2 weapon_rocketlauncher",
        "21:51 ClientConnect: 3",
        "21:51 ClientUserinfoChanged: 3 n\\Dono da Bola\\t\\0\\model\\sarge/krusade\\hmodel\\sarge/krusade\\g_redteam\\\\g_blueteam\\\\c1\\5\\c2\\5\\hc\\95\\w\\0\\l\\0\\tt\\0\\tl\\0",
        "21:53 ClientUserinfoChanged: 3 n\\Mocinha\\t\\0\\model\\sarge\\hmodel\\sarge\\g_redteam\\\\g_blueteam\\\\c1\\4\\c2\\5\\hc\\95\\w\\0\\l\\0\\tt\\0\\tl\\0",
        "21:53 ClientBegin: 3",
        "22:04 Item: 2 weapon_rocketlauncher",
        "22:04 Item: 2 ammo_rockets",
        "22:06 Kill: 2 3 7: Isgalamido killed Mocinha by MOD_ROCKET_SPLASH",
        "22:11 Item: 2 item_quad",
        "22:11 ClientDisconnect: 3",
        "22:18 Kill: 2 2 7: Isgalamido killed Isgalamido by MOD_ROCKET_SPLASH",
        "22:26 Item: 2 weapon_rocketlauncher",
        "22:27 Item: 2 ammo_rockets",
        "22:40 Kill: 2 2 7: Isgalamido killed Isgalamido by MOD_ROCKET_SPLASH",
        "22:43 Item: 2 weapon_rocketlauncher",
        "22:45 Item: 2 item_armor_body",
        "23:06 Kill: 1022 2 22: <world> killed Isgalamido by MOD_TRIGGER_HURT",
        "23:09 Item: 2 weapon_rocketlauncher",
        "23:10 Item: 2 ammo_rockets",
        "23:25 Item: 2 item_health_large",
        "23:30 Item: 2 item_health_large",
        "23:32 Item: 2 weapon_rocketlauncher",
        "23:35 Item: 2 item_armor_body",
        "23:36 Item: 2 ammo_rockets",
        "23:37 Item: 2 weapon_rocketlauncher",
        "23:40 Item: 2 item_armor_shard",
        "23:40 Item: 2 item_armor_shard",
        "23:40 Item: 2 item_armor_shard",
        "23:40 Item: 2 item_armor_combat",
        "23:43 Item: 2 weapon_rocketlauncher",
        "23:57 Item: 2 weapon_shotgun",
        "23:58 Item: 2 ammo_shells",
        "24:13 Item: 2 item_armor_shard",
        "24:13 Item: 2 item_armor_shard",
        "24:13 Item: 2 item_armor_shard",
        "24:13 Item: 2 item_armor_combat",
        "24:16 Item: 2 item_health_large",
        "24:18 Item: 2 ammo_rockets",
        "24:19 Item: 2 weapon_rocketlauncher",
        "24:22 Item: 2 item_armor_body",
        "24:24 Item: 2 ammo_rockets",
        "24:24 Item: 2 weapon_rocketlauncher",
        "24:36 Item: 2 item_health_large",
        "24:43 Item: 2 item_health_mega",
        "25:05 Kill: 1022 2 22: <world> killed Isgalamido by MOD_TRIGGER_HURT",
        "25:09 Item: 2 weapon_rocketlauncher",
        "25:09 Item: 2 ammo_rockets",
        "25:11 Item: 2 item_armor_body",
        "25:18 Kill: 1022 2 22: <world> killed Isgalamido by MOD_TRIGGER_HURT",
        "25:21 Item: 2 weapon_rocketlauncher",
        "25:22 Item: 2 ammo_rockets",
        "25:34 Item: 2 weapon_rocketlauncher",
        "25:41 Kill: 1022 2 19: <world> killed Isgalamido by MOD_FALLING",
        "25:50 Item: 2 item_armor_combat",
        "25:52 Kill: 1022 2 22: <world> killed Isgalamido by MOD_TRIGGER_HURT",
        "25:54 Item: 2 ammo_rockets",
        "25:55 Item: 2 weapon_rocketlauncher",
        "25:55 Item: 2 weapon_rocketlauncher",
        "25:59 Item: 2 item_armor_shard",
        "25:59 Item: 2 item_armor_shard",
        "26:05 Item: 2 item_armor_shard",
        "26:05 Item: 2 item_armor_shard",
        "26:05 Item: 2 item_armor_shard",
        "26:09 Item: 2 weapon_rocketlauncher ?"
      };

      mockLogReader.Setup(reader => reader.ReadLines(It.IsAny<string>())).Returns(logLines);
      var parserService = new LogParserService(mockLogReader.Object, processors);

      // Act
      var games = parserService.ParseLog("path_to_log");

      // Assert
      Assert.Single(games); // Apenas um jogo
      var game = games[0];
      Assert.Contains("Isgalamido", game.Players);
      Assert.Contains("Dono da Bola", game.Players);
      Assert.Contains("Mocinha", game.Players);
      Assert.Equal(3, game.Players.Count); // 3 jogadores
      Assert.Equal(0, game.KillsByPlayer["Dono da Bola"]); // Dono da Bola não matou ninguém
      Assert.Equal(-7, game.KillsByPlayer["Isgalamido"]); // Isgalamido morreu muito para o <world> e se matou
      Assert.Equal(0, game.KillsByPlayer["Mocinha"]); // Mocinha morreu uma vez para Isgalamido
      Assert.Equal(9, game.TotalKills); // Total de kills (soma de todas as linhas Kill)
    }
  }
}
