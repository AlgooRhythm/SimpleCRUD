using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleCRUD.Data;
using SimpleCRUD.Model;

namespace SimpleCRUD.Services
{
    public interface IUserService
    {
        Task<List<User>> GetFreelancerUsers();
        Task<List<User>> GetActiveFreelancerUsers();

    }
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetFreelancerUsers()
        {
            return await _context.Users.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<List<User>> GetActiveFreelancerUsers()
        {
            return _context.Users.Where(y => y.Status == 1).OrderBy(x => x.Id).ToList();
        }
    }
}
