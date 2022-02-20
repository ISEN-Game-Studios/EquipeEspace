﻿using System.Collections.Generic;
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

	public override void GameStarted()
	{
		Players = new List<Player>();

		usedIDs = new List<int>();
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

			if (Players.Count == 1)
				player.ready = true;
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

	public override void GotMessage(Player sender, Message message)
	{
		switch (message.Type)
		{
			case "Ready":
				sender.ready = message.GetBoolean(0);

				if (Players.Count > 1 && Players.TrueForAll(player => player.ready))
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
					Players[current].Send(CreateMessage("Board", usedIDs, 0.3));

					Console.WriteLine("Generate Board of Player " + current);
				}
				else
				{
					isRunning = true;

					foreach (Player player in Players)
					{
						GenerateOrder(player);
					}

                    Console.WriteLine("Game is running");
				}

				break;

			case "Order":
				sender.actions[message.GetInt(0)].Send("Order", message.GetString(1));

				break;

			case "Action":
				usedIDs.Add(message.GetInt(0));

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