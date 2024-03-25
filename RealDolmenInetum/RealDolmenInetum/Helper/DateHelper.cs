namespace RealDolmenInetum.Helper
{
    public static class DateHelper
    {
        // bereken van dagen sinds een bepaalde datum
        public static int CalculateDaysSince(DateTime startDate)
        {
            return (DateTime.Now - startDate).Days;
        }
    }

}
