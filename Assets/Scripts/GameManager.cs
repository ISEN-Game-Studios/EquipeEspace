using System.Collections.Generic;
using UnityEngine;
using SpaceTeam;

using System.Linq;

public class GameManager : MonoBehaviour
{
	private Board board;
	private Dictionary<int, Item> items;

	private ItemManager itemManager;

	private static GameManager instance;

	private Dictionary<int, int> goals;

	[SerializeField] private InfiniteScrollTextMesh orderText;

    private void Awake()
    {
		instance = this;
    }

    private void Start()
	{
		itemManager = GetComponent<ItemManager>();
		goals = new Dictionary<int, int>();
	}

	public List<Item> debug_items;
	public static int[] GenerateBoard(double difficulty, int[] usedIDs)
    {
		instance.DestroyChildren();

		instance.itemManager.UpdateIDs(usedIDs);

		instance.board = new Board(difficulty);

		instance.items = instance.itemManager.Generate(instance.board);
		instance.debug_items = instance.items.Values.ToList();

		instance.CreateItems();

		return instance.itemManager.GetOwnedIDs();
	}

	public static string GenerateAction(int id)
    {
		var goal = instance.items[id].GetAction();

		if (instance.goals.ContainsKey(id))
			instance.goals[id] = goal.index;
		else
			instance.goals.Add(id, goal.index);

		return instance.items[id].GetInstruction(instance.goals[id]); 
	}

	public static void OnStateChange(int id, int index)
	{
		if (!instance.goals.ContainsKey(id))
			return;

		Debug.Log(id + " " + instance.goals[id] + " " + index + " " + instance.goals[id]);

		if (index == instance.goals[id])
		{
			ClientManager.State(id);
			instance.goals.Remove(id);
		}
	}

	public static void Order(string order)
    {
		instance.orderText.SetText(order);
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

	private void DestroyChildren()
	{
		foreach (Transform child in transform)
			if (child != transform)
				Destroy(child.gameObject);
	}
}