using SimpleCRUD.Data;
using SimpleCRUD.Model;

namespace SimpleCRUD.Services
{
    public interface IUserService
    {
        ICollection<User> GetFreelancerUsers();

    }
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public ICollection<User> GetFreelancerUsers()
        {
            return _context.Users.OrderBy(x => x.Id).ToList();
        }
    }
}
