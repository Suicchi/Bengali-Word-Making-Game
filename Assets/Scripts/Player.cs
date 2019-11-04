using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Player
    {
        public int id;
        public int score;
        public bool turn;

        public Player(int playerId, int number, bool currentPlayer)
        {
            id = playerId;
            score = number;
            turn = currentPlayer;
        }
    }
}
