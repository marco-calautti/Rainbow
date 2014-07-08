Rainbow
=======

Rainbow is intended to be a multi-purpose tool written in C# to handle different graphics format from video games assets.

<img src="http://i.imgur.com/2IolZoh.png">

Currently, the image library supports the TIM2 format in all its variants,
both in swizzled and unswizzled form. A preliminary version of the GUI application is also available which allows to convert TIM2 textures to an editable format (xml+png) and then back to TIM2.

How to use Rainbow
=======

* Use the "Open" menu to open any supported texture (currently TIM2).
* Use the property grid on the left side to change some texture parameters (like swizzling).
* The "Export" menu allows to save the texture to a user editable format, like png.
* The "Import" menu allows to import graphics in user editable format such that they can eventually be saved to the original format by means of the "Save" menu.
* The "Save" menu allows to save the texture to the original format (e.g., to TIM2).

The Rainbow app also supports opening all the supported texture formats by passing the file name of the texture as first argument. In a nutshell, you can use Windows to associate the opening of known textures with Rainbow.
