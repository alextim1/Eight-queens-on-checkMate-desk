using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace eight_queens
{

    [Serializable]
    public enum CellStatus  {fre, occ, utg };

    public interface IShowable
    {
        void ShowDesk();
        void WriteToFile(string path);

    }


    public interface IOperable
    {
        List<Index> CreateListOffreCells();
    }

    [Serializable]
    public class Index
    {
        public int Row { get; set; }
        public int Column { get; set; }
    }

    [Serializable]
    public class Cell
    {
        public string Name { get; set; }
        public CellStatus Status { get; set; }


    }

    [Serializable]
    public class Desk : IShowable, IOperable
    {
        private Cell[,] _cells;


        public Desk()
        {
            string[] columnsMarkers = { "a", "b", "c", "d", "e", "f", "g", "h" };
            Cell[,] currentCells = new Cell [8, 8];
            // init cells
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    currentCells[i, j] = new Cell();
                }
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    currentCells[i, j].Name = columnsMarkers[j] + i.ToString();
                    currentCells[i, j].Status = CellStatus.fre;
                }
            }
            _cells = currentCells;
        }

        public List<Index> CreateListOffreCells()
        {
            List<Index> freCells = new List<Index>();
            for (int i=0; i<8; i++)
            {
                for (int j=0;j<8; j++)
                {
                    if (_cells[i,j].Status==CellStatus.fre)
                    {
                        Index currentCellIndex = new Index();
                        currentCellIndex.Row = i;
                        currentCellIndex.Column = j;
                        freCells.Add(currentCellIndex);
                    }
                }
            }

            return freCells;
        }

        public void PutQueen(int i, int j)
        {
            _cells[i, j].Status = CellStatus.occ;


            // current string under the gun

            for (int k = 0; k < 8; k++)
            {
                if (k!=j)
                {
                    _cells[i, k].Status = CellStatus.utg;
                }
                
            }

            // current column under the gun

            for (int k = 0; k < 8; k++)
            {
               
                if (k!=i)
                {
                    _cells[k, j].Status = CellStatus.utg;
                }
                   
                
               
            }

            // north east half diagonal under the gun
            for (int k = 1; k < 8; k++)
            {
                if (i + k == 8 || j + k == 8)
                {
                    break;
                }

                
                
                    _cells[i + k, j + k].Status = CellStatus.utg;
                
                
            }


            // south east half diagonal under the gun

            for (int k = 1; k < 8; k++)
            {
                if (i - k < 0 || j + k == 8)
                {
                    break;
                }

                
                
                    _cells[i - k, j + k].Status = CellStatus.utg;
                
                
            }

            // south west half diagonal under the gun

            for (int k = 1; k < 8; k++)
            {
                if (i - k < 0 || j - k < 0)
                {
                    break;
                }

                
                
                    _cells[i - k, j - k].Status = CellStatus.utg;
                
               
            }


            // north west half diagonal under the gun

            for (int k = 1; k < 8; k++)
            {
                if (i + k == 8 || j - k < 0)
                {
                    break;
                }

                _cells[i + k, j - k].Status = CellStatus.utg;
            }


        }

        public void ShowDesk()
        {
            for(int i=7; i>=0; i--)
            {
                for (int j=0; j<8; j++)
                {
                    Console.Write(_cells[i, j].Status + "\t");
                }

                Console.WriteLine();
            }
        }

        public void WriteToFile(string path)
        {
            var fileWrite = new StreamWriter(path);

            for(int i=7;i>=0;i--)
            {
                for(int j=0;j<8;j++)
                {
                    fileWrite.Write(_cells[i, j].Status+"\t");
                }

                fileWrite.WriteLine();
            }

            fileWrite.Close();
        }
    }


    public static class ObjectCloner<T>
    {
        public static T CloneObject( T currentObject)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, currentObject);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }



    public class Gamer
    {
        public Desk PutEight(List<Index> freCells, Desk currentDesk, int amountOfQueens)
        {

            // recursion exit condition

            if (freCells.Count==1)
            {
                currentDesk.PutQueen(freCells[0].Row, freCells[0].Column);
                if (amountOfQueens+1==8)
                {
                    return currentDesk;
                }
                else
                {
                    currentDesk.ShowDesk();
                    Console.WriteLine(amountOfQueens+1);
                    //Console.ReadKey();
                    return null;
                }
            }


            // deep search
            

            for (int i=0; i<freCells.Count; i++)
            {
                var copyCurrentDesk = new Desk();
                   copyCurrentDesk= ObjectCloner<Desk>.CloneObject(currentDesk);
                copyCurrentDesk.PutQueen(freCells[i].Row, freCells[i].Column);
                //currentDesk.ShowDesk();
                //Console.WriteLine();
                //Console.ReadKey();
                List<Index> freCellsNextStep = new List<Index>();
                    freCellsNextStep=copyCurrentDesk.CreateListOffreCells();

                Desk finalResult=PutEight(freCellsNextStep,copyCurrentDesk,amountOfQueens+1);

                if (finalResult!=null)
                {
                    return finalResult;
                }
            }

            // if all leafs on a branch don't have positive result return null

            return null;
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            Gamer gamer = new Gamer();
            Desk desk = new Desk();

            desk = gamer.PutEight(desk.CreateListOffreCells(), desk, 0);

            desk.ShowDesk();
            desk.WriteToFile(@"D:\eight_queens.txt");
            Console.ReadKey();
        }
    }
}
