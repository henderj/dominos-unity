using UnityEngine;

public class GameRunner : MonoBehaviour
{
    [SerializeField] private DisplayWrapperText displayWrapperText = null;
    private Game _game;

    private void Awake()
    {
        _game = new Game(displayWrapperText);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) RestartGame();
    }

    private void RestartGame()
    {
        _game.PlayGame();
    }
}