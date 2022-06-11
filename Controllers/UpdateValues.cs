using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using RestSharp;
using Newtonsoft.Json;
using Shares.Model;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Shares.Controllers
{
    public class UpdateValues : Controller
    {
        //https://stackify.com/restsharp/
        //Add Another Class to use the API below to refresh the Market Values and the total value in the Database then refresh the view
        //when clicking the refresh button on the home page.
        //https://rapidapi.com/alphavantage/api/alpha-vantage/
        // API Key from Alpha Vantage = KRIXYZMN9FTOEISH
        //Try the API Below: get all symbols from NSE, deserialise the JSON and filter the collection for the symbols needed.
        //https://latest-stock-price.p.rapidapi.com/any
        //X-RapidAPI-Key: 6236083a46mshc2a5feedffcd36ep1613e7jsn3891dce27e27
        //request.AddHeader("X-RapidAPI-Host", "latest-stock-price.p.rapidapi.com");
        //This Gives only NSE data, not BSE.

        //Nasdaq Data Link API Key: 4dzzDZvBaT2vU5_hUR7q
        //API Call below:
        //https://data.nasdaq.com/api/v3/datasets/BSE/BOM500470?column_index=4start_date=2009-10-05&end_date=2009-10-05&api_key=4dzzDZvBaT2vU5_hUR7q
        //Get the BOM Code by reading the metadata CSV File



        ////Get NSE Data
        //var clientNSE = new RestClient("https://google-finance4.p.rapidapi.com/search/?");
        //var requestNSE = new RestRequest();
        //clientNSE.AddDefaultParameter("q", strSymbol + ":NSE");
        //clientNSE.AddDefaultParameter("hl", "en");
        //clientNSE.AddDefaultParameter("gl", "IN");
        //clientNSE.AddDefaultHeader("X-RapidAPI-Host", "google-finance4.p.rapidapi.com");
        //clientNSE.AddDefaultHeader("X-RapidAPI-Key", "6236083a46mshc2a5feedffcd36ep1613e7jsn3891dce27e27");
        //var responseNSE = await clientNSE.ExecuteAsync(requestNSE);

        private IConfiguration Configuration;
        private string contentRoot;
        public UpdateValues(IConfiguration _configuration, IWebHostEnvironment env)
        {
            Configuration = _configuration;
            contentRoot = env.WebRootPath;
        }
        // GET: UpdateValues
        [HttpGet]
        public async Task<ActionResult> Index()
        {

            //Get BSE Stock Data
            //Nasdaq Data Link API Key: 4dzzDZvBaT2vU5_hUR7q
            //API Call below:
            //https://data.nasdaq.com/api/v3/datasets/BSE/BOM500470?column_index=4&api_key=4dzzDZvBaT2vU5_hUR7q
            //Get the BOM Code by reading the metadata CSV File
            
            //Get NSE Stock Data
            var clientNSE = new RestClient("https://latest-stock-price.p.rapidapi.com/any");
            var requestNSE = new RestRequest();
            clientNSE.AddDefaultHeader("X-RapidAPI-Host", "latest-stock-price.p.rapidapi.com");
            clientNSE.AddDefaultHeader("X-RapidAPI-Key", "6236083a46mshc2a5feedffcd36ep1613e7jsn3891dce27e27");
            var responseNSE = await clientNSE.ExecuteAsync(requestNSE);
            IEnumerable<StockData> collection = JsonConvert.DeserializeObject<IEnumerable<StockData>>(responseNSE.Content);
            //Get Data from DB
            var db = new Model.Shares_DBContext(Configuration);
            var query = from b in db.Shares orderby b.Symbol descending select b;

            //Iterate over data and call API to update the values
            var context = new Shares_DBContext(Configuration);
            foreach (var item in query)
            {
                var oneItem = collection.FirstOrDefault(x => x.symbol == item.Symbol);
                if (oneItem != null)
                {
                    {
                        var entity = context.Shares.FirstOrDefault(dbItem => dbItem.Symbol == item.Symbol);
                        if (entity != null)
                        {
                            entity.MktValue = oneItem.lastPrice;
                            entity.LastUpdateTime = oneItem.lastUpdateTime;
                        }
                    }
                }
                else
                {
                    string strCSVPath = contentRoot + @"\NASDAQ_Metadata\BSE_metadata.csv";
                    IEnumerable<BSE_Metadata> result;
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        PrepareHeaderForMatch = args => args.Header.ToLower(),
                        HasHeaderRecord = false,
                    };
                    using (var fileReader = new StreamReader(strCSVPath))
                    {
                        var csv = new CsvReader(fileReader, config);
                        csv.Read();
                        result = csv.GetRecords<BSE_Metadata>();
                    }
                    //Write Code to get BSE data from the Nasdaq API
                    Console.WriteLine(result.FirstOrDefault(x => x.name.Contains(item.Name)).code.ToString());

                    //Get NSE Data
                    var clientBSE = new RestClient("https://data.nasdaq.com/api/v3/datasets/BSE/BOM500470?");
                    var requestBSE = new RestRequest();
                    clientBSE.AddDefaultParameter("column_index", "4");
                    clientBSE.AddDefaultParameter("api_key", "4dzzDZvBaT2vU5_hUR7q");

                    var responseBSE = await clientBSE.ExecuteAsync(requestBSE);


                    Console.WriteLine("Share Symbol Not Found: " + item.Symbol.ToString());
                }
            }
            context.SaveChanges();
            Console.WriteLine("Values Updated");
            return Redirect("/Home/Index");
        }

    }
}
