namespace CourseLibrary.API.RequestParameters
{
    public abstract class CommonRequestParameters
    {
        private int _pageSize = 10;
        private int _pageNumber = 1;
        private const int MaxPages = 100;

        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                if (value > 0) _pageNumber = value;
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value is >= 1 and < MaxPages) _pageSize = value;
            }
        }
    }
}
