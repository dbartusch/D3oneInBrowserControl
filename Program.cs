using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using Newtonsoft.Json;

namespace WindowsFormsApp1
{
    static class Program
    {
        private static string MEDIA_TYPE_HAL_JSON = "application/hal+json";
        private static HttpClient Client;
        private static CookieContainer Cookies;

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Important: Tls 1.0 is no longer supported! Please use the next line of code for compatibilty settings or select the most recent .NET Framework for this project!
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //Sets the maximum number of concurrent connections allowed by a ServicePoint object.
            ServicePointManager.DefaultConnectionLimit = 10;


            var baseURI = @"https://sageb7.d-velop.cloud//";

            //basic http client with base uri and default headers
            Cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = Cookies;
            Client = new HttpClient(handler);
            Client.BaseAddress = new Uri(baseURI);
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_HAL_JSON));
            Client.DefaultRequestHeaders.Add("Origin", baseURI);

            //api key of a valid d.3one account. If not available use basic authentication with username and password as described below 
            //            var apiKey = <apiKey>;
            //id of the repository. In instance tp-dev.d-velop.cloud use: "44330b47-508c-58bd-9109-cedd2c31e418"
            //sagebaeurer: var repoId = @"67b49a02-4e84-5701-8aa5-0805bd7fc9ec";
            var repoId = @"26044701-ef54-4347-b7bf-3e7ed477613a";


            //using apiKey authentication is recommended! In this case call replace placeholders for username with empty string "" and password with the apiKey
            //to authenticate with user credentials and base authentication: replace placeholders for username and password 
            //var apiKey = AuthenticateBasic(baseURI, "<empty or username>", "kJsCtBtEsrx5teQ3JiUg7mqYzCp1pRcAa2hSIW0ozgqyZdi0V0FhlQgPatAh+7UteD2SvSJR3KiRDNZ0yF7oogFgcZkSX6+rD4hBxmltzng=&_z_A0V5ayCRJ8vFCcqHo-d0n6vglZkcADkhzeOu3BE5xohC0XlB3aGLkJ5AXEChCcadfk_1jt0pSfhME1_TyoMHI9IE0WhAQ").Result;
            // sagebaeurer: string apiKey = "tPVIXp31F/d4jXbtoiw29U5wIkdmT5iq/VVYQswgTlO+2NOdHPnginWW7vlDt0aBR/L/qOgPS55vhIc0Wb8BfuY3Zw7PIs2D2xfe4Rt9PxE=&_z_A0V5ayCQp3VhDD9r6HqVyJ7sE4VNPab_wxYQjyS_0sjeNWNWYIAk4C8emixW-bUjV27U5R24Gy2sDoih0DCU74hiSZ1p9";
            //test-dbar10:
            string apiKey = "Iv2uojEVFi25c5dBRbzv205zoS8u/V4UlEboTvUzHdByNDkkKKoh/TiBYUQKuXd/n+Y9qK9dcFoUXTywOu+pGrrLYG9GEaHNoM/Tx451PBk=&_z_A0V5ayCRqVfPqk0Owd2NZdAbpPuodbDIhgLaYuianesLWypqrxEQ3VYrxzrwv_hzrdYe-YqJMVAzQC5pB0gvDrL1F22EK";
        //*******************************************************************
        //for all further api calls use the returned key as Bearer-Token!
        //*******************************************************************
        // Requirement: Use current web browser version for the browser control 
        //https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/general-info/ee330730(v=vs.85)#browser-emulation

            //Install ChromiumWebbrowser
            //https://www.codeproject.com/Articles/881315/Display-HTML-in-WPF-and-CefSharp-Tutorial-Part
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 frmIE = new Form1();
            Form2 frmChrome = new Form2();

            var sessionId = Authenticate(baseURI, apiKey);
            byte[] authData = System.Text.UnicodeEncoding.UTF8.GetBytes("user:" + sessionId);
            string authHeader = "Authorization: Basic " + Convert.ToBase64String(authData) + "\r\nOrigin: " + baseURI + "\r\n";
            //string redirect = HttpUtility.UrlEncode("/dms/r/" + repoId + "/sr/?" + "properties=" + HttpUtility.UrlEncode("{\"23\":[\"1\"]}") + "&propertysort=property_last_modified_date&ascending=false&showdetails=false");
            string redirect = HttpUtility.UrlEncode("/dms/r/" + repoId + "/sr");

            frmIE.Navigate(baseURI + "/identityprovider/login?basic=true&redirect=" + redirect, authHeader);
            frmIE.Show();

            sessionId = Authenticate(baseURI, apiKey);
            //redirect = HttpUtility.UrlEncode("/dms/r/" + repoId + "/sr/?" + "properties=" + HttpUtility.UrlEncode("{\"23\":[\"1\"]}") + "&propertysort=property_last_modified_date&ascending=false&showdetails=false");
            redirect = HttpUtility.UrlEncode("/dms/r/" + repoId + "/sr");
            frmChrome.Navigate(baseURI + "/identityprovider/login?basic=true&redirect=" + redirect, sessionId);
            frmChrome.ShowDialog();
            
            //Application.Run();

        }
        private class AuthSessionInfoDto
        {
            public string AuthSessionId { get; set; }
            public DateTime Expire { get; set; }
        }

        //authenticate with user credentials and basic authentication
        private static string Authenticate(string baseURI, string apiKey)
        {
            var link_relation = "/identityprovider/login";
            var baseRequest = baseURI + link_relation;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new System.Uri(baseRequest);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                    client.DefaultRequestHeaders.Add("Accept", MEDIA_TYPE_HAL_JSON);

                    var result = client.GetAsync(link_relation).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var authSessionInfo = JsonConvert.DeserializeObject<AuthSessionInfoDto>(result.Content.ReadAsStringAsync().Result);

                        if (null != authSessionInfo.AuthSessionId)
                        {
                            Console.WriteLine("login ok: " + "expires: " + authSessionInfo.Expire + ", sessionId: " + authSessionInfo.AuthSessionId);
                            return authSessionInfo.AuthSessionId;
                        }
                    }
                    else
                    {
                        Console.WriteLine("login failed with status code \"" + result.StatusCode + "\": " + baseRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("login failed with error \"" + ex.Message + "\": " + baseRequest);
            }
            return null;
        }

    }
}
