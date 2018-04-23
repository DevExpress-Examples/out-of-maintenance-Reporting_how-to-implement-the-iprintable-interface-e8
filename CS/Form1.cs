using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPrinting;

namespace IPrintableImplementation {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }

        private void btnPrintPreview_Click(object sender, EventArgs e) {
            // Create a list view.
            PrintableListView printableListView = CreatePrintableListView();

            // Create a link.
            PrintableComponentLink link = new PrintableComponentLink(new PrintingSystem());

            // Assign a list view to a link.
            link.Component = printableListView;

            // Show the Print Preview for a link.
            link.ShowPreviewDialog();
        }

        // This method creates an instance of the printable list view
        // and adds some items to it.
        private PrintableListView CreatePrintableListView() {
            PrintableListView listView = new PrintableListView();

            ColumnHeader columnHeader1 = new ColumnHeader();
            ColumnHeader columnHeader2 = new ColumnHeader();
            ColumnHeader columnHeader3 = new ColumnHeader();

            columnHeader1.Text = "Country";
            columnHeader1.Width = 99;

            columnHeader2.Text = "Currency";
            columnHeader2.Width = 129;

            columnHeader3.Text = "Capital";
            columnHeader3.Width = 81;

            ListViewItem listViewItem1 = new ListViewItem(new ListViewItem.ListViewSubItem[] {
			    new ListViewItem.ListViewSubItem(null, "Belgium", SystemColors.WindowText, SystemColors.Window, 
                    new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, 
                        GraphicsUnit.Point, ((System.Byte)(1)))),
			    new ListViewItem.ListViewSubItem(null, "Belgian Franc"),
			    new ListViewItem.ListViewSubItem(null, "Brussels")}, 0);
            ListViewItem listViewItem2 = new ListViewItem(new ListViewItem.ListViewSubItem[] {
				new ListViewItem.ListViewSubItem(null, "Brazil", SystemColors.WindowText, SystemColors.Window, 
                    new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, 
                        GraphicsUnit.Point, ((System.Byte)(1)))),
				new ListViewItem.ListViewSubItem(null, "Real"),
				new ListViewItem.ListViewSubItem(null, "Brasilia")}, 1);
            ListViewItem listViewItem3 = new ListViewItem(new ListViewItem.ListViewSubItem[] {
				new ListViewItem.ListViewSubItem(null, "Canada", SystemColors.WindowText, SystemColors.Window, 
                    new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, 
                        GraphicsUnit.Point, ((System.Byte)(1)))),
				new ListViewItem.ListViewSubItem(null, "Canadian Dollar"),
				new ListViewItem.ListViewSubItem(null, "Ottawa")}, 2);
            ListViewItem listViewItem4 = new ListViewItem(new ListViewItem.ListViewSubItem[] {
				new ListViewItem.ListViewSubItem(null, "Denmark", SystemColors.WindowText, SystemColors.Window, 
                    new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, 
                        GraphicsUnit.Point, ((System.Byte)(1)))),
				new ListViewItem.ListViewSubItem(null, "Krone"),
				new ListViewItem.ListViewSubItem(null, "Copenhagen")}, 3);
            ListViewItem listViewItem5 = new ListViewItem(new ListViewItem.ListViewSubItem[] {
				new ListViewItem.ListViewSubItem(null, "Finland", SystemColors.WindowText, SystemColors.Window, 
                    new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, 
                        GraphicsUnit.Point, ((System.Byte)(1)))),
				new ListViewItem.ListViewSubItem(null, "Markka"),
				new ListViewItem.ListViewSubItem(null, "Helsinki")}, 4);

            listView.Columns.AddRange(new ColumnHeader[] {
                 columnHeader1,
                 columnHeader2,
                 columnHeader3});
            listView.GridLines = true;
            listView.Items.AddRange(new ListViewItem[] {
	             listViewItem1,
	             listViewItem2,
	             listViewItem3,
	             listViewItem4,
	             listViewItem5});

            listView.View = View.Details;

            return listView;
        }

    }

}