using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreConsoleClient
{
    class CoreConsoleClient
    {
        public HttpClient client { get; set; }

        private string baseUrl_;

        CoreConsoleClient(string url)
        {
            baseUrl_ = url;
            client = new HttpClient();
        }
        //----< upload file >--------------------------------------

        public async Task<HttpResponseMessage> SendFile(string ebookSpec)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            byte[] data = File.ReadAllBytes(ebookSpec);
            ByteArrayContent bytes = new ByteArrayContent(data);
            string ebookName = Path.GetFileName(ebookSpec);
            multiContent.Add(bytes, "files", ebookName);
            //multiContent.Add(new StringContent(id.ToString()), "id");

            return await client.PostAsync(baseUrl_, multiContent);
        }
        //----< get list of files in server FileStorage >----------

        public async Task<IEnumerable<string>> GetFileList()
        {
            HttpResponseMessage resp = await client.GetAsync(baseUrl_);
            var ebooks = new List<string>();
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                JArray jArr = (JArray)JsonConvert.DeserializeObject(json);
                foreach (var item in jArr)
                    ebooks.Add(item.ToString());
            }
            return ebooks;
        }
        //----< download the id-th file >--------------------------

        public async Task<HttpResponseMessage> GetFile(int id)
        {
            return await client.GetAsync(baseUrl_ + "/" + id.ToString());
        }
        //----< delete the id-th file from FileStorage >-----------

        public async Task<HttpResponseMessage> DeleteFile(int id)
        {
            return await client.DeleteAsync(baseUrl_ + "/" + id.ToString());
        }
        //----< usage message shown if command line invalid >------

        static void showUsage()
        {
            Console.Write("\n  Command line syntax error: expected usage:\n");
            Console.Write("\n    http[s]://machine:port /option [filespec]\n\n");
        }
        //----< validate the command line >------------------------

        static bool parseCommandLine(string[] args)
        {
            if (args.Length < 2)
            {
                showUsage();
                return false;
            }
            if (args[0].Substring(0, 4) != "http")
            {
                showUsage();
                return false;
            }
            if (args[1][0] != '/')
            {
                showUsage();
                return false;
            }
            return true;
        }
        //----< display command line arguments >-------------------

        static void showCommandLine(string[] args)
        {
            string arg0 = args[0];
            string arg1 = args[1];
            string arg2;
            if (args.Length == 3)
                arg2 = args[2];
            else
                arg2 = "";
            Console.Write("\n  CommandLine: {0} {1} {2}", arg0, arg1, arg2);
        }

        static void Main(string[] args)
        {
            Console.Write("\n  CoreConsoleClient");
            Console.Write("\n ===================\n");

            if (!parseCommandLine(args))
            {
                return;
            }
            Console.Write("Press key to start: ");
            Console.ReadKey();

            string url = args[0];
            CoreConsoleClient client = new CoreConsoleClient(url);

            showCommandLine(args);
            Console.Write("\n  sending request to {0}\n", url);

            switch (args[1])
            {
                case "/fl":
                    Task<IEnumerable<string>> tfl = client.GetFileList();
                    var resultfl = tfl.Result;
                    foreach (var item in resultfl)
                    {
                        Console.Write("\n  {0}", item);
                    }
                    break;
                case "/up":
                    //int id1 = Int32.Parse(args[3]);
                    Task<HttpResponseMessage> tup = client.SendFile(args[2]);
                    Console.Write(tup.Result);
                    break;
                case "/dn":
                    int id = Int32.Parse(args[2]);
                    Task<HttpResponseMessage> tdn = client.GetFile(id);
                    Console.Write(tdn.Result);
                    break;
                case "/dl":
                    int id2 = Int32.Parse(args[2]);
                    Task<HttpResponseMessage> tdn2 = client.DeleteFile(id2);
                    Console.Write(tdn2.Result);
                    break;
            }

            Console.WriteLine("\n  Press Key to exit: ");
            Console.ReadKey();
        }
    }
}
