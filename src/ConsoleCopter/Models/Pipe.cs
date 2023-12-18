namespace ConsoleCopter.Models;

public class Pipe
{
    public int PositionX { get; private set; }
    public int Height { get; private set; }

    private const int Speed = 2;

    public Pipe(int initialX, int height)
    {
        PositionX = initialX;
        Height = height;
    }

    public void Update()
    {
        PositionX -= Speed;
    }

    public void Draw(char[,] buffer)
    {
        int bottomOfConsole = Console.WindowHeight - 1;

        for (int i = bottomOfConsole; i > bottomOfConsole - Height; i--)
        {
            if (i >= 0 && i < Console.WindowHeight && PositionX >= 0 && PositionX < Console.WindowWidth)
            {
                buffer[PositionX, i] = '|';
            }
        }
    }
}