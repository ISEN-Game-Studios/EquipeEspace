using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Game : MonoBehaviour
{

    [SerializeField]
    private int _numberOfPlayers = 4;
    [SerializeField]
    private int _lvl = 1;
    [Tooltip("number of panel at each lvl")]
    [SerializeField]
    private int[] _numberOfPanel;

    private SpaceAction[] _tabSpaceActions; //toutes les objets de types "SpaceAction" crée
    [System.NonSerialized]
    public SpaceAction[,] tabGame; // tabGame[player, panel]
    [System.NonSerialized]
    public SpaceAction[] actionList; //list contenant les actions à faire
    // Start is called before the first frame update
    void Start()
    {
        _tabSpaceActions = GetAllInstances<SpaceAction>(); //récupération de chaque SpaceAction crée
        tabGame = randomPanel();
        ResetIsused();
        string temp = GenerateAction(1);
        Debug.Log(temp);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GenerateAction(int numberPlayer) //Generér action pour un joueur
    {
        string content = "";
        int randomPlayer;
        int randomAction;
        int randomValue;
        bool isNotused = false;

        do
        {
            randomPlayer = Random.Range(0, _numberOfPlayers - 1);
        } while (randomPlayer == numberPlayer); //récupère un joueur aléatoire différent du joueur qui va recevoir l'action

        for (int i = 0; i < tabGame.Length / _numberOfPlayers; i++)
        {
            if (!tabGame[randomPlayer, i].isUsed)
            {
                isNotused = true;
                break;
            }
        }

        if (!isNotused) // si aucune action est disponible dans ce joueur
            content = GenerateAction(numberPlayer);
        else
        {
            do
            {
                randomAction = Random.Range(0, _numberOfPanel[_lvl - 1] - 1);
            } while (tabGame[randomPlayer, randomAction].isUsed);
            
            randomValue = Random.Range(0, tabGame[randomPlayer, randomAction].maxValue);
            if (randomValue > 0 && randomValue == tabGame[randomPlayer, randomAction].value)
                randomValue -= 1;
            else if (randomValue == 0 && randomValue == tabGame[randomPlayer, randomAction].value)
                randomValue += 1;

            if (tabGame[randomPlayer, randomAction].maxValue == 1)
            {
                if (tabGame[randomPlayer, randomAction].typeOfButton == TypeOfButton.onOffButton)
                {
                    if (tabGame[randomPlayer, randomAction].value == 0)
                        content = "Activez " + tabGame[randomPlayer, randomAction].nameInstruction;
                    else
                        content = "Désactivez " + tabGame[randomPlayer, randomAction].nameInstruction;
                }
                else
                    content = "Pressez " + tabGame[randomPlayer, randomAction].nameInstruction;
            }
            else
                content += "Mettez " + tabGame[randomPlayer, randomAction].nameInstruction + " sur " + randomValue;

            tabGame[randomPlayer, randomAction].valueIntended = randomValue;
            tabGame[randomPlayer, randomAction].isUsed = true;
            actionList[numberPlayer] = (tabGame[randomPlayer, randomAction]);
        }

        return content;
    }


    private void ResetIsused()
    {
        foreach (SpaceAction action in tabGame)
        {
            action.isUsed = false;
        }
    }

    private SpaceAction[,] randomPanel()
    {
        SpaceAction[,] tabAction = new SpaceAction[_numberOfPlayers, _numberOfPanel[_lvl - 1]];
        int tempValue;
        for (int i = 0; i < _numberOfPlayers; i++)
        {
            for (int j = 0; j < _numberOfPanel[_lvl - 1]; j++)
            {
                tempValue = Random.Range(0, _tabSpaceActions.Length - 1);
                if (_tabSpaceActions[tempValue].isUsed)
                    j--;
                else
                {
                    _tabSpaceActions[tempValue].isUsed = true;
                    tabAction[i, j] = _tabSpaceActions[tempValue];

                }
            }
        }
        return tabAction;
    }

    private T[] GetAllInstances<T>() where T : ScriptableObject  // return toutes les instances crées dans le projet de <T>
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }
        return a;

    }

    public void CheckAction(SpaceAction spaceAction)
    {
        for(int i = 0; i < actionList.Length; i++)
        {
            if(actionList[i].nameInstruction == spaceAction.nameInstruction && actionList[i].valueIntended == spaceAction.value)
            {
                GenerateAction(i);
                break;
            }
        }
    }
}
