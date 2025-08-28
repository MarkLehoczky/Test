namespace VidHub.Core
{
    public class Transfer
    {
        public bool IsActive { get; set; } = true;
        public bool IsLoading { get; set; } = false;
        public int LoadedCount { get; set; } = 0;
        public int TotalCount { get; set; } = 0;


        public Transfer() { }
        public Transfer(int totalCount)
        {
            TotalCount = totalCount;
        }

        public void Increment()
        {
            LoadedCount++;
        }

        public void AddTotalCount(int totalCount)
        {
            TotalCount = totalCount;
        }


        public static Transfer operator ++(Transfer transfer)
        {
            if (transfer.LoadedCount < transfer.TotalCount)
            {
                transfer.LoadedCount++;
            }
            return transfer;
        }
        public static Transfer operator --(Transfer transfer)
        {
            if (transfer.LoadedCount > 0)
            {
                transfer.LoadedCount--;
            }
            return transfer;
        }

        public static implicit operator Transfer(int totalCount) => new(totalCount);
        public static explicit operator int(Transfer transfer) => transfer.TotalCount > 0 ? (100 * transfer.LoadedCount) / transfer.TotalCount : 0;
        public static explicit operator double(Transfer transfer) => transfer.TotalCount > 0 ? (double)transfer.LoadedCount / transfer.TotalCount : 0;
    }
}
