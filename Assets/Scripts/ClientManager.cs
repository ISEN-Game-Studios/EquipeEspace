using UnityEngine.SceneManagement;
using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine;
using System;

public class ClientManager : MonoBehaviour
{
    private static ClientManager instance;

	private Connection server;

	private Queue<Message> messages;
	private Queue<Message> waitings;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);
	}

	private void Start()
    {
		messages = new Queue<Message>();
		waitings = new Queue<Message>();
	}

	public static void Connect(string username, string roomname)
	{
		if (instance.server != null)
			return;

		PlayerIO.Authenticate(
			"equipe-espace-nmdivwrkr0qjlcp1dbska",
			"public",
			new Dictionary<string, string> {
				{ "userId", username },
			},
			null,
			delegate (Client client) {
				Debug.Log("Successfully connected to Player.IO");

				client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);

				client.Multiplayer.CreateJoinRoom(
					roomname,
					"SpaceShip",
					true,
					null,
					null,
					delegate (Connection connection) {
						Debug.Log("Joined Room.");

						instance.server = connection;

						instance.server.OnMessage += instance.OnMessage;
						instance.server.OnDisconnect += instance.OnDisconnect;

						instance.server.Send("Count");

						MenuManager.ShowMenu(MenuManager.Menu.Ready);
					},
					delegate (PlayerIOError error) {
						Debug.Log("Error Joining Room: " + error.ToString());

						MenuManager.Quit();
					}
				);
			},
			delegate (PlayerIOError error) {
				Debug.Log("Error connecting: " + error.ToString());

				MenuManager.Quit();
			}
		);
	}

	private void FixedUpdate()
	{
		while (waitings.Count > 0)
			messages.Enqueue(waitings.Dequeue());

		while (messages.Count > 0)
		{
			Message message = messages.Dequeue();

			switch (message.Type)
			{
				case "Board":
					if (SceneManager.GetActiveScene().buildIndex == 1)
					{
						double difficulty = message.GetDouble(0);

						int[] usedIDs = ExtractMessage<int>(message, 1);

						usedIDs = GameManager.GenerateBoard(difficulty, usedIDs);

						instance.server.Send(CreateMessage("Boarded", usedIDs));
					}
					else
						waitings.Enqueue(message);

					break;

				case "Action":
					int id = message.GetInt(0);

					string instruction = GameManager.GenerateAction(id);

					server.Send("Order", id, instruction);

					break;

				case "Order":
					string order = message.GetString(0);

					GameManager.Order(order);

					break;

				case "Ready":
					SceneManager.LoadScene(1);

					break;

				case "Count":
					int ready = message.GetInt(0);
					int total = message.GetInt(1);

					MenuManager.SetCount(ready, total);

					break;

				case "Update":
					GameManager.Completion((float)message.GetDouble(0), (float)message.GetDouble(1));

					break;
			}
		}
	}

	public static void Ready(bool state)
	{
		if (instance.server != null)
			instance.server.Send("Ready", state);
	}

	public static void State(int id)
    {
		if (instance.server != null)
			instance.server.Send("Action", id);
	}

	public static void Error()
	{
		State(-1);
	}

	public static void Disconnect()
    {
		if (instance.server != null)
			instance.server.Disconnect();

		instance.server = null;

		SceneManager.LoadScene(0);

    }

	private void OnMessage(object sender, Message message)
	{
		messages.Enqueue(message);
	}

	private void OnDisconnect(object sender, string reason)
	{
		Disconnect();

		Debug.Log("Deconnected : " + reason);
	}

	#region Tools

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

	#endregion
}
