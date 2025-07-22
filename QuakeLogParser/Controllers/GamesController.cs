using Microsoft.AspNetCore.Mvc;
using QuakeLogParser.Core.Services;

namespace QuakeLogParser.API.Controllers
{
  /// <summary>
  /// Controller responsável por expor relatórios dos jogos do Quake.
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]
  public class GamesController : ControllerBase
  {
    private readonly LogParserService _logParserService;

    public GamesController(LogParserService logParserService) { 
      _logParserService = logParserService;
    }

    /// <summary>
    /// Processa o arquivo de log do Quake e retorna o relatório dos jogos.
    /// </summary>
    /// <param name="filePath">Caminho completo do arquivo games.log a ser processado.</param>
    /// <returns>Relatório dos jogos no formato especificado pelo desafio.</returns>
    [HttpGet("parse")]
    public IActionResult ParseLog(string filePath)
    {     
      try
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
        return Ok(result);
      }
      catch (FileNotFoundException ex)
      {
        return NotFound(new { message = ex.Message });
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }
  }
}
