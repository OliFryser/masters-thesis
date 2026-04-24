namespace Pokémon.Json.Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    namespace Pokémon.Json
    {
        public class KeyTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            {
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object? value)
            {
                if (!(value is string input)) 
                    return base.ConvertFrom(context, culture, value);
                if (Key.TryParse(input, out Key? key))
                {
                    return key;
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override object? ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object? value, Type destinationType)
            {
                if (destinationType == typeof(string) && value is Key key)
                {
                    return key.ToString();
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}