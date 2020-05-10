
namespace Data
{
    /// <summary>
    /// Defines the class for SelectOptions
    /// </summary>
    public class SelectOptions
    {
        public bool JoinChild { get; set; }

        public object SearchParameters { get; set; }

        public object OrderByColumns { get; set; }
        public bool? OrderAscending { get; set; }

        public int? TopCount { get; set; }
        public bool IsForUpdate { get; set; }
        public string Index { get; set; }
    }
}
