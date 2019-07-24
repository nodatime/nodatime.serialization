using System.Text.Json;
using System.Text.Json.Serialization;

namespace NodaTime.Serialization.SystemText
{
    public static class Extensions
    {
        /// <summary>
        /// Resolves property name according <see cref="DefaultContractResolver.NamingStrategy"/>.
        /// <para>If serializer is not <see cref="DefaultContractResolver"/> then original <paramref name="propertyName"/> returns.</para>
        /// </summary>
        /// <param name="serializerOptions">The serializer options to use name resolve.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Resolved or original property name.</returns>
        internal static string ResolvePropertyName(this JsonSerializerOptions serializerOptions, string propertyName) =>
            (serializerOptions.PropertyNamingPolicy)?.ConvertName(propertyName) ?? propertyName;

        internal static T ReadType<T>(this JsonSerializerOptions serializerOptions, ref Utf8JsonReader reader)
        {
            var converter = (JsonConverter<T>)serializerOptions.GetConverter(typeof(T));
            return converter.Read(ref reader, typeof(T), serializerOptions);
        }

        internal static void WriteType<T>(this JsonSerializerOptions serializerOptions, Utf8JsonWriter writer, T value)
        {
            var converter = (JsonConverter<T>)serializerOptions.GetConverter(typeof(T));
            converter.Write(writer, value, serializerOptions);
        }
    }
}