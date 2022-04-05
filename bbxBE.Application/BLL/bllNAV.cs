using bbxBE.Common.NAV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;

namespace bbxBE.Application.BLL
{
    public static class bllNAV
    {
        public static bool NAVPost(string p_uri, string p_requestId, string p_content, string p_procname, out string o_response)
        {
            o_response = "";
            try
            {

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                p_content = p_content.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
                p_content = p_content.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

                //Util.Log2File(String.Format("{0} NAV POST. requestId:{1}, uri:{2}, Content:\n{3} ", p_procname, p_requestId, p_uri, p_content), Global.POSTLOG_NAME);

                var request = (HttpWebRequest)WebRequest.Create(p_uri);
                request.Method = "POST";
                request.ContentType = "application/xml; charset=utf-8";
                request.ContentType = "application/xml";
                byte[] postBytes = Encoding.UTF8.GetBytes(p_content);
                request.ContentLength = postBytes.Length;
                var requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                return GetResponse(request, p_requestId, p_content, p_procname, out o_response);


            }
            catch (Exception ex)
            {
                //Util.Log2File(String.Format("{0} NAV POST exception. p_requestId:{1}, uri:{2}, Content:\n{3}", p_procname, p_requestId, p_uri, p_content), Global.POSTLOG_NAME);
                //Util.ExceptionLog(ex);
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }


        }

        private static bool GetResponse(WebRequest request, string p_requestId, string p_content, string p_procname, out string o_response)
        {
            o_response = "";
            try
            {

                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    response = (HttpWebResponse)ex.Response;
                }

                if (response != null)
                {

                    //Response kiolvasása
                    long length = response.ContentLength;
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    o_response = reader.ReadToEnd();


                    if (response.StatusCode.ToString() == NAVGlobal.NAV_OK)
                    {
                        //Util.Log2File(String.Format("{0} NAV OK response. requestId:{1}, status:{2}, response length:{3}, response:{4}", p_procname, p_requestId, response.StatusDescription, response.ContentLength, o_response), Global.POSTLOG_NAME);
                        Console.WriteLine(String.Format("{0} NAV OK response. requestId:{1}, status:{2}, response length:{3}, response:{4}", p_procname, p_requestId, response.StatusDescription, response.ContentLength, o_response));
                        return true;
                    }
                    else
                    {
                        // Util.Log2File(String.Format("{0} NAV error response. requestId:{1}, status:{2}, response:{3}", p_procname, p_requestId, response.StatusDescription, o_response), Global.POSTLOG_NAME);
                        Console.WriteLine(String.Format("{0} NAV error response. requestId:{1}, status:{2}, response:{3}", p_procname, p_requestId, response.StatusDescription, o_response));
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //Util.Log2File(String.Format("{0} NAV GetResponse exception. requestId:{1}", p_procname, p_requestId), Global.POSTLOG_NAME);
                Console.WriteLine(String.Format("{0} NAV GetResponse exception. requestId:{1}", p_procname, p_requestId));
                //Util.ExceptionLog(we);
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }

        }

    }



}
