using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SpaceTeam;

using System.Linq;

public class GameManager : MonoBehaviour
{
	private Board board;
	private Dictionary<int, Item> items;

	private ItemManager itemManager;

	private static GameManager instance;

	private Dictionary<int, (int index, Coroutine timer)> goals;
	[SerializeField] private Timer timer;

	[SerializeField] private TextMeshWrapper orderText;

	[SerializeField] private ViewManager viewManager;

	[SerializeField] private Transform itemContainer;

	private Animator animator;

	private bool ready;
	public static bool Ready => instance != null && instance.ready;

    private void Awake()
    {
		instance = this;
	}

    private void Start()
	{
		animator = GetComponent<Animator>();
		itemManager = GetComponent<ItemManager>();
		goals = new Dictionary<int, (int index, Coroutine timer)>();

		animator.enabled = true;
		animator.SetTrigger("Start");
	}

	public static int[] GenerateBoard(double difficulty, int[] usedIDs)
    {
		instance.DestroyChildren();

		instance.itemManager.UpdateIDs(usedIDs);

		instance.board = new Board(difficulty);

		instance.items = instance.itemManager.Generate(instance.board);

		instance.CreateItems();

		return instance.itemManager.GetOwnedIDs();
	}

	public static string GenerateAction(int id, float delay)
    {
		var goal = instance.items[id].GetAction();

		Coroutine timer = instance.StartCoroutine(instance.StartCountdown(id, delay));

		if (instance.goals.ContainsKey(id))
			instance.goals[id] = (goal.index, timer);
		else
			instance.goals.Add(id, (goal.index, timer));


		return instance.items[id].GetInstruction(goal.index); 
	}

	private IEnumerator StartCountdown(int id, float delay)
    {
		yield return new WaitForSeconds(delay);

		if (instance.goals.ContainsKey(id))
			ClientManager.State(id, false);
    }

	public static void Completion(float completion, float fire)
    {
		instance.viewManager.Completion(completion, fire);
    }

	public static void OnStateChange(int id, int index)
	{
		instance.items[id].Current = index;

		if (instance.goals.ContainsKey(id) && instance.goals[id].index == index)
		{
			ClientManager.State(id, true);

			instance.StopCoroutine(instance.goals[id].timer);

			instance.goals.Remove(id);
			instance.timer.StopTimer();
		}
		else
			ClientManager.Error();
	}

	public static void Order(string order, float time)
    {
		instance.orderText.SetText(order);
		instance.timer.SetTimer(10f);
    }

	private void OnAnimationEnd()
    {
		animator.enabled = false;
    }

	private void OnSceneReady()
    {
		ready = true;
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

			// Saint artefact incompr�hensible des Dieux
			//position = new Vector3((position.x + 1f) / board.Width - (large ? 0f : (1f / 2f * board.Width)) - 0.5f, (position.y + 1) / board.Width - (high ? 0f : (1f / 2f * board.Width)) - 0.5f);

			GameObject gameObject = Instantiate(item.Data.Prefab, itemContainer);
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
		foreach (Transform child in itemContainer)
			Destroy(child.gameObject);
	}
}