using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using Rainbow.ImgLib;
using Rainbow.ImgLib.Formats;
using Rainbow.ImgLib.Formats.Serializers;

namespace TIM2Conv
{
    class PreviewForm : Form
    {
        private PictureBox box;
        private TextureFormat img;

        public PreviewForm(TextureFormat tex, string filename)
        {
            this.Text = "Rainbow - Preview of " + Path.GetFileName(filename);
            box = new PictureBox();

            img = tex;

            box.Image = img.GetImage();
            box.SizeMode = PictureBoxSizeMode.AutoSize;

            box.MouseClick += (o, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    TIM2Texture tim = img as TIM2Texture;
                    if (tim != null)
                        tim.Swizzled = !tim.Swizzled;
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
            Console.WriteLine("Rainbow - (C) SadNEScITy Translations");
            Console.WriteLine("Usage: input_image.ext exported_output [-noswizzle]");
        }
        static void Main(string[] args)
        {
            if ((args.Length > 3 || args.Length < 1) || (args.Length == 3 && args[2] != "-noswizzle"))
            {
                PrintUsage();
                return;
            }

            try
            {

                var serializer = TextureFormatSerializerProvider.FromFile(args[0]);
                if (serializer == null)
                {
                    Console.WriteLine("Unsupported format");
                    return;
                }
                TextureFormat tex = null;
                using (Stream s = File.OpenRead(args[0]))
                {
                    if (serializer.IsValidFormat(s))
                        tex = serializer.Open(s);
                    else
                        tex = serializer.Import(s, Path.GetDirectoryName(args[0]), Path.GetFileNameWithoutExtension(args[0]));
                }



                if (args.Length == 1)
                {
                    PreviewForm form = new PreviewForm(tex, args[0]);
                    form.Show();
                    Application.Run(form);
                }
                else
                {
                    bool swizzled = args.Length == 2;
                    TIM2Texture tim = tex as TIM2Texture;

                    if(swizzled&&tim!=null)
                    {
                        tim.Swizzled = swizzled;

                    }

                    using (Stream s = File.Open(args[1], FileMode.Create))
                    {
                        serializer.Export(tex, s, Path.GetDirectoryName(args[1]), Path.GetFileNameWithoutExtension(args[0]));
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
