Rainbow
=======

Rainbow is a tool, written in C#, intended to handle different graphics formats from video games assets.

![ScreenShot](http://i.imgur.com/FsrZ2SY.png)

Features
=======

* Almost complete support for TIM2 texture files usually found in PS2 and PSP games. The app supports multi-layer, multi-clut, swizzled (PSP)/unswizzled TIM2 images with both linear, interleaved palettes, and segments headers eventually
extended with custom user data (usually used by programmers).
* Can open textures in any format supported by the underlying image library.
* Can open whole folders in search of supported texture formats. All known texture files are the displayed in a list.
* Can export textures to an editable format (like png).
* Can import editable formats to be then saved to the original texture format.
* Any additional information specific to the texture is preserved when exporting/importing (like the TIM2 header data), in order to have a one-to-one correspondence with the original texture.
* Customizable background color for transparent and semi-transparent images with chessboard like pattern.

To-do
=======
* Add support to mipmap TIM2 textures. They are rare and usually used just for materials.
* Add support to GIM textures (eventually through GimSharp).
* Add scanning of textures inside other files in order to extract and reinsert textures with one click. Actually, there is a tool from Vash that allows to achieve such a task http://www.romhacking.net/utilities/659/.
* Improve performance of rendering and import code.

How to use Rainbow
=======

* Use the "Open" menu to open any supported texture (currently TIM2).
* Use the property grid on the left side to change some texture parameters (like swizzle).
* The "Export" menu allows to save textures to a user editable format, like png.
* The "Import" menu allows to import graphics in user editable format so that they can eventually be saved to the original format by means of the "Save" menu.
* The "Save" menu allows to save a texture to its original format (e.g., to TIM2).

Finally, Rainbow supports parameters via command line: the first parameter is the name of a texture you would like to open.


Note for multi clut TIM2 images. When dealing with single clut TIM2 files, Rainbow will take care of everything by applying quantization to the given png in order to reduce the number of used colors. 
When a multi-clut TIM2 is exported, instead, a png file for each clut is created. When editing such pngs, make sure that the pixel "structure" of every png is preserved. Rainbow will use the first clut png as reference to construct the pixels indexes of the image and then will create the palettes from each pngs' colors. Make also sure that the number of colors used by these pngs does not exceed the maximum allowed by the original TIM2. Because of how multi-clut TIM2s work, Rainbow cannot apply quantization to all the exported pngs.
