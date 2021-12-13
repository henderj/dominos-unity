using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRunner : MonoBehaviour
{
    private Game _game;
    [SerializeField] private DisplayWrapperText displayWrapperText;

    private void Awake()
    {
        _game = new Game(displayWrapperText);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        _game.PlayGame();
    }
}
