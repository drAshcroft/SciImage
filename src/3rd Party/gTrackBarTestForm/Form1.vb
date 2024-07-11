Imports System.Drawing.Imaging

Public Class Form1
    Private Dude As Bitmap

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dude = PictureBox1.Image.Clone
        Dude.MakeTransparent(Dude.GetPixel(0, 0))

    End Sub

    Private Sub GTrackBar1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GTrackBar1.ValueChanged
        Me.Invalidate(New Rectangle(PictureBox1.Location.X, PictureBox1.Location.Y, PictureBox1.Width, PictureBox1.Height))
    End Sub

    Private Sub GTBarRed_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GTBarRed.ValueChanged
        pRed.BackColor = DimTheColor(Color.Red, 255 - GTBarRed.Value)
        pRGB.BackColor = Color.FromArgb(GTBarRed.Value, GTBarGreen.Value, GTBarBlue.Value)
    End Sub

    Private Sub GTBarGreen_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GTBarGreen.ValueChanged
        pGreen.BackColor = DimTheColor(Color.Lime, 255 - GTBarGreen.Value)
        pRGB.BackColor = Color.FromArgb(GTBarRed.Value, GTBarGreen.Value, GTBarBlue.Value)
    End Sub

    Private Sub GTBarBlue_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GTBarBlue.ValueChanged
        pBlue.BackColor = DimTheColor(Color.Blue, 255 - GTBarBlue.Value)
        pRGB.BackColor = Color.FromArgb(GTBarRed.Value, GTBarGreen.Value, GTBarBlue.Value)
    End Sub

    Function DimTheColor(ByVal DimColor As Color, ByVal DimDegree As Integer) As Color
        If DimColor = Color.Transparent Or DimDegree = 0 Then Return DimColor
        Dim ColorR As Integer = DimColor.R + DimDegree
        Dim ColorG As Integer = DimColor.G + DimDegree
        Dim ColorB As Integer = DimColor.B + DimDegree

        If ColorR > 255 Then ColorR = 255
        If ColorG > 255 Then ColorG = 255
        If ColorB > 255 Then ColorB = 255
        If ColorR < 0 Then ColorR = 0
        If ColorG < 0 Then ColorG = 0
        If ColorB < 0 Then ColorB = 0

        Return Color.FromArgb(ColorR, ColorG, ColorB)

    End Function


    Private Sub Form1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        Try

            'Use a ColorMatrix to create a Transparent Image
            Dim ia As ImageAttributes = New ImageAttributes()
            Dim cm As ColorMatrix = New ColorMatrix
            cm.Matrix33 = GTrackBar1.Value / 100
            ia.SetColorMatrix(cm)
            Using bm As Bitmap = New Bitmap(Dude.Width, Dude.Height)

                e.Graphics.DrawImage(Dude, _
                               New Rectangle(PictureBox1.Location.X, PictureBox1.Location.Y, _
                               Dude.Width, Dude.Height), 0, 0, _
                               Dude.Width, Dude.Height, GraphicsUnit.Pixel, ia)
            End Using
        Catch ex As Exception

        End Try
    End Sub

    Private Sub GTrackBar17_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GTrackBar17.ValueChanged
        GTrackBar14.Value = GTrackBar17.Value
        GTrackBar16.Value = GTrackBar17.Value

    End Sub

    'Private Sub GTrackBar6_ValueChanged(sender As Object, e As EventArgs) Handles GTrackBar6.ValueChanged
    '    Me.Text = GTrackBar6.ValueAdjusted.ToString

    'End Sub

Private Sub GTrackBar24_Scroll( sender As Object,  e As ScrollEventArgs) Handles GTrackBar24.Scroll
		Me.Text = e.NewValue & " " & e.OldValue & " " &  e.ScrollOrientation & " " &  e.Type

End Sub
End Class
