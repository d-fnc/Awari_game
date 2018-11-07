using System;

namespace Awari_game
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Szia, üdvözöllek az Awari játékban! A kezdéshez add meg a neved(opcionális)!");
            string name = Console.ReadLine();
            Player human = name == ""?new Player():new Player(name);
        }
    }
}
