namespace ProjectManagement.Web.Data.Initialization
{
    public class RandomDataGenerator
    {
        private static readonly Random Random = new Random();

        public List<UserData> GetUsersData()
        {
            var usersData = new List<UserData>();

            for (int counter = 1; counter <= GlobalConstants.UsersCount; counter++)
            {
                var user = new UserData
                {
                    FirstName = GlobalConstants.UserFirstNames[Random.Next(GlobalConstants.UserFirstNames.Length)],
                    LastName = GlobalConstants.UserLastNames[Random.Next(GlobalConstants.UserLastNames.Length)],
                    Email = GlobalConstants.UserEmailDomains[Random.Next(GlobalConstants.UserEmailDomains.Length)]
                };

                user.Email = $"{user.FirstName}.{user.LastName}@{user.Email}";

                if (usersData.Any(u => u.Email == user.Email))
                {
                    counter--;
                    continue;
                }

                user.Id = counter;
                usersData.Add(user);
            }

            return usersData;
        }

        public List<TimeLogData> GetTimeLogs(UserData user)
        {
            var timeLogsData = new List<TimeLogData>();
            var timeLogsCount = Random.Next(GlobalConstants.UserTimeLogsMinCount, GlobalConstants.UserTimeLogsMaxCount + 1);

            for (int counter = 1; counter <= timeLogsCount; counter++)
            {
                var timeLog = new TimeLogData
                {
                    UserId = user.Id,
                    ProjectId = Random.Next(1, GlobalConstants.ProjectNames.Length + 1),
                    Date = DateOnly.FromDateTime(GlobalConstants.TimeLogMinDate.AddDays(
                        Random.Next((DateTime.Today.Date - GlobalConstants.TimeLogMinDate).Days))),
                    Hours = float.MaxValue
                };

                while (timeLog.Hours > GlobalConstants.TimeLogMaxHours)
                {
                    var randomFloat = GlobalConstants.TimeLogMinHours + Random.NextSingle() * GlobalConstants.TimeLogMaxHours;
                    timeLog.Hours = float.Parse(randomFloat.ToString("f2"));
                }

                var userWorkedHours = timeLogsData.Where(tl => tl.Date == timeLog.Date).Sum(tl => tl.Hours);

                if (userWorkedHours + timeLog.Hours > GlobalConstants.UserMaxWorkHoursPerDay)
                {
                    counter--;
                    continue;
                }

                timeLogsData.Add(timeLog);
            }

            return timeLogsData;
        }
    }
}
