using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRManagementSystem.Models
{
    public class Model<T>
    {
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public string nextLink { get; set; }
        public string prevLink { get; set; }
        public T[] Results { get; set; }
    }
    public class ModelPaging<T>
    {
        public IPagedList<T> Results { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public string nextLink { get; set; }
        public string prevLink { get; set; }
    }
    public class ModelList<T>
    {
        public T[]Results { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
        public string nextLink { get; set; }
        public string prevLink { get; set; }
    }
}