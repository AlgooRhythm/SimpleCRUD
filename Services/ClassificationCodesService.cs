using Microsoft.EntityFrameworkCore;
using SimpleCRUD.Data;
using SimpleCRUD.Model;

namespace SimpleCRUD.Services
{
    public interface IClassificationCodesService
    {
        Task<List<CountryCodes>> GetAllClassificationCodes();
    }

    public class ClassificationCodesService
    {
        private readonly DataContext _context;

        public ClassificationCodesService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<ClassificationCodes>> GetAllClassificationCodes()
        {
            return await _context.ClassificationCodes.ToListAsync();
        }
    }
}
