using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudentLearningHistory.Attributes
{
    public class InfoToStrListConverterAttribute: JsonConverter<List<string>>
    {
        public override List<string> Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                List<string> list = new();
                string text = jsonDoc.RootElement.GetRawText();

                int sIndex = 0;
                int eIndex = 0;

                while (true)
                {
                    sIndex = text.IndexOf("{", sIndex);

                    if (sIndex == -1) { break; }

                    eIndex = text.IndexOf("}", sIndex);

                    string getText = text.Substring(sIndex, eIndex - sIndex+1);
                    list.Add(getText);
                    sIndex++;
                }

                return list;
            }
        }

        public override void Write( Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
