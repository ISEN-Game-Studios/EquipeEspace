using System.Collections.Generic;
using PlayerIO.GameLibrary;
using System;

public class Player : BasePlayer
{
	public bool ready = false;
	public bool boarded = false;

	public int[] usedIDs;

	public Dictionary<int, Player> actions = new Dictionary<int, Player>();
}

[RoomType("SpaceShip")]
public class GameCode : Game<Player>
{
	private int current;

	private new List<Player> Players;

	private List<int> usedIDs;

	private bool isReady = false;
	private bool isRunning = false;

	private const double difficulty = 0.3;
	
	DateTime startTime;
	int actionCount;

	public override void GameStarted()
	{
		Players = new List<Player>();

		usedIDs = new List<int>();

		AddTimer(Update, 50);
	}

	public override void GameClosed()
	{

	}

	public override void UserJoined(Player player)
	{
		if (isReady)
			player.Disconnect();
		else
		{
			Players.Add(player);
		}
	}

	public override void UserLeft(Player player)
	{
		if (isReady && Players.Contains(player))
		{
			foreach (Player _player in Players)
				_player.Disconnect();
		}
	}

	private void Update()
    {
		double goalFrequency = 1.0 / (-10.0 * difficulty + 13.0);

		TimeSpan span = DateTime.Now - startTime;

		double frequency = actionCount / span.TotalSeconds;

        Console.WriteLine("Goal : " + goalFrequency + " / Current : " + frequency);
    }

	public override void GotMessage(Player sender, Message message)
	{
		switch (message.Type)
		{
			case "Ready":
				sender.ready = message.GetBoolean(0);

				if (Players.Count > 0 && Players.TrueForAll(player => player.ready))
                {
					isReady = true;

					GenerateBoards();
				}

				break;

			case "Boarded":
				int[] ids = ExtractMessage<int>(message);

				sender.usedIDs = ids;
				usedIDs.AddRange(ids);

				if (++current < Players.Count)
				{
					Players[current].Send(CreateMessage("Board", usedIDs, difficulty));

					Console.WriteLine("Generate Board of Player " + current);
				}
				else
				{
					isRunning = true;

					startTime = DateTime.Now;

					foreach (Player player in Players)
						GenerateOrder(player);

                    Console.WriteLine("Game is running");
				}

				break;

			case "Order":
				sender.actions[message.GetInt(0)].Send("Order", message.GetString(1));

				break;

			case "Action":
				usedIDs.Add(message.GetInt(0));

				++actionCount;

				GenerateOrder(sender.actions[message.GetInt(0)]);

				break;

			case "Count":
				sender.Send("Count", PlayerCount);

				break;
		}
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

		Console.WriteLine(player.ConnectUserId + " will order " + target.ConnectUserId);

		target.Send("Action", id);
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
		return list[new Random().Next(list.Count)];
	}

	private T GetRandom<T>(T[] list)
	{
		return list[new Random().Next(list.Length)];
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
		T[] items = new T[message.Count];

		for (uint i = startIndex; i < message.Count; ++i)
			items[i] = (T)message[i];

		return items;
	}

    #endregion
}