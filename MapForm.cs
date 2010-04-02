using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Brother_Robot
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MapForm : System.Windows.Forms.Form
    {
        private Button btnGenerateMap;
        private Button btnSaveMap;
        private Button btnLoadMap;
        private MapControl TheMap;
        private Button button1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MapForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            Map oMap = new Map();
            oMap.GenerateMap();
            TheMap.Map = oMap;
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapForm));
            this.btnGenerateMap = new System.Windows.Forms.Button();
            this.btnSaveMap = new System.Windows.Forms.Button();
            this.btnLoadMap = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.TheMap = new Brother_Robot.MapControl();
            this.SuspendLayout();
            // 
            // btnGenerateMap
            // 
            this.btnGenerateMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateMap.Location = new System.Drawing.Point(498, 428);
            this.btnGenerateMap.Name = "btnGenerateMap";
            this.btnGenerateMap.Size = new System.Drawing.Size(100, 27);
            this.btnGenerateMap.TabIndex = 1;
            this.btnGenerateMap.Text = "Generate";
            this.btnGenerateMap.UseVisualStyleBackColor = true;
            this.btnGenerateMap.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnSaveMap
            // 
            this.btnSaveMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveMap.Location = new System.Drawing.Point(343, 428);
            this.btnSaveMap.Name = "btnSaveMap";
            this.btnSaveMap.Size = new System.Drawing.Size(73, 27);
            this.btnSaveMap.TabIndex = 2;
            this.btnSaveMap.Text = "Save";
            this.btnSaveMap.UseVisualStyleBackColor = true;
            this.btnSaveMap.Click += new System.EventHandler(this.btnSaveMap_Click);
            // 
            // btnLoadMap
            // 
            this.btnLoadMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadMap.Location = new System.Drawing.Point(422, 428);
            this.btnLoadMap.Name = "btnLoadMap";
            this.btnLoadMap.Size = new System.Drawing.Size(73, 27);
            this.btnLoadMap.TabIndex = 3;
            this.btnLoadMap.Text = "Load";
            this.btnLoadMap.UseVisualStyleBackColor = true;
            this.btnLoadMap.Click += new System.EventHandler(this.btnLoadMap_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(222, 428);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 26);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TheMap
            // 
            this.TheMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TheMap.Location = new System.Drawing.Point(12, 12);
            this.TheMap.Map = null;
            this.TheMap.Name = "TheMap";
            this.TheMap.Size = new System.Drawing.Size(640, 410);
            this.TheMap.TabIndex = 4;
            // 
            // MapForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(664, 462);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.TheMap);
            this.Controls.Add(this.btnLoadMap);
            this.Controls.Add(this.btnSaveMap);
            this.Controls.Add(this.btnGenerateMap);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MapForm";
            this.Text = "Brother Robot";
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MapForm());
		}

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Map oMap = new Map();
            oMap.GenerateMap();
            TheMap.Map = oMap;
        }

        private void btnSaveMap_Click(object sender, EventArgs e)
        {
        }

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int x = Helpers.GetRandom().Next(TheMap.Map.Columns),
                y = Helpers.GetRandom().Next(TheMap.Map.Rows),
                dir = Helpers.GetRandom().Next(4);

            TheMap.SetRobotPosition( x, y, dir );

            int[] arrVision = TheMap.Map.QueryVision(x, y, Defs.iVisionRange, (Direction) dir);
            string s = "";
            foreach( int i in arrVision )
                s += Convert.ToString(i) + ", ";
            MessageBox.Show(s);
        }
    }
}
