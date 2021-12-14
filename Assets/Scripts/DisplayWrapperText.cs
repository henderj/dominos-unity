using TMPro;
using UnityEngine;

public class DisplayWrapperText : MonoBehaviour, IDisplayWrapper
{
    public TMP_Text text;

    public void DisplayGame(Game game)
    {
        var players = game.Players;
        var gameText =
            $"{players[0].Name}, {players[2].Name}: {players[0].Score} | {players[1].Name}, {players[3].Name}: {players[1].Score}";
        gameText += $"\nin play: {game.PlayedDominoesString}";

        for (var i = 0; i < players.Length; i++)
        {
            var indicator = i == game.CurrentTurnIndex ? ">" : " ";
            gameText += $"\n{indicator}{players[i].Name}: {players[i].HandString}";
        }
        text.SetText(gameText);
    }
}