using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleCRUD.Data;
using SimpleCRUD.Model;
using SimpleCRUD.Services;

namespace SimpleCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly DataContext _dbContext;

        private readonly UserService _userService;

        public HomeController(DataContext dbContext, UserService service)
        {
            _dbContext = dbContext;
            _userService = service;
        }


        [HttpGet("AllUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userService.GetFreelancerUsers();

            if (users.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return users;
        }

        [HttpGet("ActiveUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetActiveUsers()
        {
            var users = await _userService.GetActiveFreelancerUsers();

            if (users.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return users;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            //For data audit purposes
            user.Status = 1;
            user.CreatedBy = 1;
            user.CreatedDate = DateTime.Now;
            user.UpdatedBy = 1;
            user.UpdatedDate = DateTime.Now;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            //For data audit purposes
            var existingEntity = _dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);
            user.Status = existingEntity.Status;
            user.CreatedBy = existingEntity.CreatedBy;
            user.CreatedDate = existingEntity.CreatedDate;
            user.UpdatedBy = 1;
            user.UpdatedDate = DateTime.Now;

            _dbContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (UserAvailable(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
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

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        private bool UserAvailable(int id)
        {
            return (_dbContext.Users?.Any(x => x.Id == id)).GetValueOrDefault();
        }
    }
}
