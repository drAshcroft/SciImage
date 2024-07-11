namespace SciImage_Effects.Adjustments
{
    partial class BrightnessAndContrastForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrightnessAndContrastForm));
            gTrackBar.ColorPack colorPack5 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack6 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack7 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack8 = new gTrackBar.ColorPack();
            gTrackBar.ColorLinearGradient colorLinearGradient2 = new gTrackBar.ColorLinearGradient();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gTContrast = new gTrackBar.gTrackBar();
            this.gtBrightness = new gTrackBar.gTrackBar();
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(380, 146);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // bCancel
            // 
            this.bCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bCancel.Location = new System.Drawing.Point(257, 123);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(120, 20);
            this.bCancel.TabIndex = 0;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOk
            // 
            this.bOk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bOk.Location = new System.Drawing.Point(132, 123);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(119, 20);
            this.bOk.TabIndex = 1;
            this.bOk.Text = "Ok";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
            this.panel1.Controls.Add(this.gTContrast);
            this.panel1.Controls.Add(this.gtBrightness);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel1.SetRowSpan(this.panel1, 2);
            this.panel1.Size = new System.Drawing.Size(374, 114);
            this.panel1.TabIndex = 2;
            // 
            // gTContrast
            // 
            colorPack1.Border = System.Drawing.Color.Black;
            colorPack1.Face = System.Drawing.Color.Silver;
            colorPack1.Highlight = System.Drawing.Color.Honeydew;
            this.gTContrast.AButColor = colorPack1;
            this.gTContrast.ArrowColorDown = System.Drawing.Color.White;
            this.gTContrast.ArrowColorHover = System.Drawing.Color.Black;
            this.gTContrast.ArrowColorUp = System.Drawing.Color.Silver;
            this.gTContrast.BackColor = System.Drawing.Color.Transparent;
            colorPack2.Border = System.Drawing.Color.Black;
            colorPack2.Face = System.Drawing.Color.Silver;
            colorPack2.Highlight = System.Drawing.Color.White;
            this.gTContrast.ColorDown = colorPack2;
            colorPack3.Border = System.Drawing.Color.Black;
            colorPack3.Face = System.Drawing.Color.Silver;
            colorPack3.Highlight = System.Drawing.Color.White;
            this.gTContrast.ColorHover = colorPack3;
            colorPack4.Border = System.Drawing.Color.Black;
            colorPack4.Face = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            colorPack4.Highlight = System.Drawing.Color.White;
            this.gTContrast.ColorUp = colorPack4;
            this.gTContrast.Dock = System.Windows.Forms.DockStyle.Top;
            this.gTContrast.FloatValueFontColor = System.Drawing.Color.Black;
            this.gTContrast.Label = "Contrast";
            this.gTContrast.LabelColor = System.Drawing.Color.Black;
            this.gTContrast.LabelPadding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.gTContrast.LabelShow = true;
            this.gTContrast.Location = new System.Drawing.Point(0, 50);
            this.gTContrast.Margin = new System.Windows.Forms.Padding(2);
            this.gTContrast.MaxValue = 100;
            this.gTContrast.MinValue = -100;
            this.gTContrast.Name = "gTContrast";
            this.gTContrast.ShowFocus = false;
            this.gTContrast.Size = new System.Drawing.Size(374, 50);
            this.gTContrast.SliderCapEnd = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
            colorLinearGradient1.ColorA = System.Drawing.Color.Black;
            colorLinearGradient1.ColorB = System.Drawing.Color.White;
            this.gTContrast.SliderColorLow = colorLinearGradient1;
            this.gTContrast.SliderHighlightPt = ((System.Drawing.PointF)(resources.GetObject("gTContrast.SliderHighlightPt")));
            this.gTContrast.SliderShape = gTrackBar.gTrackBar.eShape.ArrowDown;
            this.gTContrast.SliderSize = new System.Drawing.Size(8, 12);
            this.gTContrast.SliderWidthHigh = 3F;
            this.gTContrast.SliderWidthLow = 6F;
            this.gTContrast.TabIndex = 5;
            this.gTContrast.TickThickness = 1F;
            this.gTContrast.Value = 0;
            this.gTContrast.ValueAdjusted = 0F;
            this.gTContrast.ValueBox = gTrackBar.gTrackBar.eValueBox.Left;
            this.gTContrast.ValueBoxBorder = System.Drawing.Color.Black;
            this.gTContrast.ValueBoxFont = new System.Drawing.Font("Arial", 8F);
            this.gTContrast.ValueBoxFontColor = System.Drawing.Color.Black;
            this.gTContrast.ValueBoxSize = new System.Drawing.Size(25, 25);
            this.gTContrast.ValueDivisor = gTrackBar.gTrackBar.eValueDivisor.e1;
            this.gTContrast.ValueStrFormat = null;
            this.gTContrast.ValueChanged += new gTrackBar.gTrackBar.ValueChangedEventHandler(this.gTContrast_ValueChanged);
            // 
            // gtBrightness
            // 
            colorPack5.Border = System.Drawing.Color.Black;
            colorPack5.Face = System.Drawing.Color.Silver;
            colorPack5.Highlight = System.Drawing.Color.Honeydew;
            this.gtBrightness.AButColor = colorPack5;
            this.gtBrightness.ArrowColorDown = System.Drawing.Color.White;
            this.gtBrightness.ArrowColorHover = System.Drawing.Color.Black;
            this.gtBrightness.ArrowColorUp = System.Drawing.Color.Silver;
            this.gtBrightness.BackColor = System.Drawing.Color.Transparent;
            colorPack6.Border = System.Drawing.Color.Black;
            colorPack6.Face = System.Drawing.Color.Silver;
            colorPack6.Highlight = System.Drawing.Color.White;
            this.gtBrightness.ColorDown = colorPack6;
            colorPack7.Border = System.Drawing.Color.Black;
            colorPack7.Face = System.Drawing.Color.Silver;
            colorPack7.Highlight = System.Drawing.Color.White;
            this.gtBrightness.ColorHover = colorPack7;
            colorPack8.Border = System.Drawing.Color.Black;
            colorPack8.Face = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            colorPack8.Highlight = System.Drawing.Color.White;
            this.gtBrightness.ColorUp = colorPack8;
            this.gtBrightness.Dock = System.Windows.Forms.DockStyle.Top;
            this.gtBrightness.FloatValueFontColor = System.Drawing.Color.Black;
            this.gtBrightness.Label = "Brightness";
            this.gtBrightness.LabelColor = System.Drawing.Color.Black;
            this.gtBrightness.LabelPadding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.gtBrightness.LabelShow = true;
            this.gtBrightness.Location = new System.Drawing.Point(0, 0);
            this.gtBrightness.Margin = new System.Windows.Forms.Padding(2);
            this.gtBrightness.MaxValue = 100;
            this.gtBrightness.MinValue = -100;
            this.gtBrightness.Name = "gtBrightness";
            this.gtBrightness.ShowFocus = false;
            this.gtBrightness.Size = new System.Drawing.Size(374, 50);
            this.gtBrightness.SliderCapEnd = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
            colorLinearGradient2.ColorA = System.Drawing.Color.Black;
            colorLinearGradient2.ColorB = System.Drawing.Color.White;
            this.gtBrightness.SliderColorLow = colorLinearGradient2;
            this.gtBrightness.SliderHighlightPt = ((System.Drawing.PointF)(resources.GetObject("gtBrightness.SliderHighlightPt")));
            this.gtBrightness.SliderShape = gTrackBar.gTrackBar.eShape.ArrowDown;
            this.gtBrightness.SliderSize = new System.Drawing.Size(8, 12);
            this.gtBrightness.SliderWidthHigh = 3F;
            this.gtBrightness.SliderWidthLow = 6F;
            this.gtBrightness.TabIndex = 4;
            this.gtBrightness.TickThickness = 1F;
            this.gtBrightness.Value = 0;
            this.gtBrightness.ValueAdjusted = 0F;
            this.gtBrightness.ValueBox = gTrackBar.gTrackBar.eValueBox.Left;
            this.gtBrightness.ValueBoxBorder = System.Drawing.Color.Black;
            this.gtBrightness.ValueBoxFont = new System.Drawing.Font("Arial", 8F);
            this.gtBrightness.ValueBoxFontColor = System.Drawing.Color.Black;
            this.gtBrightness.ValueBoxSize = new System.Drawing.Size(25, 25);
            this.gtBrightness.ValueDivisor = gTrackBar.gTrackBar.eValueDivisor.e1;
            this.gtBrightness.ValueStrFormat = null;
            this.gtBrightness.ValueChanged += new gTrackBar.gTrackBar.ValueChangedEventHandler(this.gtBrightness_ValueChanged);
            // 
            // BrightnessAndContrastForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 146);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "BrightnessAndContrastForm";
            this.Text = "BrightnessAndContrastForm";
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
        internal gTrackBar.gTrackBar gTContrast;
        internal gTrackBar.gTrackBar gtBrightness;
    }
}