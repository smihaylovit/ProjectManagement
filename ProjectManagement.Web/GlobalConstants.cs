namespace ProjectManagement.Web
{
    public static class GlobalConstants
    {
        public const string DbInitializationStoredProcedureName = "usp_Initialize_DB";
        public const string UserMaxWorkHoursPerDayTriggerName = "TR_TimeLogs_UserMaxWorkHoursPerDay_AI_AU";

        public const int UsersCount = 100;
        public const int UsersPageSize = 10;
        public const int UsersPerformanceSize = 10;
        public const int UserTimeLogsMinCount = 1;
        public const int UserTimeLogsMaxCount = 20;
        public const float UserMaxWorkHoursPerDay = 8.00f;

        public const float TimeLogMinHours = 0.25f;
        public const float TimeLogMaxHours = 8.00f;
        public static readonly DateTime TimeLogMinDate = new DateTime(2023, 1, 1);

        public static readonly string[] ProjectNames = { "My own", "Free Time", "Work" };

        public static readonly string[] UserFirstNames =
            { "John", "Gringo", "Mark", "Lisa", "Maria", "Sonya", "Philip", "Jose", "Lorenzo", "George", "Justin" };
        public static readonly string[] UserLastNames =
            { "Johnson", "Lamas", "Jackson", "Brown", "Mason", "Rodriguez", "Roberts", "Thomas", "Rose", "McDonalds" };
        public static readonly string[] UserEmailDomains = { "hotmail.com", "gmail.com", "live.com" };
    }
}
