using Microsoft.EntityFrameworkCore;
using SimpleCRUD.Data;
using SimpleCRUD.Model;

namespace SimpleCRUD.Services
{
    public interface ICountryCodesService
    {
        Task<List<CountryCodes>> GetAllCountryCodes();
    }

    public class CountryCodesService
    {
        private readonly DataContext _context;

        public CountryCodesService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<CountryCodes>> GetAllCountryCodes()
        {
            return await _context.CountryCodes.ToListAsync();
        }
    }
}
