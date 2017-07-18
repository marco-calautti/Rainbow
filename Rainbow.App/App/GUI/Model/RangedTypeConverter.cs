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
using System.ComponentModel;
using System.Linq;

namespace Rainbow.App.GUI.Model
{
    public abstract class RangedTypeConverter<T> : TypeConverter
    {

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public abstract override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context);

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() != typeof(string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return _ConvertFrom(context, culture, value);
        }

        protected abstract T _ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value);
    }

    public abstract class RangedTypeConverterInt : RangedTypeConverter<int>
    {
        
        protected override int _ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            int result;
            int.TryParse(value.ToString(), out result);
            return result;
        }
    }

    public class RangedTypeConveterFrames : RangedTypeConverterInt
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            TexturePropertyGridObject obj = context.Instance as TexturePropertyGridObject;
            if (obj == null)
            {
                return new TypeConverter.StandardValuesCollection(Enumerable.Empty<int>().ToArray());
            }

            return new TypeConverter.StandardValuesCollection(Enumerable.Range(0, obj.Texture.FramesCount).ToArray());
        }
    }

    public class RangedTypeConveterPalettes : RangedTypeConverterInt
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            TexturePropertyGridObject obj = context.Instance as TexturePropertyGridObject;
            if (obj == null)
            {
                return new TypeConverter.StandardValuesCollection(Enumerable.Empty<int>().ToArray());
            }

            return new TypeConverter.StandardValuesCollection(Enumerable.Range(0, obj.Texture.PalettesCount).ToArray());
        }
    }
}
