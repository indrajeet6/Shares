using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Newtonsoft.Json;
using Shares.Model;

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
        public UpdateValues(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: UpdateValues
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            //Get All Stock Data
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
                    Console.WriteLine("Share Symbol Not Found: " + item.Symbol.ToString());
                }
            }
            context.SaveChanges();
            Console.WriteLine("Values Updated");
            return Redirect("/Home/Index");
        }

    }
}
