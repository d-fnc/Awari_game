using System;
using System.Linq;

namespace Awari_game
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.SetWindowSize(60, 45);
            Console.WriteLine("Szia, üdvözöllek az Awari játékban!");
            Console.WriteLine("A játékban felváltva jön a játékos, és a gép.");
            Console.WriteLine("A játékosé a bal oldal, a gépé a jobb.");
            Console.WriteLine("A játéknak akkor van vége, ha valamely játékos eléri a 24 pontot!");
            Console.WriteLine("Jó Szórakozást!");

            Console.WriteLine("A kezdéshez add meg a neved(opcionális)!");
            string name = Console.ReadLine();
            Player human = name == ""?new Player():new Player(name);
            Player cpu = new Player("HAL9000");
            
            //A játékot véletlenszerűen kezdjük el. 
            bool next = CoinToss();
            bool gameEnd = false;

            //Addig jönnek felváltva a játékosok amíg nem érjük el a játék végét. (nem teljesül a játék vége kritérium)
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
                gameEnd = CheckIfEndGame(ref gameEnd, cpu, human);
                Console.Clear();
            }

            PrintGameEnd(next, human, cpu);

        }

        static bool CheckIfEndGame(ref bool endGame, Player cpu, Player human)
        {
            // A specifikációban 25 pont volt megjelölve mint a játék vége, de akkor kialakulhat 24-23 as állás, úgy hogy az utolsó mag játékban marad: soha nincs vége. Így kevésbé valószínű, hogy ez vagy hasonló szituáció kialakulhat.
            if(cpu.Points >= 24 || human.Points >=24 || (cpu.Points==23 && human.Points == 23))
            {
                return true;
            }
            return endGame;
        }

        //A gép által használt metódus, hogy eldöntse melyik lépés által szerezné a legtöbb pontot.
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
            //Megnézzük melyik lépés által szerezné a legtöbb pontot a gép, majd meglépjük.
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
            //Ha a gép nem tud pontot szerezni akkor azt a házat választja, ahol a legtöbb a mag(így érdekesebb a játék; ha az első nem nulla házat választaná, nagyon unalmas lenne)
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
                //Ha nincs valid lépése, passzol
                if (maxM == 0)
                {
                    Console.WriteLine("A gép nem tud lépni!");
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

        //A játékos lépését feldolgozó metódus
        static void PlayerInput(Player human, Player cpu, ref bool gameEnd, ref bool next)
        {
            //Megnézzük, hogy a játékos tud-e lépni. Ha nem, akkor passzol automatikusan
            bool canMakeValidMove = false;
            int i = 0;
            while(!canMakeValidMove && i<6)
            {
                if (human.Holes[i].Marbles > 0) { canMakeValidMove = true; } else { i++; }
            }
            if (!canMakeValidMove)
            {
                Console.WriteLine(human.Name + " nem tud lépni!");
                Console.ReadLine();
                return;
            }
            string input = Console.ReadLine();
            //Cheatcode, hogy a játékos egyből nyerjen
            if (input == "f")
            {
                gameEnd = true;
                next = true;
            }
            else
            {
                int parsedInput = 0;
                bool validStep = false;
                while (parsedInput == 0 || !validStep)
                {
                    if (Int32.TryParse(input, out parsedInput) && (parsedInput > 0 && parsedInput < 7))
                    {
                        if (human.Holes[parsedInput - 1].Marbles == 0)
                        {
                            Console.WriteLine("Sajnos ebben a lyukban nincs egy golyó sem! válassz másikat!");
                            input = Console.ReadLine();
                        }
                        else
                        {
                            validStep = true;
                            ConcludeStep(human, cpu, parsedInput);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Rossz input! 1 és 6 között adj meg számot!");
                        input = Console.ReadLine();
                    }
                }
            }
        }

        //Az adott lépést feldolgozó metódus.
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

        //A játéktáblát megjelenítő metódus
        static void PrintCurrentState(Player human, Player cpu)
        {
            Console.Write(Environment.NewLine);
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
            
            Console.Write(Environment.NewLine);
            Console.Write(Environment.NewLine);

            Console.WriteLine(human.ToString() + " - " + cpu.ToString());
        }

        //A játék végét kiíró metódus
        static void PrintGameEnd(bool next, Player human, Player cpu)
        {
            Console.WriteLine("         Pontok: " + cpu.Name + " : " + cpu.Points + " - " + human.Name + " : " + human.Points);

            Console.Write("" +
            "         _______ _______ _______ _______         " + Environment.NewLine +
            "        (_______|_______|_______|_______)        " + Environment.NewLine +
            "         _   ___ _______ _  _  _ _____           " + Environment.NewLine +
            "        | | (_  |  ___  | ||_|| |  ___)          " + Environment.NewLine +
            "        | |___) | |   | | |   | | |_____         " + Environment.NewLine +
            "         \\_____/|_|   |_|_|   |_|_______)        " + Environment.NewLine +
            "         _______ _     _ _______ ______          " + Environment.NewLine +
            "                                                 " + Environment.NewLine +
            "        (_______|_)   (_|_______|_____ \\         " + Environment.NewLine +
            "         _     _ _     _ _____   _____) )        " + Environment.NewLine +
            "        | |   | | |   | |  ___) |  __  /         " + Environment.NewLine +
            "        | |___| |\\ \\ / /| |_____| |  \\ \\         " + Environment.NewLine +
            "         \\_____/  \\___/ |_______)_|   |_|        " + Environment.NewLine +
            "                                                 ");
            Console.WriteLine();
            if(human.Points == cpu.Points)
            {
                Console.WriteLine("         Döntetlen lett a játék vége!");
                Console.ReadLine();
            } else
            {
                Console.WriteLine(!next ? "         A gép nyert sajnos" : "         Gratulálok " + human.Name + "!");
                Console.ReadLine();
            }
        }

        static bool CoinToss()
        {
            Random rand = new Random();
            bool start = rand.Next(0, 11)%2 ==0;

            return start;
        }
    }
}
