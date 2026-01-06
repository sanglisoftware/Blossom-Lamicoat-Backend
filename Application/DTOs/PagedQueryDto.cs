namespace Api.Application.DTOs
{
    public class FilterDto
    {
        public string Field { get; set; } = "";
        public string Type  { get; set; } = "";   // e.g. "like", "eq"
        public string Value { get; set; } = "";
    }

    public class SortDto
    {
        public string Field { get; set; } = "";
        public string Dir   { get; set; } = "asc"; // "asc" or "desc"
    }

    public class PagedQueryDto
    {
        public List<FilterDto> filter { get; set; } = new();
        public List<SortDto>   sort   { get; set; } = new();
        public int             page   { get; set; } = 1;
        public int             size   { get; set; } = 10;
    }

    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items      { get; set; } = Array.Empty<T>();
        public int            TotalCount { get; set; }
        public int            Page       { get; set; }
        public int            Size       { get; set; }
    }
}
