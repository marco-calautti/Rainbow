using Rainbow.ImgLib.Formats;
using System.Drawing;
using System.Windows.Forms;

namespace Rainbow.App.GUI.Controls
{
    public class TransparentPictureBox : UserControl
    {
        private TextureFormat texture;
        private Color color;
        private bool chessboard;

        public TransparentPictureBox()
        {
            Chessboard = true;
            color = PreferredTransparencyColor;
        }

        public void SetTexture(TextureFormat tex)
        {
            this.Width = tex.Width;
            this.Height = tex.Height;
            texture = tex;
            this.Invalidate();
        }

        public void SetTransparencyColor(Color color)
        {
            this.color = color;
            this.Invalidate();
        }

        public static Color PreferredTransparencyColor { get { return Color.LightGray; } }
        public bool Chessboard { get { return chessboard; } set { chessboard = value; this.Invalidate(); } }

        protected override void OnPaint(PaintEventArgs pe)
        {

            base.OnPaint(pe);
            if (texture == null)
                return;

            Graphics graphics = pe.Graphics;

            Brush brush1, brush2;
            if (chessboard)
            {
                brush1 = new SolidBrush(color.IsEmpty ? PreferredTransparencyColor : color);
                brush2 = new SolidBrush(Color.White);
            }
            else
            {
                brush1 = brush2 = new SolidBrush(color.IsEmpty ? PreferredTransparencyColor : color);
            }


            int squareSize = 8;
            for (int y = 0; y < this.Height; y += squareSize)
                for (int x = 0; x < this.Width; x += squareSize)
                {
                    graphics.FillRectangle((x / squareSize + y / squareSize) % 2 == 0 ? brush1 : brush2, x, y, squareSize, squareSize);
                }


            if (texture != null)
            {
                Image img = texture.GetImage();
                graphics.DrawImage(img, 0, 0, img.Width, img.Height);
            }

        }
    }
}
