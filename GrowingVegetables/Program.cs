using System;
using System.Collections.Generic;
using System.Threading;

namespace GrowingVegetables
{
    enum CellState
    {
        Empty,
        Planted,
        Green,
        Yellow,
        Red,
        Overgrow
    }

    class Cell
    {
        private int cellCost;

        public CellState state = CellState.Empty;

        public void Plant()
        {
            state = CellState.Planted;
            cellCost = 0;
        }

        public int Harvest()
        {
            state = CellState.Empty;
            int tempVal = cellCost;
            cellCost = 0;
            return tempVal;
        }

        public void NextState()
        {
            if ((state != CellState.Empty) && (state != CellState.Overgrow))
            {
                state++;
            }

            switch (state)
            {
                case CellState.Yellow:
                    cellCost = 2;
                    break;
                case CellState.Red:
                    cellCost = 5;
                    break;
                case CellState.Overgrow:
                    cellCost = -1;
                    break;
            }
        }

        public override string ToString()
        {
            switch (state)
            {
                case CellState.Planted:
                    return ".";
                case CellState.Green:
                    return "o";
                case CellState.Yellow:
                    return "i";
                case CellState.Red:
                    return "Y";
                case CellState.Overgrow:
                    return "@";
            }

            return " ";
        }
    }

    internal class Program
    {
        private int period = 2000;
        private int balance = 100;
        private const int fieldSize = 10;
        Cell[] field = new Cell[fieldSize];

        private void Run()
        {
            for (int i = 0; i < fieldSize; i++)
                field[i] = new Cell();
            Timer t = new Timer(TimerCallback, null, 0, period);
            char c = '-';
            while (c != 'q')
            {
                c = Console.ReadKey().KeyChar;
                if ((c >= '0') && (c <= '9'))
                {
                    int cellIndex = int.Parse(c.ToString());
                    if (field[cellIndex].state == CellState.Empty && balance >= 2)
                    {
                        balance -= 2;
                        field[cellIndex].Plant();
                    }
                    else
                    {
                        if (field[cellIndex].state == CellState.Overgrow && balance < 1)
                        {
                            continue;
                        }
                        balance += field[cellIndex].Harvest();
                    }
                }
                else if (c == '>' || c == '<')
                {
                    Console.Write("\b \b"); //Clearing speed change symbol
                    period = (c == '>') ? period - 100 : period + 100; //Changing timer period
                    period = (period <= 0) ? 1 : period; // Making sure timer period > 0
                    t.Change(0, period); // Changing timer
                }
                else
                {
                    Console.Write("\b \b"); //Clearing every other symbol
                }

            }
        }

        private void PrintField()
        {
            Console.WriteLine();
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Current balance: {balance}, refresh rate: {(float)period/1000:F1}s");
            Console.ForegroundColor = ConsoleColor.White;
            
            for (int i = 0; i < fieldSize; i++)
            {
                switch (field[i].state)
                {
                    case (CellState.Green):
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case (CellState.Yellow):
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case (CellState.Red):
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    case (CellState.Overgrow):
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.Write(field[i].ToString() + " ");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        private void TimerCallback(Object o)
        {
            for (int i = 0; i < fieldSize; i++)
            {
                field[i].NextState();
            }
            PrintField();
            GC.Collect();
        }

        public static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}
