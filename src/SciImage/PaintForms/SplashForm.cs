/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using SciImage.SystemLayer;
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SciImage.Forms;
using SciImage.PaintForms.UserControls.Widgets;
using SciImage.SciResources;

namespace SciImage.PaintForms
{
    public class SplashForm 
        : SciBaseForm
    {
        private System.Windows.Forms.Label copyrightLabel;
        private SciBanner banner;
        private ProgressBar progressBar;

        public SplashForm()
        {
            //SuspendLayout();

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // Fill in the status label
            banner.BannerText = SciResources.SciResources.GetString("SplashForm.StatusLabel.Text");

            // Fill in the copyright label
            copyrightLabel.Text = SciInfo.GetCopyrightString();
            this.FormBorderStyle = FormBorderStyle.None;
            this.SizeGripStyle = SizeGripStyle.Hide;
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.banner = new SciBanner();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.progressBar = new ProgressBar();
            this.SuspendLayout();
            //
            // banner
            //
            this.banner.Name = "banner";
            this.banner.Location = new Point(0, 0);
            this.banner.Dock = DockStyle.Top;
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.BackColor = System.Drawing.Color.White;
            this.copyrightLabel.Dock = DockStyle.Top;
            this.copyrightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(this.banner.ClientSize.Width, 28);
            this.copyrightLabel.TabIndex = 3;
            this.copyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // progressBar
            //
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = 0;
            this.progressBar.Value = 0;
            this.progressBar.Style = ProgressBarStyle.Marquee;
            this.progressBar.MarqueeAnimationSpeed = 30;
            this.progressBar.Dock = DockStyle.Top;
            this.progressBar.Size = new Size(this.banner.ClientSize.Width, 0);
            // 
            // SplashForm
            // 
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(
                this.banner.ClientSize.Width, 
                this.banner.ClientSize.Height + this.copyrightLabel.ClientSize.Height + this.progressBar.ClientSize.Height);
            this.ControlBox = false;
            this.Controls.Add(this.copyrightLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.banner);
            
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplashForm";
            this.ShowInTaskbar = false;
            
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }
        #endregion

    }
}
