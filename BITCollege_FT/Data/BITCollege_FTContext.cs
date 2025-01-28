using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BITCollege_FT.Data
{
    public class BITCollege_FTContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        //Instantiate BITCollege_FTContext object
        //BITCollege_FTContext db = new BITCollege_FTContext();
    
        public BITCollege_FTContext() : base("name=BITCollege_FTContext")
        {
        }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.AcademicProgram> AcademicPrograms { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.GradePointState> GradePointStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.Registration> Registrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.Student> Students { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.SuspendedState> SuspendedStates  { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.ProbationState> ProbationStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.RegularState> RegularStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.HonoursState> HonoursStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.GradedCourse> GradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.AuditCourse> AuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.MasteryCourse> MasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.NextUniqueNumber> NextUniqueNumbers { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.NextAuditCourse> NextAuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.NextGradedCourse> NextGradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.NextMasteryCourse> NextMasteryCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.NextRegistration> NextRegistrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_FT.Models.NextStudent> NextStudents { get; set; }
    }
}
