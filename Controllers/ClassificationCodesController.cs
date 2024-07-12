using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SimpleCRUD.Data;
using SimpleCRUD.Model;
using SimpleCRUD.Services;
using System.Data;

namespace SimpleCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassificationCodesController : Controller
    {
        private readonly DataContext _dbContext;

        private readonly ClassificationCodesService _classificationCodesService;

        private IConfiguration _configuration;

        private readonly IHttpClientFactory _httpClientFactory;
        public ClassificationCodesController(DataContext dbContext, ClassificationCodesService service, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _classificationCodesService = service;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("GetAllClassificationCodes")]
        public JsonResult GetAllClassificationCodes()
        {
            string query = "select * from dbo.ClassificationCodes order by code";
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

        /// <summary>
        /// Create new classification codes manually
        /// </summary>
        [HttpPost]
        [Route("CreateNewClassificationCodes")]
        public async Task<IActionResult> CreateNewClassificationCodes([FromForm] ClassificationCodes ClassificationCode)
        {
            //For data audit purposes
            ClassificationCode.Status = 1;
            ClassificationCode.CreatedBy = 1;
            ClassificationCode.CreatedDate = DateTime.Now;
            ClassificationCode.UpdatedBy = 1;
            ClassificationCode.UpdatedDate = DateTime.Now;

            try
            {
                //Check duplicate
                var exists = await _dbContext.ClassificationCodes
                    .AnyAsync(cc => cc.Code == ClassificationCode.Code || cc.Description == ClassificationCode.Description);

                if (exists)
                {
                    // Handle the duplicate case (e.g., return an error response or throw an exception)
                    return BadRequest("Duplicate classification code");
                }

                // Add user to the DbContext
                _dbContext.ClassificationCodes.Add(ClassificationCode);

                // Save changes to the database asynchronously
                await _dbContext.SaveChangesAsync();

                // Return success response in JsonResult format
                return new JsonResult(new { Success = true, Message = "Classification Codes created successfully" });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return new JsonResult(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            }
        }

        /// <summary>
        /// Imports ClassificationCodes JSON data from LHDN (URL : https://sdk.myinvois.hasil.gov.my/files/ClassificationCodes.json) and save/updates the database.
        /// </summary>
        [HttpPost]
        [Route("import-ClassificationCode-fromLHDN")]
        public async Task<IActionResult> ImportClassificationCodeJsonFromLHDNUrl()
        {
            var jsonFilePath = "https://sdk.myinvois.hasil.gov.my/files/ClassificationCodes.json";

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(jsonFilePath);
                var classification = System.Text.Json.JsonSerializer.Deserialize<List<ClassificationCodes>>(response);

                if (classification == null)
                {
                    return BadRequest("Invalid JSON response.");
                }

                foreach (var NewClassificationCode in classification)
                {
                    var existingCountry = await _dbContext.ClassificationCodes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Code == NewClassificationCode.Code);

                    if (existingCountry == null)
                    {
                        //For data audit purposes
                        NewClassificationCode.Status = 1;
                        NewClassificationCode.CreatedBy = 1;
                        NewClassificationCode.CreatedDate = DateTime.Now;
                        NewClassificationCode.UpdatedBy = 1;
                        NewClassificationCode.UpdatedDate = NewClassificationCode.CreatedDate;

                        _dbContext.ClassificationCodes.Add(NewClassificationCode);
                    }
                    else
                    {
                        existingCountry.UpdatedBy = 1;
                        existingCountry.UpdatedDate = DateTime.Now;

                        _dbContext.Entry(existingCountry).State = EntityState.Modified;
                    }
                }

                await _dbContext.SaveChangesAsync();

                return Ok("Classification codes import successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
