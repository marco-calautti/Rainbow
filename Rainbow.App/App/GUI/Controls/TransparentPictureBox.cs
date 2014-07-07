using Rainbow.ImgLib.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Rainbow.App.GUI.Controls
{
    public class TransparentPictureBox : UserControl
    {
        TextureFormat texture;

        public void SetTexture(TextureFormat tex)
        {
            Image img = tex.GetImage();
            this.Width = img.Width;
            this.Height = img.Height;
            texture = tex;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {

            base.OnPaint(pe);
            if (texture == null)
                return;
            Graphics graphics = pe.Graphics;

            int squareSize = 8;
            for (int y = 0; y < this.Height; y+=squareSize)
                for (int x = 0; x < this.Width ; x+=squareSize)
                    graphics.FillRectangle((x/squareSize + y/squareSize) % 2 == 0 ? Brushes.LightGray : Brushes.White, x, y, squareSize, squareSize);

            if (texture != null)
            {
                Image img = texture.GetImage();
                graphics.DrawImage(img, 0, 0, img.Width, img.Height);
            }

        }
    }
}
