using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine;
using SpaceTeam;

public class GameManager : MonoBehaviour
{
	private Board board;
	private Dictionary<int, Item> items;

	private Queue<Message> messages;

	private ItemManager itemManager;

	private Connection server;

	private static GameManager instance;

	private (int id, int index) goal;

	[SerializeField] private InfiniteScrollTextMesh orderText;

    private void Awake()
    {
		instance = this;
    }

    private void Start()
	{
		messages = new Queue<Message>();
		itemManager = GetComponent<ItemManager>();
		int id = Random.Range(0, 100);
		Debug.Log("Je suis " + id);

        PlayerIO.Authenticate(
			"equipe-espace-nmdivwrkr0qjlcp1dbska",
			"public",
			new Dictionary<string, string> {
				{ "userId", "Zalphug " + id.ToString() },
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

				    board = new Board(difficulty);

					items = itemManager.Generate(board);

					CreateItems();

					server.Send(CreateMessage("Boarded", itemManager.GetOwnedIDs()));

					break;

				case "Action":
					int id = message.GetInt(0);

					goal = items[id].GetAction();

					server.Send("Order", id, items[id].GetInstruction(goal.index));

					break;

				case "Order":
					string order = message.GetString(0);

					orderText.SetText(order);

					break;
			}
        }
    }

	private void CreateItems()
    {
		foreach (var item in items.Values)
		{
			float width = (item.Data.Shape == Shape.Horizontal || item.Data.Shape == Shape.Big) ? 2f : 1f;
			float height = (item.Data.Shape == Shape.Vertical || item.Data.Shape == Shape.Big) ? 2f : 1f;

			// Base Position
			Vector3 position = new Vector3(item.Position.x, item.Position.y);

			// Grid Mapped Position
			position = (position - Vector3.one * board.Width / 2f) / board.Width;

			// Compensate Centered Anchor
			position += new Vector3(width, height) / (2f * board.Width);

			// Saint artefact incompréhensible des Dieux
			//position = new Vector3((position.x + 1f) / board.Width - (large ? 0f : (1f / 2f * board.Width)) - 0.5f, (position.y + 1) / board.Width - (high ? 0f : (1f / 2f * board.Width)) - 0.5f);

			GameObject gameObject = Instantiate(item.Data.Prefab, transform);
			gameObject.transform.localPosition = position;
			gameObject.name += item.Position.ToString();
			gameObject.GetComponent<Interactable>().itemData = item.Data;

			gameObject.transform.localScale *= (board.Binary ? 3f : 4f) / 12f;

			if (item.Data.Shape == Shape.Big)
				gameObject.transform.localScale *= 2f;
		}
	}

    private void OnMessage(object sender, Message message)
    {
		messages.Enqueue(message);
	}

	public static void OnStateChange(int id, int index)
	{
		Debug.Log(id + " " + instance.goal.id + " " + index + " " + instance.goal.index);

		if (id == instance.goal.id && index == instance.goal.index)
			instance.server.Send("Action", id);
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