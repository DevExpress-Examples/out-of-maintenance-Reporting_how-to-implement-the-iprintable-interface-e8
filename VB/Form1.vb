Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.Utils
Imports DevExpress.XtraPrinting

Namespace docIPrintable
    Partial Public Class Form1
        Inherits Form
        Public Sub New()
            InitializeComponent()
        End Sub
    End Class

    Public Class PrintableListView
        Inherits System.Windows.Forms.ListView
        Implements IPrintable
        Private imageList As ImageList = Nothing
        ' You have to work with interfaces but not classes.
        Private ps As IPrintingSystem
        Private brickGraph As IBrickGraphics


#Region "IBasePrintable implementation"
        Private Sub Initialize(ByVal ps As IPrintingSystem, ByVal link As ILink) Implements IBasePrintable.Initialize
            Me.ps = ps
            If (View = View.SmallIcon OrElse View = View.List OrElse View = View.Details) Then
                imageList = SmallImageList
            Else
                If (View = View.LargeIcon) Then
                    imageList = LargeImageList
                Else
                    imageList = Nothing
                End If
            End If

        End Sub
        Private Overloads Sub Finalize(ByVal ps As IPrintingSystem, ByVal link As ILink) Implements IBasePrintable.Finalize
        End Sub
        ' Constructs different bricks based on section type and information provided by the control.
        Private Sub CreateArea(ByVal areaName As String, ByVal graph As IBrickGraphics) Implements IBasePrintable.CreateArea
            Me.brickGraph = graph
            If areaName.Equals("PageFooter") Then
                CreatePageFooter()
            ElseIf areaName.Equals("DetailHeader") Then
                CreateDetailHeader()
            ElseIf areaName.Equals("Detail") Then
                CreateDetail()
            End If
        End Sub
