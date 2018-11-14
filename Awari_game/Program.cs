using System;
using System.Linq;

namespace Awari_game
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.SetWindowSize(100, 43);
            //Console.WriteLine(Console.LargestWindowWidth+"  "+Console.LargestWindowHeight);
            Console.WriteLine("Szia, üdvözöllek az Awari játékban! A kezdéshez add meg a neved(opcionális)!");
            string name = Console.ReadLine();
            Player human = name == ""?new Player():new Player(name);
            Player cpu = new Player("HAL9000");

            bool next = CoinToss();
            bool gameEnd = false;

            while(!gameEnd)
            {
                next = !next;
                PrintCurrentState(human, cpu);
                if (next)
                {
                    PlayerInput(human, cpu, ref gameEnd, ref next);
                } else
                {
                    CompInput(cpu, human);
                }
                gameEnd = CheckIfEndGame(ref gameEnd);
                Console.Clear();
            }

            Console.WriteLine("GAME OVER");
            Console.WriteLine(next?"A gép nyert sajnos":"Gratulálok "+human.Name);
            Console.ReadLine();

        }

        static bool CheckIfEndGame(ref bool endGame)
        {
            return endGame;
        }

        static void CompInput(Player cpu, Player human)
        {
            
        }

        static void PlayerInput(Player human, Player cpu, ref bool gameEnd, ref bool next)
        {
            string input = Console.ReadLine();
            if (input == "f")
            {
                gameEnd = true;
                next = true;
            }
            else
            {
                int parsedInput = int.Parse(input);
                while (!(parsedInput > 0 && parsedInput < 7))
                {
                    Console.WriteLine("Rossz input! 1 és 6 között adj meg számot!");
                    parsedInput = int.Parse(Console.ReadLine());
                }
                int holeNumber = parsedInput - 1;
                Hole selectedHole = human.Holes[holeNumber];
                bool playerSide = true;
                int marbles = selectedHole.Marbles;
                selectedHole.Marbles = 0;
                holeNumber++;
                for (int i = 0; i < marbles; i++)
                {
                    if (holeNumber == 6)
                    {
                        playerSide = !playerSide;
                        holeNumber = 0;
                    }
                    if (playerSide)
                    {
                        human.Holes[holeNumber].AddMarble();
                    }
                    else
                    {
                        cpu.Holes[holeNumber].AddMarble();
                    }
                    holeNumber++;
                }
            }
        }

        static void PrintCurrentState(Player human, Player cpu)
        {
            //Console.Write(Enumerable.Repeat("----------", 10));
            Console.Write(Environment.NewLine);
            
            for (int i = 0; i < 6; i++)
            {
                int humanMarbles = human.Holes[i].Marbles;
                int cpuMarbles = cpu.Holes[i].Marbles;

                Console.WriteLine(" "+(i+1)+"  "+"  .-''-.    |     .-''-.    ");
                Console.WriteLine("    " + " /      \\   |    /      \\   ");

                Console.WriteLine("    " + "|    "+ humanMarbles + (Math.Ceiling(Math.Log10(humanMarbles+1))==2?"  ":"   ")+"|  |   |    "+ cpuMarbles + (Math.Ceiling(Math.Log10(cpuMarbles + 1)) == 2 ? "  " : "   ") + "|  ");

                Console.WriteLine("    " + " \\      /   |    \\      /   ");
                Console.WriteLine("    " + "  `-..-'    |     `-..-'    ");
                Console.WriteLine();
            }
            
            //Console.Write(Enumerable.Repeat("----------", 10));
            Console.Write(Environment.NewLine);
        }

        static bool CoinToss()
        {
            bool start = true;

            return start;
        }
    }
}
