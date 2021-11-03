using System;
using System.Linq;

namespace Jeroen
{
    class Display
    {
        bool[,] display;

        public Display(int rows, int cols)
        {
            display = new bool[rows,cols];
        }

        public int Count
        {
            get { return display.OfType<bool>().Count(b => b); }
        }

        public void Rect(int a, int b)
        {
            for (int row = 0; row < a; row++)
            for (int col = 0; col < b; col++)
            {
                display[col, row] = true;
            }
        }
        
        public void Draw()
        {
            Console.Clear();
            display.Display(b => b?"#":".");
        }

        public void RotateCol(int col, int d)
        {
            display.RotateCol(col, d);
        }

        public void RotateRow(int row, int d)
        {
            display.RotateRow(row, d);
        }


    }
}