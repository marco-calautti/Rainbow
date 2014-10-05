//Copyright (C) 2014+ Marco (Phoenix) Calautti.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 2.0.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License 2.0 for more details.

//A copy of the GPL 2.0 should have been included with the program.
//If not, see http://www.gnu.org/licenses/

//Official repository and contact information can be found at
//http://github.com/marco-calautti/Rainbow

using Rainbow.ImgLib.Formats;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Rainbow.App.GUI.Controls
{
    public class TransparentPictureBox : UserControl
    {
        private TextureFormat texture;
        private Color color;
        private bool chessboard;
        private int originalWidth, originalHeight;
        private float scaleFactor = 1.0f;

        public TransparentPictureBox()
        {
            Chessboard = true;
            color = PreferredTransparencyColor;
        }

        public void SetTexture(TextureFormat tex)
        {
            if (texture != null)
                texture.TextureChanged -= OnTextureChanged;

            texture = tex;
            originalWidth = tex.Width;
            originalHeight = tex.Height;
            Scale();
            texture.TextureChanged += OnTextureChanged;
            this.Invalidate();
        }

        public void SetTransparencyColor(Color color)
        {
            this.color = color;
            this.Invalidate();
        }

        public static Color PreferredTransparencyColor { get { return Color.LightGray; } }
        public bool Chessboard { get { return chessboard; } set { chessboard = value; this.Invalidate(); } }

        private void Scale()
        {
            int newWidth = (int)(originalWidth * scaleFactor);
            int newHeight = (int)(originalHeight * scaleFactor);

            Width = newWidth;
            Height = newHeight;
        }

        public void ScaleImage(float factor)
        {
            scaleFactor *= factor;
            Scale();
            this.Invalidate();
        }

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

            Image img = texture.GetImage();

            Bitmap b = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(img, 0, 0, b.Width, b.Height);
            g.Dispose();

            graphics.DrawImage(b, 0, 0, b.Width, b.Height);
        }

        private void OnTextureChanged(object sender, EventArgs e)
        {
            SetTexture((TextureFormat)sender);
        }
    }
}
