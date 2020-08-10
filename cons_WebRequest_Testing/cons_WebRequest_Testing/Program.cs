using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace cons_WebRequest_Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            //Call Soap web service with WebClient
            Console.Write("Please entry first value : ");
            int firstValue = Convert.ToInt32(Console.ReadLine());
            Console.Write("Please entry second value : ");
            int secondValue = Convert.ToInt32(Console.ReadLine());


            string soapResult = CallWebService(firstValue,secondValue);
            Console.WriteLine("Result : "+soapResult);
            Console.ReadLine();
        }

        private static string CallWebService(int firstValue, int secondValue)
        {
            var _url = "http://localhost:53001/Service.svc";
            var _action = "http://tempuri.org/IService1/GetData";

            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(firstValue, secondValue);
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
            }
            return soapResult;
        }
        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            /*
            String username = "CentralWarehouse_TR";
            String password = "UAuXhoYqTxAPeD5DlokB1";
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            webRequest.Headers.Add("Authorization", "Basic " + encoded);
            */
            //webRequest.Credentials = new NetworkCredential("CentralWarehouse_TR", "UAuXhoYqTxAPeD5DlokB1");
            return webRequest;
        }
        private static XmlDocument CreateSoapEnvelope(int firstValue, int secondValue)
        {
            /*
            string soapXml = string.Format(@"<Envelope xmlns=""http://schemas.xmlsoap.org/soap/envelope/"" >
                                            <Body>
                                                <GetData xmlns=""http://tempuri.org/"">
                                                    <firstValue>{0}</firstValue>
                                                    <secondValue>{1}</secondValue>
                                                </GetData>
                                            </Body>
                                        </Envelope>",firstValue,secondValue);
            */
            string soapXml = string.Format(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
                                           <soapenv:Header/>
                                            <soapenv:Body>
                                                <tem:GetData>
                                                    <!--Optional:-->
                                                     <tem:firstValue>{0}</tem:firstValue>
                                                      <!--Optional:-->
                                                       <tem:secondValue>{1}</tem:secondValue>
                                                     </tem:GetData>
                                                   </soapenv:Body>
                                                 </soapenv:Envelope>",firstValue,secondValue);

            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(soapXml);

            return soapEnvelopeDocument;
        }
        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
        private static string RemoveInvalidChars(string src)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in src)
            {
                if (XmlConvert.IsXmlChar(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
