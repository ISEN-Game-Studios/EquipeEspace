using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SpaceTeam;
using TMPro;

public class GameManager : MonoBehaviour
{
	private Board board;

	private ItemManager itemManager;

	private static GameManager instance;

	private Queue<string> orders;
	private Dictionary<int, (int index, Coroutine timer)> goals;
	private Dictionary<int, (Item item, Interactable instance)> interactables;

	[SerializeField]
	private Timer timer;

	[SerializeField]
	private TextMeshWrapper orderText;

	[SerializeField]
	private TextMesh stageText;

	[SerializeField]
	private TextMeshTyper messageText;

	[SerializeField]
	private ViewManager viewManager;

	[SerializeField]
	private GameObject transition;

	[SerializeField]
	private Transform itemContainer;

	[SerializeField]
	private EffectOrder effectOrder;

	[SerializeField]
	private ObstacleOrder obstacleOrder;

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
		orders = new Queue<string>();
		goals = new Dictionary<int, (int index, Coroutine timer)>();
		animator.enabled = true;
		transition.SetActive(false);
		animator.SetTrigger("Start");
	}

	public static int[] GenerateBoard(double difficulty, int[] usedIDs)
    {
		instance.DestroyChildren();

		instance.itemManager.UpdateIDs(usedIDs);

		instance.board = new Board(difficulty);

		instance.interactables = new Dictionary<int, (Item item, Interactable instance)>();

		instance.CreateItems(instance.itemManager.Generate(instance.board));

		return instance.itemManager.GetOwnedIDs();
	}

	public static string GenerateAction(int id, float delay)
    {
		var goal = instance.interactables[id].item.GetAction();

		Coroutine timer = instance.StartCoroutine(instance.StartCountdown(id, delay));

		if (instance.goals.ContainsKey(id))
			instance.goals[id] = (goal.index, timer);
		else
			instance.goals.Add(id, (goal.index, timer));

		return instance.interactables[id].item.GetInstruction(goal.index); 
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
		instance.interactables[id].item.Current = index;

		if (instance.goals.ContainsKey(id) && instance.goals[id].index == index)
		{
			ClientManager.State(id, true);

			instance.StopCoroutine(instance.goals[id].timer);

			instance.goals.Remove(id);
		}
		else
			ClientManager.Error();
	}

	public static void Order(string order, float time, bool succed)
    {
		if (instance.orders.Count > 0)
        {
			instance.timer.StopTimer();
			instance.effectOrder.GetOrderState(succed, time, order);
			instance.orders.Dequeue();
		}
		else
			ShowOrder(order, time);

		instance.orders.Enqueue(order);
	}

	public static void ObstacleOrder(string action)
    {
		instance.obstacleOrder.GetOrder(action);
		if (SystemInfo.deviceType == DeviceType.Desktop)
        {
			ClientManager.UpsideDownChange(true);
			ClientManager.ShakedChange(true);
		}
    }

	public static void ObstacleResolved()
    {
		instance.obstacleOrder.Succes();
    }

	public static void ShowOrder(string order, float timer)
    {
		instance.orderText.SetText(order);
		instance.timer.SetTimer(timer);
	}

	public static void EndStage(int stage, string instruction)
    {
		instance.stageText.text = stage.ToString();
		instance.messageText.SetText(instruction);

		instance.StopAllCoroutines();

		instance.viewManager.Completion(0.5f, 0f);

		instance.orders.Clear();
		instance.goals.Clear();

		foreach (Transform child in instance.itemContainer)
			Destroy(child.gameObject);

		instance.ready = false;
		instance.transition.SetActive(false);

		instance.animator.enabled = true;
		instance.animator.SetTrigger("Start");

		instance.obstacleOrder.Cancel();
    }

	private void OnAnimationEnd()
    {
		animator.enabled = false;
		transition.SetActive(false);
	}

	private void EndGame()
    {

    }

	private void OnSceneReady()
    {
		ready = true;
		transition.SetActive(true);
	}

	private void CreateItems(List<Item> items)
    {
		foreach (var item in items)
		{
			float width = (item.Data.Shape == Shape.Horizontal || item.Data.Shape == Shape.Big) ? 2f : 1f;
			float height = (item.Data.Shape == Shape.Vertical || item.Data.Shape == Shape.Big) ? 2f : 1f;

			// Base Position
			Vector3 position = new Vector3(item.Position.x, item.Position.y);

			// Grid Mapped Position
			position = (position - Vector3.one * board.Width / 2f) / board.Width;

			// Compensate Centered Anchor
			position += new Vector3(width, height) / (2f * board.Width);

			// Saint artefact incomprehensible des Dieux
			//position = new Vector3((position.x + 1f) / board.Width - (large ? 0f : (1f / 2f * board.Width)) - 0.5f, (position.y + 1) / board.Width - (high ? 0f : (1f / 2f * board.Width)) - 0.5f);

			GameObject gameObject = Instantiate(item.Data.Prefab, itemContainer);
			gameObject.transform.localPosition = position;
			gameObject.name += item.Position.ToString();
			Interactable interactable = gameObject.GetComponent<Interactable>();
			interactable.itemData = item.Data;

			gameObject.transform.localScale *= (board.Binary ? 3f : 4f) / 12f;

			if (item.Data.Shape == Shape.Big)
				gameObject.transform.localScale *= 2f;

			interactables.Add(item.Data.ID, (item, interactable));
		}
	}

	public static void Break(int id)
    {
		instance.interactables[id].instance.Break();
    }

	private void DestroyChildren()
	{
		foreach (Transform child in itemContainer)
			Destroy(child.gameObject);
	}
}