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

using Rainbow.App.GUI.Controls;
using Rainbow.App.GUI.Model;
using Rainbow.ImgLib.Formats;
using Rainbow.ImgLib.Formats.Serialization;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Rainbow.App.GUI
{
    public partial class MainForm : Form
    {
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenImportTexture(TextureFormatMode.Format);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenImportTexture(TextureFormatMode.Metadata);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveExportTexture(TextureFormatMode.Format);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveExportTexture(TextureFormatMode.Metadata);
        }


        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenImportFolder(TextureFormatMode.Format);

        }

        private void transparentColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.CustomColors = new int[] { TransparentPictureBox.PreferredTransparencyColor.ToArgb() };
            dialog.FullOpen = true;

            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            transparentPictureBox.SetTransparencyColor(dialog.Color);
        }

        private void chessboardBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null)
            {
                return;
            }

            item.Checked = !item.Checked;
            transparentPictureBox.Chessboard = item.Checked;
        }

        private void OnZoomMenuItem(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null)
            {
                return;
            }

            if (item == zoomPlusToolStripMenuItem)
            {
                Zoom(1.2f);
            }
            else if (item == zoomMinusToolStripMenuItem)
            {
                Zoom(0.8f);
            }
            else
            {
                return;
            }
        }

        private void OnOriginalSizeMenuItem(object sender, EventArgs e)
        {
            SetScaleFactor(1.0f);
        }

        private void nextFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextFrame();
        }

        private void previousFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviousFrame();
        }

        private void nextPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextPalette();
        }

        private void previousPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviousPalette();
        }
    }
}
