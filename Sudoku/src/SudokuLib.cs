using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

file static class Extensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        Random rng = new Random();
        return source.OrderBy<T, int>((item) => rng.Next());
    }
}

public class SudokuLibOld
{
    private int m_Size = 9;

    private int[,] m_Board;
    private int[,] m_SolvedBoard;
    private int[,] m_BoardInProgress;

    public SudokuLibOld(int colCount = 9)
    {
        m_Size = colCount;
        m_Board = new int[m_Size, m_Size];
        m_SolvedBoard = new int[m_Size, m_Size];
        m_BoardInProgress = new int[m_Size, m_Size];
    }

    public int[,] Board => m_Board;
    public int[,] SolvedBoard => m_SolvedBoard;
    public int[] Board1D => m_Board.Cast<int>().ToArray();
    public int[] SolvedBoard1D => m_SolvedBoard.Cast<int>().ToArray();

    public void Generate(int difficulty = 1)
    {
        ResetBoard();

        // Generate a complete and valid Sudoku board
        Solve();

        // Store the original board
        m_SolvedBoard = (int[,])m_BoardInProgress.Clone();

        // Remove cells based on difficulty level
        Random random = new Random();

        int cellsToRemove = difficulty * 10;
        
        while (cellsToRemove > 0)
        {
            int row = random.Next(0, 9);
            int col = random.Next(0, 9);

            if (m_BoardInProgress[row, col] != 0)
            {
                m_BoardInProgress[row, col] = 0;
                cellsToRemove--;
            }
        }


        m_Board = (int[,])m_BoardInProgress.Clone();

        if (!Solve())
        {
            Generate(difficulty);
        }
    }

    

    private bool Solve()
    {
        int row = 0;
        int col = 0;
        bool isEmpty = FindEmptyCell(ref row, ref col);

        if (!isEmpty)
            return true; // Solution found

        /*List<int> vals = new();
        for (int i = 1; i <= m_Size; i++) vals.Add(i);

        vals.Shuffle();*/


        /*foreach (int num in vals)
        {
            Console.Write($"{num}, ");
            Console.WriteLine();
            if (IsSafe(row, col, num))
            {
                m_BoardInProgress[row, col] = num;

                if (Solve())
                    return true;

                m_BoardInProgress[row, col] = 0; // Backtrack
            }
        }*/

        for (int num = 1; num <= 9; num++)
        {
            if (IsSafe(row, col, num))
            {
                m_BoardInProgress[row, col] = num;

                if (Solve())
                    return true;

                m_BoardInProgress[row, col] = 0; // Backtrack
            }
        }

        return false; // No solution found
    }

    private bool FindEmptyCell(ref int row, ref int col)
    {
        for (row = 0; row < m_Size; row++)
        {
            for (col = 0; col < m_Size; col++)
            {
                if (m_BoardInProgress[row, col] == 0)
                    return true;
            }
        }

        return false;
    }

    private bool IsSafe(int row, int col, int num)
    {
        return !UsedInRow(row, num) && !UsedInColumn(col, num) && !UsedInBox(row - row % 3, col - col % 3, num);
    }

    private bool UsedInRow(int row, int num)
    {
        for (int col = 0; col < m_Size; col++)
        {
            if (m_BoardInProgress[row, col] == num)
                return true;
        }

        return false;
    }

    private bool UsedInColumn(int col, int num)
    {
        for (int row = 0; row < m_Size; row++)
        {
            if (m_BoardInProgress[row, col] == num)
                return true;
        }

        return false;
    }

    private bool UsedInBox(int boxStartRow, int boxStartCol, int num)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (m_BoardInProgress[row + boxStartRow, col + boxStartCol] == num)
                    return true;
            }
        }

        return false;
    }

    public void ResetBoard()
    {
        m_Board = new int[m_Size, m_Size];
        m_SolvedBoard = new int[m_Size, m_Size];
        m_BoardInProgress = new int[m_Size, m_Size];
    }
}



public class SudokuLib
{
    private int m_Size = 9;

    private int[,] m_Board;
    private int[,] m_SolvedBoard;
    private int[,] m_BoardInProgress;

