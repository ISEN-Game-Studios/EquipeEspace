using System.Collections.Generic;
using PlayerIO.GameLibrary;
using System;

public class Player : BasePlayer
{
	public bool ready = false;
	public bool boarded = false;

	public int[] usedIDs;

	public bool isUpsideDown = false;
	public bool isShaked = false;

	public bool lastOrder = false;

	public Dictionary<int, Player> actions = new Dictionary<int, Player>();
}

public class Stats
{
	public int stage = 1;
	public int playerCount;
	public int time;
	public int actions = 0;
	public int meteorites = 0;
	public int holes = 0;
	public int errors = 0;

	public int[] Pack()
    {
		return new int[7]
		{
			stage,
			playerCount,
			time,
			actions,
			meteorites,
			holes,
			errors
		};
    }
}

[RoomType("SpaceShip")]
public class GameCode : Game<Player>
{
	public enum GroupAction
	{
		None,
		Shake,
		Reverse
	}

	private GroupAction action = GroupAction.None;

	private int current;

	private new List<Player> Players;

	private List<int> usedIDs;

	private bool isReady = false;
	private bool isRunning = false;

	private double difficulty = 0.3;
	private const double delay = 8.5;

	private double completion;

	private Timer timer;
	private Timer eventTimer;

	private Stats stats;

	private Random randomizer;
	
	private DateTime startTime;
	int actionCount;
	int errorCount;

	public override void GameStarted()
	{
		Players = new List<Player>();

		usedIDs = new List<int>();

		randomizer = new Random();
	}

	public override void GameClosed()
	{

	}

    public override bool AllowUserJoin(Player player)
    {
		return !isReady;
    }

    public override void UserJoined(Player player)
	{
		Players.Add(player);
	}

	public override void UserLeft(Player player)
	{
		if (isReady && Players.Contains(player))
		{
			foreach (Player _player in Players)
				_player.Disconnect();

			timer.Stop();
		}
	}

	private void Update()
    {
		double goalFrequency = 1.0 / (-10.0 * difficulty + 13.0);

		TimeSpan span = DateTime.Now - startTime;

		double frequency = (actionCount - errorCount) / span.TotalSeconds;

		double direction = frequency < goalFrequency ? -1.0 : 1.0;

		double fire = span.TotalSeconds / 120.0;

		completion = Math.Min(1.0, Math.Max(completion + direction * 0.0005, 0.0));

		if (Math.Abs(completion - fire) < 0.2 && randomizer.NextDouble() < 1.0 / 10.0 * goalFrequency)
		{
			Player player = Players[randomizer.Next(Players.Count)];
			player.Send("Break", player.usedIDs[randomizer.Next(player.usedIDs.Length)]);
		}

		if (action != GroupAction.None)
        {
			if ((action == GroupAction.Shake && Players.TrueForAll(p => p.isShaked)) ||
				(action == GroupAction.Reverse && Players.TrueForAll(p => p.isUpsideDown)))
            {
				action = GroupAction.None;
				eventTimer?.Stop();
				Broadcast("Resolved");
            }
        }
		else if (randomizer.NextDouble() < 1.0 / 20.0 * goalFrequency)
        {
			double a = randomizer.NextDouble();
			int b = randomizer.Next(Players.Count);
			action = a < 0.5 ? GroupAction.Shake : GroupAction.Reverse;

			Players[b].Send("GroupAction", action.ToString());

			eventTimer = AddTimer(delegate()
            {
				action = GroupAction.None;
				EndGame();
            }, (int)Math.Ceiling(1000.0 / goalFrequency));
        }

        Console.WriteLine("Fire : {0}, Completion {1}, Action {2}", fire, completion, action.ToString());

		if (fire > completion)
			EndGame();
		else if (completion >= 1.0)
			NextStage();
		else
			Broadcast("Update", completion, fire);
	}

