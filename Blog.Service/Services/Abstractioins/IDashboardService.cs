using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Service.Services.Abstractioins
{
    public interface IDashboardService
    {
        Task<List<int>> GetYearlyArtileCount();
        Task<int> GetTotalArticleCount();
        Task<int> GetTotalCategoryCount();
    }
}
