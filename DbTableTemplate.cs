using SQLite;

namespace Punch_In_App
{
    public class DbTableTemplate
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string currentDate { get; set; }

        public string clockIn { get; set; }

        public string breakOut { get; set; }

        public string breakIn { get; set; }

        public string clockOut { get; set; }

        public string TotalWorkedHours { get; set; }

        public DbTableTemplate()
        {
            currentDate = string.Empty;
            clockIn = string.Empty;
            breakOut = string.Empty;
            breakIn = string.Empty;
            clockOut = string.Empty;
        }
    }
}
