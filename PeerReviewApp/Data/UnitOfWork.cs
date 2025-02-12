using Microsoft.EntityFrameworkCore;
using PeerReviewApp.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PeerReviewApp.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext context { get; set; }
        public UnitOfWork(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        #region Queries for Model Objects
        public Class GetClass(int id)
        {
            return context.Classes
                .Include(e => e.Enrollments.Where(e => e.ClassId == id))  
                .Include(t => t.Term).Where(t => t.ClassId == id)
                .Include(i => i.Instructor).Where(i => i.ClassId == id)
                .Include(c => c.Course).Where(c => c.ClassId == id)
                .Include(a => a.Assignments).Where(a => a.ClassId == id)
                .Where(s => s.ClassId == id).FirstOrDefault();
        }
        public Term GetTerm(int id)
        {
            return context.Terms.Where(t => t.TermId == id).FirstOrDefault();
        }
        public List<Term> GetTermsByInstitution(int institutionId)
        {
            return context.Terms.Where(t => t.InstitutionId == institutionId).ToList();
        }
        public Assignment GetAssignment(int id)
        {
            return context.Assignments.Where(a => a.AssignmentId == id).FirstOrDefault();
        }
        public Grade GetGrade(int id)
        {
            return context.Grades.Where(g => g.GradeId == id).FirstOrDefault();
        }
        public Course GetCourse(int id)
        {
            return context.Courses
                .Where(c => c.CourseId == id).FirstOrDefault();
        }
        public IQueryable<Class> GetInstructorClasses(string id)
        {
            return context.Classes.Where(c => c.InstructorId == id);
        }
        public Institution GetInstitution(int id)
        {
            return context.Institutions
                .Include(x => x.Members)
                .Include(x => x.Departments)
                .Include(x => x.Terms)
                .Where(i => i.InstitutionId == id).FirstOrDefault();
        }
        public Department GetDepartment(int id)
        {
            return context.Departments
                .Include(x => x.Institution)
                .Where(i => i.DepartmentId == id).FirstOrDefault();
        }
        public Enrollment GetEnrollment(int id)
        {
            return context.Enrollments
                .Include(s => s.Student).Where(s => s.EnrollmentId == id)
                .Include(s => s.Class).Where(s => s.EnrollmentId == id)
                .Where(e => e.EnrollmentId == id).FirstOrDefault();
        }
        public List<Class> GetClassesByTerm(int id)
        {
            return context.Classes.Where(t => t.TermId == id).ToList();
        }
        public List<Class> GetClassesByStudent(string studentId)
        {
            var queryStudentClasses = from c in context.Classes
                       join e in context.Enrollments on c.ClassId equals e.ClassId
                       where e.StudentId == studentId
                       select c;
            return queryStudentClasses.Include(x => x.Term).ToList();
        }
        public List<Enrollment> GetEnrollmentsByStudent(string studentId)
        {
            var queryStudentEnrollments = from e in context.Enrollments
                                      join s in context.Users on e.StudentId equals s.Id
                                      join c in context.Classes on e.ClassId equals c.ClassId
                                      where e.StudentId == studentId
                                      select e;

            return queryStudentEnrollments.ToList();
        }
        public List<Class> GetClassesByInstructor(string instructorId)
        {
            return context.Classes
                .Include(x => x.Term)
                .Where(i => i.InstructorId == instructorId).ToList();
        }

        public List<PeerReviewPartners> GetPRPartnersByClass(int id)
        {
            return context.prPartners
                .Where(p => p.ClassId == id).ToList();
        }

        public async Task<PeerReviewPartners> GetPRPartnersByStudent(int classId, string studentId)
        {
            return await context.prPartners
                .Include(p => p.Student1)
                .Include(p => p.Student2)
                .Where(p => p.ClassId == classId)
                .Where(p => p.Student1Id == studentId || p.Student2Id == studentId)
                .FirstOrDefaultAsync();
        }

        public AppUser GetPeerReviewPartnerByClass(int classId, string studentId)
        {

             var student = context.prPartners
                    .Where(p => p.Student1Id == studentId)
                    .Where(p => p.ClassId == classId)
                    .FirstOrDefault();

            if (student != null) //If student is not null then they are "student1" in the PR partner model, therefore, return student 2 to get the partner
            {
                return context.Users
                    .Where(u => u.Id == student.Student2Id)
                    .FirstOrDefault();
            }
            else //If student was null then check if student exists as "studen2" in the PR partner model, then return student 2 to get the partner. If return is null then partner
            {
                    student = context.prPartners
                       .Where(p => p.Student2Id == studentId)
                       .Where(p => p.ClassId == classId)
                       .FirstOrDefault();

                return context.Users
                    .Where(u => u.Id == student.Student1Id)
                    .FirstOrDefault();
            }

        }

        public async Task<AssignmentSubmission> GetAssignmentSubmissionByStudent(int assignmentId, string studentId)
        {
            return await context.AssignmentSubmissions
                .Where(x => x.AssignmentId == assignmentId)
                .Where(x => x.StudentId == studentId)
                .FirstOrDefaultAsync();
        }
        
        public PeerReviewPartners GetPeerReviewPartners(int id)
        {
            return context.prPartners
                .Where(p => p.Id == id).FirstOrDefault();
        }

        public List<Enrollment> GetEnrollmentsByClass(int classId)
        {
            return context.Enrollments
                .Where(e => e.ClassId == classId).ToList();
        }

        public List<AppUser> GetStudentsByClass(int classId)
        {
            var queryStudents = from s in context.Users
                                join e in context.Enrollments on s.Id equals e.StudentId
                                where e.ClassId == classId
                                select s;
            return queryStudents.ToList();
        }

        public async Task<AssignmentTemplate> GetAssignmentTemplate(int assignmentId)
        {
            return await context.AssignmentTemplates
                .Where(t => t.AssignmentId == assignmentId)
                .FirstOrDefaultAsync();
        }

        public async Task<PeerReviewTemplate> GetPeerReviewTemplate(int assignmentId)
        {
            return await context.PeerReviewTemplates
                .Where(t => t.AssignmentId == assignmentId)
                .FirstOrDefaultAsync();
        }

        public async Task<AssignmentSubmission> GetAssignmentSubmission(int submissionId)
        {
            return await context.AssignmentSubmissions
                .Where(x => x.SubmissionId == submissionId)
                .FirstOrDefaultAsync();
        }
        
        public async Task<PeerReviewSubmission> GetPeerReviewSubmissionByStudent(int assignmentId, string studentId)
        {
            return await context.PeerReviewSubmissions
                .Where(x => x.AssignmentId == assignmentId)
                .Where(x => x.StudentId == studentId)
                .FirstOrDefaultAsync();
        }
        
        public async Task<PeerReviewSubmission> GetPeerReviewSubmission(int submissionId)
        {
            return await context.PeerReviewSubmissions
                .Where(x => x.PeerReviewId == submissionId)
                .FirstOrDefaultAsync();
        }

        public List<AssignmentSubmission> GetAssignmentSubmissions(int assignmentId)
        {
            return context.AssignmentSubmissions.Where(x => x.AssignmentId == assignmentId).ToList();
        }

        public List<PeerReviewSubmission> GetPeerReviewSubmissions(int assignmentId)
        {
            return context.PeerReviewSubmissions.Where(x => x.AssignmentId == assignmentId).ToList();
        }
        #endregion

        #region IQueryable Queries
        public IQueryable<Assignment> Assignments { get { return context.Assignments.AsQueryable(); } }
        public IQueryable<AssignmentSubmission> AssignmentSubmissions { get { return context.AssignmentSubmissions.AsQueryable(); } }
        public IQueryable<AssignmentTemplate> AssignmentTemplates { get { return context.AssignmentTemplates.AsQueryable(); } }
        public IQueryable<Course> Courses { get { return context.Courses.AsQueryable(); } }
        public IQueryable<Department> Departments { get { return context.Departments.AsQueryable(); } }
        public IQueryable<Enrollment> Enrollments
        {
            get
            {
                return context.Enrollments
                    .Include(x => x.Student)
                    .Include(x => x.Class)
                    .AsQueryable();
            }
        }
        public IQueryable<Grade> Grades { get { return context.Grades.AsQueryable(); } }
        public IQueryable<Institution> Institutions
        {
            get
            {
                return context.Institutions
                    .Include(x => x.Members)
                    .Include(x => x.Departments)
                    .AsQueryable();
            }
        } 
        public IQueryable<PeerReviewSubmission> PeerReviewSubmissions { get { return context.PeerReviewSubmissions.AsQueryable(); } }
        public IQueryable<PeerReviewTemplate> PeerReviewTemplates { get { return context.PeerReviewTemplates.AsQueryable(); } }
        public IQueryable<Class> Classes
        {
            get
            {
                return context.Classes
                    .Include(x => x.Term)
                    .Include(x => x.Enrollments)
                        .ThenInclude(x => x.Student)
                    .Include(x => x.Assignments)
                    .AsQueryable();
            }
        }
        public IQueryable<Term> Terms (int id)
        { 
                return context.Terms.Where(x => x.InstitutionId == id).AsQueryable(); 
        }

        #endregion

        #region Repositories

        private IRepository<Assignment> AssignmentData;
        public IRepository<Assignment> AssignmentsRepo
        {
            get
            {
                if (AssignmentData == null)
                    AssignmentData = new Repository<Assignment>(context);
                return AssignmentData;
            }
        }
        private IRepository<AssignmentSubmission> AssignmentSubmissionData;
        public IRepository<AssignmentSubmission> AssignmentSubmissionsRepo
        {
            get
            {
                if (AssignmentSubmissionData == null)
                    AssignmentSubmissionData = new Repository<AssignmentSubmission>(context);
                return AssignmentSubmissionData;
            }
        }
        private IRepository<AssignmentTemplate> AssignmentTemplateData;
        public IRepository<AssignmentTemplate> AssignmentTemplatesRepo
        {
            get
            {
                if (AssignmentTemplateData == null)
                    AssignmentTemplateData = new Repository<AssignmentTemplate>(context);
                return AssignmentTemplateData;
            }
        }
        private IRepository<Course> CourseData;
        public IRepository<Course> CourseRepo
        {
            get
            {
                if (CourseData == null)
                    CourseData = new Repository<Course>(context);
                return CourseData;
            }
        }
        private IRepository<Department> DepartmentData;
        public IRepository<Department> DepartmentsRepo
        {
            get
            {
                if (DepartmentData == null)
                    DepartmentData = new Repository<Department>(context);
                return DepartmentData;
            }
        }
        private IRepository<Enrollment> EnrollmentData;
        public IRepository<Enrollment> EnrollmentsRepo
        {
            get
            {
                if (EnrollmentData == null)
                    EnrollmentData = new Repository<Enrollment>(context);
                return EnrollmentData;
            }
        }
        private IRepository<Grade> GradeData;
        public IRepository<Grade> GradesRepo
        {
            get
            {
                if (GradeData == null)
                    GradeData = new Repository<Grade>(context);
                return GradeData;
            }
        }
        private IRepository<Institution> InstitutionData;
        public IRepository<Institution> InstitutionsRepo
        {
            get
            {
                if (InstitutionData == null)
                    InstitutionData = new Repository<Institution>(context);
                return InstitutionData;
            }
        }
        private IRepository<PeerReviewSubmission> PeerReviewSubmissionData;
        public IRepository<PeerReviewSubmission> PeerReviewSubmissionsRepo
        {
            get
            {
                if (PeerReviewSubmissionData == null)
                    PeerReviewSubmissionData = new Repository<PeerReviewSubmission>(context);
                return PeerReviewSubmissionData;
            }
        }
        private IRepository<PeerReviewTemplate> PeerReviewTemplateData;
        public IRepository<PeerReviewTemplate> PeerReviewTemplatesRepo
        {
            get
            {
                if (PeerReviewTemplateData == null)
                    PeerReviewTemplateData = new Repository<PeerReviewTemplate>(context);
                return PeerReviewTemplateData;
            }
        }
        private IRepository<Class> ClassData;
        public IRepository<Class> ClassesRepo
        {
            get
            {
                if (ClassData == null)
                    ClassData = new Repository<Class>(context);
                return ClassData;
            }
        }
        private IRepository<Term> TermData;
        public IRepository<Term> TermsRepo
        {
            get
            {
                if (TermData == null)
                    TermData = new Repository<Term>(context);
                return TermData;
            }
        }
        private IRepository<PeerReviewPartners> prPartnersData;
        public IRepository<PeerReviewPartners> prPartnersRepo
        {
            get
            {
                if (prPartnersData == null)
                    prPartnersData = new Repository<PeerReviewPartners>(context);
                return prPartnersData;
            }
        }
        #endregion
    }
}
