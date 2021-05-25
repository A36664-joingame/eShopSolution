using eShopSolution.Data.EF;
using eShopSolution.ViewModels.Utilities.Slides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Utilities.Slides
{
    public class SlideService : ISlideService
    {
        private readonly IConfiguration _configuration;
        private readonly EShopDbContext _context;

        public SlideService(IConfiguration configuration, EShopDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<List<SlideVm>> GetAll()
        {
            var slids = await _context.Slides.OrderBy(x => x.SortOrder)
                .Select(x => new SlideVm()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Url = x.Url,
                    Image = x.Image
                }).ToListAsync();

            return slids;
        }
    }
}