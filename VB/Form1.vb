Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports DevExpress.XtraPrinting

Namespace IPrintableImplementation

    Public Partial Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub btnPrintPreview_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Create a list view.
            Dim printableListView As PrintableListView = CreatePrintableListView()
            ' Create a link.
            Dim link As PrintableComponentLink = New PrintableComponentLink(New PrintingSystem())
            ' Assign a list view to a link.
            link.Component = printableListView
            ' Show the Print Preview for a link.
            link.ShowPreviewDialog()
        End Sub

        ' This method creates an instance of the printable list view
        ' and adds some items to it.
        Private Function CreatePrintableListView() As PrintableListView
            Dim listView As PrintableListView = New PrintableListView()
            Dim columnHeader1 As ColumnHeader = New ColumnHeader()
            Dim columnHeader2 As ColumnHeader = New ColumnHeader()
            Dim columnHeader3 As ColumnHeader = New ColumnHeader()
            columnHeader1.Text = "Country"
            columnHeader1.Width = 99
            columnHeader2.Text = "Currency"
            columnHeader2.Width = 129
            columnHeader3.Text = "Capital"
            columnHeader3.Width = 81
            Dim listViewItem1 As ListViewItem = New ListViewItem(New ListViewItem.ListViewSubItem() {New ListViewItem.ListViewSubItem(Nothing, "Belgium", SystemColors.WindowText, SystemColors.Window, New Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (CByte(1)))), New ListViewItem.ListViewSubItem(Nothing, "Belgian Franc"), New ListViewItem.ListViewSubItem(Nothing, "Brussels")}, 0)
            Dim listViewItem2 As ListViewItem = New ListViewItem(New ListViewItem.ListViewSubItem() {New ListViewItem.ListViewSubItem(Nothing, "Brazil", SystemColors.WindowText, SystemColors.Window, New Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (CByte(1)))), New ListViewItem.ListViewSubItem(Nothing, "Real"), New ListViewItem.ListViewSubItem(Nothing, "Brasilia")}, 1)
            Dim listViewItem3 As ListViewItem = New ListViewItem(New ListViewItem.ListViewSubItem() {New ListViewItem.ListViewSubItem(Nothing, "Canada", SystemColors.WindowText, SystemColors.Window, New Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (CByte(1)))), New ListViewItem.ListViewSubItem(Nothing, "Canadian Dollar"), New ListViewItem.ListViewSubItem(Nothing, "Ottawa")}, 2)
            Dim listViewItem4 As ListViewItem = New ListViewItem(New ListViewItem.ListViewSubItem() {New ListViewItem.ListViewSubItem(Nothing, "Denmark", SystemColors.WindowText, SystemColors.Window, New Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (CByte(1)))), New ListViewItem.ListViewSubItem(Nothing, "Krone"), New ListViewItem.ListViewSubItem(Nothing, "Copenhagen")}, 3)
            Dim listViewItem5 As ListViewItem = New ListViewItem(New ListViewItem.ListViewSubItem() {New ListViewItem.ListViewSubItem(Nothing, "Finland", SystemColors.WindowText, SystemColors.Window, New Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, (CByte(1)))), New ListViewItem.ListViewSubItem(Nothing, "Markka"), New ListViewItem.ListViewSubItem(Nothing, "Helsinki")}, 4)
            listView.Columns.AddRange(New ColumnHeader() {columnHeader1, columnHeader2, columnHeader3})
            listView.GridLines = True
            listView.Items.AddRange(New ListViewItem() {listViewItem1, listViewItem2, listViewItem3, listViewItem4, listViewItem5})
            listView.View = View.Details
            Return listView
        End Function
    End Class
End Namespace
