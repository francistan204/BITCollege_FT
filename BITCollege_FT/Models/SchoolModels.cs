/*
* Name: Francis Tan
* Program: Business Information Technology
* Course: ADEV-3008 Programming 3
* Created: January 4, 2024
* Updated: February 5, 2024
*/
using BITCollege_FT.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Utility;

namespace BITCollege_FT.Models
{
    /// <summary>
    /// Student Model. Represents the Students table in the database.
    /// </summary>
    public class Student
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("GradePointState")]
        public int GradePointStateId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Display(Name = "Student\nNumber")]
        public long StudentNumber { get; set; }

        [Required]
        [Display(Name = "First\nName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last\nName")]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required(ErrorMessage = "Enter a valid Canadian province code")]
        [RegularExpression("^(N[BLSTU]|[AMN]B|[BQ]C|ON|PE|SK|YT)")]
        public string Province { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Grade Point\nAverage")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Range(0, 4.5)]
        public double? GradePointAverage { get; set; }

        [Required]
        [Display(Name = "Fees")]
        [DisplayFormat(DataFormatString = "{0:c2}")]
        public double OutstandingFees { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        [Display(Name = "Address")]
        public string FullAddress
        {
            get
            {
                return String.Format("{0} {1} {2}", Address, City, Province);
            }
        }

        /// <summary>
        /// This method will initiate the process of ensuring that the Student is always
        /// associated with the correct state.
        /// </summary>
        public void ChangeState()
        {
            BITCollege_FTContext db = new BITCollege_FTContext();

            GradePointState current = db.GradePointStates.Find(GradePointStateId);

            int next = 0;

            while (current.GradePointStateId != next)
            {
                current.StateChangeCheck(this);
                next = current.GradePointStateId;
                current = db.GradePointStates.Find(GradePointStateId);
            }
        }

        public void SetNextStudentNumber()
        {
            StudentNumber = (long)StoredProcedures.NextNumber("NextStudent");
        }

        //navigation properties
        public virtual GradePointState GradePointState { get; set; }
        public virtual AcademicProgram AcademicProgram { get; set; }
        public virtual ICollection<Registration> Registration { get; set; }

    }

    /// <summary>
    /// AcademicProgram Model. Represents the AcademicProgram table in the database.
    /// </summary>
    public class AcademicProgram
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AcademicProgramId { get; set; }

        [Required]
        [Display(Name = "Program")]
        public string ProgramAcronym { get; set; }

        [Required]
        [Display(Name = "Program\nName")]
        public string Description { get; set; }

        //navigational properties

        public virtual ICollection<Student> Student { get; set; }
        public virtual ICollection<Course> Course { get; set; }
    }

    /// <summary>
    /// GradePointState model. Represents the GradePointState table in the database.
    /// </summary>
    public abstract class GradePointState
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int GradePointStateId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Lower\nLimit")]
        public double LowerLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Upper\nLimit")]
        public double UpperLimit { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n3}")]
        [Display(Name = "Tuition\nLimit\nFactor")]
        public double TuitionRateFactor { get; set; }

        [Display(Name = "State")]
        public string Description { get { return BusinessRules.ParseString(GetType().Name, "State"); } }

        /// <summary>
        /// Method will check to see whether a GradePointAverage is above an upper limit or below a lower limit and assigns the correct GradePointStateId
        /// which will switch the state of the student to either the next or previous state.
        /// </summary>
        /// <param name="student"></param>
        public virtual void StateChangeCheck(Student student) { }

        /// <summary>
        /// Adjusts tuition rate factor based on GradePointAverage and/or classes completed.
        /// </summary>
        /// <param name="Student"></param>
        /// <returns> The tuition rate adjustment </returns>
        public virtual double TuitionRateAdjustment(Student Student)
        {
            return this.TuitionRateFactor;
        }

        protected static BITCollege_FTContext db = new BITCollege_FTContext();
       
        //navigational properties
        public virtual ICollection<Student> Student { get; set; }
    }

    /// <summary>
    /// SuspendedState model. Represents the SuspendedState table in the database.
    /// </summary>
    public class SuspendedState : GradePointState
    {
        private static SuspendedState suspendedState;

        /// <summary>
        /// Initializes a new instance of the SuspendedState class.
        /// </summary>
        private SuspendedState()
        {
            LowerLimit = 0.00;
            UpperLimit = 1.0;
            TuitionRateFactor = 1.1;
        }

        /// <summary>
        /// This method will implement the Singleton pattern. it will return an instance
        /// of the SuspendedState class. If there is no instance of the class, it will create a new instance
        /// and persist it to the database.
        /// </summary>
        /// <returns> An instance of SuspendedState class</returns>
        public static SuspendedState GetInstance()
        {
            if (suspendedState == null)
            {
                suspendedState = db.SuspendedStates.SingleOrDefault();

                if (suspendedState == null)
                {
                    suspendedState = new SuspendedState();
                    db.GradePointStates.Add(suspendedState);
                    db.SaveChanges();
                }
            }
            return suspendedState;
        }

        /// <summary>
        /// Method will check to see whether a GradePointAverage is above an upper limit or below a lower limit and assigns the correct GradePointStateId
        /// which will switch the state of the student to either the next or previous state.
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage > UpperLimit)
            {
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Adjusts tuition rate factor based on GradePointAverage and/or classes completed.
        /// </summary>
        /// <param name="Student"></param>
        /// <returns> The tuition rate adjustment </returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double value = this.TuitionRateFactor;
            if (student.GradePointAverage < 0.75 && student.GradePointAverage >= 0.50)
            {
                value += 0.02;
            }
            if (student.GradePointAverage < 0.50)
            {
                value += 0.05;
            }
            return value;
        }
    }

    /// <summary>
    /// ProbationState model. Represents the ProbationState table in the database.
    /// </summary>
    public class ProbationState : GradePointState
    {
        private static ProbationState probationState;

        /// <summary>
        /// Initializes a new instance of the ProbationState class.
        /// </summary>
        private ProbationState()
        {
            LowerLimit = 1.00;
            UpperLimit = 2.00;
            TuitionRateFactor = 1.075;
        }

        /// <summary>
        /// This method will implement the Singleton pattern. it will return an instance
        /// of the ProbationState class. If there is no instance of the class, it will create a new instance
        /// and persist it to the database.
        /// </summary>
        /// <returns> An instance of ProbationState class</returns>
        public static ProbationState GetInstance()
        {
            if (probationState == null)
            {
                probationState = db.ProbationStates.SingleOrDefault();

                if (probationState == null)
                {
                    probationState = new ProbationState();
                    db.GradePointStates.Add(probationState);
                    db.SaveChanges();
                }
            }
            return probationState;
        }

        /// <summary>
        /// Method will check to see whether a GradePointAverage is above an upper limit or below a lower limit and assigns the correct GradePointStateId
        /// which will switch the state of the student to either the next or previous state.
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage > UpperLimit)
            {
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
            }
            else if (student.GradePointAverage < LowerLimit)
            {
                student.GradePointStateId = SuspendedState.GetInstance().GradePointStateId;
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Adjusts tuition rate factor based on GradePointAverage and/or classes completed.
        /// </summary>
        /// <param name="Student"></param>
        /// <returns> The tuition rate adjustment </returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double value = this.TuitionRateFactor;

            IQueryable<Registration> studentCourses = db.Registrations.Where(x => x.StudentId == student.StudentId
                                  && x.Grade != null);
            int courseCount = studentCourses.Count();

            if (courseCount >= 5)
            {
                value -= 0.04;
            }
            return value;
        }
    }

    /// <summary>
    /// RegularState model. Represents the RegularState table in the database.
    /// </summary>
    public class RegularState : GradePointState
    {
        private static RegularState regularState;

        /// <summary>
        /// Initializes a new instance of the RegularState class.
        /// </summary>
        private RegularState()
        {
            LowerLimit = 2.00;
            UpperLimit = 3.70;
            TuitionRateFactor = 1.0;
        }

        /// <summary>
        /// This method will implement the Singleton pattern. it will return an instance
        /// of the RegularState class. If there is no instance of the class, it will create a new instance
        /// and persist it to the database.
        /// </summary>
        /// <returns> An instance of RegularState class</returns>
        public static RegularState GetInstance()
        {
            if (regularState == null)
            {
                regularState = db.RegularStates.SingleOrDefault();

                if (regularState == null)
                {
                    regularState = new RegularState();
                    db.GradePointStates.Add(regularState);
                    db.SaveChanges();
                }
            }
            return regularState;
        }

        /// <summary>
        /// Method will check to see whether a GradePointAverage is above an upper limit or below a lower limit and assigns the correct GradePointStateId
        /// which will switch the state of the student to either the next or previous state.
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage > UpperLimit)
            {
                student.GradePointStateId = HonoursState.GetInstance().GradePointStateId;
            }
            else if (student.GradePointAverage < LowerLimit)
            {
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Adjusts tuition rate factor based on GradePointAverage and/or classes completed.
        /// </summary>
        /// <param name="Student"></param>
        /// <returns> The tuition rate adjustment </returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double value = this.TuitionRateFactor;
            return value;
        }
    }

    /// <summary>
    /// HonoursState model. Represents the HonoursState table in the database.
    /// </summary>
    public class HonoursState : GradePointState
    {
        private static HonoursState honoursState;

        /// <summary>
        /// Initializes a new instance of the HonoursState class.
        /// </summary>
        private HonoursState()
        {
            LowerLimit = 3.70;
            UpperLimit = 4.50;
            TuitionRateFactor = 0.9;
        }

        /// <summary>
        /// This method will implement the Singleton pattern. it will return an instance
        /// of the HonoursState class. If there is no instance of the class, it will create a new instance
        /// and persist it to the database.
        /// </summary>
        /// <returns> An instance of HonoursState class</returns>
        public static HonoursState GetInstance()
        {
            if (honoursState == null)
            {
                honoursState = db.HonoursStates.SingleOrDefault();

                if (honoursState == null)
                {
                    honoursState = new HonoursState();
                    db.GradePointStates.Add(honoursState);
                    db.SaveChanges();
                }
            }
            return honoursState;
        }

        /// <summary>
        /// Method will check to see whether a GradePointAverage is above an upper limit or below a lower limit and assigns the correct GradePointStateId
        /// which will switch the state of the student to either the next or previous state.
        /// </summary>
        /// <param name="student"></param>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage < LowerLimit)
            {
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Adjusts tuition rate factor based on GradePointAverage and/or classes completed.
        /// </summary>
        /// <param name="Student"></param>
        /// <returns> The tuition rate adjustment </returns>
        public override double TuitionRateAdjustment(Student student)
        {
            double value = this.TuitionRateFactor;

            IQueryable<Registration> studentCourses = db.Registrations.Where(x => x.StudentId == student.StudentId
                                  && x.Grade != null);
            int courseCount = studentCourses.Count();

            if (courseCount >= 5)
            {
                value -= 0.05;
            }
            if (student.GradePointAverage > 4.25)
            {
                value -= 0.02;
            }
            return value;
        }
    }

    /// <summary>
    /// Course model. Represents the Course table in the database.
    /// </summary>
    public abstract class Course
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int CourseId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Display(Name = "Course\nNumber")]
        public string CourseNumber { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Credit\nHours")]
        public double CreditHours { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        [Display(Name = "Tuition")]
        public double TuitionAmount { get; set; }

        [Display(Name = "Course\nType")]
        public string CourseType { get { return BusinessRules.ParseString(GetType().Name, "Course"); } }

        public string Notes { get; set; }

        public virtual void SetNextCourseNumber()
        {

        }

        //navigational properties
        public virtual AcademicProgram AcademicProgram { get; set; }
        public virtual ICollection<Registration> Registration { get; set; }
    }

    /// <summary>
    /// GradedCourse model. Represents the GradedCourse table in the database.
    /// </summary>
    public class GradedCourse : Course
    {
        [Required]
        [Display(Name = "Assignments")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public double AssignmentWeight { get; set; }

        [Required]
        [Display(Name = "Exams")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public double ExamWeight { get; set; }

        /// <summary>
        /// Sets the NextCourse number.
        /// </summary>
        public override void SetNextCourseNumber()
        {
            CourseNumber = "G-200000";
        }
    }

    /// <summary>
    /// AuditCourse model. Represents the AuditCourse table in the database.
    /// </summary>
    public class AuditCourse : Course
    {
        /// <summary>
        /// Sets the NextCourse number.
        /// </summary>
        public override void SetNextCourseNumber()
        {
            CourseNumber = "A-2000";
        }
    }

    /// <summary>
    /// MasteryCourse model. Represents the MasteryCourse table in the database.
    /// </summary>
    public class MasteryCourse : Course
    {
        [Required]
        [Display(Name = "Maximum\nAttempts")]
        public int MaximumAttempts { get; set; }

        /// <summary>
        /// Sets the NextCourse number.
        /// </summary>
        public override void SetNextCourseNumber()
        {
            CourseNumber = "M-20000";
        }
    }

    /// <summary>
    /// Registration model. Represents the Registration table in the database.
    /// </summary>
    public class Registration
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RegistrationId { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Display(Name = "Registration\nNumber")]
        public long RegistrationNumber { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime RegistrationDate { get; set; }

        [DisplayFormat(NullDisplayText = "Ungraded")]
        [Range(0, 1)]
        public double? Grade { get; set; }

        public string Notes { get; set; }

        /// <summary>
        /// Sets the next registration number
        /// </summary>
        public void SetNextRegistrationNumber()
        {
            RegistrationNumber = (long)StoredProcedures.NextNumber("NextRegistration");
        }

        //navigational properties

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }

    /// <summary>
    /// NextUniqueNumber model. Represents the NextUniqueNumber table in the database.
    /// </summary>
    public abstract class NextUniqueNumber
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int NextUniqueNumberId { get; set; }

        [Required]
        public long NextAvailableNumber { get; set; }

        protected static BITCollege_FTContext db = new BITCollege_FTContext();
    }

    /// <summary>
    /// NextStudent model. Represents the NextStudent table in the database.
    /// </summary>
    public class NextStudent : NextUniqueNumber
    {
        private static NextStudent nextStudent;

        /// <summary>
        /// Initializes a new instance of the NextStudent class and sets the
        /// next available number for the instance.
        /// </summary>
        private NextStudent()
        {
            NextAvailableNumber = 20000000;
        }

        /// <summary>
        /// This method will implement the Singleton pattern. it will return an instance
        /// of the NextStudent class. If there is no instance of the class, it will create a new instance
        /// and persist it to the database.
        /// </summary>
        /// <returns> An instance of NextStudent class</returns>
        public static NextStudent GetInstance()
        {
            if (nextStudent == null)
            {
                nextStudent = db.NextStudents.SingleOrDefault();

                if (nextStudent == null)
                {
                    nextStudent = new NextStudent();
                    db.NextUniqueNumbers.Add(nextStudent);
                    db.SaveChanges();
                }
            }
            return nextStudent;
        }
    }

    /// <summary>
    /// NextGradedCourse model. Represents the NextGradedCourse table in the database.
    /// </summary>
    public class NextGradedCourse : NextUniqueNumber
    {
        private static NextGradedCourse nextGradedCourse;

        /// <summary>
        /// Initializes a new instance of the NextGradedCourse class and sets the
        /// next available number for the instance.
        /// </summary>
        private NextGradedCourse()
        {
            NextAvailableNumber = 200000;
        }

        /// <summary>
        /// This method will implement the Singleton pattern. it will return an instance
        /// of the NextGradedCourse class. If there is no instance of the class, it will create a new instance
        /// and persist it to the database.
        /// </summary>
        /// <returns> An instance of NextGradedCourse class</returns>
        public static NextGradedCourse GetInstance()
        {
            if (nextGradedCourse == null)
            {
                nextGradedCourse = db.NextGradedCourses.SingleOrDefault();

                if (nextGradedCourse == null)
                {
                    nextGradedCourse = new NextGradedCourse();
                    db.NextUniqueNumbers.Add(nextGradedCourse);
                    db.SaveChanges();
                }
            }
            return nextGradedCourse;
        }
    }

    /// <summary>
    /// NextMasteryCourse model. Represents the NextMasteryCourse table in the database.
    /// </summary>
    public class NextMasteryCourse : NextUniqueNumber
    {
        private static NextMasteryCourse nextMasteryCourse;

        /// <summary>
        /// Initializes a new instance of the NextMasteryCourse class and sets the
        /// next available number for the instance.
        /// </summary>
        private NextMasteryCourse()
        {
            NextAvailableNumber = 20000;
        }

        /// <summary>
        /// This method will implement the Singleton pattern. it will return an instance
        /// of the NextMasteryCourse class. If there is no instance of the class, it will create a new instance
        /// and persist it to the database.
        /// </summary>
        /// <returns> An instance of NextMasteryCourse class</returns>
        public static NextMasteryCourse GetInstance()
        {
            if (nextMasteryCourse == null)
            {
                nextMasteryCourse = db.NextMasteryCourses.SingleOrDefault();

                if (nextMasteryCourse == null)
                {
                    nextMasteryCourse = new NextMasteryCourse();
                    db.NextUniqueNumbers.Add(nextMasteryCourse);
                    db.SaveChanges();
                }
            }
            return nextMasteryCourse;
        }
    }

    /// <summary>
    /// NextRegistration model. Represents the NextRegistration table in the database.
    /// </summary>
    public class NextRegistration : NextUniqueNumber
    {
        private static NextRegistration nextRegistration;

        /// <summary>
        /// Initializes a new instance of the NextRegistration class and sets the
        /// next available number for the instance
        /// </summary>
        private NextRegistration()
        {
            NextAvailableNumber = 700;
        }

        /// <summary>
        /// This method will implement the Singleton pattern. it will return an instance
        /// of the NextRegistration class. If there is no instance of the class, it will create a new instance
        /// and persist it to the database.
        /// </summary>
        /// <returns> An instance of NextRegistration class</returns>
        public static NextRegistration GetInstance()
        {
            if (nextRegistration == null)
            {
                nextRegistration = db.NextRegistrations.SingleOrDefault();

                if (nextRegistration == null)
                {
                    nextRegistration = new NextRegistration();
                    db.NextUniqueNumbers.Add(nextRegistration);
                    db.SaveChanges();
                }
            }
            return nextRegistration;
        }
    }

    /// <summary>
    /// NextAuditCourse model. Represents the NextAuditCourse table in the database.
    /// </summary>
    public class NextAuditCourse : NextUniqueNumber
    {
        private static NextAuditCourse nextAuditCourse;

        /// <summary>
        /// Initializes a new instance of the NextAuditCourse class and sets the
        /// next available number for the instance.
        /// </summary>
        private NextAuditCourse()
        {
            NextAvailableNumber = 2000;
        }

        /// <summary>
        /// This method will implement the Singleton pattern. it will return an instance
        /// of the NextAuditCourse class. If there is no instance of the class, it will create a new instance
        /// and persist it to the database.
        /// </summary>
        /// <returns> An instance of NextAuditCourse class</returns>
        public static NextAuditCourse GetInstance()
        {
            if (nextAuditCourse == null)
            {
                nextAuditCourse = db.NextAuditCourses.SingleOrDefault();

                if (nextAuditCourse == null)
                {
                    nextAuditCourse = new NextAuditCourse();
                    db.NextUniqueNumbers.Add(nextAuditCourse);
                    db.SaveChanges();
                }
            }
            return nextAuditCourse;
        }
    }

    /// <summary>
    /// Represents the StoredProcedures in the database.
    /// </summary>
    public static class StoredProcedures
    {
        /// <summary>
        /// Retrieves the next available number.
        /// </summary>
        /// <param name="discriminator"></param>
        /// <returns>The next available number.</returns>
        public static long? NextNumber(string discriminator)
        {
            try
            {
                long? returnValue = 0;
                SqlConnection connection = new SqlConnection("Data Source=FRANCIS-LAPTOP\\FRANCIS; " +
                "Initial Catalog=BITCollege_FTContext;Integrated Security=True");
                SqlCommand storedProcedure = new SqlCommand("next_number", connection);
                storedProcedure.CommandType = CommandType.StoredProcedure;
                storedProcedure.Parameters.AddWithValue("@Discriminator", discriminator);
                SqlParameter outputParameter = new SqlParameter("@NewVal", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                storedProcedure.Parameters.Add(outputParameter);
                connection.Open();
                storedProcedure.ExecuteNonQuery();
                connection.Close();
                returnValue = (long?)outputParameter.Value;
                return returnValue;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
