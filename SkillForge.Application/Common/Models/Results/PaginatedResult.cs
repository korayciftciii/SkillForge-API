using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Application.Common.Models.Results
{
    public class PaginatedResult<T>
    {
        public bool Success { get; set; }
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PaginatedResult(List<T> data, int totalCount, int page, int pageSize)
        {
            Success = true;
            Data = data;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }
}
