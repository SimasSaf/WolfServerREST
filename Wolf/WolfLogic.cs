using NLog;

public class RabbitDesc
{
	public int RabbitID { get; set; }
	public string RabbitName { get; set; }
	public int Weight { get; set; }

	public int DistanceToWolf { get; set; }

	public bool isRabbitAlive { get; set; }
}

public class WaterDesc
{
	public int WaterID { get; set; }
	public int Volume { get; set; }

	public int x { get; set; }
	public int y { get; set; }
}

public class WolfState
{
    public readonly object AccessLock = new object();

    public int LastUniqueID;

    public int x;
    public int y;

    public int WolfWeight;

    public List<RabbitDesc> RabbitsNearby = new List<RabbitDesc>();

    public List<WaterDesc> WaterNearby = new List<WaterDesc>();
}

//Where all the logic happens (initializes variables, starts background task, defines logic methods)
class WolfLogic
{
    static readonly int WOLF_MAX_WEIGHT = 30;

    static readonly int EAT_IF_LESS_DISTANCE = 30;

    static bool WOLF_IS_FULL = false;
    private Thread backgroundTaskThread;

    private Logger mLog = LogManager.GetCurrentClassLogger();

    private WolfState wolfState = new WolfState();

    Random rng = new Random();

    public WolfLogic()
    {
        backgroundTaskThread = new Thread(BackgroundTask);
        backgroundTaskThread.Start();
    }

    //Adds a rabbit to the vicinity of the wolf
    public int EnterWolfArea(RabbitDesc rabbit)
    {
        mLog.Info("A Rabbit has entered the Wolf area");

        lock(wolfState.AccessLock)
        {
            wolfState.LastUniqueID += 1;
            rabbit.RabbitID = wolfState.LastUniqueID;
            wolfState.RabbitsNearby.Add(rabbit);

            return rabbit.RabbitID;
        }
    }

    //Adds water near wolf
    public int SpawnWaterNearWolf(WaterDesc water)
    {
        mLog.Info("~~~ Spawning Water near Wolf ~~~");

        lock(wolfState.AccessLock)
        {
            wolfState.LastUniqueID += 1;
            water.WaterID = wolfState.LastUniqueID;
            wolfState.WaterNearby.Add(water);

            return water.WaterID;
        }
    }

    //Updates rabbit in the rabbit pool near wolf
    public void UpdateRabbitDistanceToWolf(RabbitDesc rabbit)
    {
        lock(wolfState.AccessLock)
        {
                mLog.Info("Updating rabbit distance " + rabbit.DistanceToWolf);
                var rabbitNearby = wolfState.RabbitsNearby.Find(rabbitNearby => rabbitNearby.RabbitID.Equals(rabbit.RabbitID));

                if(rabbitNearby != null)
                {
                    rabbitNearby.DistanceToWolf = rabbit.DistanceToWolf;
                }
        }
    }

    //Check if rabbit is alive
    public bool IsRabbitAlive(RabbitDesc rabbit)
    {
        lock(wolfState.AccessLock)
        {
            return wolfState.RabbitsNearby.Any(rabbitNearby => rabbitNearby.RabbitID == rabbit.RabbitID);
        }
    }

    public bool IsWaterAlive(WaterDesc water)
    {
        lock(wolfState.AccessLock)
        {
            return wolfState.WaterNearby.Any(waterNearby => waterNearby.WaterID == water.WaterID);
        }
    }

    private void BackgroundTask()
    {
        while(true)
        {
            lock(wolfState.AccessLock)
            {
                mLog.Info($"The wolf ({wolfState.WolfWeight}) is moving...");

                GenerateRandomWolfCoordinates();
                
                mLog.Info($"The Wolf is currently at [{wolfState.x},{wolfState.y}]");
                
                //This is just to not spam, let's imagine its doing calculations :D
                Thread.Sleep(1000);

                CheckRabbitsNearby();

                CheckWaterNearby();
            };

            if (WOLF_IS_FULL)
            {
                lock(wolfState.AccessLock)
                {
                    mLog.Info("!!! Wolf is Full");
                    wolfState.WolfWeight = 0;
                }

                Thread.Sleep(5000);
                WOLF_IS_FULL = false;
                mLog.Info("!!! Wolf is no longer Full");
            }
        }
    }

    private void CheckRabbitsNearby()
    {
        List<RabbitDesc> newRabbitsNearby = wolfState.RabbitsNearby;

        for (int i = newRabbitsNearby.Count - 1; i >= 0; i--)
        {
            if (wolfState.WolfWeight < WOLF_MAX_WEIGHT)
            {
                mLog.Info("Wolf is sniffing out the rabbits...");

                var rabbit = wolfState.RabbitsNearby[i];
                mLog.Info("Rabbit distance: " + rabbit.DistanceToWolf);

                if (rabbit.DistanceToWolf <= EAT_IF_LESS_DISTANCE)
                {
                    mLog.Info("Rabbit distance: " + rabbit.DistanceToWolf);

                    EatRabbit(rabbit);
                }
            }
            else
            {
                WOLF_IS_FULL = true;
                break;
            }
        }
    }

        private void CheckWaterNearby()
    {
        List<WaterDesc> newWaterNearby = wolfState.WaterNearby;

        for (int i = newWaterNearby.Count - 1; i >= 0; i--)
        {
            if (wolfState.WolfWeight < WOLF_MAX_WEIGHT)
            {
                mLog.Info("Wolf is looking for Water...");

                var water = wolfState.WaterNearby[i];

                if (Math.Abs(wolfState.x - water.x) <= 5 || Math.Abs(wolfState.y - water.y) <= 5)
                {
                    DrinkWater(water);
                }
            }
            else
            {
                WOLF_IS_FULL = true;
                break;
            }
        }
    }

    private void EatRabbit(RabbitDesc rabbit)
    {
        mLog.Info($"Eating {rabbit.RabbitName} The Rabbit");
        wolfState.WolfWeight += rabbit.Weight;
        wolfState.RabbitsNearby.Remove(rabbit);
    }

    private void DrinkWater(WaterDesc water)
    {
        mLog.Info("Drinking water...");
        wolfState.WolfWeight += water.Volume;
        wolfState.WaterNearby.Remove(water);
    }

    private void GenerateRandomWolfCoordinates()
    {
        wolfState.x = rng.Next(-50, 50);
        wolfState.y = rng.Next(-50, 50);
    }
}

