Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel
Imports DevExpress.XtraPrinting

Namespace IPrintableImplementation

    Public Class PrintableListView
        Inherits ListView
        Implements IPrintable

        Private components As Container = Nothing

        Private ps As IPrintingSystem

        Private graph As IBrickGraphics

        Private offsetx As Integer = 0

        Private imageList As ImageList = Nothing

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If components IsNot Nothing Then components.Dispose()
            End If

            MyBase.Dispose(disposing)
        End Sub

#Region "#IBasePrintable"
        Private Sub Initialize(ByVal ps As PrintingSystemBase, ByVal link As LinkBase) Implements IBasePrintable.Initialize
            Me.ps = ps
            imageList = If(View = View.SmallIcon OrElse View = View.List OrElse View = View.Details, SmallImageList, If(View = View.LargeIcon, LargeImageList, Nothing))
            offsetx = If(imageList Is Nothing, 0, imageList.ImageSize.Height)
        End Sub

        Private Sub IBasePrintable_Finalize(ByVal ps As PrintingSystemBase, ByVal link As LinkBase) Implements IBasePrintable.Finalize
        End Sub

        ' Constructs different bricks based on section type and information provided by the control.
        Private Sub CreateArea(ByVal areaName As String, ByVal graph As BrickGraphics) Implements IBasePrintable.CreateArea
            Me.graph = graph
            If areaName.Equals("PageFooter") Then
                CreatePageFooter()
            ElseIf areaName.Equals("DetailHeader") Then
                CreateDetailHeader()
            ElseIf areaName.Equals("Detail") Then
                CreateDetail()
            End If
        End Sub

        ' Determines whether intersected bricks are created by this link.
        Private ReadOnly Property CreatesIntersectedBricks As Boolean Implements IBasePrintable.CreatesIntersectedBricks
            Get
                Return True
            End Get
        End Property

#End Region  ' #IBasePrintable
#Region "#IPrintable"
        ' Enables the Property Editor form.
        Private Function HasPropertyEditor() As Boolean Implements IPrintable.HasPropertyEditor
            Return True
        End Function

        ' Overrides the corresponding property and specifies the Property Editor to display.
        Private ReadOnly Property PropertyEditorControl As UserControl Implements IPrintable.PropertyEditorControl
            Get
                Dim ctrl As UserControl = New UserControl()
                Return ctrl
            End Get
        End Property

        ' Enables the help system for the Property Editor.
        Private Function SupportsHelp() As Boolean Implements IPrintable.SupportsHelp
            Return False
        End Function

        ' Invokes the help system for the Property editor. 
        Private Sub ShowHelp() Implements IPrintable.ShowHelp
        End Sub

        ' Applies all changes made by the Property Editor.
        Private Sub AcceptChanges() Implements IPrintable.AcceptChanges
        End Sub

        ' Cancels changes made by the user in the Property editor. 
        Private Sub RejectChanges() Implements IPrintable.RejectChanges
        End Sub

#End Region  ' #IPrintable
#Region "#DrawBrick"
        Private Function DrawBrick(ByVal typeName As String, ByVal rect As RectangleF) As IBrick
            Dim brick As IBrick = ps.CreateBrick(typeName)
            Return graph.DrawBrick(brick, rect)
        End Function

        Private Function DrawBrick(ByVal typeName As String, ByVal properties As Object(,), ByVal rect As RectangleF) As IBrick
            Dim brick As IBrick = ps.CreateBrick(typeName)
            brick.SetProperties(properties)
            Return graph.DrawBrick(brick, rect)
        End Function

