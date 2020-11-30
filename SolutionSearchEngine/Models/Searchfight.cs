using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Entity;
using Newtonsoft.Json;

namespace Models
{
    public class Searchfight
    {
        private static List<Seacher> seachers;
        private static List<Result> results;
        public static void ConfigSearchers() {

            // read JSON directly from a file config
            string basepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = "\\ApiConfig.json";
            seachers = JsonConvert.DeserializeObject<List<Seacher>>(File.ReadAllText(basepath + path));
            
         }
        public static async Task SearchEngineAsync(string[] words) {
            decimal totalResultsSearch;
            results = new List<Result>();

            // setting keys and uri for the call api's
            ConfigSearchers();

            //read word input from console
            foreach (string word in words)
            {
                Dictionary<string, decimal> searchdetail = new Dictionary<string, decimal>();

                //search word on the web for each search engine
                foreach (Seacher searcher in seachers) {

                    totalResultsSearch = await SearchInWeb(searcher, word);
                    //save results
                    results.Add(new Result { query = word, searcher_name = searcher.name_searcher, totalresult = totalResultsSearch });
                }
            }
            //print results
            DrawResults();
        }
        public static void DrawResults() {

            //print winner by searcher
            foreach (Seacher searcher in seachers) {
                var maxbyseacher = results.OrderByDescending(o => o.totalresult).Where(s => s.searcher_name == searcher.name_searcher).Select(x => x.query).First();
                Console.WriteLine(String.Format("Winner in {0} : {1} ", searcher.name_searcher, maxbyseacher));
            }

            //print total winner
            var listgroup = results.GroupBy(item => item.query).Select(c => new { query = c.Key, Total = c.Sum(p => p.totalresult) });
            var maxtotal = listgroup.OrderByDescending(x => x.Total).Select(y => y.query).First();

            Console.WriteLine(String.Format("Total Winner: {0} ", maxtotal));
        }
        public static async Task<decimal> SearchInWeb(Seacher searcher, string searchString)
        {
            decimal totalresults = 0;

            try
            {
                
                var request = searcher.Build_Uri.Replace("{query}", searchString);

                HttpResponseMessage response = await RequestAsync(request, searcher.headers);

                var contentString = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {
                    totalresults = GetTotalResults(contentString, searcher.nodes_result);
                  
                    Console.WriteLine(String.Format("find word {0} in {1} , total results {2} ", searchString, searcher.name_searcher, totalresults.ToString("#,#", CultureInfo.InvariantCulture)));
                }
                else
                {
                    totalresults = 0;
                }

                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return totalresults;
        }
        private static async Task<HttpResponseMessage> RequestAsync(string request, Dictionary<string, string> headers)
        {
            var client = new HttpClient();
            if (headers.Count > 0) {
                foreach (KeyValuePair<string, string> header in headers) {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }  
            }
            return (await client.GetAsync(request));
        }
        private static decimal GetTotalResults(string contentString, Dictionary<string, string> nodes) {
            decimal totalresults;

            Dictionary<string, object> searchResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentString);
            Dictionary<string, object> parent = JsonConvert.DeserializeObject<Dictionary<string, object>>(searchResponse[nodes.ElementAt(0).Key].ToString());
            var child = parent[nodes.ElementAt(0).Value];
            totalresults = Convert.ToDecimal(child);

            return totalresults;
        }
        
    }
}
