using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class InertiaController
{
    private readonly InertiaService _service;

    public InertiaController(InertiaService service)
    {
        _service = service;
    }

    [HttpPost]
    public ActionResult<StartResponse> Start([FromBody] StartRequest request)
    {
        return _service.Start(request);
    }

    [HttpPut]
    [Route("{playerColor}/{directionCode}")]
    public ActionResult<UpdateResponse> Update(string playerColor, 
        string directionCode)
    {
        return _service.Update(playerColor, directionCode);
    }

    [HttpPost]
    [Route("results/save")]
    public ActionResult SaveResults()
    {
        _service.SaveResults();
        return new OkResult();
    }
    
    [HttpGet]
    [Route("leaderboard/{count:int}")]
    public ActionResult<LeaderboardEntry[]> GetTopResults(int count)
    {
        return _service.GetTopResults(count);
    }
}