using System.Text;

namespace ProjectManagement.Web.Data.Initialization
{
    public class DbInitializationSqlGenerator
    {
        public string CreateDbInitializationStoredProcedureExecutionSql()
        {
            return $"EXEC {GlobalConstants.DbInitializationStoredProcedureName}";
        }

        public string CreateUserMaxWorkHoursPerDayTriggerSql()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"CREATE OR ALTER TRIGGER {GlobalConstants.UserMaxWorkHoursPerDayTriggerName}");
            sb.AppendLine("ON TimeLogs");
            sb.AppendLine("AFTER INSERT, UPDATE");
            sb.AppendLine("AS");
            sb.AppendLine("IF EXISTS");
            sb.AppendLine("(SELECT SUM(t.Hours) FROM Users u");
            sb.AppendLine("JOIN TimeLogs t ON u.Id = t.UserId");
            sb.AppendLine("GROUP BY u.Id, t.Date");
            sb.AppendLine($"HAVING SUM(t.Hours) > {GlobalConstants.UserMaxWorkHoursPerDay})");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"RAISERROR('User work hours should be maximum {GlobalConstants.UserMaxWorkHoursPerDay} per day', 16, 10)");
            sb.AppendLine("ROLLBACK TRANSACTION");
            sb.AppendLine("END");

            return sb.ToString();
        }

        public string CreateDbInitializationStoredProcedureSql()
        {
            var gen = new RandomDataGenerator();
            var users = gen.GetUsersData();
            var projects = gen.GetProjectsData();
            var timeLogs = new List<TimeLogData>();

            foreach (var user in users)
            {
                timeLogs.AddRange(gen.GetTimeLogs(user));
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"CREATE OR ALTER PROCEDURE {GlobalConstants.DbInitializationStoredProcedureName}");
            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine("IF EXISTS (SELECT Id FROM TimeLogs)");
            sb.AppendLine("BEGIN");
            sb.AppendLine("DELETE FROM TimeLogs");
            sb.AppendLine("DBCC CHECKIDENT (TimeLogs, RESEED, 0)");
            sb.AppendLine("END");
            sb.AppendLine("IF EXISTS (SELECT Id FROM Projects)");
            sb.AppendLine("BEGIN");
            sb.AppendLine("DELETE FROM Projects");
            sb.AppendLine("DBCC CHECKIDENT (Projects, RESEED, 0)");
            sb.AppendLine("END");
            sb.AppendLine("IF EXISTS (SELECT Id FROM Users)");
            sb.AppendLine("BEGIN");
            sb.AppendLine("DELETE FROM Users");
            sb.AppendLine("DBCC CHECKIDENT (Users, RESEED, 0)");
            sb.AppendLine("END");

            foreach (var project in projects)
            {
                sb.AppendLine($"INSERT INTO Projects (Name) VALUES ('{project.Name}')");
            }

            foreach (var user in users)
            {
                sb.AppendLine($"INSERT INTO Users (FirstName, LastName, Email) VALUES ('{user.FirstName}', '{user.LastName}', '{user.Email}')");
            }

            foreach (var timeLog in timeLogs)
            {
                sb.AppendLine($"INSERT INTO TimeLogs (UserId, ProjectId, Date, Hours) VALUES ({timeLog.UserId}, {timeLog.ProjectId}, '{timeLog.Date:yyyy-MM-dd}', {timeLog.Hours:f2})");
            }

            sb.AppendLine("END");
            return sb.ToString();
        }
    }
}
