using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Domino
{
    public int LeftSide;
    public int RightSide;

    public Domino(int leftSide, int rightSide)
    {
        this.LeftSide = leftSide;
        this.RightSide = rightSide;
    }
    
    public static Domino[] GetAllDominos()
    {
        return new[]
        {
            new Domino(0, 0),
            new Domino(0,1),
            new Domino(0,2),
            new Domino(0,3),
            new Domino(0,4),
            new Domino(0,5),
            new Domino(0,6),
            new Domino(1,1),
            new Domino(1,2),
            new Domino(1,3),
            new Domino(1,4),
            new Domino(1,5),
            new Domino(1,6),
            new Domino(2,2),
            new Domino(2,3),
            new Domino(2,4),
            new Domino(2,5),
            new Domino(2,6),
            new Domino(3,3),
            new Domino(3,4),
            new Domino(3,5),
            new Domino(3,6),
            new Domino(4,4),
            new Domino(4,5),
            new Domino(4,6),
            new Domino(5,5),
            new Domino(5,6),
            new Domino(6,6),
        };
    }
}

public class Player
{
    public void Reset()
    {
        
    }
}

public class Game : MonoBehaviour
{
    

    private int _numPlayers = 4;
    private int _terminatingScore = 200;
    private int _gameNum = 1;
    private int _roundNum = 1;
    private List<int> _playedDominos = new List<int>(60);
    private List<Domino> _roundHistory = new List<Domino>(60);
    private Player[] _players = new Player[4];
    

        // self._display_wrapper = display_wrapper

    // def _reset(self):
    //     self.played_dominos = []
    //     self.round_history = []
    //     for p in self.players:
    //         p.reset()

    private void Reset()
    {
        _playedDominos.Clear();
        _roundHistory.Clear();
        foreach (var p in _players)
        {
            p.Reset();
        }
    }


    // def new_game(self, _players=None, shuffle_players=False):
    //     if _players == None:
    //         _players = [
    //             Player(f'P{i+1}', ActionChooser_Random())
    //             for i in range(self.num_players)
    //         ]
    //     if shuffle_players:
    //         random.shuffle(_players)
    //     self.players = _players
    //     self._reset()
    //     self.winner = self.players[0]
    //     self.first_game_round_step = True
    //
    // def new_round(self):
    //     self._reset()
    //     pool = list(self.DOMINOS)
    //     random.shuffle(pool)
    //     for i in range(len(self.players)):
    //         self.players[i].give_dominos(pool[i::4])
    //     if self.first_game_round_step:
    //         for p in self.players:
    //             if p.has_double_six():
    //                 self.winner = p
    //                 break
    //     self.next_player_index = self.players.index(self.winner)
    //
    // def step(self):
    //     next_player = self.players[self.next_player_index]
    //     did_play = False
    //     domino_played = None
    //     if self.first_game_round_step:
    //         self.played_dominos = next_player.play_double_six(
    //             self.played_dominos)
    //         did_play = True
    //         domino_played = (6,6)
    //         self.first_game_round_step = False
    //     else:
    //         did_play, self.played_dominos, domino_played = next_player.take_turn(self.played_dominos)
    //     self.round_history.append(TurnDataPoint(next_player._name, did_play, domino_played))
    //
    //     self.check_for_bonus_points()
    //
    //     if next_player.is_hand_empty():
    //         self.winner = next_player
    //         return False
    //     if self.is_locked():
    //         p_current = next_player
    //         p_next = self.players[self.get_next_player_index()]
    //         self.winner = p_current if p_current.get_points(
    //         ) < p_next.get_points() else p_next
    //         return False
    //     self.next_player_index = self.get_next_player_index()
    //     return True
    //
    // def check_for_bonus_points(self):
    //   bonus_points = 25
    //   if len(self.round_history) < 2: return
    //
    //   last_move = self.round_history[-1]
    //   if last_move.did_play: return
    //   last_player = self.get_player_by_name(last_move.player_name)
    //   if len(self.round_history) == 2:
    //     self.give_points_to_team(last_player, bonus_points)
    //     return
    //   
    //   if len(self.round_history) < 4: return
    //   last_four = self.round_history[-4:]
    //   if last_four[0].did_play and not last_four[1].did_play and not last_four[2].did_play and not last_four[3].did_play:
    //     player = self.get_player_by_name(last_four[0].player_name)
    //     self.give_points_to_team(player, bonus_points)
    //     return
    //
    //   if not last_player.is_hand_empty(): return
    //   last_domino = last_move.domino_played
    //   dominos_played_wout_last = self.played_dominos.remove(last_domino)
    //   head = dominos_played_wout_last[0][0]
    //   tail = dominos_played_wout_last[-1][1]
    //   if (last_domino[0] == head and last_domino[1] == tail) or (last_domino[1] == head and last_domino[0] == tail):
    //     self.give_points_to_team(last_player, bonus_points)
    //     return
    //
    //
    // def get_player_by_name(self, name):
    //   players = [p for p in self.players if p._name == name]
    //   return players[0]
    //
    // def get_next_player_index(self):
    //   return (self.next_player_index + 1) % len(
    //         self.players)
    //
    // def give_points_to_team(self, player, points):
    //     player_index = self.players.index(self.winner)
    //     team = self.players[
    //         0::2] if player_index == 0 or player_index == 2 else self.players[
    //             1::2]
    //     for p in team:
    //         p.score += points
    //
    // def update_score(self):
    //     total_points = sum(p.get_points() for p in self.players)
    //     self.give_points_to_team(self.winner, total_points)
    //     # winner_index = self.players.index(self.winner)
    //     # winner_team = self.players[
    //     #     0::2] if winner_index == 0 or winner_index == 2 else self.players[
    //     #         1::2]
    //     # for p in winner_team:
    //     #     p.score += total_points
    //
    // def is_locked(self):
    //     head = self.played_dominos[0][0]
    //     tail = self.played_dominos[-1][1]
    //     if head != tail: return False
    //
    //     count = 0
    //     for d in self.played_dominos:
    //         if d[0] == head: count += 1
    //         if d[1] == head: count += 1
    //
    //     if count == 8: return True
    //     return False
    //
    // def display_score(self):
    //     for p in self.players:
    //         print(f'{p._name}: {p.score}')
    //
    // def play_round(self):
    //     self.new_round()
    //     while self.step():
    //         self._display_wrapper.display_game(self.to_game_data())
    //     self._display_wrapper.display_game(self.to_game_data())
    //     self.update_score()
    //
    // def to_game_data(self):
    //     return GameData(self.played_dominos,
    //                     [p.to_player_data() for p in self.players],
    //                     self.next_player_index)
    //
    // def play_game(self, _players=None, shuffle_players=False):
    //     self.new_game(_players, shuffle_players)
    //     while self.winner.score < self.round_score:
    //         self.play_round()
    //
    // def __str__(self):
    //     string = f'in play: {self.played_dominos}'
    //     for i, p in enumerate(self.players):
    //         indicator = '>' if i == self.next_player_index else ' '
    //         string += f'\n{indicator}{p}'
    //     return string

}
