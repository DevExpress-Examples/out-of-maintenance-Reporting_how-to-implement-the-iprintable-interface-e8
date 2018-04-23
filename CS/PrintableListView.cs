using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraPrinting;

namespace IPrintableImplementation {

    public class PrintableListView : ListView, IPrintable {
        private Container components = null;
        private IPrintingSystem ps;
        private IBrickGraphics graph;
        private int offsetx = 0;
        private ImageList imageList = null;

        public PrintableListView() {
            InitializeComponent();
        }

        private void InitializeComponent() {
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region #IBasePrintable
        void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
            this.ps = ps;
            imageList = (View == View.SmallIcon || View == View.List || View == View.Details)
                ? SmallImageList : (View == View.LargeIcon) ? LargeImageList : null;
            offsetx = (imageList == null) ? 0 : imageList.ImageSize.Height;
        }

        void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
        }

        // Constructs different bricks based on section type and information provided by the control.
        void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
            this.graph = graph;
            if (areaName.Equals("PageFooter"))
                CreatePageFooter();
            else if (areaName.Equals("DetailHeader"))
                CreateDetailHeader();
            else if (areaName.Equals("Detail"))
                CreateDetail();
        }
        #endregion #IBasePrintable

        #region #IPrintable
        // Enables the Property Editor form.
        bool IPrintable.HasPropertyEditor() {
            return true;
        }

        // Overrides the corresponding property and specifies the Property Editor to display.
        UserControl IPrintable.PropertyEditorControl {
            get {
                UserControl ctrl = new UserControl();
                return ctrl;
            }
        }

        // Enables the help system for the Property Editor.
        bool IPrintable.SupportsHelp() {
            return false;
        }

        // Invokes the help system for the Property editor. 
        void IPrintable.ShowHelp() {
        }

        // Determines whether intersected bricks are created by this link.
        bool IPrintable.CreatesIntersectedBricks {
            get { return true; }
        }

        // Applies all changes made by the Property Editor.
        void IPrintable.AcceptChanges() {
        }

        // Cancels changes made by the user in the Property editor. 
        void IPrintable.RejectChanges() {
        }
        #endregion #IPrintable

        #region #DrawBrick
        private IBrick DrawBrick(string typeName, RectangleF rect) {
            IBrick brick = ps.CreateBrick(typeName);
            return graph.DrawBrick(brick, rect);
        }

        private IBrick DrawBrick(string typeName, object[,] properties, RectangleF rect) {
            IBrick brick = ps.CreateBrick(typeName);
            brick.SetProperties(properties);
            return graph.DrawBrick(brick, rect);
        }
        #endregion #DrawBrick

        #region #CreateDetail
        private void CreateDetail() {
            if (View == View.Details) CreateDetails();
            else if (View == View.LargeIcon || View == View.SmallIcon || View == View.List)
                CreateIcons();
        }

        private void CreateDetails() {
            Rectangle r = Rectangle.Empty;

            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit);
            sf.LineAlignment = StringAlignment.Near;
            graph.DefaultBrickStyle.StringFormat = new BrickStringFormat(sf);
            graph.DefaultBrickStyle.BackColor = SystemColors.Window;
            graph.DefaultBrickStyle.BorderColor = SystemColors.Control;
            graph.DefaultBrickStyle.Sides = GridLines ? BorderSide.All : BorderSide.None;

            Point pt = Point.Empty;
            if (Items.Count > 0) {
                Rectangle bounds = Items[0].Bounds;
                pt = bounds.Location;
                pt.Y -= 3;
            }

            for (int i = 0; i < Items.Count; i++) {
                ListViewItem item = Items[i];
                graph.DefaultBrickStyle.Font = item.Font;
                graph.DefaultBrickStyle.BackColor = item.BackColor;
                graph.DefaultBrickStyle.ForeColor = item.ForeColor;

                r = item.Bounds;
                r.Offset(-pt.X, -pt.Y);

                for (int j = 0; j < Columns.Count; j++) {
                    ColumnHeader column = Columns[j];
                    r.Width = column.Width;

                    if (j == 0 && imageList != null) {
                        DrawBrick("VisualBrick", r);
                        DrawDetailImage(item, r);
                        DrawDetailText(item, r);
                    } else {
                        TextBrick brick = (TextBrick)DrawBrick("TextBrick", r);
                        brick.Text = item.SubItems[j].Text;
                    }
                    r.Offset(r.Width, 0);
                }
            }
        }

        private void CreateIcons() {
            graph.DefaultBrickStyle.BackColor = Color.Transparent;
            graph.DefaultBrickStyle.BorderColor = Color.Black;
            graph.DefaultBrickStyle.Sides = BorderSide.None;
            Size imageSize = Size.Empty;

            if (offsetx != 0) {
                for (int i = 0; i < Items.Count; i++) {
                    ListViewItem item = Items[i];
                    int index = item.ImageIndex;
                    Image image = imageList.Images[index];

                    Rectangle r = item.Bounds;
                    imageSize = imageList.ImageSize;
                    r.Size = imageSize;

                    if (index < 0)
                        DrawBrick("VisualBrick", r);
                    else {
                        ImageBrick brick = (ImageBrick)DrawBrick("ImageBrick", r);
                        brick.Image = image;
                    }
                }
                offsetx += 3;
            }

            graph.DefaultBrickStyle.StringFormat = new BrickStringFormat(StringFormatFlags.LineLimit,
                StringAlignment.Near, StringAlignment.Near);

            for (int i = 0; i < Items.Count; i++) {
                ListViewItem item = Items[i];
                graph.DefaultBrickStyle.Font = item.Font;
                graph.DefaultBrickStyle.BackColor = (item.BackColor == SystemColors.Window)
                    ? Color.Transparent : item.BackColor;
                graph.DefaultBrickStyle.ForeColor = item.ForeColor;

                RectangleF r = RectangleF.Empty;
                r.Size = MeasureString(item.Text);
                if (r.Width > 59) {
                    r.Width = 59;
                    r.Height = 29;
                }

                r.X = item.Bounds.Left;
                if (item.Bounds.Width > item.Bounds.Height) r.X += imageSize.Width;
                r.Y = item.Bounds.Bottom - r.Height;
                TextBrick brick = (TextBrick)DrawBrick("TextBrick", r);
                brick.Text = item.Text;
            }
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

        private Rectangle GetCellBounds(int pi, int pj) {
            Rectangle r = Rectangle.Empty;
            for (int i = 0; i < pi; i++)
                r.X += Columns[i].Width;
                
            r.Y += Font.Height + 4;

            for (int i = 0; i < pj; i++)
                r.Y += Items[i].Bounds.Y;
            r.Width = Columns[pi].Width;
            r.Height = Items[pj].Bounds.Height;
            return r;
        }

        private SizeF MeasureString(string text) {
            Graphics graphics = Graphics.FromHwnd(new IntPtr(0));
            SizeF size = graphics.MeasureString(text, Font);
            size.Width += 2;	// Border size
            size.Height += 2;	// Border size
            graphics.Dispose();
            return size;
        }
        #endregion #CreateDetail

        #region #CreatePageFooter
        private void CreatePageFooter() {
            string format = "Page {0} of {1}";
            Font font = new Font("Arial", 9);
            graph.DefaultBrickStyle = new BrickStyle(BorderSide.None, 1,
                Color.Black, Color.Transparent, Color.Black, font,
                new BrickStringFormat(StringAlignment.Center, StringAlignment.Center));

            float height = font.Height + 2;

            RectangleF r = new RectangleF(0, 0, 0, height);

            DrawBrick("PageInfoBrick", new object[,] { {"PageInfo",PageInfo.NumberOfTotal}, 
				{"Format",format}, {"Alignment",BrickAlignment.Far}, {"AutoWidth",true} }, r);
            DrawBrick("PageInfoBrick", new object[,] { {"Alignment",BrickAlignment.Near},
				{"AutoWidth",true}, {"PageInfo",PageInfo.DateTime} }, r);
        }
        #endregion #CreatePageFooter

        #region #CreateDetailHeader
        private void CreateDetailHeader() {
            if (View != View.Details) return;

            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
            sf.LineAlignment = StringAlignment.Near;

            graph.DefaultBrickStyle = new BrickStyle(BorderSide.All, 1, Color.Black,
                SystemColors.Control, SystemColors.ControlText, this.Font, new BrickStringFormat(sf));

            Rectangle r = Rectangle.Empty;
            r.Y = 1;
            for (int i = 0; i < Columns.Count; i++) {
                r.Width = Columns[i].Width;
                r.Height = Font.Height + 4;
                TextBrick brick = (TextBrick)DrawBrick("TextBrick", r);
                brick.Text = Columns[i].Text;
                r.Offset(Columns[i].Width, 0);
            }
        }
        #endregion #CreateDetailHeader
        
    }
}
