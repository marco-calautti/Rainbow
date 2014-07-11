using Rainbow.ImgLib.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rainbow.App.GUI.Model
{
    public static class PropertyGridObjectFactory
    {
        public static TexturePropertyGridObject Create(TextureFormat texture)
        {
            if (texture.GetType() == typeof(TIM2Texture))
                return new TIM2PropertyGridObject((TIM2Texture)texture);
            else
                return new TexturePropertyGridObject(texture);
        }
    }
}
