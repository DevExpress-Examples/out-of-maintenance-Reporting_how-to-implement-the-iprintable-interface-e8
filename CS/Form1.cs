using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraPrinting;

namespace docIPrintable {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }
    }

    public class PrintableListView : System.Windows.Forms.ListView, IPrintable {

        private ImageList imageList = null;
        // You have to work with interfaces but not classes.
        private IPrintingSystem ps;
        private IBrickGraphics brickGraph;

        #region IBasePrintable implementation
        void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
            this.ps = ps;
            imageList = (View == View.SmallIcon || View == View.List || View == View.Details) ? SmallImageList :
                (View == View.LargeIcon) ? LargeImageList : null;
        }
        void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
        }
        // Constructs different bricks based on section type and information provided by the control.
        void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
            this.brickGraph = graph;
            if (areaName.Equals("InnerPageFooter"))
                CreatePageFooter();
            else if (areaName.Equals("DetailHeader"))
                CreateDetailHeader();
            else if (areaName.Equals("Detail"))
                CreateDetail();
        }
        #endregion

        // Enables the PropertyEditor form.
        bool IPrintable.HasPropertyEditor() {
            return true;
        }

        // Overrides the corresponding property and specifies the Printing Property editor to display.
        UserControl IPrintable.PropertyEditorControl {
            get {
                UserControl ctrl = new UserControl();
                return ctrl;
            }
        }

        // Enables the help system for the Property editor.
        bool IPrintable.SupportsHelp() {
            return false;
        }

        // Determines whether intersected bricks are created by this link.
        bool IPrintable.CreatesIntersectedBricks {
            get {
                return false;
            }
        }


        // Invokes the help system for the Property editor. 
        void IPrintable.ShowHelp() {
        }

        // Applies all changes made by the Property editor.
        void IPrintable.AcceptChanges() {
        }

        // Cancels changes made by the user in the Property editor. 
        void IPrintable.RejectChanges() {
        }

        // Implements methods for drawing bricks.
        private IBrick DrawBrick(string typeName, RectangleF rect) {
            IBrick brick = ps.CreateBrick(typeName);
            return brickGraph.DrawBrick(brick, rect);
        }

        private IBrick DrawBrick(string typeName, object[,] properties, RectangleF rect) {
            IBrick brick = ps.CreateBrick(typeName);
            brick.SetProperties(properties);
            return brickGraph.DrawBrick(brick, rect);
        }

        private void CreatePageFooter() {
            string formatPagination = "Page {0} of {1}";
            string formatDate = "{0:MMMM yyyy}";
            brickGraph.SetProperty("Modifier", BrickModifier.MarginalFooter);

            Font font = new Font("Arial", 9);
            float height = font.Height + 2;
            RectangleF r = new RectangleF(0, 0, 0, height);

            DrawBrick("PageInfoBrick", new object[,] { {"PageInfo",PageInfo.NumberOfTotal}, {"Format",formatPagination},
                {"Alignment",BrickAlignment.Far}, {"AutoWidth",true}, {"BorderWidth",0} }, r);
            DrawBrick("PageInfoBrick", new object[,] { { "PageInfo", PageInfo.DateTime }, { "Format", formatDate },
                { "Alignment", BrickAlignment.Near }, { "AutoWidth",true }, {"BorderWidth",0} }, r);
        }

        private void CreateDetailHeader() {
            if (View != View.Details) return;

            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
            sf.LineAlignment = StringAlignment.Near;
            BrickStyle brStyle = new BrickStyle(BorderSide.All, 1, Color.Black,
                SystemColors.Control, SystemColors.ControlText, this.Font, new BrickStringFormat(sf));
            brickGraph.DefaultBrickStyle = brStyle;

            Rectangle r = Rectangle.Empty;
            r.Y = 1;
            for (int i = 0; i < Columns.Count; i++) {
                r.Width = Columns[i].Width;
                r.Height = Font.Height + 4;
                IBrick brick = DrawBrick("TextBrick", r);
                brick.SetProperty("Text", Columns[i].Text);
                r.Offset(Columns[i].Width, 0);
            }
        }

        private void CreateDetail() {
            if (View == View.Details) CreateDetails();
            //else if (View == View.LargeIcon || View == View.SmallIcon || View == View.List)
            //    CreateIcons();
        }

        private void DrawDetailImage(ListViewItem item, Rectangle bounds) {
            int index = item.ImageIndex;
            if (index < 0) return;
            Rectangle r = bounds;
            r.Size = imageList.ImageSize;
            r.Offset(2, (bounds.Height - r.Height) / 2);
            IBrick brick = DrawBrick("ImageBrick", r);
            brick.SetProperties(new object[,] { { "Image", imageList.Images[index] },
                { "Sides", BorderSide.None }, { "BackColor", Color.Transparent } });
        }

        private void DrawDetailText(ListViewItem item, Rectangle bounds) {
            Rectangle r = bounds;
            r.Width = bounds.Width - (imageList.ImageSize.Width + 2);
            r.Offset(bounds.Width - r.Width, 0);
            IBrick brick = DrawBrick("TextBrick", r);
            brick.SetProperties(new object[,] { { "Text", item.SubItems[0].Text },
                { "Sides", BorderSide.None }, { "BackColor", Color.Transparent } });
        }


        private void CreateDetails() {
            Rectangle r = Rectangle.Empty;
            IBrick brick;

            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit);
            sf.LineAlignment = StringAlignment.Near;
            BrickStyle brStyle = new BrickStyle(GridLines ? BorderSide.All : BorderSide.None, 1, SystemColors.Control,
                SystemColors.Window, SystemColors.ControlText, this.Font, new BrickStringFormat(sf));
            brickGraph.DefaultBrickStyle = brStyle;

            Point pt = Point.Empty;
            if (Items.Count > 0) {
                Rectangle bounds = Items[0].Bounds;
                pt = bounds.Location;
                pt.Y -= 3;
            }

            for (int i = 0; i < Items.Count; i++) {
                ListViewItem item = Items[i];
                brickGraph.DefaultBrickStyle.Font = item.Font;
                brickGraph.DefaultBrickStyle.BackColor = item.BackColor;
                brickGraph.DefaultBrickStyle.ForeColor = item.ForeColor;

                r = item.Bounds;
                r.Offset(-pt.X, -pt.Y);

                for (int j = 0; j < Columns.Count; j++) {
                    ColumnHeader column = Columns[j];
                    r.Width = column.Width;

                    if (j == 0 && imageList != null) {
                        brick = DrawBrick("VisualBrick", r);
                        DrawDetailImage(item, r);
                        DrawDetailText(item, r);
                    }
                    else {
                        brick = DrawBrick("TextBrick", r);
                        brick.SetProperty("Text", item.SubItems[j].Text);
                    }
                    r.Offset(r.Width, 0);
                }
            }
        }

    }

}