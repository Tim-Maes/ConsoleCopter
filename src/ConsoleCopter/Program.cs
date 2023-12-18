namespace ConsoleCopter
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.SetWindowSize(80, 30);
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
            Console.CursorVisible = false;

            Game game = new Game(); 
            game.Run();
        }
    }
}
