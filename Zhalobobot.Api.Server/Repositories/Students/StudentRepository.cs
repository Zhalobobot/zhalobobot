using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Zhalobobot.Api.Server.Repositories.Common;
using Zhalobobot.Common.Helpers.Helpers;
using Zhalobobot.Common.Models.Commons;
using Zhalobobot.Common.Models.Student;

namespace Zhalobobot.Api.Server.Repositories.Students
{
    public class StudentRepository : GoogleSheetsRepositoryBase, IStudentRepository
    {
        private string StudentsRange { get; }

        public StudentRepository(IConfiguration configuration) 
            : base(configuration, configuration["FeedbackSpreadSheetId"])
        {
            StudentsRange = configuration["StudentsRange"];
        }

        public async Task Add(Student student)
        {
            var values = new List<object>
            {
                student.Id,
                student.Username,
                student.Course,
                student.Group,
                student.Subgroup,
                student.Name?.LastName ?? string.Empty,
                student.Name?.FirstName ?? string.Empty,
                student.Name?.MiddleName ?? string.Empty
            };

            await AppendRequest(StudentsRange, values).ExecuteAsync();
        }

        public async Task<Student[]> GetAll()
        {
            var values = await GetRequest(StudentsRange).ExecuteAsync();

            return values.Values.Select(student => new Student(
                student[0] as string ?? throw new ValidationException("Id is empty!"),
                student[1] as string ?? string.Empty,
                (Course)ParsingHelper.ParseInt(student[2]),
                (Group)ParsingHelper.ParseInt(student[3]),
                (Subgroup)ParsingHelper.ParseInt(student[4]),
                null)) // TODO: вернуть имя вместо null (пока не нужно)
                .ToArray();
        }

        public async Task<Student[]> GetByCourseAndGroupAndSubgroup(Course course, Group group, Subgroup subgroup)
        {
            var students = await GetAll();
            
            return students
                .Where(s => s.Course == course && s.Group == group && s.Subgroup == subgroup)
                .ToArray();
        }
    }
}