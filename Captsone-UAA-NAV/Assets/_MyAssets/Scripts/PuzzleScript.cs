using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleScript : MonoBehaviour
{
    public void ReplayPuzzle()
    {
        GameObject.FindWithTag("PageManager").GetComponent<PageManager>().RenewPuzzleGame();
    }

    public void QuitGame()
    {
        GameObject.FindWithTag("PageManager").GetComponent<PageManager>().QuitPuzzleGame();
    }
}
