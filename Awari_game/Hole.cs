using System;
using System.Collections.Generic;
using System.Text;

namespace Awari_game
{
    public class Hole
    {
        private int Marbles { get; set; }

        public Hole()
        {
            Marbles = 4;
        }

        public Hole(int marbles)
        {
            Marbles = marbles;
        }

        public void AddMarble()
        {
            Marbles++;
        }

        public void RemoveMarble()
        {
            Marbles--;
        }

    }
}
