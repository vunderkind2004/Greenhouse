using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CryptoTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var date = "Sat, 07 May 2016 12:37:11 GMT";
            ////stringTosign += "\n";
            //var res = "oleksandr1litvinov/testTable()";

            //var stringTosign = String.Format(
            //        "{0}\n{1}\n{2}\n{3}\n{4}\n{5}",
            //        "POST",
            //        "",
            //        "application/json",
            //        "f7523003-969b-4730-9cd8-d4f69e816e90",
            //        date,
            //        res);

            //var okSign = "9npCHu73lIEwwD74TadLuc5BRUP+e1ijiUshQqFNE70=";


            //Console.WriteLine(stringTosign);
            //var mySign = GetSignature(stringTosign);
            //Console.WriteLine("ok: {0}", okSign);
            //Console.WriteLine("my: {0}", mySign);
            ArduinoTest();
            Console.Read();
            return;

            Console.Read();
            var key2 = Guid.NewGuid().ToString();
            InsertEntity("testTable", "AAA", key2);
            Console.Read();
        }

        static void ArduinoTest()
        {
            string stringToSign = "Sun, 08 May 2016 08:57:00 GMT\n/oleksandr1litvinov/testTable()";
            string arduinoSign = "aSBlwZHOyjX2avE2DU3yDc7vv06dqTzPLydQNQZEZwk=";
            var mySign = GetSignature(stringToSign);
            if (arduinoSign != mySign)
            {
                Console.WriteLine("Sign is wrong");
            }
            else {
                Console.WriteLine("SUCCESS");
            }

            Console.WriteLine("{0}\t mySign", mySign);
            Console.WriteLine("{0}\t arduinoSign", arduinoSign);

        }

        public static void InsertEntity(String tableName, String artist, String title)
        {
            String requestMethod = "POST";

            String urlPath = tableName + "()";

            String storageServiceVersion = "2014-02-14";
             var requestId = Guid.NewGuid().ToString();

            String dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            Console.WriteLine(dateInRfc1123Format);
            String contentMD5 = String.Empty;
            String contentType = "application/json";
            String canonicalizedResource = String.Format("/{0}/{1}", AzureStorageConstants.Account, urlPath);
            //String stringToSign = String.Format(
            //        "{0}\n{1}\n{2}\n{3}\n{4}",
            //        requestMethod,
            //        contentMD5,
            //        contentType,
            //        dateInRfc1123Format,
            //        canonicalizedResource);


            //for SharedKeyLite:
            String stringToSign = String.Format(
                    "{0}\n{1}",            
                    dateInRfc1123Format,
                    canonicalizedResource);
            Console.WriteLine(stringToSign);
            String authorizationHeader = CreateAuthorizationHeader(stringToSign);

            UTF8Encoding utf8Encoding = new UTF8Encoding();
            Byte[] content = utf8Encoding.GetBytes(GetJsonContent(artist, title));

            Uri uri = new Uri(AzureStorageConstants.TableEndPoint + urlPath);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Accept = "application/json;odata=minimalmetadata";
            request.ContentLength = content.Length;
            request.ContentType = contentType;
            request.Method = requestMethod;
            request.Headers.Add(string.Format("x-ms-date: {0}", dateInRfc1123Format));
            request.Headers.Add(string.Format("x-ms-version: {0}",storageServiceVersion));
            request.Headers.Add("Authorization: " + authorizationHeader);
            request.Headers.Add("Accept-Charset: UTF-8");
            request.Headers.Add("DataServiceVersion: 3.0;NetFx");
            request.Headers.Add("MaxDataServiceVersion: 3.0;NetFx");
            request.Headers.Add("Prefer: return-no-content");
            request.Headers.Add("x-ms-client-request-id: " + requestId);
            //request.Headers.Add("Prefer: return-no-content");


            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(content, 0, content.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    String responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);
                }
            }
        }

        [Obsolete]
        private static String GetRequestContentInsertXml(String artist, String title)
        {
            String defaultNameSpace = "http://www.w3.org/2005/Atom";
            String dataservicesNameSpace = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            String metadataNameSpace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.OmitXmlDeclaration = false;
            xmlWriterSettings.Encoding = Encoding.UTF8;

            StringBuilder entry = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(entry))
            {
                xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                xmlWriter.WriteWhitespace("\n");
                xmlWriter.WriteStartElement("entry", defaultNameSpace);
                xmlWriter.WriteAttributeString("xmlns", "d", null, dataservicesNameSpace);
                xmlWriter.WriteAttributeString("xmlns", "m", null, metadataNameSpace);
                xmlWriter.WriteElementString("title", null);
                xmlWriter.WriteElementString("updated", String.Format("{0:o}", DateTime.UtcNow));
                xmlWriter.WriteStartElement("author");
                xmlWriter.WriteElementString("name", null);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteElementString("id", null);
                xmlWriter.WriteStartElement("content");
                xmlWriter.WriteAttributeString("type", "application/xml");
                xmlWriter.WriteStartElement("properties", metadataNameSpace);
                xmlWriter.WriteElementString("PartitionKey", dataservicesNameSpace, artist);
                xmlWriter.WriteElementString("RowKey", dataservicesNameSpace, title);
                xmlWriter.WriteElementString("Artist", dataservicesNameSpace, artist);
                xmlWriter.WriteElementString("Title", dataservicesNameSpace, title + "\n" + title);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.Close();
            }
            String requestContent = entry.ToString();
            return requestContent;
        }

        private static string GetJsonContent(string key1, string key2)
        {
            return "{'PartitionKey':'" + key1 + "','RowKey':'" + key2 + "','TestField':'test'}";
        }

        public static String CreateAuthorizationHeader(String canonicalizedString)
        {
            

            String authorizationHeader = String.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}:{2}",
                AzureStorageConstants.SharedKeyAuthorizationScheme,
                AzureStorageConstants.Account,
                GetSignature(canonicalizedString)
            );

            return authorizationHeader;
        }

        public static string GetSignature(string stringTosign)
        {
            String signature = String.Empty;

            var keyBytes = Convert.FromBase64String(AzureStorageConstants.StorageAccountKey);
            var keyBytesString = BytesToStringInt(keyBytes);


            using (HMACSHA256 hmacSha256 = new HMACSHA256(keyBytes))
            {
                Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(stringTosign);
                var dataToHmacString = BytesToString(dataToHmac);
                Console.WriteLine(dataToHmacString);
                var hash = hmacSha256.ComputeHash(dataToHmac);
                var hashString = BytesToString(hash);
                Console.WriteLine(hashString);
                signature = Convert.ToBase64String(hash);
            }
            return signature;
        }

        static string BytesToString(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Count()*2);
            for(var i = 0; i<bytes.Length; i++)
            {
                builder.AppendFormat("{0:x2}", bytes[i]);
            }
            return builder.ToString();
        }

        static string BytesToStringInt(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Count());
            for (var i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i]);
            }
            return builder.ToString();
        }
    }
}
