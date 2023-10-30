namespace Servers;

using Microsoft.AspNetCore.Mvc;

[Route("/wolf")][ApiController]
public class WolfController : ControllerBase
{
    //singleton call to wolfLogic
    private static readonly WolfLogic wolfLogic = new WolfLogic();

    [HttpPost("/enterWolfArea")]
    public ActionResult<int> EnterWolfArea(RabbitDesc rabbit)
    {
        return wolfLogic.EnterWolfArea(rabbit);
    }

    [HttpPost("/spawnWaterNearWolf")]
    public ActionResult<int> SpawnWaterNearWolf(WaterDesc water)
    {
        return wolfLogic.SpawnWaterNearWolf(water); 
    }

    // Patch for updateing a single var in an object
    [HttpPatch("/updateRabbitDistanceToWolf")]
    public IActionResult UpdateRabbitDistanceToWolf(RabbitDesc rabbit)
    {
        wolfLogic.UpdateRabbitDistanceToWolf(rabbit);
        return Ok();
    }

    [HttpGet("/isRabbitAlive")]
    public ActionResult<bool> IsRabbitAlive(RabbitDesc rabbit)
    {
        return wolfLogic.IsRabbitAlive(rabbit);
    }

    [HttpGet("/isWaterAlive")]
    public bool IsWaterAlive(WaterDesc water)
    {
        return wolfLogic.IsWaterAlive(water);
    }
}