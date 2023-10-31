namespace Servers;

using Microsoft.AspNetCore.Mvc;

// Endpoints for connecting to the REST API of the server

[Route("/wolf")][ApiController]
public class WolfController : ControllerBase
{
    //singleton call to wolfLogic
    private static readonly WolfLogic wolfLogic = new WolfLogic();

    //send rabbit obj to server
    [HttpPost("/enterWolfArea")]
    public ActionResult<int> EnterWolfArea(RabbitDesc rabbit)
    {
        return wolfLogic.EnterWolfArea(rabbit);
    }

    //Send water obj to server
    [HttpPost("/spawnWaterNearWolf")]
    public ActionResult<int> SpawnWaterNearWolf(WaterDesc water)
    {
        return wolfLogic.SpawnWaterNearWolf(water); 
    }

    //updates rabbit.distanceToWolf var in rabbit
    // Patch for updateing a single var in an object
    [HttpPatch("/updateRabbitDistanceToWolf")]
    public IActionResult UpdateRabbitDistanceToWolf(RabbitDesc rabbit)
    {
        wolfLogic.UpdateRabbitDistanceToWolf(rabbit);
        return Ok();
    }

    //check if rabbit obj still exists in the server
    [HttpGet("/isRabbitAlive")]
    public ActionResult<bool> IsRabbitAlive(RabbitDesc rabbit)
    {
        return wolfLogic.IsRabbitAlive(rabbit);
    }

    //check if water obj still exists in the server
    [HttpGet("/isWaterAlive")]
    public bool IsWaterAlive(WaterDesc water)
    {
        return wolfLogic.IsWaterAlive(water);
    }
}