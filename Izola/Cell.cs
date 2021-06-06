using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Izola
{
    public class Cell
    {
        //Tells the way a cell is being allocated
        public enum SelectionType
        {
            PLAYER1_POSITION,
            PLAYER2_POSITION,
            EMPTY
        }

        public enum AllocationType
        {
            PLAYER1_TERITORY,
            PLAYER2_TERITORY,
            EMPTY
        }


        public static ConsoleColor PLAYER1_COLOR = ConsoleColor.Blue;
        public static ConsoleColor PLAYER2_COLOR = ConsoleColor.Red;
        /*private static ConsoleColor SELECTED_COLOR = ConsoleColor.Yellow;*/
        private static string PLAYER1_RENDER = "1";
        private static string PLAYER2_RENDER = "2";
        private static string ALLOCATED_RENDER = "*";
        private static string EMPTY_RENDER = " ";

        public AllocationType Allocation { get; set; }
        public SelectionType Selection { get; set; }
        /*public bool Selected { get; set; }*/


        public Cell()
        {
            Reset();
        }

        public void Render()
        {
            
            string renderStr = EMPTY_RENDER;

            if(Selection == SelectionType.PLAYER1_POSITION)
            {
                Console.ForegroundColor = PLAYER1_COLOR;
                renderStr = PLAYER1_RENDER;
            }
            else if(Selection == SelectionType.PLAYER2_POSITION)
            {
                Console.ForegroundColor = PLAYER2_COLOR;
                renderStr = PLAYER2_RENDER;
            }
            else if(Allocation == AllocationType.PLAYER1_TERITORY)
            {
                Console.ForegroundColor = PLAYER1_COLOR;
                renderStr = ALLOCATED_RENDER;
            }
            else if(Allocation == AllocationType.PLAYER2_TERITORY)
            {
                Console.ForegroundColor = PLAYER2_COLOR;
                renderStr = ALLOCATED_RENDER;
            }

            Console.Write(renderStr);

            //Console.BackgroundColor = prevBackColor;
            Console.ResetColor();
        }

        public bool IsEmpty()
        {
            return Allocation == AllocationType.EMPTY && Selection == SelectionType.EMPTY;
        }

        private void Reset()
        {
            Allocation = AllocationType.EMPTY;
            Selection = SelectionType.EMPTY;
        }
    }
}
