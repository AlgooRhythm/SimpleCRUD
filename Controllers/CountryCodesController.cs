using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimpleCRUD.Data;
using SimpleCRUD.Model;
using SimpleCRUD.Services;
using System.Data;
using System.Net.Http;

namespace SimpleCRUD.Controllers
{
    public class CountryCodesController : Controller
    {
        private readonly DataContext _dbContext;

        private readonly CountryCodesService _countryCodesService;

        private IConfiguration _configuration;

        private readonly IHttpClientFactory _httpClientFactory;

        public CountryCodesController(DataContext dbContext, CountryCodesService service, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _countryCodesService = service;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("GetAllCountryCodes")]
        public JsonResult GetAllCountryCodes()
        {
            string query = "select * from dbo.CountryCodes";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("DefaultConnection");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        [Route("CreateNewCountryCodes")]
        public async Task<IActionResult> CreateNewCountryCodes([FromForm] CountryCodes CountryCode)
        {
            //For data audit purposes
            CountryCode.Status = 1;
            CountryCode.CreatedBy = 1;
            CountryCode.CreatedDate = DateTime.Now;
            CountryCode.UpdatedBy = 1;
            CountryCode.UpdatedDate = DateTime.Now;

            try
            {
                //Check duplicate
                var exists = await _dbContext.CountryCodes
                    .AnyAsync(cc => cc.Code == CountryCode.Code || cc.Country == CountryCode.Country);

                if (exists)
                {
                    // Handle the duplicate case (e.g., return an error response or throw an exception)
                    return BadRequest("Duplicate country code");
                }

                // Add user to the DbContext
                _dbContext.CountryCodes.Add(CountryCode);

                // Save changes to the database asynchronously
                await _dbContext.SaveChangesAsync();

                // Return success response in JsonResult format
                return new JsonResult(new { Success = true, Message = "Country Codes created successfully" });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return new JsonResult(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("import-json")]
        public async Task<IActionResult> ImportJsonFromUrl()
        {
            var jsonFilePath = "https://sdk.myinvois.hasil.gov.my/files/CountryCodes.json";

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(jsonFilePath);
                var countries = System.Text.Json.JsonSerializer.Deserialize<List<CountryCodes>>(response);

                if (countries == null)
                {
                    return BadRequest("Invalid JSON response.");
                }

                foreach (var countryCode in countries)
                {
                    // Check for duplicate
                    var existingCountryCode = await _dbContext.CountryCodes
                        .FirstOrDefaultAsync(cc => cc.Code == countryCode.Code);

                    if (existingCountryCode != null)
                    {
                        //countryCode.UpdatedBy = 2;
                        //countryCode.UpdatedDate = DateTime.Now;

                        //_dbContext.Entry(countryCode).State = EntityState.Modified;
                    }
                    else
                    {
                        //For data audit purposes
                        countryCode.Status = 1;
                        countryCode.CreatedBy = 1;
                        countryCode.CreatedDate = DateTime.Now;
                        countryCode.UpdatedBy = 1;
                        countryCode.UpdatedDate = countryCode.CreatedDate;

                        _dbContext.CountryCodes.Add(countryCode);
                    }
                }

                _dbContext.SaveChangesAsync();

                return Ok("Country codes import successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("import-json2")]
        public async Task<IActionResult> ImportJsonFromUrl2()
        {
            var jsonFilePath = "https://sdk.myinvois.hasil.gov.my/files/CountryCodes.json";

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(jsonFilePath);
                var countries = System.Text.Json.JsonSerializer.Deserialize<List<CountryCodes>>(response);

                if (countries == null)
                {
                    return BadRequest("Invalid JSON response.");
                }

                foreach (var NewCountryCode in countries)
                {
                    var existingCountry = await _dbContext.CountryCodes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Code == NewCountryCode.Code);

                    if (existingCountry == null)
                    {
                        //For data audit purposes
                        NewCountryCode.Status = 1;
                        NewCountryCode.CreatedBy = 1;
                        NewCountryCode.CreatedDate = DateTime.Now;
                        NewCountryCode.UpdatedBy = 1;
                        NewCountryCode.UpdatedDate = NewCountryCode.CreatedDate;

                        _dbContext.CountryCodes.Add(NewCountryCode);
                    }
                    else
                    {
                        existingCountry.CreatedBy = 1;

                        existingCountry.UpdatedBy = 3;
                        existingCountry.UpdatedDate = DateTime.Now;

                        _dbContext.Entry(existingCountry).State = EntityState.Modified;
                    }
                }

                await _dbContext.SaveChangesAsync();

                return Ok("Country codes import successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
