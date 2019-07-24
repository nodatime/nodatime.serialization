using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime.Text;

namespace NodaTime.Serialization.SystemText
{
    public sealed class NodaPatternConverter<T> : JsonConverter<T>
    {
        private readonly IPattern<T> pattern;
        private readonly Action<T> validator;

        public NodaPatternConverter(IPattern<T> pattern, Action<T> validator = null)
        {
            this.pattern = pattern;
            this.validator = validator;
        }

        public void Read(ref Utf8JsonReader reader,
            Type type, JsonSerializerOptions options,
            out T value)
        {
            string text = reader.GetString();
            value = pattern.Parse(text).Value;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string text = reader.GetString();
            return pattern.Parse(text).Value;
        }

        public override void Write(Utf8JsonWriter writer,
            T value, JsonSerializerOptions options)
        {
            validator?.Invoke(value);
            writer.WriteStringValue(pattern.Format(value));
        }
    }
}