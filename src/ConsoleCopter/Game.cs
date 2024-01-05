using ConsoleCopter.Models;
using System.Text;

namespace ConsoleCopter;

internal class Game
{
    private Copter copter;
    private List<Pipe> pipes;
    private bool gameOver;
    private int pipeSpawnInterval = 20;
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

        pipes = new List<Pipe>();

        AddNewPipe();
    }

    private void AddNewPipe()
    {
        int pipeHeight = new Random().Next(5, Console.WindowHeight - 5);
        int pipeX = Console.WindowWidth - 1;
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
            if (!gameOver && Console.KeyAvailable)
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
        if (!gameOver)
        {
            copter.UpdateAnimationFrame();
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
                gameOver = true;
                keepRunningInputThread = false;
            }

            frameCount++;
            if (frameCount >= pipeSpawnInterval)
            {
                AddNewPipe();
                frameCount = 0;
            }

            pipes.RemoveAll(p => p.PositionX < 0);
        }
    }

    private bool CheckCollision()
    {
        int pipeWidth = 3; 

        pipes = pipes.OrderBy(p => p.PositionX).ToList();

        if (copter.PositionY >= Console.WindowHeight - 1)
        {
            return true;
        }

        foreach (var pipe in pipes)
        {
            if (copter.PositionX >= pipe.PositionX && copter.PositionX < pipe.PositionX + pipeWidth)
            {
                int bottomOfConsole = Console.WindowHeight - 1;
                int topOfPipe = bottomOfConsole - pipe.Height;

                if (copter.PositionY >= topOfPipe)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ShowGameOverScreen()
    {
        Console.WriteLine($"YOU LOST, your score is: {score}");
        Console.WriteLine("Press 'R' to Restart or hit ESCAPE to Exit.");

        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(true);
            key = keyInfo.Key;
        }
        while (key != ConsoleKey.R && key != ConsoleKey.Escape);

        if (key == ConsoleKey.R)
        {
            RestartGame();
        }
        else
        {
            keepRunningInputThread = false;
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
    }

    private void RenderFrame()
    {
        ClearBuffer();
        DrawHeader();

        copter.Draw(buffer);
        foreach (var pipe in pipes)
        {
            pipe.Draw(buffer);
        }

        DrawBufferToConsole();
    }

    private void DrawHeader()
    {
        string header = $"Score: {score} | High Score: {highScore} | ConsoleCopter game by Tim Maes";

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
