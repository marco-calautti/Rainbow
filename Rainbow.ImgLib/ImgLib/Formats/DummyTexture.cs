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

using System;
using System.Drawing;

namespace Rainbow.ImgLib.Formats
{
    /// <summary>
    /// Texture format used as a place holder for frames not representing image data. For example
    /// in animation/transformation packages where each frame might be an image or a transformation.
    /// </summary>
	internal class DummyTexture : TextureFormatBase
	{
		private readonly Bitmap img;

        /// <summary>
        /// Constructs a dummy texture having as image the given text.
        /// </summary>
        /// <param name="text">Text.</param>
		public DummyTexture(string text)
			: this(text,new Font("Arial",20,FontStyle.Bold),Color.WhiteSmoke,Color.Red)
		{ }

        /// <summary>
        /// Constructs a dummy texture having as image the given text written with the given font,
        /// background color and text color.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="font">Font.</param>
        /// <param name="bgColor">Background color.</param>
        /// <param name="textColor">Text color.</param>
		public DummyTexture (string text, Font font, Color bgColor, Color textColor)
		{
			SizeF textSize;

			//first, create a dummy bitmap just to get a graphics object
			using (img = new Bitmap(1, 1))
			{
				using (var g = Graphics.FromImage(img))
				{
					//measure the string to see how big the image needs to be
					textSize = g.MeasureString(text, font);
				}
			}


			img = new Bitmap((int) textSize.Width, (int)textSize.Height);

			using (var g = Graphics.FromImage(img))
			{
				//paint the background
				g.Clear(bgColor);

				//create a brush for the text
				using (var textBrush = new SolidBrush(textColor))
				{
					g.DrawString(text, font, textBrush, 0, 0);
					g.Save();
				}
			}
		}

		public override int FramesCount 
		{
			get 
			{
				return 1;
			}
		}

		public override string Name 
		{
			get 
			{
				return "Dummy Texture";
			}
		}

		public override int PalettesCount 
		{
			get 
			{
				return 0;
			}
		}

		protected override Image GetImage (int activeFrame, int activePalette)
		{
			return img;
		}

        protected override Color[] GetPalette(int activePalette)
        {
            return null;
        }

		public override Image GetReferenceImage ()
		{
			return null;
		}

		public override int Height 
		{
			get 
			{
				return img.Height;
			}
		}

		public override int Width 
		{
			get 
			{
				return img.Width;
			}
		}
			
	}
}

