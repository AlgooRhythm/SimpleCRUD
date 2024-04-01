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
    public class StaffController : Controller
    {
        private readonly DataContext _dbContext;

        private readonly UserService _userService;

        private IConfiguration _configuration;

        public StaffController(DataContext dbContext, UserService service, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userService = service;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllStaff")]
        public JsonResult GetAllStaff()
        {
            string query = "select * from dbo.Users";
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

        [HttpGet("GetStaffById/{id}")]
        public async Task<ActionResult<User>> GetStaffById(int id)
        {
            if (_dbContext.Users == null)
            {
                return NotFound();
            }
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        [Route("CreateNewStaff")]
        public async Task<IActionResult> CreateNewStaff([FromForm] User user)
        {
            //For data audit purposes
            user.Status = 1;
            user.CreatedBy = 1;
            user.CreatedDate = DateTime.Now;
            user.UpdatedBy = 1;
            user.UpdatedDate = DateTime.Now;

            try
            {
                // Add user to the DbContext
                _dbContext.Users.Add(user);

                // Save changes to the database asynchronously
                await _dbContext.SaveChangesAsync();

                // Return success response in JsonResult format
                return new JsonResult(new { Success = true, Message = "Staff created successfully" });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return new JsonResult(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("EditStaff")]
        public async Task<IActionResult> EditStaff([FromForm] User user)
        {
            //For data audit purposes
            var existingEntity = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == user.Id);
            user.Status = existingEntity.Status;
            user.CreatedBy = existingEntity.CreatedBy;
            user.CreatedDate = existingEntity.CreatedDate;
            user.UpdatedBy = 1;
            user.UpdatedDate = DateTime.Now;

            // Edit user 
            _dbContext.Entry(user).State = EntityState.Modified;

            try
            {
                // Save changes to the database asynchronously
                await _dbContext.SaveChangesAsync();

                // Return success response in JsonResult format
                return new JsonResult(new { Success = true, Message = "Staff updated successfully" });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return new JsonResult(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpDelete("DeleteStaff/{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            if (_dbContext.Users == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);

            try
            {
                // Save changes to the database asynchronously
                await _dbContext.SaveChangesAsync();

                // Return success response in JsonResult format
                return new JsonResult(new { Success = true, Message = "Staff delete successfully" });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return new JsonResult(new { Success = false, Message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
