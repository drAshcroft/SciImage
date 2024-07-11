<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.GTrackBar2 = New gTrackBar.gTrackBar()
        Me.SuspendLayout()
        '
        'GTrackBar2
        '
        Me.GTrackBar2.BackColor = System.Drawing.SystemColors.Control
        Me.GTrackBar2.Label = Nothing
        Me.GTrackBar2.Location = New System.Drawing.Point(56, 92)
        Me.GTrackBar2.Margin = New System.Windows.Forms.Padding(2, 3, 2, 3)
        Me.GTrackBar2.MaxValue = 100
        Me.GTrackBar2.MinValue = -100
        Me.GTrackBar2.Name = "GTrackBar2"
        Me.GTrackBar2.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.GTrackBar2.Size = New System.Drawing.Size(43, 300)
        Me.GTrackBar2.SliderSize = New System.Drawing.Size(10, 20)
        Me.GTrackBar2.SliderWidthHigh = 1.0!
        Me.GTrackBar2.SliderWidthLow = 1.0!
        Me.GTrackBar2.TabIndex = 0
        Me.GTrackBar2.TickThickness = 1.0!
        Me.GTrackBar2.TickType = gTrackBar.gTrackBar.eTickType.Both
        Me.GTrackBar2.Value = 50
        Me.GTrackBar2.ValueAdjusted = 50.0!
        Me.GTrackBar2.ValueDivisor = gTrackBar.gTrackBar.eValueDivisor.e1
        Me.GTrackBar2.ValueStrFormat = Nothing
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(410, 467)
        Me.Controls.Add(Me.GTrackBar2)
        Me.Name = "Form2"
        Me.Text = "Test"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GTrackBar2 As gTrackBar.gTrackBar
End Class
