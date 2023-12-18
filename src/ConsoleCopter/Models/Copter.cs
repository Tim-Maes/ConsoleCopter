﻿namespace ConsoleCopter.Models;

public class Copter
{
    public int PositionX { get; private set; }
    public int PositionY { get; private set; }

    private int velocityY;
    private const int Gravity = 2;

    public Copter(int initialX, int initialY)
    {
        PositionX = initialX;
        PositionY = initialY;
        velocityY = 0;
    }

    public void Update()
    {
        velocityY += Gravity;
        PositionY += velocityY;

        PositionY = Math.Max(0, Math.Min(Console.WindowHeight - 1, PositionY));
    }

    public void Jump()
    {
        velocityY = -5;
    }

    public void Draw(char[,] buffer)
    {
        if (PositionX >= 0 && PositionX < Console.WindowWidth &&
            PositionY >= 0 && PositionY < Console.WindowHeight)
        {
            buffer[PositionX, PositionY] = 'H'; // 'H' represents the helicopter
        }
    }

    public bool CheckCollision(Copter copter, Pipe pipe)
    {
        // Check if the copter's position overlaps with the pipe's position
        if (copter.PositionX == pipe.PositionX && copter.PositionY < pipe.Height)
        {
            return true; // Collision detected
        }

        return false; // No collision
    }
}