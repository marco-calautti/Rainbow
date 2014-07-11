Rainbow
=======

Rainbow is a tool, written in C#, intended to handle different graphics formats from video games assets.

![ScreenShot](http://i.imgur.com/Di52PBE.png)

Features
=======

* Almost complete support for TIM2 texture files usually found in PS2 and PSP games. The app supports multi-layer, multi-clut, swizzled (PSP)/unswizzled TIM2 images with both linear and interleaved palettes and segments headers eventually
extended with custom user data (usually used by programmers).
* Can open textures in any format supported by the underlying image library.
* Can open whole folders in search of supported texture formats. All known texture files are the displayed in a list.
* Can export textures to a editable format (like png).
* Can import editable formats to be then saved to the original texture format.
* Any additional information specific to the texture is preserved when exporting/importing (like the TIM2 header data), in order to have a one-to-one correspondence with the original texture.
* Customizable background color for transparent and semi-transparent images with chessboard like pattern.

To-do
=======
* Add support to mipmap TIM2 textures. They are rare and usually used just for materials.
* Add support to GIM textures (eventually through GimSharp).
* Add scanning of textures inside other files in order to extract and reinsert textures with one click. Actually, there is a tool from Vash that allows to achieve such a task http://www.romhacking.net/utilities/659/.
* Improve performance of the rendering and the import code.

How to use Rainbow
=======

* Use the "Open" menu to open any supported texture (currently TIM2).
* Use the property grid on the left side to change some texture parameters (like swizzling).
* The "Export" menu allows to save the texture to a user editable format, like png.
* The "Import" menu allows to import graphics in user editable format such that they can eventually be saved to the original format by means of the "Save" menu.
* The "Save" menu allows to save the texture to the original format (e.g., to TIM2).

The Rainbow app also supports opening all the supported texture formats by passing the file name of the texture as the first argument. In a nutshell, you can use Windows to associate the opening of known textures with Rainbow.
