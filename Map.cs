using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Brother_Robot
{
    public enum Direction { North = 0, East = 1, South = 2, West = 3 }

    public class Map
    {
        private struct MapField
        {
            public int iDifficulty;
        };

        // Map size
        private int m_iRowCount = 25;
        public int Rows { get { return m_iRowCount; } }
        private int m_iColumnCount = 80;
        public int Columns { get { return m_iColumnCount; } }
        private MapField[,] m_arrFields;
        private bool m_bInitialized = false;
        public bool Initialized { get { return m_bInitialized; } }

        public void SetField(
            int iCol,
            int iRow,
            int iDifficulty)
        {
            Debug.Assert(0 <= iDifficulty && iDifficulty < Defs.iDifficultyCount);
            Debug.Assert(0 <= iCol && iCol < m_iColumnCount);
            Debug.Assert(0 <= iRow && iRow < m_iRowCount);
            m_arrFields[iCol, iRow].iDifficulty = iDifficulty;
        }

        public int GetField(
            int iCol,
            int iRow)
        {
            Debug.Assert(0 <= iCol && iCol < m_iColumnCount);
            Debug.Assert(0 <= iRow && iRow < m_iRowCount);
            int iDifficulty = m_arrFields[iCol, iRow].iDifficulty;
            Debug.Assert(0 <= iDifficulty && iDifficulty < Defs.iDifficultyCount);
            return iDifficulty;
        }

        private int GetFieldVision(
            int iCol,
            int iRow)
        {
            int iDifficulty;

            if (0 <= iCol && iCol < m_iColumnCount &&
                0 <= iRow && iRow < m_iRowCount)
            {
                iDifficulty = m_arrFields[iCol, iRow].iDifficulty;
                Debug.Assert(0 <= iDifficulty && iDifficulty < Defs.iDifficultyCount);
            }
            else
                iDifficulty = Defs.iDifficultyCount;

            return iDifficulty;
        }

        public int[] QueryVision(
            int iCol,
            int iRow,
            int iVisionRange,
            Direction Direction)
        {
            // Robot sees a triangle ahead.  The number of fields in such a triangle
            // is the square of the vision range.
            int iFieldCount = iVisionRange * iVisionRange;

            int[] arrVision = new int[iFieldCount];
            int i = 0, x, y;
            for (int iRange = 0; iRange < iVisionRange; iRange++)
            {
                switch( Direction )
                {
                    case Direction.North:
                        y = iRow - iRange;
                        for (x = -iRange; x <= iRange; x++)
                            arrVision[i++] = GetFieldVision(iCol + x, y);
                        break;
                    case Direction.East:
                        x = iCol + iRange;
                        for (y = -iRange; y <= iRange; y++)
                            arrVision[i++] = GetFieldVision(x, iRow + y);
                        break;
                    case Direction.South:
                        y = iRow + iRange;
                        for (x = iRange; x >= -iRange; x--)
                            arrVision[i++] = GetFieldVision(iCol + x, y);
                        break;
                    case Direction.West:
                        x = iCol - iRange;
                        for (y = iRange; y >= -iRange; y--)
                            arrVision[i++] = GetFieldVision(x, iRow + y);
                        break;
                    default:
                        Debug.Assert(false, "Onbekende direction.");
                        break;
                }
            }
            return arrVision;
        }

        private void GenerateMapRandom()
        {
            for (int iCol = 0; iCol < m_iColumnCount; iCol++)
                for (int iRow = 0; iRow < m_iRowCount; iRow++)
                    SetField(iCol, iRow, Helpers.GetRandom().Next(Defs.iDifficultyCount));
        }

        private void GenerateMapSmother()
        {
            // Prepare random
            int[,] RandomSquares = new int[m_iColumnCount, m_iRowCount];
            int[] arrFrequency = new int[Defs.iDifficultyCount];

            for (int iCol = 0; iCol < m_iColumnCount; iCol++)
                for (int iRow = 0; iRow < m_iRowCount; iRow++)
                    RandomSquares[iCol, iRow] = Helpers.GetRandom().Next(Defs.iDifficultyCount);

            // Smother
            for (int iCol = 0; iCol < m_iColumnCount; iCol++)
            {
                for (int iRow = 0; iRow < m_iRowCount; iRow++)
                {
                    for (int iDifficulty = 0; iDifficulty < Defs.iDifficultyCount; iDifficulty++)
                        arrFrequency[iDifficulty] = 0;

                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (iCol + i < 0 || iCol + i >= m_iColumnCount ||
                                iRow + j < 0 || iRow + j >= m_iRowCount)
                                arrFrequency[Defs.iDifficultyCount - 1]++;
                            else
                            {
                                Debug.Assert(0 <= RandomSquares[iCol + i, iRow + j] &&
                                    RandomSquares[iCol + i, iRow + j] < Defs.iDifficultyCount,
                                    "New smother map fails.");
                                arrFrequency[RandomSquares[iCol + i, iRow + j]]++;
                            }
                        }
                    }

                    int iNewDifficulty = 0;
                    for (int i = 0; i < Defs.iDifficultyCount; i++)
                        if (arrFrequency[i] > arrFrequency[iNewDifficulty])
                            iNewDifficulty = i;

                    SetField(iCol, iRow, iNewDifficulty);
                }
            }
        }

        private void GenerateMapSlowChange()
        {
            int i, j, j_max, avg;

            for (i = 0; i < m_iColumnCount + m_iRowCount - 1; i++)
            {
                if (i < m_iColumnCount)
                    j = 0;
                else
                    j = i - m_iColumnCount + 1;

                if (i < m_iRowCount)
                    j_max = i + 1;
                else
                    j_max = m_iRowCount;

                for (; j < j_max; j++)
                {
                    if (j == 0 || i - j == 0)
                        // First square isn't 4 or finish.
                        SetField(i - j, j, Helpers.GetRandom().Next(Defs.iDifficultyCount - 2));
                    else
                    {
                        avg = GetField(i - j - 1, j) + GetField(i - j, j - 1);
                        avg = (10 * avg) / 17;  // Not by 20 but less.. give
                        // mountains a chance!

                        // 50% chance for change
                        if (Helpers.GetRandom().Next(2) == 0)
                        {
                            if (Helpers.GetRandom().Next(8) == 0)
                                // Every once in a while, utterly random
                                // Not finish or out-of-map of course :)
                                avg = 1 + Helpers.GetRandom().Next(Defs.iDifficultyCount - 1);
                            else
                                // Add or subtract one to or from average
                                avg += Helpers.GetRandom().Next(3) - 1;
                        }

                        if (avg >= Defs.iDifficultyCount)
                            avg = Defs.iDifficultyCount - 1;
                        else if (avg < 0)
                            avg = 0;

                        SetField(i - j, j, avg);
                    }
                }
            }
        }

        public void GenerateMap()
        {
            // Allocate map
            m_arrFields = new MapField[m_iColumnCount, m_iRowCount];

            // Choose random method for map generation
            switch (Helpers.GetRandom().Next(2))
            {
                case 0:
                    GenerateMapSmother();
                    break;
                case 1:
                    GenerateMapSlowChange();
                    break;
                default:
                    Debug.Assert(false, "Unexpected map generation type.");
                    break;
            }
            m_bInitialized = true;
        }
    }
}
