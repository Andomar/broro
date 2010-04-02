using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace Brother_Robot
{
	/// <summary>
	/// Summary description for UserControl1.
	/// </summary>
    [Serializable]
	public class MapControl : System.Windows.Forms.UserControl
    {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private Pen m_GridPen = new Pen(Color.Black);
        Brush m_BackgroundBrush;
        private Brush m_RobotBrush = new SolidBrush(Color.White);
        private Brush m_Dif0Brush = new SolidBrush(Color.Green);
        private Brush m_Dif1Brush = new SolidBrush(Color.YellowGreen);
        private Brush m_Dif2Brush = new SolidBrush(Color.Orange);
        private Brush m_Dif3Brush = new SolidBrush(Color.Red);
        private Brush[] m_DifBrushes;

        private Map m_Map;
        public Map Map { get { return m_Map; } set { m_Map = value; Refresh();  } }
        // Robot position
        private int m_iRobotColumn = 0;
        private int m_iRobotRow = 0;
        private Direction m_RobotDirection = Direction.North;
        // Cache displayed sizes for resize speed.
        private int m_iColumnWidth = -1;
        private int m_iRowHeight = -1;

		public MapControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            // Initialize list of brushes
            m_DifBrushes = new Brush[Defs.iDifficultyCount ] { m_Dif0Brush, m_Dif1Brush, m_Dif2Brush, m_Dif3Brush };

            m_BackgroundBrush = new SolidBrush(this.BackColor);
            
            // Opaque style tells our parent (a form?) not to draw our background.
            SetStyle(ControlStyles.Opaque, true);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.SuspendLayout();
            // 
            // BrotherMap
            // 
            this.Name = "BrotherMap";
            this.Size = new System.Drawing.Size(554, 417);
            this.Resize += new System.EventHandler(this.BrotherMap_Resize);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.BrotherMap_Paint);
            this.ResumeLayout(false);

		}
		#endregion

        private void InvalidateField(
            int iCol,
            int iRow)
        {
            Invalidate(new Region(new Rectangle(
                iCol * m_iColumnWidth + 1,
                iRow * m_iRowHeight + 1,
                m_iColumnWidth - 1,
                m_iRowHeight - 1
            )));
        }

        public void SetRobotPosition(
            int iCol,
            int iRow,
            int iDirection)
        {
            Debug.Assert(0 <= iCol && iCol < m_Map.Columns);
            Debug.Assert(0 <= iRow && iRow < m_Map.Rows);
            Debug.Assert(Enum.IsDefined(typeof(Direction), iDirection));

            // Invalidate old robot location
            InvalidateField(m_iRobotColumn, m_iRobotRow);

            // Change robot location
            m_iRobotColumn = iCol;
            m_iRobotRow = iRow;
            m_RobotDirection = (Direction) iDirection;

            // Invalidate new robot location
            InvalidateField(m_iRobotColumn, m_iRobotRow);
        }

        private int FindColumnWidth()
        {
            return (this.Width - 1) / m_Map.Columns;
        }

        private int FindRowHeight()
        {
            return (this.Height-1) / m_Map.Rows;
        }

        private void DrawRobot(
            Graphics graph )
        {
            // Draw robot
            int iRobotLeft = m_iRobotColumn * m_iColumnWidth;
            int iRobotTop = m_iRobotRow * m_iRowHeight;
            Point[] arrPoints;
            switch (m_RobotDirection)
            {
                case Direction.North:
                    arrPoints = new Point[3] { 
                        new Point( iRobotLeft + m_iColumnWidth / 2, iRobotTop + m_iRowHeight / 10 ),
                        new Point( iRobotLeft + m_iColumnWidth / 10, iRobotTop + m_iRowHeight * 9 / 10 ),
                        new Point( iRobotLeft + m_iColumnWidth * 9 / 10, iRobotTop + m_iRowHeight * 9 / 10 )
                    };
                    break;
                case Direction.East:
                    arrPoints = new Point[3] { 
                        new Point( iRobotLeft + m_iColumnWidth / 10, iRobotTop + m_iRowHeight / 10 ),
                        new Point( iRobotLeft + m_iColumnWidth * 9  / 10, iRobotTop + m_iRowHeight / 2 ),
                        new Point( iRobotLeft + m_iColumnWidth / 10, iRobotTop + m_iRowHeight * 9 / 10 )
                    };
                    break;
                case Direction.South:
                    arrPoints = new Point[3] { 
                        new Point( iRobotLeft + m_iColumnWidth / 10, iRobotTop + m_iRowHeight / 10 ),
                        new Point( iRobotLeft + m_iColumnWidth * 9 / 10, iRobotTop + m_iRowHeight / 10 ),
                        new Point( iRobotLeft + m_iColumnWidth / 2, iRobotTop + m_iRowHeight * 9 / 10 )
                    };
                    break;
                case Direction.West:
                    arrPoints = new Point[3] { 
                        new Point( iRobotLeft + m_iColumnWidth / 10, iRobotTop + m_iRowHeight / 2 ),
                        new Point( iRobotLeft + m_iColumnWidth * 9 / 10, iRobotTop + m_iRowHeight / 10 ),
                        new Point( iRobotLeft + m_iColumnWidth * 9 / 10, iRobotTop + m_iRowHeight * 9 / 10 )
                    };
                    break;
                default:
                    Debug.Assert(false, "Onbekende direction.");
                    arrPoints = null;
                    break;
            }

            graph.FillPolygon(m_RobotBrush, arrPoints);
        }

        private void DrawField(
            Graphics graph,
            int iCol,
            int iRow)
        {
            Brush brush = m_DifBrushes[m_Map.GetField(iCol, iRow)];
            graph.FillRectangle(brush, 
                m_iColumnWidth * iCol + 1, 
                m_iRowHeight * iRow + 1, 
                m_iColumnWidth - 1, 
                m_iRowHeight - 1);

            if (iCol == m_iRobotColumn && iRow == m_iRobotRow)
                DrawRobot( graph );
        }

        private void DrawGrid(
            Graphics graph )
        {
            if (m_Map == null || !m_Map.Initialized)
            {
                graph.FillRectangle(m_BackgroundBrush, 0, 0, this.Width, this.Height);
                return;
            }

            m_iColumnWidth = FindColumnWidth();
            int iWidth = m_iColumnWidth * m_Map.Columns;
            m_iRowHeight = FindRowHeight();
            int iHeight = m_iRowHeight * m_Map.Rows;

            // Unused space to the right, including the bottom-right square.
            graph.FillRectangle(m_BackgroundBrush, 0, iHeight, iWidth, this.Height);
            // Unused space at the bottom, excluding the bottom-right square.
            graph.FillRectangle(m_BackgroundBrush, iWidth, 0, this.Width, this.Height);

            if (m_iRowHeight == 0 || m_iColumnWidth == 0)
                // Nothing to draw. Hehe.
                return;

            // Draw vertical lines
            for (int x = 0; x <= iWidth; x += m_iColumnWidth)
            {
                graph.DrawLine(m_GridPen, x, 0, x, iHeight);
            }
            // Draw horizontal lines
            for (int y = 0; y <= iHeight; y += m_iRowHeight)
            {
                graph.DrawLine(m_GridPen, 0, y, iWidth, y);
            }
            // Draw map content
            int iLeft = 1;
            for (int iCol = 0; iCol < m_Map.Columns; iCol++)
            {
                int iTop = 1;
                for (int iRow = 0; iRow < m_Map.Rows; iRow++)
                {
                    // Does not use DrawField for speed
                    Brush brush = m_DifBrushes[ m_Map.GetField(iCol, iRow) ];
                    graph.FillRectangle(brush, iLeft, iTop, m_iColumnWidth - 1, m_iRowHeight - 1);

                    iTop += m_iRowHeight;
                }
                iLeft += m_iColumnWidth;
            }
            // Draw the robot
            DrawRobot( graph );
        }

        private void BrotherMap_Paint(object sender, PaintEventArgs e)
        {
            Graphics graph = e.Graphics;
            DrawGrid(graph);
        }

        private void BrotherMap_Resize(object sender, EventArgs e)
        {
            if (m_Map == null || !m_Map.Initialized)
                return;

            if (m_iColumnWidth != FindColumnWidth() ||
                m_iRowHeight != FindRowHeight())
            {
                // Redraw entire map if size has changed
                Invalidate();
            }
        }
    }
}
