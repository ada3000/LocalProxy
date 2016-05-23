using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace LocalProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            //var c = new WebClient();
            //var s = c.DownloadString("https://yandex.ru");

            ////ProxyServer.AddEndPoint(new TransparentProxyEndPoint(IPAddress.Any, 1236, true));
            //ProxyServer.AddEndPoint(new ExplicitProxyEndPoint(IPAddress.Any, 1236, true));
            //ProxyServer.Start();

            //Console.ReadKey();

            Console.Title = "LP Port: " + Config.Port + " SSL: " + Config.EnableSsl;

            ProxyServer.BeforeRequest += OnRequest;
            ProxyServer.BeforeResponse += OnResponse;

            //var explicitEndPoint = new TransparentProxyEndPoint(IPAddress.Any, 1237, true);

            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, Config.Port, Config.EnableSsl)
            {
                //Exclude Https addresses you don't want to proxy/cannot be proxied
                //for example exclude dropbox client which use certificate pinning
                ExcludedHttpsHostNameRegex = new List<string>() { ".*" }
            };

            //Add an explicit endpoint where the client is aware of the proxy
            //So client would send request in a proxy friendly manner
            ProxyServer.AddEndPoint(explicitEndPoint);
            ProxyServer.Start();

            //Only explicit proxies can be set as a system proxy!
            //ProxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            //ProxyServer.SetAsSystemHttpsProxy(explicitEndPoint);

            foreach (var endPoint in ProxyServer.ProxyEndPoints)
                Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ",
                    endPoint.GetType().Name, endPoint.IpAddress, endPoint.Port);

            //wait here (You can use something else as a wait function, I am using this as a demo)
            Console.Read();

            //Unsubscribe & Quit
            ProxyServer.BeforeRequest -= OnRequest;
            ProxyServer.BeforeResponse -= OnResponse;
            ProxyServer.Stop();

        }

        public static void OnRequest(object sender, SessionEventArgs e)
        {
            Console.WriteLine(e.ProxySession.Request.Url);

            //read request headers
            var requestHeaders = e.ProxySession.Request.RequestHeaders;

            if ((e.RequestMethod.ToUpper() == "POST" || e.RequestMethod.ToUpper() == "PUT"))
            {
                //Get/Set request body bytes
                byte[] bodyBytes = e.GetRequestBody();
                e.SetRequestBody(bodyBytes);

                //Get/Set request body as string
                string bodyString = e.GetRequestBodyAsString();
                e.SetRequestBodyString(bodyString);

            }

            //To cancel a request with a custom HTML content
            //Filter URL

            //if (e.ProxySession.Request.RequestUri.AbsoluteUri.Contains("google.com"))
            //{
            //    e.Ok("<!DOCTYPE html>" +
            //        "<html><body><h1>" +
            //        "Website Blocked" +
            //        "</h1>" +
            //        "<p>Blocked by titanium web proxy.</p>" +
            //        "</body>" +
            //        "</html>");
            //}
        }

        //Test script injection
        //Insert script to read the Browser URL and send it back to proxy
        public static void OnResponse(object sender, SessionEventArgs e)
        {

            //read response headers
            var responseHeaders = e.ProxySession.Response.ResponseHeaders;


            if (e.RequestMethod == "GET" || e.RequestMethod == "POST")
            {
                if (e.ProxySession.Response.ResponseStatusCode == "200")
                {
                    if (e.ProxySession.Response.ContentType.Trim().ToLower().Contains("text/html"))
                    {
                        string body = e.GetResponseBodyAsString();
                    }
                }
            }
        }
    }
}
