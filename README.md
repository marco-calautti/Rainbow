Rainbow
=======

Rainbow is intended to be a multi-purpose tool written in C# to handle different graphics format from video games assets.

Currently, the image library supports the TIM2 format in all its variants,
both in swizzled and unswizzled form. A preliminary version of the GUI application is also available which allows to convert TIM2 textures to an editable format (xml+png) and then back.

How to use Rainbow.App
=======
Use the "Open" menu to open any supported texture (currently TIM2)
Use the property grid on the left side to change some texture parameters (like swizzling).
The "Save" menu allows to save the texture to the original format (e.g., to TIM2).
The "Export" menu instead allows to save the texture to a user editable format, like png.
The "Import" menu allows to import graphics in user editable format such that they can eventually be saved to the original format by means of the "Save" menu.

The application supports opening all the supported texture formats by passing the file name as first argument. In a nutshell,
you can use Windows to associate the opening of known texture with Rainbow.
