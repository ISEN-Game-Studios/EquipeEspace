using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine;
using SpaceTeam;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private List<Item> items;

    private Queue<Message> messages;

    private ItemManager itemManager;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        itemManager = GetComponent<ItemManager>();

        PlayerIO.Authenticate(
            "equipe-espace-nmdivwrkr0qjlcp1dbska",
            "public",
            new Dictionary<string, string> {
                { "userId", "Zalphug" },
            },
            null,
            delegate (Client client)
            {
                Debug.Log("Successfully connected to Player.IO");

                client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);

                client.Multiplayer.CreateJoinRoom(
                    "room_0",
                    "SpaceShip",
                    true,
                    null,
                    null,
                    delegate (Connection connection)
                    {
                        Debug.Log("Joined Room.");

                        connection.OnMessage += OnMessage;
                    },
                    delegate (PlayerIOError error)
                    {
                        Debug.Log("Error Joining Room: " + error.ToString());
                    }
                );
            },
            delegate (PlayerIOError error)
            {
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
                    itemManager.UpdateIDs((int[])message[1]);

                    double difficulty = message.GetDouble(0);
                    Board board = new Board(difficulty);
                    items = itemManager.Generate(board);
                    foreach (var item in items)
                    {
                        GameObject gameObject = Instantiate(item.Data.Prefab, new Vector3(item.Position.x, item.Position.y) * board.Width, Quaternion.identity);
                        if (board.Binary)
                            gameObject.transform.localScale *= (board.Binary ? 3 : 4) / 12;
                        if (item.Data.Shape == Shape.Big)
                            gameObject.transform.localScale *= 2;
                    }
                    break;
            }
        }
    }

    private void OnMessage(object sender, Message message)
    {
        messages.Enqueue(message);
    }

    public void SendGoalComplete()
    {

    }
}
