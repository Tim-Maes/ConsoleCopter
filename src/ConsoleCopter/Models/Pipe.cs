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
        int width = 3; 

        for (int i = bottomOfConsole; i > bottomOfConsole - Height; i--)
        {
            if (i >= 0 && i < Console.WindowHeight)
            {
                for (int j = 0; j < width; j++)
                {
                    if (PositionX + j >= 0 && PositionX + j < Console.WindowWidth)
                    {
                        if (j == 0 || j == width - 1)
                        {
                            buffer[PositionX + j, i] = '|';
                        }
                        else
                        {
                            if (i == bottomOfConsole - Height + 1 || i == bottomOfConsole)
                            {
                                buffer[PositionX + j, i] = '-';
                            }
                            else
                            {
                                buffer[PositionX + j, i] = ' ';
                            }
                        }
                    }
                }
            }
        }
    }
}