    public SudokuLib(int colCount = 9)
    {
        m_Size = colCount;
        m_Board = new int[m_Size, m_Size];
        m_SolvedBoard = new int[m_Size, m_Size];
        m_BoardInProgress = new int[m_Size, m_Size];
    }

    public int[,] Board => m_Board;
    public int[,] SolvedBoard => m_SolvedBoard;
    public int[] Board1D => m_Board.Cast<int>().ToArray();
    public int[] SolvedBoard1D => m_SolvedBoard.Cast<int>().ToArray();

    public void Generate(int difficulty = 1)
    {
        // Clear the board
        ResetBoard();

        Random RNG = new();

        // Fill the diagonal boxes with random numbers
        for (int i = 0; i < m_Size; i += (int)Math.Sqrt(m_Size)) 
        {
            List<int> nums = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            nums = this.Shuffle(nums);

            for (int j = 0; j < (int)Math.Sqrt(m_Size); j++) 
            {
                for (int k = 0; k < (int)Math.Sqrt(m_Size); k++) 
                {
                    m_Board[i + j, i + k] = nums[j * (int)Math.Sqrt(m_Size) + k];
                    //m_Board[i + j, i + k] = RNG.Next(1, 9);
                }
            }
        }

        // Solve the puzzle starting from the top-left corner
        if (!Solve(0, 0))
        {
            Generate();
            return;
        }

        m_SolvedBoard = (int[,])m_Board.Clone();

        // Remove cells based on difficulty level
        Random random = new Random();

        int cellsToRemove = difficulty * 10;

        while (cellsToRemove > 0)
        {
            int row = random.Next(0, m_Size);
            int col = random.Next(0, m_Size);

            if (m_Board[row, col] != 0)
            {
                m_Board[row, col] = 0;
                cellsToRemove--;
            }
        }

    }

    private List<T> Shuffle<T>(List<T> list)
    {
        var random = new Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;
    }

    private bool Solve(int row, int col)
    {
        // If we've reached the end of the board, return true
        if (row == m_Size)
        {
            return true;
        }

        // If the current cell is already filled, move to the next cell
        if (m_Board[row,col] != 0)
        {
            if (col == 8)
            {
                return Solve(row + 1, 0);
            }
            else
            {
                return Solve(row, col + 1);
            }
        }

        // Try filling the current cell with each possible number
        for (int i = 1; i <= m_Size; i++)
        {
            if (IsValid(row, col, i))
            {
                m_Board[row, col] = i;

                if(col == m_Size - 1)
                {
                    if (Solve(row + 1, 0))
                    {
                        return true;
                    }
                }
                else
                {
                    if(Solve(row, col + 1))
                    {
                        return true;
                    }
                }

                m_Board[row, col] = 0;
            }
        }

        // If no number works, backtrack
        return false;
    }

    private bool IsValid(int row, int col, int num)
    {
        // Check row
        for(int i=0;i<m_Size;i++)
        {
            if (m_Board[row, i] == num)
            {
                return false;
            }
        }

        // Check column
        for(int i = 0; i < m_Size; i++)
        {
            if (m_Board[i,col] == num)
            {
                return false;
            }
        }

        // Check 3x3 box
        int boxRow = row - row % (int)Math.Sqrt(m_Size);
        int boxCol = col - col % (int)Math.Sqrt(m_Size);

        for (int i = 0; i < (int)Math.Sqrt(m_Size); i++) 
        {
            for (int j = 0; j < (int)Math.Sqrt(m_Size); j++) 
            {
                if (Board[boxRow + i, boxCol + j] == num) 
                {
                    return false;
                }
            }
        }

        // If the number is valid in all directions, return true
        return true;
    }

    private bool FindEmptyCell(ref int row, ref int col)
    {
        for (row = 0; row < m_Size; row++)
        {
            for (col = 0; col < m_Size; col++)
            {
                if (m_BoardInProgress[row, col] == 0)
                    return true;
            }
        }

        return false;
    }

    public void ResetBoard()
    {
        m_Board = new int[m_Size, m_Size];
        m_SolvedBoard = new int[m_Size, m_Size];
        m_BoardInProgress = new int[m_Size, m_Size];
    }

    public int Size { get => m_Size; }
}
