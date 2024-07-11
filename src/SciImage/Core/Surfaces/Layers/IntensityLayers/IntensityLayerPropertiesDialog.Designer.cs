using SciImage.PaintForms.UserControls.Widgets;

namespace SciImage.Core.Surfaces.Layers.IntensityLayers
{
    partial class IntensityLayerPropertiesDialog
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cBLayerVisible = new System.Windows.Forms.CheckBox();
            this.tBLayerName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Details = new System.Windows.Forms.GroupBox();
            this.opacityTrackBar = new System.Windows.Forms.TrackBar();
            this.OpacityUpDown = new PdnNumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cBBlendModes = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.Details.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.opacityTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OpacityUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cBLayerVisible);
            this.groupBox1.Controls.Add(this.tBLayerName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 83);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // cBLayerVisible
            // 
            this.cBLayerVisible.AutoSize = true;
            this.cBLayerVisible.Location = new System.Drawing.Point(47, 51);
            this.cBLayerVisible.Name = "cBLayerVisible";
            this.cBLayerVisible.Size = new System.Drawing.Size(56, 17);
            this.cBLayerVisible.TabIndex = 2;
            this.cBLayerVisible.Text = "Visible";
            this.cBLayerVisible.UseVisualStyleBackColor = true;
            this.cBLayerVisible.CheckedChanged += new System.EventHandler(this.cBLayerVisible_CheckedChanged);
            // 
            // tBLayerName
            // 
            this.tBLayerName.Location = new System.Drawing.Point(47, 16);
            this.tBLayerName.Name = "tBLayerName";
            this.tBLayerName.Size = new System.Drawing.Size(191, 20);
            this.tBLayerName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // Details
            // 
            this.Details.Controls.Add(this.opacityTrackBar);
            this.Details.Controls.Add(this.OpacityUpDown);
            this.Details.Controls.Add(this.label3);
            this.Details.Controls.Add(this.label2);
            this.Details.Controls.Add(this.cBBlendModes);
            this.Details.Location = new System.Drawing.Point(12, 101);
            this.Details.Name = "Details";
            this.Details.Size = new System.Drawing.Size(337, 120);
            this.Details.TabIndex = 2;
            this.Details.TabStop = false;
            this.Details.Text = "Layer Blending";
            // 
            // opacityTrackBar
            // 
            this.opacityTrackBar.Location = new System.Drawing.Point(122, 61);
            this.opacityTrackBar.Maximum = 255;
            this.opacityTrackBar.Name = "opacityTrackBar";
            this.opacityTrackBar.Size = new System.Drawing.Size(193, 50);
            this.opacityTrackBar.TabIndex = 4;
            this.opacityTrackBar.TickFrequency = 10;
            this.opacityTrackBar.ValueChanged += new System.EventHandler(this.opacityTrackBar_ValueChanged);
            // 
            // OpacityUpDown
            // 
            this.OpacityUpDown.Location = new System.Drawing.Point(71, 68);
            this.OpacityUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.OpacityUpDown.Name = "OpacityUpDown";
            this.OpacityUpDown.Size = new System.Drawing.Size(45, 20);
            this.OpacityUpDown.TabIndex = 3;
            this.OpacityUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.OpacityUpDown.ValueChanged += new System.EventHandler(this.OpacityUpDown_ValueChanged);
            this.OpacityUpDown.Leave += new System.EventHandler(this.OpacityUpDown_Leave);
            this.OpacityUpDown.Enter += new System.EventHandler(this.OpacityUpDown_Enter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Opacity";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Blend Mode";
            // 
            // cBBlendModes
            // 
            this.cBBlendModes.FormattingEnabled = true;
            this.cBBlendModes.Location = new System.Drawing.Point(97, 25);
            this.cBBlendModes.Name = "cBBlendModes";
            this.cBBlendModes.Size = new System.Drawing.Size(116, 21);
            this.cBBlendModes.TabIndex = 0;
            this.cBBlendModes.SelectedIndexChanged += new System.EventHandler(this.cBBlendModes_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(184, 242);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(68, 28);
            this.button1.TabIndex = 3;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(258, 242);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(78, 27);
            this.button2.TabIndex = 4;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // BitmapLayerPropertiesDialog2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 280);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Details);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "BitmapLayerPropertiesDialog2";
            this.Text = "Layer Properties";
            this.Controls.SetChildIndex(this.Details, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.Controls.SetChildIndex(this.button2, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.Details.ResumeLayout(false);
            this.Details.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.opacityTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OpacityUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cBLayerVisible;
        private System.Windows.Forms.TextBox tBLayerName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox Details;
        private PdnNumericUpDown OpacityUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cBBlendModes;
        private System.Windows.Forms.TrackBar opacityTrackBar;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}