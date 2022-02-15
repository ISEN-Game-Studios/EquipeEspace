using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerIOClient;

public class Board : MonoBehaviour
{
    private void Start()
	{
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
}