#End Region  ' #DrawBrick
#Region "#CreateDetail"
        Private Sub CreateDetail()
            If View = View.Details Then
                CreateDetails()
            ElseIf View = View.LargeIcon OrElse View = View.SmallIcon OrElse View = View.List Then
                CreateIcons()
            End If
        End Sub

        Private Sub CreateDetails()
            Dim r As Rectangle = Rectangle.Empty
            Dim sf As StringFormat = New StringFormat(StringFormatFlags.NoWrap Or StringFormatFlags.LineLimit)
            sf.LineAlignment = StringAlignment.Near
            graph.DefaultBrickStyle.StringFormat = New BrickStringFormat(sf)
            graph.DefaultBrickStyle.BackColor = SystemColors.Window
            graph.DefaultBrickStyle.BorderColor = SystemColors.Control
            graph.DefaultBrickStyle.Sides = If(GridLines, BorderSide.All, BorderSide.None)
            Dim pt As Point = Point.Empty
            If Items.Count > 0 Then
                Dim bounds As Rectangle = Items(0).Bounds
                pt = bounds.Location
                pt.Y -= 3
            End If

            For i As Integer = 0 To Items.Count - 1
                Dim item As ListViewItem = Items(i)
                graph.DefaultBrickStyle.Font = item.Font
                graph.DefaultBrickStyle.BackColor = item.BackColor
                graph.DefaultBrickStyle.ForeColor = item.ForeColor
                r = item.Bounds
                r.Offset(-pt.X, -pt.Y)
                For j As Integer = 0 To Columns.Count - 1
                    Dim column As ColumnHeader = Columns(j)
                    r.Width = column.Width
                    If j = 0 AndAlso imageList IsNot Nothing Then
                        DrawBrick("VisualBrick", r)
                        DrawDetailImage(item, r)
                        DrawDetailText(item, r)
                    Else
                        Dim brick As TextBrick = CType(DrawBrick("TextBrick", r), TextBrick)
                        brick.Text = item.SubItems(j).Text
                    End If

                    r.Offset(r.Width, 0)
                Next
            Next
        End Sub

        Private Sub CreateIcons()
            graph.DefaultBrickStyle.BackColor = Color.Transparent
            graph.DefaultBrickStyle.BorderColor = Color.Black
            graph.DefaultBrickStyle.Sides = BorderSide.None
            Dim imageSize As Size = Size.Empty
            If offsetx <> 0 Then
                For i As Integer = 0 To Items.Count - 1
                    Dim item As ListViewItem = Items(i)
                    Dim index As Integer = item.ImageIndex
                    Dim image As Image = imageList.Images(index)
                    Dim r As Rectangle = item.Bounds
                    imageSize = imageList.ImageSize
                    r.Size = imageSize
                    If index < 0 Then
                        DrawBrick("VisualBrick", r)
                    Else
                        Dim brick As ImageBrick = CType(DrawBrick("ImageBrick", r), ImageBrick)
                        brick.Image = image
                    End If
                Next

                offsetx += 3
            End If

            graph.DefaultBrickStyle.StringFormat = New BrickStringFormat(StringFormatFlags.LineLimit, StringAlignment.Near, StringAlignment.Near)
            For i As Integer = 0 To Items.Count - 1
                Dim item As ListViewItem = Items(i)
                graph.DefaultBrickStyle.Font = item.Font
                graph.DefaultBrickStyle.BackColor = If((item.BackColor = SystemColors.Window), Color.Transparent, item.BackColor)
                graph.DefaultBrickStyle.ForeColor = item.ForeColor
                Dim r As RectangleF = RectangleF.Empty
                r.Size = MeasureString(item.Text)
                If r.Width > 59 Then
                    r.Width = 59
                    r.Height = 29
                End If

                r.X = item.Bounds.Left
                If item.Bounds.Width > item.Bounds.Height Then r.X += imageSize.Width
                r.Y = item.Bounds.Bottom - r.Height
                Dim brick As TextBrick = CType(DrawBrick("TextBrick", r), TextBrick)
                brick.Text = item.Text
            Next
        End Sub

        Private Sub DrawDetailImage(ByVal item As ListViewItem, ByVal bounds As Rectangle)
            Dim index As Integer = item.ImageIndex
            If index < 0 Then Return
            Dim r As Rectangle = bounds
            r.Size = imageList.ImageSize
            r.Offset(2, (bounds.Height - r.Height) \ 2)
            Dim brick As IBrick = DrawBrick("ImageBrick", r)
            brick.SetProperties(New Object(,) {{"Image", imageList.Images(index)}, {"Sides", BorderSide.None}, {"BackColor", Color.Transparent}})
        End Sub

        Private Sub DrawDetailText(ByVal item As ListViewItem, ByVal bounds As Rectangle)
            Dim r As Rectangle = bounds
            r.Width = bounds.Width - (imageList.ImageSize.Width + 2)
            r.Offset(bounds.Width - r.Width, 0)
            Dim brick As IBrick = DrawBrick("TextBrick", r)
            brick.SetProperties(New Object(,) {{"Text", item.SubItems(0).Text}, {"Sides", BorderSide.None}, {"BackColor", Color.Transparent}})
        End Sub

        Private Function GetCellBounds(ByVal pi As Integer, ByVal pj As Integer) As Rectangle
            Dim r As Rectangle = Rectangle.Empty
            For i As Integer = 0 To pi - 1
                r.X += Columns(i).Width
            Next

            r.Y += Font.Height + 4
            For i As Integer = 0 To pj - 1
                r.Y += Items(i).Bounds.Y
            Next

            r.Width = Columns(pi).Width
            r.Height = Items(pj).Bounds.Height
            Return r
        End Function

        Private Function MeasureString(ByVal text As String) As SizeF
            Dim graphics As Graphics = Graphics.FromHwnd(New IntPtr(0))
            Dim size As SizeF = graphics.MeasureString(text, Font)
            size.Width += 2 ' Border size
            size.Height += 2 ' Border size
            graphics.Dispose()
            Return size
        End Function