#End Region

        ' Enables the PropertyEditor form.
        Private Function HasPropertyEditor() As Boolean Implements IPrintable.HasPropertyEditor
            Return True
        End Function

        ' Overrides the corresponding property and specifies the Printing Property editor to display.
        Private ReadOnly Property PropertyEditorControl() As UserControl Implements IPrintable.PropertyEditorControl
            Get
                Dim ctrl As UserControl = New UserControl()
                Return ctrl

            End Get
        End Property

        ' Determines whether this links generates intersected bricks.
        Private ReadOnly Property CreatesIntersectedBricks() As Boolean Implements IPrintable.CreatesIntersectedBricks
            Get
                Return False
            End Get
        End Property

        ' Enables the help system for the Property editor.
        Private Function SupportsHelp() As Boolean Implements IPrintable.SupportsHelp
            Return False
        End Function

        ' Invokes the help system for the Property editor. 
        Private Sub ShowHelp() Implements IPrintable.ShowHelp
        End Sub

        ' Applies all changes made by the Property editor.
        Private Sub AcceptChanges() Implements IPrintable.AcceptChanges
        End Sub

        ' Cancels changes made by the user in the Property editor. 
        Private Sub RejectChanges() Implements IPrintable.RejectChanges
        End Sub

        ' Implements methods for drawing bricks.
        Private Function DrawBrick(ByVal typeName As String, ByVal rect As RectangleF) As IBrick
            Dim brick As IBrick = ps.CreateBrick(typeName)
            Return brickGraph.DrawBrick(brick, rect)
        End Function

        Private Function DrawBrick(ByVal typeName As String, ByVal properties As Object(,), ByVal rect As RectangleF) As IBrick
            Dim brick As IBrick = ps.CreateBrick(typeName)
            brick.SetProperties(properties)
            Return brickGraph.DrawBrick(brick, rect)
        End Function

        Private Sub CreatePageFooter()
            Dim formatPagination As String = "Page {0} of {1}"
            Dim formatDate As String = "{0:MMMM yyyy}"
            brickGraph.SetProperty("Modifier", BrickModifier.MarginalFooter)

            Dim font As Font = New Font("Arial", 9)
            Dim height As Single = font.Height + 2
            Dim r As RectangleF = New RectangleF(0, 0, 0, height)

            DrawBrick("PageInfoBrick", New Object(,) {{"PageInfo", PageInfo.NumberOfTotal}, _
                {"Format", formatPagination}, {"Alignment", BrickAlignment.Far}, {"AutoWidth", True}, {"BorderWidth", 0}}, r)
            DrawBrick("PageInfoBrick", New Object(,) {{"PageInfo", PageInfo.DateTime}, _
                {"Format", formatDate}, {"Alignment", BrickAlignment.Near}, {"AutoWidth", True}, {"BorderWidth", 0}}, r)
        End Sub

        Private Sub CreateDetailHeader()
            If View <> View.Details Then
                Return
            End If

            Dim sf As StringFormat = New StringFormat(StringFormatFlags.NoWrap)
            sf.LineAlignment = StringAlignment.Near

            brickGraph.DefaultBrickStyle = New BrickStyle(BorderSide.All, 1, Color.Black, SystemColors.Control, SystemColors.ControlText, Me.Font, New BrickStringFormat(sf))

            Dim r As Rectangle = Rectangle.Empty
            r.Y = 1
            Dim i As Integer = 0
            Do While i < Columns.Count
                r.Width = Columns(i).Width
                r.Height = Font.Height + 4
                Dim brick As IBrick = DrawBrick("TextBrick", r)
                brick.SetProperty("Text", Columns(i).Text)
                r.Offset(Columns(i).Width, 0)
                i += 1
            Loop
        End Sub

        Private Sub CreateDetail()
            If View = View.Details Then CreateDetails()
        End Sub

        Private Sub DrawDetailImage(ByVal item As ListViewItem, ByVal bounds As Rectangle)
            Dim index As Integer = item.ImageIndex
            If index < 0 Then Return
            Dim r As Rectangle = bounds
            r.Size = imageList.ImageSize
            r.Offset(2, (bounds.Height - r.Height) Mod 2)
            Dim brick As IBrick = DrawBrick("ImageBrick", r)
            brick.SetProperties(New Object(,) {{"Image", imageList.Images(index)}, _
                {"Sides", BorderSide.None}, {"BackColor", Color.Transparent}})
        End Sub

        Private Sub DrawDetailText(ByVal item As ListViewItem, ByVal bounds As Rectangle)
            Dim r As Rectangle = bounds
            r.Width = bounds.Width - (imageList.ImageSize.Width + 2)
            r.Offset(bounds.Width - r.Width, 0)
            Dim brick As IBrick = DrawBrick("TextBrick", r)
            brick.SetProperties(New Object(,) {{"Text", item.SubItems(0).Text}, _
                {"Sides", BorderSide.None}, {"BackColor", Color.Transparent}})
        End Sub

        Private Sub CreateDetails()
            Dim r As Rectangle = Rectangle.Empty
            Dim brick As IBrick

            Dim sf As StringFormat = New StringFormat(StringFormatFlags.NoWrap Or StringFormatFlags.LineLimit)
            sf.LineAlignment = StringAlignment.Near
            brickGraph.DefaultBrickStyle.StringFormat = New BrickStringFormat(sf)
            brickGraph.DefaultBrickStyle.BackColor = SystemColors.Window
            brickGraph.DefaultBrickStyle.BorderColor = SystemColors.Control
            If GridLines Then
                brickGraph.DefaultBrickStyle.Sides = BorderSide.All
            Else
                brickGraph.DefaultBrickStyle.Sides = BorderSide.None
            End If

            Dim pt As Point = Point.Empty
            If Items.Count > 0 Then
                Dim bounds As Rectangle = Items(0).Bounds
                pt = bounds.Location
                pt.Y -= 3
            End If

            Dim i As Integer = 0
            Do While i < Items.Count
                Dim item As ListViewItem = Items(i)
                brickGraph.DefaultBrickStyle.Font = item.Font
                brickGraph.DefaultBrickStyle.BackColor = item.BackColor
                brickGraph.DefaultBrickStyle.ForeColor = item.ForeColor

                r = item.Bounds
                r.Offset(-pt.X, -pt.Y)

                Dim j As Integer = 0
                Do While j < Columns.Count
                    Dim column As ColumnHeader = Columns(j)
                    r.Width = column.Width

                    If j = 0 AndAlso Not imageList Is Nothing Then
                        brick = DrawBrick("VisualBrick", r)
                        DrawDetailImage(item, r)
                        DrawDetailText(item, r)
                    Else
                        brick = DrawBrick("TextBrick", r)
                        brick.SetProperty("Text", item.SubItems(j).Text)
                    End If
                    r.Offset(r.Width, 0)
                    j += 1
                Loop
                i += 1
            Loop
        End Sub

    End Class


End Namespace