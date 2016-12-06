using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OrganisationInspector
{
    public static class ParserUtils
    {

        public static string SerializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        public static void WriteTemporaryFile(string content, string name)
        {
            XmlTextWriter wr = new XmlTextWriter(name, Encoding.Unicode);
            wr.WriteRaw(content);
            wr.Close();
        }

        public static string HexToUUID(string hex)
        {
            Guid guidString = new Guid(StringToByteArray(hex));

            return guidString.ToString("D");
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

    }
}
