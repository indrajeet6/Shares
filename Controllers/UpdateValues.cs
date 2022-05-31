using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Newtonsoft.Json;

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
                float fltCurrentValue = 0.00F;

                //Get BSE Data
                var clientBSE = new RestClient("https://www.alphavantage.co/");
                var requestBSE = new RestRequest("query");
                clientBSE.AddDefaultQueryParameter("function", "GLOBAL_QUOTE");
                clientBSE.AddDefaultQueryParameter("symbol", strSymbol + ".BSE");
                clientBSE.AddDefaultParameter("apikey", "KRIXYZMN9FTOEISH");
                var responseBSE = await clientBSE.ExecuteAsync(requestBSE);

                //Get NSE Data
                var clientNSE = new RestClient("https://www.alphavantage.co/");
                var requestNSE = new RestRequest("query");
                clientNSE.AddDefaultQueryParameter("function", "GLOBAL_QUOTE");
                clientNSE.AddDefaultQueryParameter("symbol", strSymbol + ".NSE");
                clientNSE.AddDefaultParameter("apikey", "KRIXYZMN9FTOEISH");
                var responseNSE = await clientNSE.ExecuteAsync(requestNSE);

                if (responseBSE.IsSuccessful == true && responseBSE.Content != "{}")
                {
                    dynamic collection = JsonConvert.DeserializeObject(responseBSE.Content);
                    try
                    {
                        fltCurrentValue = collection["Global Quote"]["08. previous close"];
                        Console.WriteLine(fltCurrentValue);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    
                }
                else if (responseNSE.IsSuccessful == true && responseNSE.Content !="{}")
                {
                    dynamic collection = JsonConvert.DeserializeObject(responseNSE.Content);
                    try
                    {
                        fltCurrentValue = collection["Global Quote"]["08. previous close"];
                        Console.WriteLine(fltCurrentValue);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
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
