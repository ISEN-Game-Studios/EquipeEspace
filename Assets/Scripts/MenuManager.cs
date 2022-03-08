using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public enum Menu
    {
        Main,
        Ready,
        End,
        Settings,
        Login,
        About
    }

    [SerializeField]
    private GameObject[] menus;

    private static MenuManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ToggleAll(loginSelectables, true);

        ToggleAll(readySelectables, true);

        if (PlayerPrefs.HasKey("username"))
            username.text = PlayerPrefs.GetString("username");

        if (PlayerPrefs.HasKey("roomname"))
            roomname.text = PlayerPrefs.GetString("roomname");

        int[] stats = ClientManager.Stats;

        if (stats != null)
            ShowStats(stats);
    }

    #region Login

    [SerializeField]
    private InputField username;

    [SerializeField]
    private InputField roomname;

    [SerializeField]
    private Selectable[] loginSelectables;

    private bool ValidateLogin()
    {
        return username.text.Length >= 3
            && roomname.text.Length >= 3;
    }

    public void OnLoginChanged()
    {
        loginSelectables[0].interactable = ValidateLogin();
    }

    public void Connect()
    {
        ToggleAll(loginSelectables, false);

        if (ValidateLogin())
        {
            PlayerPrefs.SetString("username", username.text);
            PlayerPrefs.SetString("roomname", roomname.text);

            ClientManager.Connect(username.text, roomname.text);
        }
    }

    public void Count()
    {
        ClientManager.Count();
    }

    private void OnConnection()
    {

    }

    #endregion

    #region Ready

    [SerializeField]
    private Selectable[] readySelectables;

    [SerializeField]
    private Text count;

    public void ToggleReady(bool state)
    {
        ClientManager.Ready(state);

        ToggleAll(readySelectables, !state);
    }

    public static void SetCount(int ready, int total)
    {
        if (instance != null)
            instance.count.text = ready + "/" + total + " joueurs prêts";
    }

    #endregion

    #region End

    [SerializeField]
    private Text level1, level2;

    [SerializeField]
    private Text playerCount;

    [SerializeField]
    private Text time;

    [SerializeField]
    private Text actions;

    [SerializeField]
    private Text meteorites;

    [SerializeField]
    private Text holes;

    [SerializeField]
    private Text errors;

    public int[] stats;
    public void ShowStats(int[] stats)
    {
        this.stats = stats;
        ShowMenu(Menu.End);

        level1.text = Regex.Replace(level1.text, @"{.*}", stats[0].ToString());
        level2.text = Regex.Replace(level2.text, @"{.*}", stats[0].ToString());

        playerCount.text = Regex.Replace(playerCount.text, @"{.*}", stats[1].ToString());

        time.text = Regex.Replace(time.text, @"{.*}", stats[2].ToString());

        actions.text = Regex.Replace(actions.text, @"{.*}", stats[3].ToString());

        meteorites.text = Regex.Replace(meteorites.text, @"{.*}", stats[4].ToString());

        holes.text = Regex.Replace(holes.text, @"{.*}", stats[5].ToString());

        errors.text = Regex.Replace(errors.text, @"{.*}", stats[6].ToString());
    }

    #endregion

    public static void Quit()
    {
        ShowMenu(Menu.Main);

        ToggleAll(instance.loginSelectables, true);

        ToggleAll(instance.readySelectables, true);

        ClientManager.Disconnect();
    }

    public static void ShowMenu(Menu menu)
    {
        int menuIndex = (int)menu;

        for (int i = 0; i < instance.menus.Length; ++i)
            instance.menus[i].SetActive(i == menuIndex);
    }

    private static void ToggleAll(Selectable[] selectables, bool state)
    {
        foreach (Selectable selectable in selectables)
            selectable.interactable = state;
    }
}
