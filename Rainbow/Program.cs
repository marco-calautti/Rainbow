using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using ImgLib;
using ImgLib.Formats;
using ImgLib.Formats.Serializers;

namespace TIM2Conv
{
    class PreviewForm : Form
    {
        private PictureBox box;
        private TIM2Texture img;

        public PreviewForm(TIM2Texture tim,string filename)
        {
            this.Text = "TIM2Conv - Preview of " + Path.GetFileName(filename);
            box = new PictureBox();

            img = tim;

            box.Image = img.GetImage();
            box.SizeMode = PictureBoxSizeMode.AutoSize;

            box.MouseClick += (o, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    img.Swizzled = !img.Swizzled;
                }
                else if (e.Button == MouseButtons.Left)
                {

                    if (img.PalettesCount > 0)
                        img.SelectActivePalette((img.GetActivePalette() + 1) % img.PalettesCount);
                        
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    img.SelectActiveFrame((img.GetActiveFrame() + 1) % img.FramesCount);
                }

                box.Image = img.GetImage();

            };
           
           
            this.Controls.Add(box);
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            StartPosition = FormStartPosition.CenterScreen;
        }

        
    }

    class Program
    {
        static void PrintUsage()
        {
            Console.WriteLine("TIM2 Converter - (C) SadNEScITy Translations");
            Console.WriteLine("Usage: input_image.tm2 exported_output.xml [-noswizzle]");
        }
        static void Main(string[] args)
        {
            if((args.Length>3 || args.Length<1) || (args.Length==3 && args[2] != "-noswizzle"))
            {
                PrintUsage();
                return;
            }

            try
            {
                TextureFormatSerializer serializer = new TIM2TextureSerializer();
                TIM2Texture tim = null;
                switch (Path.GetExtension(args[0]))
                {
                    case ".tm2":
                        using (Stream s = File.OpenRead(args[0]))
                            tim = (TIM2Texture)serializer.Open(s);
                        break;
                    case ".xml":
                        using (Stream s = File.OpenRead(args[0]))
                            tim = (TIM2Texture)serializer.Import(s, Path.GetDirectoryName(args[0]));
                        break;
                    default:
                        Console.WriteLine("Unsupported format");
                        break;
                }

                if (args.Length == 1)
                {
                    
                    PreviewForm form = new PreviewForm(tim,args[0]);
                    form.Show();
                    Application.Run(form);
                }
                else
                {
                    bool swizzled = args.Length == 2;
                    
                    using (Stream s=File.Open(args[1],FileMode.Create))
                        new TIM2TextureSerializer().Export(tim,s,Path.GetDirectoryName(args[1]),Path.GetFileNameWithoutExtension(args[1]));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
