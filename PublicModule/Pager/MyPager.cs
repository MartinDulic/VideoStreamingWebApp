namespace AdministrationModule.Pager
{
    public class MyPager
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        
        public MyPager(int totalItems, int currentPage, int pageSize = 10)
        {
            CurrentPage = currentPage;
            TotalItems = totalItems;
            PageSize = pageSize;

            TotalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

            StartPage = 1;
            
            EndPage = (int)Math.Ceiling((decimal)TotalItems / (decimal)PageSize);
            if(TotalPages == 0) EndPage = 1;
        }
    }
}
