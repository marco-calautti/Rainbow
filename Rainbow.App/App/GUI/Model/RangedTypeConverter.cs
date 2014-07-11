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
                return base.ConvertFrom(context, culture, value);

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
                return new TypeConverter.StandardValuesCollection(Enumerable.Empty<int>().ToArray());

            return new TypeConverter.StandardValuesCollection(Enumerable.Range(0, obj.Texture.FramesCount).ToArray());
        }
    }

    public class RangedTypeConveterPalettes : RangedTypeConverterInt
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            TexturePropertyGridObject obj = context.Instance as TexturePropertyGridObject;
            if (obj == null)
                return new TypeConverter.StandardValuesCollection(Enumerable.Empty<int>().ToArray());

            return new TypeConverter.StandardValuesCollection(Enumerable.Range(0, obj.Texture.PalettesCount).ToArray());
        }
    }
}
