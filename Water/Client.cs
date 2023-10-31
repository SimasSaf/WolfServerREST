namespace Clients;

using System.Net.Http;

using NLog;

using Services;

class Client
{
    private readonly WaterDesc water = new WaterDesc();

    private readonly Random rng = new Random();

    Logger mLog = LogManager.GetCurrentClassLogger();

	private void ConfigureLogging()
	{
		var config = new NLog.Config.LoggingConfiguration();

		var console =
			new NLog.Targets.ConsoleTarget("console")
			{
				Layout = @"${date:format=HH\:mm\:ss}|${level}| ${message} ${exception}"
			};
		config.AddTarget(console);
		config.AddRuleForAllLevels(console);

		LogManager.Configuration = config;
	}

    private void Run()
    {
        ConfigureLogging();

        while(true)
        {
            try
            {
                var wolf = new WolfClient("http://127.0.0.1:5000", new HttpClient());

                InitializeWater(wolf);

                while(true)
                {
                    //checks if water has not been drunk at the wolf end
                    //if it has, sleep and spawn again
                    while(wolf.IsWaterAlive(water))
                    {
                        mLog.Info("~~~~~~~~~~~~~~~~~");
                        //Checks every 0.5s
                        Thread.Sleep(500);
                    }

                    mLog.Info("The water is empty");
                    Thread.Sleep(5000);
                    InitializeWater(wolf);
                }

            }
            catch (Exception err)
            {
                mLog.Error("Error has occured...", err);
                Thread.Sleep(3000);
            }
        }
    }

    static void Main(string[] args)
	{
		var self = new Client();
		self.Run();
	}

    //Sets water volume, coordinates and then sends to wolf server
    private void InitializeWater(WolfClient wolf)
    {
        water.Volume = rng.Next(0, 10);
        water.X = rng.Next(-50, 50);
        water.Y = rng.Next(-50, 50);
        water.WaterID = wolf.SpawnWaterNearWolf(water);
    }
}