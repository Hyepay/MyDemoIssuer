//using mdes_digitization.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MyIssuerDemo.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options):base(options)
        {

        }
        
    }
    
}
