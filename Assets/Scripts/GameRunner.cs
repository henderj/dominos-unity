using UnityEngine;

public class GameRunner : MonoBehaviour
{
    [SerializeField] private DisplayWrapperText displayWrapperText = null;
    private Game _game;
    private bool _keepRunning;

    private void Awake()
    {
        _game = new Game(displayWrapperText);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) RestartGame();

        if (_keepRunning == false) return;
        _keepRunning = _game.Step();
    }

    private void RestartGame()
    {
        _game.NewGame();
        _game.NewRound();
        _keepRunning = true;
    }
}