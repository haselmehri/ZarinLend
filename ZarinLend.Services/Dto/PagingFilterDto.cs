using System;
using System.Collections.Generic;

namespace Services.Dto
{
    [Serializable]
    public class PagingFilterDto<T> : PagingFilterDto 
        where T : class
    {
        public T Filter { get; set; }
    }

    [Serializable]
    public class PagingFilterDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; } = 10;
        public string? SortDirection { get; set; }
        public string? SortOrder { get; set; }
        public List<Filter>? FilterList { get; set; }
    }

    [Serializable]
    public class Filter
    {
        public string? PropertyName { get; set; }
        public Operator Operator { get; set; } = Operator.Equal;
        public dynamic? PropertyValue { get; set; }
    }

    public enum Operator
    {
        Equal = 1,
        NotEqual = 2,
        GreaterThanOrEqual = 3,
        LesserThanOrEqual = 4,
        GreaterThan = 5,
        LesserThan  = 6,
        Contain = 7,//Sql Like 
    }
}
