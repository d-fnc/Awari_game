using System;
using System.Linq;

namespace Awari_game
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.SetWindowSize(100, 43);
            Console.WriteLine("Szia, üdvözöllek az Awari játékban! A kezdéshez add meg a neved(opcionális)!");
            string name = Console.ReadLine();
            Player human = name == ""?new Player():new Player(name);
            Player cpu = new Player("HAL9000");

            //for(int i = 0; i < 6; i++)
            //{
            //    human.Holes[i].Marbles = 2;
            //}
            
            bool next = CoinToss();
            bool gameEnd = false;

            while(!gameEnd)
            {
                next = !next;
                PrintCurrentState(human, cpu);
                if (next)
                {
                    Console.WriteLine(human.Name + " jön!");
                    PlayerInput(human, cpu, ref gameEnd, ref next);
                } else
                {
                    Console.WriteLine("A gép jön!");
                    Console.ReadLine();
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

        static int CheckWhatItWouldYield(Player cpu, Player human, int holeNumber)
        {
            int gainedPoints = 0;
            int multiplier = 0;
            int freeMarbles = cpu.Holes[holeNumber].Marbles - (5 - holeNumber);
            if (freeMarbles <= 0 || (freeMarbles>6 && freeMarbles<=12))
            {
                return 0;
            }
            multiplier = Convert.ToInt32(Math.Floor((double)freeMarbles / 6));
            freeMarbles = freeMarbles % 6 == 0?6:freeMarbles%6;
            if (multiplier == 0 || (multiplier == 1 && freeMarbles == 6))
            {
                while(freeMarbles-1>=0 && (human.Holes[freeMarbles - 1].Marbles==1 || human.Holes[freeMarbles - 1].Marbles == 2))
                {
                    gainedPoints += (human.Holes[freeMarbles - 1].Marbles + 1);
                    freeMarbles -= 1;
                }
            } else if((multiplier%2==1 && freeMarbles == 6) || multiplier%2==0)
            {
                while (freeMarbles - 1 >= 0 && (human.Holes[freeMarbles - 1].Marbles + multiplier == 2 || human.Holes[freeMarbles - 1].Marbles + multiplier == 3))
                {
                    gainedPoints += (human.Holes[freeMarbles - 1].Marbles + multiplier);
                    freeMarbles -= 1;
                }
            }

            return gainedPoints;
            
        }

        static void CompInput(Player cpu, Player human)
        {
            int maxIndex = 0;
            int wouldYieldMax = CheckWhatItWouldYield(cpu, human, 0);
            for(int i = 1; i< 6; i++)
            {
                int partialMax = CheckWhatItWouldYield(cpu, human, i);
                if (partialMax > wouldYieldMax)
                {
                    wouldYieldMax = partialMax;
                    maxIndex = i;
                }
            }
            if(wouldYieldMax == 0)
            {
                int i = 0;
                int maxM = 0;
                int maxInd = 0;
                for(i = 0; i < cpu.Holes.Length; i++)
                {
                    Hole hole = cpu.Holes[i];
                    if(maxM < hole.Marbles)
                    {
                        maxM = hole.Marbles;
                        maxInd = i;
                    }
                }
                if (maxM == 0)
                {
                    Console.WriteLine("A gép nem tud lépni! Nyertél!");
                } else
                {
                    ConcludeStep(cpu, human, maxInd + 1);
                }
            }
            else
            {
                ConcludeStep(cpu, human, maxIndex + 1);
            }
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
                int parsedInput = 0;
                while (parsedInput == 0)
                {
                    if (Int32.TryParse(input, out parsedInput) && (parsedInput > 0 && parsedInput < 7))
                    {
                        ConcludeStep(human, cpu, parsedInput);
                    }
                    else
                    {
                        Console.WriteLine("Rossz input! 1 és 6 között adj meg számot!");
                        input = Console.ReadLine();
                    }
                }
            }
        }

        static void ConcludeStep(Player current, Player enemy, int input)
        {
            int originalHoleNumber = input - 1;
            int holeNumber = originalHoleNumber;
            Hole selectedHole = current.Holes[holeNumber];
            bool playerSide = true;
            int marbles = selectedHole.Marbles;
            selectedHole.Marbles = 0;
            holeNumber++;
            while (marbles != 0)
            {
                if (holeNumber == 6)
                {
                    playerSide = !playerSide;
                    holeNumber = 0;
                }
                if (playerSide)
                {
                    if (holeNumber != originalHoleNumber)
                    {
                        current.Holes[holeNumber].AddMarble();
                        marbles -= 1;
                    }
                }
                else
                {
                    enemy.Holes[holeNumber].AddMarble();
                    marbles -= 1;
                }
                if(marbles>0) holeNumber++;
            }
            while (!playerSide && (enemy.Holes[holeNumber].Marbles == 2 || enemy.Holes[holeNumber].Marbles == 3))
            {
                current.Points += enemy.Holes[holeNumber].Marbles;
                enemy.Holes[holeNumber].Marbles = 0;
                if (holeNumber > 0)
                {
                    holeNumber -= 1;
                }
                else
                {
                    playerSide = !playerSide;
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
            Console.WriteLine(human.ToString() + " - " + cpu.ToString());
        }

        static bool CoinToss()
        {
            bool start = true;

            return start;
        }
    }
}
