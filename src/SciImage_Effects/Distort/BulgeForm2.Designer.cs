namespace SciImage_Effects.Distort
{
    partial class BulgeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gTrackBar.ColorPack colorPack1 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack2 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack3 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack4 = new gTrackBar.ColorPack();
            gTrackBar.ColorLinearGradient colorLinearGradient1 = new gTrackBar.ColorLinearGradient();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BulgeForm));
            gTrackBar.ColorPack colorPack5 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack6 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack7 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack8 = new gTrackBar.ColorPack();
            gTrackBar.ColorLinearGradient colorLinearGradient2 = new gTrackBar.ColorLinearGradient();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gtP2 = new gTrackBar.gTrackBar();
            this.gtP1 = new gTrackBar.gTrackBar();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.Controls.Add(this.bCancel, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.bOk, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(504, 159);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // bCancel
            // 
            this.bCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bCancel.Location = new System.Drawing.Point(340, 135);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(161, 21);
            this.bCancel.TabIndex = 0;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOk
            // 
            this.bOk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bOk.Location = new System.Drawing.Point(174, 135);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(160, 21);
            this.bOk.TabIndex = 1;
            this.bOk.Text = "Ok";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
            this.panel1.Controls.Add(this.gtP2);
            this.panel1.Controls.Add(this.gtP1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel1.SetRowSpan(this.panel1, 2);
            this.panel1.Size = new System.Drawing.Size(498, 126);
            this.panel1.TabIndex = 2;
            // 
            // gtP2
            // 
            colorPack1.Border = System.Drawing.Color.Black;
            colorPack1.Face = System.Drawing.Color.Silver;
            colorPack1.Highlight = System.Drawing.Color.Honeydew;
            this.gtP2.AButColor = colorPack1;
            this.gtP2.ArrowColorDown = System.Drawing.Color.White;
            this.gtP2.ArrowColorHover = System.Drawing.Color.Black;
            this.gtP2.ArrowColorUp = System.Drawing.Color.Silver;
            this.gtP2.BackColor = System.Drawing.Color.Transparent;
            colorPack2.Border = System.Drawing.Color.Black;
            colorPack2.Face = System.Drawing.Color.Silver;
            colorPack2.Highlight = System.Drawing.Color.White;
            this.gtP2.ColorDown = colorPack2;
            colorPack3.Border = System.Drawing.Color.Black;
            colorPack3.Face = System.Drawing.Color.Silver;
            colorPack3.Highlight = System.Drawing.Color.White;
            this.gtP2.ColorHover = colorPack3;
            colorPack4.Border = System.Drawing.Color.Black;
            colorPack4.Face = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            colorPack4.Highlight = System.Drawing.Color.White;
            this.gtP2.ColorUp = colorPack4;
            this.gtP2.Dock = System.Windows.Forms.DockStyle.Top;
            this.gtP2.FloatValueFontColor = System.Drawing.Color.Black;
            this.gtP2.Label = "Coarseness";
            this.gtP2.LabelColor = System.Drawing.Color.Black;
            this.gtP2.LabelPadding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.gtP2.LabelShow = true;
            this.gtP2.Location = new System.Drawing.Point(0, 50);
            this.gtP2.Margin = new System.Windows.Forms.Padding(2);
            this.gtP2.MaxValue = 200;
            this.gtP2.Name = "gtP2";
            this.gtP2.ShowFocus = false;
            this.gtP2.Size = new System.Drawing.Size(498, 50);
            this.gtP2.SliderCapEnd = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
            colorLinearGradient1.ColorA = System.Drawing.Color.Black;
            colorLinearGradient1.ColorB = System.Drawing.Color.White;
            this.gtP2.SliderColorLow = colorLinearGradient1;
            this.gtP2.SliderHighlightPt = ((System.Drawing.PointF)(resources.GetObject("gtP2.SliderHighlightPt")));
            this.gtP2.SliderShape = gTrackBar.gTrackBar.eShape.ArrowDown;
            this.gtP2.SliderSize = new System.Drawing.Size(8, 12);
            this.gtP2.SliderWidthHigh = 3F;
            this.gtP2.SliderWidthLow = 6F;
            this.gtP2.TabIndex = 5;
            this.gtP2.TickThickness = 1F;
            this.gtP2.Value = 100;
            this.gtP2.ValueAdjusted = 100F;
            this.gtP2.ValueBox = gTrackBar.gTrackBar.eValueBox.Left;
            this.gtP2.ValueBoxBorder = System.Drawing.Color.Black;
            this.gtP2.ValueBoxFont = new System.Drawing.Font("Arial", 8F);
            this.gtP2.ValueBoxFontColor = System.Drawing.Color.Black;
            this.gtP2.ValueBoxSize = new System.Drawing.Size(25, 25);
            this.gtP2.ValueDivisor = gTrackBar.gTrackBar.eValueDivisor.e1;
            this.gtP2.ValueStrFormat = null;
            this.gtP2.ValueChanged += new gTrackBar.gTrackBar.ValueChangedEventHandler(this.gtP2_ValueChanged);
            // 
            // gtP1
            // 
            colorPack5.Border = System.Drawing.Color.Black;
            colorPack5.Face = System.Drawing.Color.Silver;
            colorPack5.Highlight = System.Drawing.Color.Honeydew;
            this.gtP1.AButColor = colorPack5;
            this.gtP1.ArrowColorDown = System.Drawing.Color.White;
            this.gtP1.ArrowColorHover = System.Drawing.Color.Black;
            this.gtP1.ArrowColorUp = System.Drawing.Color.Silver;
            this.gtP1.BackColor = System.Drawing.Color.Transparent;
            colorPack6.Border = System.Drawing.Color.Black;
            colorPack6.Face = System.Drawing.Color.Silver;
            colorPack6.Highlight = System.Drawing.Color.White;
            this.gtP1.ColorDown = colorPack6;
            colorPack7.Border = System.Drawing.Color.Black;
            colorPack7.Face = System.Drawing.Color.Silver;
            colorPack7.Highlight = System.Drawing.Color.White;
            this.gtP1.ColorHover = colorPack7;
            colorPack8.Border = System.Drawing.Color.Black;
            colorPack8.Face = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            colorPack8.Highlight = System.Drawing.Color.White;
            this.gtP1.ColorUp = colorPack8;
            this.gtP1.Dock = System.Windows.Forms.DockStyle.Top;
            this.gtP1.FloatValueFontColor = System.Drawing.Color.Black;
            this.gtP1.Label = "Brush Size";
            this.gtP1.LabelColor = System.Drawing.Color.Black;
            this.gtP1.LabelPadding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.gtP1.LabelShow = true;
            this.gtP1.Location = new System.Drawing.Point(0, 0);
            this.gtP1.Margin = new System.Windows.Forms.Padding(2);
            this.gtP1.MaxValue = 180;
            this.gtP1.MinValue = -180;
            this.gtP1.Name = "gtP1";
            this.gtP1.ShowFocus = false;
            this.gtP1.Size = new System.Drawing.Size(498, 50);
            this.gtP1.SliderCapEnd = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
            colorLinearGradient2.ColorA = System.Drawing.Color.Black;
            colorLinearGradient2.ColorB = System.Drawing.Color.White;
            this.gtP1.SliderColorLow = colorLinearGradient2;
            this.gtP1.SliderHighlightPt = ((System.Drawing.PointF)(resources.GetObject("gtP1.SliderHighlightPt")));
            this.gtP1.SliderShape = gTrackBar.gTrackBar.eShape.ArrowDown;
            this.gtP1.SliderSize = new System.Drawing.Size(8, 12);
            this.gtP1.SliderWidthHigh = 3F;
            this.gtP1.SliderWidthLow = 6F;
            this.gtP1.TabIndex = 4;
            this.gtP1.TickThickness = 1F;
            this.gtP1.Value = 0;
            this.gtP1.ValueAdjusted = 0F;
            this.gtP1.ValueBox = gTrackBar.gTrackBar.eValueBox.Left;
            this.gtP1.ValueBoxBorder = System.Drawing.Color.Black;
            this.gtP1.ValueBoxFont = new System.Drawing.Font("Arial", 8F);
            this.gtP1.ValueBoxFontColor = System.Drawing.Color.Black;
            this.gtP1.ValueBoxSize = new System.Drawing.Size(25, 25);
            this.gtP1.ValueDivisor = gTrackBar.gTrackBar.eValueDivisor.e1;
            this.gtP1.ValueStrFormat = null;
            this.gtP1.ValueChanged += new gTrackBar.gTrackBar.ValueChangedEventHandler(this.gtP1_ValueChanged);
            // 
            // OilPaintingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 159);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "OilPaintingForm";
            this.Text = "HueAndSaturationForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOk;
        private System.Windows.Forms.Panel panel1;
        internal gTrackBar.gTrackBar gtP2;
        internal gTrackBar.gTrackBar gtP1;
    }
}