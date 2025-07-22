using Microsoft.AspNetCore.Mvc;
using QuakeLogParser.Core.Services;

namespace QuakeLogParser.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class GamesController : ControllerBase
  {
    private readonly LogParserService _logParserService;

    public GamesController(LogParserService logParserService) { 
      _logParserService = logParserService;
    }

    [HttpGet("parse")]
    public IActionResult ParseLog(string filePath)
    {
      var games = _logParserService.ParseLog(filePath);
      var result = games.ToDictionary(
                        g => g.GameId,
                        g => new {
                            total_kills = g.TotalKills,
                            players = g.Players,
                            kills = g.KillsByPlayer
                        }
                    );
      return Ok(games);
    }
  }
}
