using ConsoleCopter.Models;
using System.Text;

namespace ConsoleCopter;

internal class Game
{
    private Copter copter;
    private List<Pipe> pipes;
    private bool gameOver;
    private int pipeSpawnInterval = 20; // Frames until a new pipe is added
    private int frameCount = 0;
    private int score = 0;
    private int highScore = 0;
    private char[,] buffer;
    private volatile bool keepRunningInputThread;

    public Game()
    {
        keepRunningInputThread = true;

        buffer = new char[Console.WindowWidth, Console.WindowHeight];

        int initialCopterX = 10;
        int initialCopterY = Console.WindowHeight / 2;
        copter = new Copter(initialCopterX, initialCopterY);

        // Initialize the list of pipes
        pipes = new List<Pipe>();

        AddNewPipe();
    }

    private void AddNewPipe()
    {
        int pipeHeight = new Random().Next(5, Console.WindowHeight - 5); // Random height for the pipe
        int pipeX = Console.WindowWidth - 1; // Starting at the right edge of the console
        pipes.Add(new Pipe(pipeX, pipeHeight));
    }

    public void Run()
    {
        Thread inputThread = new Thread(HandleInput);
        inputThread.Start();

        while (!gameOver)
        {
            UpdateGame();
            RenderFrame();
        }

        keepRunningInputThread = false;
        inputThread.Join();

        ShowGameOverScreen();
    }

    private void HandleInput()
    {
        while (keepRunningInputThread)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Spacebar)
                {
                    copter.Jump();
                }
            }
        }
    }

    private void UpdateGame()
    {
        copter.Update();
        foreach (var pipe in pipes)
        {
            pipe.Update();
        }
        if (!CheckCollision())
        {
            score++;
            highScore = Math.Max(score, highScore);
        }
        else
        {
            gameOver = true; // End the game if a collision is detected
            keepRunningInputThread = false; // Signal the input thread to stop
        }

        // Increment frame counter and add a new pipe periodically
        frameCount++;
        if (frameCount >= pipeSpawnInterval)
        {
            AddNewPipe();
            frameCount = 0;
        }

        // Remove pipes that have moved off-screen
        pipes.RemoveAll(p => p.PositionX < 0);
    }

    private bool CheckCollision()
    {
        pipes = pipes.OrderBy(p => p.PositionX).ToList();

        foreach (var pipe in pipes)
        {
            // Check only the pipes that are close to the copter
            if (Math.Abs(copter.PositionX - pipe.PositionX) <= 1) // SomeThreshold depends on the copter and pipe width
            {
                int bottomOfConsole = Console.WindowHeight - 1;
                int topOfPipe = bottomOfConsole - pipe.Height;

                // Check if copter's Y position is within the height of the pipe
                if (copter.PositionY >= topOfPipe)
                {
                    return true; // Collision detected
                }
            }
        }
        return false; // No collision
    }

    private void ShowGameOverScreen()
    {
        Console.WriteLine("YOU LOST");
        Console.WriteLine("Press 'R' to Restart or any other key to Exit.");

        var key = Console.ReadKey();
        if (key.Key == ConsoleKey.R)
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        Console.Clear();
        InitializeGameState();
        Run();
    }

    private void InitializeGameState()
    {
        score = 0;
        int initialCopterX = 10;
        int initialCopterY = Console.WindowHeight / 2;
        copter = new Copter(initialCopterX, initialCopterY);
        pipes.Clear();
        gameOver = false;
        keepRunningInputThread = true;
        // Reset any other game state variables as needed
    }

    private void RenderFrame()
    {
        ClearBuffer();
        DrawHeader(); // New method to draw the header

        copter.Draw(buffer);
        foreach (var pipe in pipes)
        {
            pipe.Draw(buffer);
        }

        DrawBufferToConsole();
    }

    private void DrawHeader()
    {
        string header = $"Score: {score} | High Score: {highScore}";

        for (int x = 0; x < header.Length; x++)
        {
            if (x < Console.WindowWidth)
            {
                buffer[x, 0] = header[x];
            }
        }
    }

    private void ClearBuffer()
    {
        for (int x = 0; x < Console.WindowWidth; x++)
            for (int y = 0; y < Console.WindowHeight; y++)
                buffer[x, y] = ' ';
    }

    private void DrawBufferToConsole()
    {
        Console.SetCursorPosition(0, 0);
        StringBuilder sb = new StringBuilder();

        for (int y = 0; y < Console.WindowHeight; y++)
        {
            for (int x = 0; x < Console.WindowWidth; x++)
            {
                sb.Append(buffer[x, y]);
            }
            if (y < Console.WindowHeight - 1)
                sb.AppendLine();
        }

        Console.Write(sb.ToString());
    }
}
