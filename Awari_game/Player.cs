using System;
using System.Collections.Generic;
using System.Text;

namespace Awari_game
{
    class Player
    {
        public Hole[] Holes { get; }
        public string Name { get; set; }

        public Player()
        {
            Name = "a Játékos";
            Holes = new Hole[6];
        }

        public Player(string name)
        {
            Name = name;
            Holes = new Hole[6];
            for(int i=0; i< 6; i++)
            {
                Holes[i] = new Hole();
            }
        }

    }
}
