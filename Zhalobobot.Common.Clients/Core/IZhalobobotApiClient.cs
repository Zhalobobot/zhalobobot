using Zhalobobot.Common.Clients.Feedback;
using Zhalobobot.Common.Clients.Student;
using Zhalobobot.Common.Clients.Subject;

namespace Zhalobobot.Common.Clients.Core
{
    public interface IZhalobobotApiClient
    {
        IFeedbackClient Feedback { get; }
        ISubjectClient Subject { get; }
        IStudentClient Student { get; }
    }
}