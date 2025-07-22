using QuakeLogParser.Core.Interfaces;
using QuakeLogParser.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeLogParser.Core.Services
{
  public class LogParserService
  {
    private readonly ILogReader _logReader;
    private readonly IEnumerable<ILogProcessor> _eventProcessors;

    public LogParserService(ILogReader logReader, IEnumerable<ILogProcessor> eventProcessors)
    {
      _logReader = logReader ?? throw new ArgumentNullException(nameof(logReader));
      _eventProcessors = eventProcessors ?? throw new ArgumentNullException(nameof(eventProcessors));
    }

    public List<Game> ParseLog(string filePath)
    {

      if(string.IsNullOrEmpty(filePath))
        throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

      var games = new List<Game>();
      Game currentGame = null;
      int gameCounter = 1;

      try
      {
        foreach (var line in _logReader.ReadLines(filePath))
        {
          // Inicia um novo jogo quando encontrar "InitGame"
          if (IsInitGame(line))
          {
            currentGame = CreateNewGame(gameCounter++);
            games.Add(currentGame);
            continue; // Não processa a linha de InitGame como evento
          }

          if (currentGame != null)
            ProcessLineWithProcessors(currentGame, line);

        }
      }
      catch (IOException ex)
      {
        // Log ou rethrow da exceção
        throw new ApplicationException("An error occurred while reading the log file.", ex);
      }

      // Garante que todos os jogadores estejam no dicionário de kills
      EnsureAllPlayersInKills(games);

      return games;
    }

    // Métodos auxiliares privados:
    private bool IsInitGame(string line) => line.Contains("InitGame");
    private Game CreateNewGame(int gameCounter) => new Game { GameId = $"game_{gameCounter}" };

    private void ProcessLineWithProcessors(Game game, string line)
    {
      foreach (var processor in _eventProcessors)
      {
        game.ApplyLogProcessor(line, processor);
      }
    }

    private void EnsureAllPlayersInKills(List<Game> games)
    {
      foreach (var game in games)
      {
        foreach (var player in game.Players)
        {
          if (!game.KillsByPlayer.ContainsKey(player))
            game.KillsByPlayer[player] = 0;
        }
      }
    }
  }

  
  public static class GameExtensions
  {
    public static void ApplyLogProcessor(this Game game, string line, ILogProcessor processor)
    {
      if (game != null && processor != null)
        processor.Processor(line, game);
    }
  }
}
