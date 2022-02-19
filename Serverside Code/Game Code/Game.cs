using System.Collections.Generic;
using PlayerIO.GameLibrary;
using System;

public class Player : BasePlayer
{
	public bool ready = false;
	public bool boarded = false;

	public int[] usedIDs;
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

	public override void GotMessage(Player player, Message message)
	{
		switch (message.Type)
		{
			case "Ready":
				player.ready = message.GetBoolean(0);

				if (Players.TrueForAll(waiter => waiter.ready))
                {
					isReady = true;

					GenerateBoards();
				}

				break;

			case "Boarded":
				int[] ids = (int[])message[0];

				player.usedIDs = ids;
				usedIDs.AddRange(ids);

				if (++current < Players.Count)
					Players[current].Send("Board", usedIDs.ToArray());
				else
					isRunning = true;

				break;

			case "Count":
				player.Send("Count", PlayerCount);

				break;
		}
	}

	private void GenerateBoards()
    {
		current = 0;
		usedIDs.Clear();

		Players[0].Send("Board", usedIDs);
    }
}