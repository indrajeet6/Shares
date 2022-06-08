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
        private IConfiguration Configuration;
        public UpdateValues(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: UpdateValues
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            //Get Data from DB
            var db = new Model.Shares_DBContext(Configuration);
            var query = from b in db.Shares orderby b.Symbol descending select b;

            //Iterate over data and call API to update the values
            foreach(var item in query)
            {
                string strSymbol = item.Symbol;
                decimal decCurrentValue = 0.00M;

                ////Get BSE Data
                //var clientBSE = new RestClient("https://www.alphavantage.co/");
                //var requestBSE = new RestRequest("query");
                //clientNSE.AddDefaultQueryParameter("function", "GLOBAL_QUOTE");
                //clientNSE.AddDefaultQueryParameter("symbol", strSymbol + ".NSE");
                //clientNSE.AddDefaultParameter("apikey", "KRIXYZMN9FTOEISH");
                //var responseBSE = await clientNSE.ExecuteAsync(requestBSE);

                //Get NSE Data
                var clientNSE = new RestClient("https://google-finance4.p.rapidapi.com/search/?");
                var requestNSE = new RestRequest();
                clientNSE.AddDefaultParameter("q", strSymbol + ":NSE");
                clientNSE.AddDefaultParameter("hl", "en");
                clientNSE.AddDefaultParameter("gl", "IN");
                clientNSE.AddDefaultHeader("X-RapidAPI-Host", "google-finance4.p.rapidapi.com");
                clientNSE.AddDefaultHeader("X-RapidAPI-Key", "6236083a46mshc2a5feedffcd36ep1613e7jsn3891dce27e27");
                var responseNSE = await clientNSE.ExecuteAsync(requestNSE);

                if (responseNSE.IsSuccessful == true && responseNSE.Content != null)
                {
                    dynamic collection = JsonConvert.DeserializeObject(responseNSE.Content);
                    if(collection != null)
                    {
                        try
                        {
                            decCurrentValue = collection[0]["price"]["previous_close"];
                            using (var context = new Shares_DBContext(Configuration))
                            {
                                var entity = context.Shares.FirstOrDefault(item => item.Symbol == strSymbol);
                                if (entity != null)
                                {
                                    entity.MktValue = decCurrentValue;
                                }
                                context.SaveChanges();
                            }
                            //Console.WriteLine(decCurrentValue);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Share Symbol Not Found");
                }
            }
            Console.WriteLine("Values Updated");
            return Redirect("/Home/Index");
        }

    }
}
