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
        instance.count.text = ready + "/" + total + " joueurs prêts";
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