	public override void GotMessage(Player sender, Message message)
	{
		switch (message.Type)
		{
			case "Ready":
			{
				sender.ready = message.GetBoolean(0);

				Broadcast("Count", Players.FindAll(player => player.ready).Count, Players.Count);

				if (Players.Count > 0 && Players.TrueForAll(player => player.ready))
				{
					isReady = true;

					Broadcast("Ready");

					GenerateBoards();
				}

				break;
			}

			case "Boarded":
			{
				int[] ids = ExtractMessage<int>(message);

				sender.usedIDs = ids;
				usedIDs.AddRange(ids);

				if (++current < Players.Count)
				{
					Players[current].Send(CreateMessage("Board", usedIDs, difficulty));
				}
				else
				{
					isRunning = true;

					startTime = DateTime.Now;

					completion = 0.5;

					if (stats == null)
						stats = new Stats();

					timer = AddTimer(Update, 50);

					foreach (Player player in Players)
						GenerateOrder(player);
				}

				break;
			}

			case "Order":
			{
				if (!isRunning)
					break;

				int id = message.GetInt(0);
				bool success = sender.actions[id].lastOrder;

				sender.actions[message.GetInt(0)].Send("Order", message.GetString(1), delay - 0.5, success);

				break;
			}

			case "Action":
			{
				if (!isRunning)
					break;

				int id = message.GetInt(0);

				if (id >= 0)
				{
					bool success = message.GetBoolean(1);

					usedIDs.Add(id);

					if (success)
						++actionCount;
					else
					{
						++errorCount;
						sender.Send("Break", id);
					}

					sender.actions[id].lastOrder = success;
					GenerateOrder(sender.actions[id]);
				}
				else
					++errorCount;

				break;
			}

			case "UpsideDown":
            {
				sender.isUpsideDown = message.GetBoolean(0);

				break;
			}

			case "Shaked":
			{
				sender.isShaked = message.GetBoolean(0);

				break;
			}

			case "Count":
			{
				Broadcast("Count", Players.FindAll(player => player.ready).Count, Players.Count);

				break;
			}
		}
	}

	private void NextStage()
    {
		isRunning = false;
		difficulty *= 1.128;

		foreach (Player player in Players)
		{
			player.ready = false;
			player.actions.Clear();
		}

		isReady = false;

		timer?.Stop();
		eventTimer?.Stop();

		Broadcast("Next", ++stats.stage, "fdp");

		GenerateBoards();
	}

	private void EndGame()
	{
		foreach (Player player in Players)
			player.ready = false;

		isReady = false;

		timer?.Stop();
		 
		stats.actions += actionCount;
		stats.errors += errorCount;
		stats.playerCount = Players.Count;
		stats.time = (int)Math.Ceiling((DateTime.Now - startTime).TotalMinutes);

		Broadcast(CreateMessage("End", stats.Pack()));
    }

	private Player GetPlayerById(int id)
    {
		return Players.Find(player => Array.Exists(player.usedIDs, playerID => playerID == id));
    }

	private void GenerateOrder(Player player)
	{
		int id = GetRandom(usedIDs);

		usedIDs.Remove(id);

		Player target = GetPlayerById(id);

		if (target.actions.ContainsKey(id))
			target.actions[id] = player;
		else
			target.actions.Add(id, player);

		target.Send("Action", id, delay);
    }

	private void GenerateBoards()
    {
		current = 0;
		usedIDs.Clear();

        Console.WriteLine("Generate Board of Player " + current);

		Players[0].Send(CreateMessage("Board", usedIDs, 0.3));
	}

    #region Tools

    private T GetRandom<T>(List<T> list)
	{
		return list[randomizer.Next(list.Count)];
	}

	private T GetRandom<T>(T[] list)
	{
		return list[randomizer.Next(list.Length)];
	}

	private Message CreateMessage<T>(string type, List<T> list, params object[] parameters)
    {
		return CreateMessage(type, list.ToArray(), parameters);
	}

	private Message CreateMessage<T>(string type, T[] list, params object[] parameters)
	{
		Message message = Message.Create(type);

		foreach (object parameter in parameters)
			message.Add(parameter);

		foreach (T item in list)
			message.Add(item);

		return message;
	}

	private T[] ExtractMessage<T>(Message message, uint startIndex = 0)
	{
		T[] items = new T[message.Count - startIndex];

		for (uint i = startIndex; i < message.Count; ++i)
			items[i - startIndex] = (T)message[i];

		return items;
	}

    #endregion
}