namespace SciImage_Effects.Adjustments
{
    partial class HueAndSaturationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HueAndSaturationForm));
            gTrackBar.ColorPack colorPack5 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack6 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack7 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack8 = new gTrackBar.ColorPack();
            gTrackBar.ColorLinearGradient colorLinearGradient2 = new gTrackBar.ColorLinearGradient();
            gTrackBar.ColorPack colorPack9 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack10 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack11 = new gTrackBar.ColorPack();
            gTrackBar.ColorPack colorPack12 = new gTrackBar.ColorPack();
            gTrackBar.ColorLinearGradient colorLinearGradient3 = new gTrackBar.ColorLinearGradient();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOk = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gtLightness = new gTrackBar.gTrackBar();
            this.gtSaturation = new gTrackBar.gTrackBar();
            this.gtHue = new gTrackBar.gTrackBar();
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(504, 202);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // bCancel
            // 
            this.bCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bCancel.Location = new System.Drawing.Point(340, 179);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(161, 20);
            this.bCancel.TabIndex = 0;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOk
            // 
            this.bOk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bOk.Location = new System.Drawing.Point(174, 179);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(160, 20);
            this.bOk.TabIndex = 1;
            this.bOk.Text = "Ok";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
            this.panel1.Controls.Add(this.gtLightness);
            this.panel1.Controls.Add(this.gtSaturation);
            this.panel1.Controls.Add(this.gtHue);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel1.SetRowSpan(this.panel1, 2);
            this.panel1.Size = new System.Drawing.Size(498, 170);
            this.panel1.TabIndex = 2;
            // 
            // gtLightness
            // 
            colorPack1.Border = System.Drawing.Color.Black;
            colorPack1.Face = System.Drawing.Color.Silver;
            colorPack1.Highlight = System.Drawing.Color.Honeydew;
            this.gtLightness.AButColor = colorPack1;
            this.gtLightness.ArrowColorDown = System.Drawing.Color.White;
            this.gtLightness.ArrowColorHover = System.Drawing.Color.Black;
            this.gtLightness.ArrowColorUp = System.Drawing.Color.Silver;
            this.gtLightness.BackColor = System.Drawing.Color.Transparent;
            colorPack2.Border = System.Drawing.Color.Black;
            colorPack2.Face = System.Drawing.Color.Silver;
            colorPack2.Highlight = System.Drawing.Color.White;
            this.gtLightness.ColorDown = colorPack2;
            colorPack3.Border = System.Drawing.Color.Black;
            colorPack3.Face = System.Drawing.Color.Silver;
            colorPack3.Highlight = System.Drawing.Color.White;
            this.gtLightness.ColorHover = colorPack3;
            colorPack4.Border = System.Drawing.Color.Black;
            colorPack4.Face = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            colorPack4.Highlight = System.Drawing.Color.White;
            this.gtLightness.ColorUp = colorPack4;
            this.gtLightness.Dock = System.Windows.Forms.DockStyle.Top;
            this.gtLightness.FloatValueFontColor = System.Drawing.Color.Black;
            this.gtLightness.Label = "Lightness";
            this.gtLightness.LabelColor = System.Drawing.Color.Black;
            this.gtLightness.LabelPadding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.gtLightness.LabelShow = true;
            this.gtLightness.Location = new System.Drawing.Point(0, 100);
            this.gtLightness.Margin = new System.Windows.Forms.Padding(2);
            this.gtLightness.MaxValue = 100;
            this.gtLightness.MinValue = -100;
            this.gtLightness.Name = "gtLightness";
            this.gtLightness.ShowFocus = false;
            this.gtLightness.Size = new System.Drawing.Size(498, 50);
            this.gtLightness.SliderCapEnd = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
            colorLinearGradient1.ColorA = System.Drawing.Color.Black;
            colorLinearGradient1.ColorB = System.Drawing.Color.White;
            this.gtLightness.SliderColorLow = colorLinearGradient1;
            this.gtLightness.SliderHighlightPt = ((System.Drawing.PointF)(resources.GetObject("gtLightness.SliderHighlightPt")));
            this.gtLightness.SliderShape = gTrackBar.gTrackBar.eShape.ArrowDown;
            this.gtLightness.SliderSize = new System.Drawing.Size(8, 12);
            this.gtLightness.SliderWidthHigh = 3F;
            this.gtLightness.SliderWidthLow = 6F;
            this.gtLightness.TabIndex = 6;
            this.gtLightness.TickThickness = 1F;
            this.gtLightness.Value = 0;
            this.gtLightness.ValueAdjusted = 0F;
            this.gtLightness.ValueBox = gTrackBar.gTrackBar.eValueBox.Left;
            this.gtLightness.ValueBoxBorder = System.Drawing.Color.Black;
            this.gtLightness.ValueBoxFont = new System.Drawing.Font("Arial", 8F);
            this.gtLightness.ValueBoxFontColor = System.Drawing.Color.Black;
            this.gtLightness.ValueBoxSize = new System.Drawing.Size(25, 25);
            this.gtLightness.ValueDivisor = gTrackBar.gTrackBar.eValueDivisor.e1;
            this.gtLightness.ValueStrFormat = null;
            this.gtLightness.ValueChanged += new gTrackBar.gTrackBar.ValueChangedEventHandler(this.gtLightness_ValueChanged);
            // 
            // gtSaturation
            // 
            colorPack5.Border = System.Drawing.Color.Black;
            colorPack5.Face = System.Drawing.Color.Silver;
            colorPack5.Highlight = System.Drawing.Color.Honeydew;
            this.gtSaturation.AButColor = colorPack5;
            this.gtSaturation.ArrowColorDown = System.Drawing.Color.White;
            this.gtSaturation.ArrowColorHover = System.Drawing.Color.Black;
            this.gtSaturation.ArrowColorUp = System.Drawing.Color.Silver;
            this.gtSaturation.BackColor = System.Drawing.Color.Transparent;
            colorPack6.Border = System.Drawing.Color.Black;
            colorPack6.Face = System.Drawing.Color.Silver;
            colorPack6.Highlight = System.Drawing.Color.White;
            this.gtSaturation.ColorDown = colorPack6;
            colorPack7.Border = System.Drawing.Color.Black;
            colorPack7.Face = System.Drawing.Color.Silver;
            colorPack7.Highlight = System.Drawing.Color.White;
            this.gtSaturation.ColorHover = colorPack7;
            colorPack8.Border = System.Drawing.Color.Black;
            colorPack8.Face = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            colorPack8.Highlight = System.Drawing.Color.White;
            this.gtSaturation.ColorUp = colorPack8;
            this.gtSaturation.Dock = System.Windows.Forms.DockStyle.Top;
            this.gtSaturation.FloatValueFontColor = System.Drawing.Color.Black;
            this.gtSaturation.Label = "Saturation";
            this.gtSaturation.LabelColor = System.Drawing.Color.Black;
            this.gtSaturation.LabelPadding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.gtSaturation.LabelShow = true;
            this.gtSaturation.Location = new System.Drawing.Point(0, 50);
            this.gtSaturation.Margin = new System.Windows.Forms.Padding(2);
            this.gtSaturation.MaxValue = 200;
            this.gtSaturation.Name = "gtSaturation";
            this.gtSaturation.ShowFocus = false;
            this.gtSaturation.Size = new System.Drawing.Size(498, 50);
            this.gtSaturation.SliderCapEnd = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
            colorLinearGradient2.ColorA = System.Drawing.Color.Black;
            colorLinearGradient2.ColorB = System.Drawing.Color.White;
            this.gtSaturation.SliderColorLow = colorLinearGradient2;
            this.gtSaturation.SliderHighlightPt = ((System.Drawing.PointF)(resources.GetObject("gtSaturation.SliderHighlightPt")));
            this.gtSaturation.SliderShape = gTrackBar.gTrackBar.eShape.ArrowDown;
            this.gtSaturation.SliderSize = new System.Drawing.Size(8, 12);
            this.gtSaturation.SliderWidthHigh = 3F;
            this.gtSaturation.SliderWidthLow = 6F;
            this.gtSaturation.TabIndex = 5;
            this.gtSaturation.TickThickness = 1F;
            this.gtSaturation.Value = 100;
            this.gtSaturation.ValueAdjusted = 100F;
            this.gtSaturation.ValueBox = gTrackBar.gTrackBar.eValueBox.Left;
            this.gtSaturation.ValueBoxBorder = System.Drawing.Color.Black;
            this.gtSaturation.ValueBoxFont = new System.Drawing.Font("Arial", 8F);
            this.gtSaturation.ValueBoxFontColor = System.Drawing.Color.Black;
            this.gtSaturation.ValueBoxSize = new System.Drawing.Size(25, 25);
            this.gtSaturation.ValueDivisor = gTrackBar.gTrackBar.eValueDivisor.e1;
            this.gtSaturation.ValueStrFormat = null;
            this.gtSaturation.ValueChanged += new gTrackBar.gTrackBar.ValueChangedEventHandler(this.gtSaturation_ValueChanged);
            // 
            // gtHue
            // 
            colorPack9.Border = System.Drawing.Color.Black;
            colorPack9.Face = System.Drawing.Color.Silver;
            colorPack9.Highlight = System.Drawing.Color.Honeydew;
            this.gtHue.AButColor = colorPack9;
            this.gtHue.ArrowColorDown = System.Drawing.Color.White;
            this.gtHue.ArrowColorHover = System.Drawing.Color.Black;
            this.gtHue.ArrowColorUp = System.Drawing.Color.Silver;
            this.gtHue.BackColor = System.Drawing.Color.Transparent;
            colorPack10.Border = System.Drawing.Color.Black;
            colorPack10.Face = System.Drawing.Color.Silver;
            colorPack10.Highlight = System.Drawing.Color.White;
            this.gtHue.ColorDown = colorPack10;
            colorPack11.Border = System.Drawing.Color.Black;
            colorPack11.Face = System.Drawing.Color.Silver;
            colorPack11.Highlight = System.Drawing.Color.White;
            this.gtHue.ColorHover = colorPack11;
            colorPack12.Border = System.Drawing.Color.Black;
            colorPack12.Face = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            colorPack12.Highlight = System.Drawing.Color.White;
            this.gtHue.ColorUp = colorPack12;
            this.gtHue.Dock = System.Windows.Forms.DockStyle.Top;
            this.gtHue.FloatValueFontColor = System.Drawing.Color.Black;
            this.gtHue.Label = "Hue";
            this.gtHue.LabelColor = System.Drawing.Color.Black;
            this.gtHue.LabelPadding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.gtHue.LabelShow = true;
            this.gtHue.Location = new System.Drawing.Point(0, 0);
            this.gtHue.Margin = new System.Windows.Forms.Padding(2);
            this.gtHue.MaxValue = 180;
            this.gtHue.MinValue = -180;
            this.gtHue.Name = "gtHue";
            this.gtHue.ShowFocus = false;
            this.gtHue.Size = new System.Drawing.Size(498, 50);
            this.gtHue.SliderCapEnd = System.Drawing.Drawing2D.LineCap.DiamondAnchor;
            colorLinearGradient3.ColorA = System.Drawing.Color.Black;
            colorLinearGradient3.ColorB = System.Drawing.Color.White;
            this.gtHue.SliderColorLow = colorLinearGradient3;
            this.gtHue.SliderHighlightPt = ((System.Drawing.PointF)(resources.GetObject("gtHue.SliderHighlightPt")));
            this.gtHue.SliderShape = gTrackBar.gTrackBar.eShape.ArrowDown;
            this.gtHue.SliderSize = new System.Drawing.Size(8, 12);
            this.gtHue.SliderWidthHigh = 3F;
            this.gtHue.SliderWidthLow = 6F;
            this.gtHue.TabIndex = 4;
            this.gtHue.TickThickness = 1F;
            this.gtHue.Value = 0;
            this.gtHue.ValueAdjusted = 0F;
            this.gtHue.ValueBox = gTrackBar.gTrackBar.eValueBox.Left;
            this.gtHue.ValueBoxBorder = System.Drawing.Color.Black;
            this.gtHue.ValueBoxFont = new System.Drawing.Font("Arial", 8F);
            this.gtHue.ValueBoxFontColor = System.Drawing.Color.Black;
            this.gtHue.ValueBoxSize = new System.Drawing.Size(25, 25);
            this.gtHue.ValueDivisor = gTrackBar.gTrackBar.eValueDivisor.e1;
            this.gtHue.ValueStrFormat = null;
            this.gtHue.ValueChanged += new gTrackBar.gTrackBar.ValueChangedEventHandler(this.gtHue_ValueChanged);
            // 
            // HueAndSaturationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 202);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HueAndSaturationForm";
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
        internal gTrackBar.gTrackBar gtLightness;
        internal gTrackBar.gTrackBar gtSaturation;
        internal gTrackBar.gTrackBar gtHue;
    }
}