#End Region  ' #CreateDetail
#Region "#CreatePageFooter"
        Private Sub CreatePageFooter()
            Dim format As String = "Page {0} of {1}"
            Dim font As Font = New Font("Arial", 9)
            graph.DefaultBrickStyle = New BrickStyle(BorderSide.None, 1, Color.Black, Color.Transparent, Color.Black, font, New BrickStringFormat(StringAlignment.Center, StringAlignment.Center))
            Dim height As Single = font.Height + 2
            Dim r As RectangleF = New RectangleF(0, 0, 0, height)
            DrawBrick("PageInfoBrick", New Object(,) {{"PageInfo", PageInfo.NumberOfTotal}, {"Format", format}, {"Alignment", BrickAlignment.Far}, {"AutoWidth", True}}, r)
            DrawBrick("PageInfoBrick", New Object(,) {{"Alignment", BrickAlignment.Near}, {"AutoWidth", True}, {"PageInfo", PageInfo.DateTime}}, r)
        End Sub

#End Region  ' #CreatePageFooter
#Region "#CreateDetailHeader"
        Private Sub CreateDetailHeader()
            If View <> View.Details Then Return
            Dim sf As StringFormat = New StringFormat(StringFormatFlags.NoWrap)
            sf.LineAlignment = StringAlignment.Near
            graph.DefaultBrickStyle = New BrickStyle(BorderSide.All, 1, Color.Black, SystemColors.Control, SystemColors.ControlText, Font, New BrickStringFormat(sf))
            Dim r As Rectangle = Rectangle.Empty
            r.Y = 1
            For i As Integer = 0 To Columns.Count - 1
                r.Width = Columns(i).Width
                r.Height = Font.Height + 4
                Dim brick As TextBrick = CType(DrawBrick("TextBrick", r), TextBrick)
                brick.Text = Columns(i).Text
                r.Offset(Columns(i).Width, 0)
            Next
        End Sub
#End Region  ' #CreateDetailHeader
    End Class
End Namespace
