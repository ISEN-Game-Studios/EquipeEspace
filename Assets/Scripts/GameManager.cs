using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine;
using SpaceTeam;

public class GameManager : MonoBehaviour
{
	public List<Item> items;

	private Queue<Message> messages;

	private ItemManager itemManager;

	private Connection server;

    private void Start()
	{
		messages = new Queue<Message>();
		itemManager = GetComponent<ItemManager>();

        PlayerIO.Authenticate(
			"equipe-espace-nmdivwrkr0qjlcp1dbska",
			"public",
			new Dictionary<string, string> {
				{ "userId", "Zalphug" },
			},
			null,
			delegate (Client client) {
				Debug.Log("Successfully connected to Player.IO");

				client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);

				client.Multiplayer.CreateJoinRoom(
					"room_0",
					"SpaceShip",
					true,
					null,
					null,
					delegate (Connection connection) {
						Debug.Log("Joined Room.");

						server = connection;

						server.Send("Ready", true);
						server.OnMessage += OnMessage;
					},
					delegate (PlayerIOError error) {
						Debug.Log("Error Joining Room: " + error.ToString());
					}
				);
			},
			delegate (PlayerIOError error) {
				Debug.Log("Error connecting: " + error.ToString());
			}
		);
	}

    private void FixedUpdate()
    {
        while (messages.Count > 0)
        {
			Message message = messages.Dequeue();

			switch (message.Type)
            {
				case "Board":
					int[] usedIDs = ExtractMessage<int>(message, 1);

					itemManager.UpdateIDs(usedIDs);

					double difficulty = message.GetDouble(0);

					items = itemManager.Generate(new Board(difficulty));

					server.Send(CreateMessage("Boarded", itemManager.GetOwnedIDs()));

					break;
			}
        }
    }

    private void OnMessage(object sender, Message message)
    {
		messages.Enqueue(message);
	}

	private Message CreateMessage<T>(string type, List<T> list)
	{
		Message message = Message.Create(type);

		foreach (T item in list)
			message.Add(item);

		return message;
	}

	private Message CreateMessage<T>(string type, T[] list)
	{
		Message message = Message.Create(type);

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
}
