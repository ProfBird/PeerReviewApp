using PeerReviewApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PeerReviewApp.Data
{
    public interface IUnitOfWork
    {
        public Class GetClass(int id);
        public Course GetCourse(int id);
        public Term GetTerm(int id);
        public Assignment GetAssignment(int id);
        public Institution GetInstitution(int id);
        public Department GetDepartment(int id);
        public Grade GetGrade(int id);
        public List<Class> GetClassesByTerm(int id);
        public List<Class> GetClassesByStudent(string studentId);
        public List<Class> GetClassesByInstructor(string instructorId);
        public List<Enrollment> GetEnrollmentsByClass(int classId);
        public List<Enrollment> GetEnrollmentsByStudent(string studentId);
        public List<AppUser> GetStudentsByClass(int classId);
        public Enrollment GetEnrollment(int id);
        public PeerReviewPartners GetPeerReviewPartners(int id);
        public IQueryable<Class> GetInstructorClasses(string id);
        public List<AssignmentSubmission> GetAssignmentSubmissions(int assignmentId);
        public List<PeerReviewSubmission> GetPeerReviewSubmissions(int assignmentId);
        public List<PeerReviewPartners> GetPRPartnersByClass(int id);
        public Task<PeerReviewPartners> GetPRPartnersByStudent(int classId, string studentId);
        public Task<AssignmentTemplate> GetAssignmentTemplate(int assignmentId);
        public Task<PeerReviewTemplate> GetPeerReviewTemplate(int assignmentId);
        public Task<AssignmentSubmission> GetAssignmentSubmissionByStudent(int assignmentId, string studentId);
        public AppUser GetPeerReviewPartnerByClass(int classId, string studentId);
        public Task<AssignmentSubmission> GetAssignmentSubmission(int submissionId);
        public Task<PeerReviewSubmission> GetPeerReviewSubmissionByStudent(int assignmentId, string studentId);
        public Task<PeerReviewSubmission> GetPeerReviewSubmission(int submissionId);
        IQueryable<Institution> Institutions { get;  }
        IQueryable<Assignment> Assignments { get; }
        IQueryable<AssignmentSubmission> AssignmentSubmissions { get; }
        IQueryable<AssignmentTemplate> AssignmentTemplates { get; }
        IQueryable<Course> Courses { get; }
        IQueryable<Department> Departments { get; }
        IQueryable<Enrollment> Enrollments { get; }
        IQueryable<Grade> Grades { get; }       
        IQueryable<PeerReviewSubmission> PeerReviewSubmissions { get; }
        IQueryable<PeerReviewTemplate> PeerReviewTemplates { get; }
        IQueryable<Class> Classes { get; }
        IQueryable<Term> Terms(int id);
        public IRepository<Assignment> AssignmentsRepo { get; }
        public IRepository<AssignmentSubmission> AssignmentSubmissionsRepo { get; }
        public IRepository<AssignmentTemplate> AssignmentTemplatesRepo { get; }
        public IRepository<Course> CourseRepo { get; }
        public IRepository<Department> DepartmentsRepo { get; }
        public IRepository<Enrollment> EnrollmentsRepo { get; }
        public IRepository<Grade> GradesRepo { get; }
        public IRepository<Institution> InstitutionsRepo { get; }
        public IRepository<PeerReviewSubmission> PeerReviewSubmissionsRepo { get; }
        public IRepository<PeerReviewTemplate> PeerReviewTemplatesRepo { get; }
        public IRepository<Class> ClassesRepo { get; }
        public IRepository<Term> TermsRepo { get; }
        public IRepository<PeerReviewPartners> prPartnersRepo { get; }

    }
}
