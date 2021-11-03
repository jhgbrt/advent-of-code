using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jeroen
{
    class Program
    {

        const string input =  @"";
        static void Main(string[] args)
        {
            int[,] x =
{
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
                {41, 42, 43, 44, 45}
            };


            x.Display();


            Regex rotate = new Regex("rotate (?<op>(row|column)) (x|y)=(?<i>\\d*) by (?<by>\\d*)", RegexOptions.Compiled);
            Regex rect = new Regex("rect (?<rows>\\d*)x(?<cols>\\d)*", RegexOptions.Compiled);

            var display = new Display(6, 50);
            foreach (var line in InputReader.Actual())
            //var display = new Display(3, 7);
            //foreach (var line in InputReader.Test())
            {
                //Console.WriteLine();
                //Console.WriteLine(line);
                //Console.ReadKey();
                var matchRect = rect.Match(line);
                if (matchRect.Success)
                {
                    var rows = int.Parse(matchRect.Groups["rows"].ToString());
                    var cols = int.Parse(matchRect.Groups["cols"].ToString());
                    display.Rect(rows,cols);
                }
                var matchRotate = rotate.Match(line);
                if (matchRotate.Success)
                {

                    var op = matchRotate.Groups["op"].ToString();
                    var i = int.Parse(matchRotate.Groups["i"].ToString());
                    var by = int.Parse(matchRotate.Groups["by"].ToString());
                    if (op == "row")
                        display.RotateRow(i, by);
                    else
                        display.RotateCol(i, by);
                }
                display.Draw();
                //Console.ReadKey();
            }

            Console.WriteLine(display.Count);

        }
    }
}
