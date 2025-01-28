using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Drawing.Drawing2D;
using System.EnterpriseServices;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Services.Protocols;
using BITCollege_FT.Controllers;
using BITCollege_FT.Data;
using BITCollege_FT.Models;
using Utility;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CollegeRegistration" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CollegeRegistration.svc or CollegeRegistration.svc.cs at the Solution Explorer and start debugging.
    public class CollegeRegistration : ICollegeRegistration
    {
        BITCollege_FTContext db = new BITCollege_FTContext();

        public void DoWork()
        {
        }

        /// <summary>
        /// Method will remove course record from database and return true or false depending on if task was done.
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns> true or false </returns>
        public bool DropCourse(int registrationId)
        {
            //retrieve registration record corresponding to the argument passed
            IEnumerable<Registration> registration = db.Registrations.Where(r=>r.RegistrationId == registrationId);

            try
            {
                    //remove record corresponding to registration value from db
                    db.Registrations.Remove(registration.First());
                    db.SaveChanges();

                    return true;
            }
            //return false if exception occurs
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Registers a course for a student with the given student ID and course ID, and stores any additional notes. 
        /// It also handles logic for checking prerequisites, maximum attempts for mastery courses, and calculates tuition fees.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="courseId"></param>
        /// <param name="notes"></param>
        /// <returns>an int value</returns>
        public int RegisterCourse(int studentId, int courseId, string notes)
        {
            int returnCode = 0;


            //retrieve all records from registration table corresponding to param values
            IQueryable<Registration> allRegistrations = db.Registrations.Where(r => r.StudentId == studentId && r.CourseId == courseId);
            //retrieve course record corresponding to param value
            Course course = db.Courses.Where(r => r.CourseId == courseId).SingleOrDefault();
            //retrieve student record corresponding to param value
            Student student = db.Students.Where(r => r.StudentId == studentId).SingleOrDefault();
            //retrieve registration records that have a null value for grade
            IEnumerable<Registration> incompleteRegistrations = allRegistrations.Where(r => r.Grade == null);

            if (incompleteRegistrations.Count() > 0)
            {
                returnCode = -100;
            }

            //Retrieve course name
            string courseName = course.CourseType;
            //Get enum value for course
            CourseType courseEnum = BusinessRules.CourseTypeLookup(courseName);
            //Check to see if course is equal to mastery
            if (courseEnum == CourseType.MASTERY)
            {
                //Get maximum attempts for the course
                int maximumAttempt = db.MasteryCourses.Where(r => r.CourseId == courseId).Select(x => x.MaximumAttempts).SingleOrDefault();
                //Get completed courses where there are grades
                IEnumerable<Registration> completedRegistrations = allRegistrations.Where(r => r.Grade != null);

                if (completedRegistrations.Count() >= maximumAttempt)
                {
                    returnCode = -200;
                }
            }

            if (returnCode == 0)
            {
                try
                {
                    Registration registration = new Registration();
                    registration.StudentId = studentId;
                    registration.CourseId = courseId;
                    registration.Notes = notes;
                    registration.RegistrationDate = DateTime.Now;
                    registration.RegistrationNumber = (long)StoredProcedures.NextNumber("NextRegistration");
                    //registration.SetNextRegistrationNumber();
                    //Save changes to database
                    db.Registrations.Add(registration);
                    db.SaveChanges();

                    GradePointState gradePointState = db.GradePointStates.Where(r => r.GradePointStateId == student.GradePointStateId).SingleOrDefault();
                    //Calculate tuition fee for student
                    double adjustedTuition = course.TuitionAmount * gradePointState.TuitionRateAdjustment(student);
                    //Get any outstanding fees
                    double outstandingFees = student.OutstandingFees;
                    //Calculate full tuition 
                    double fullTuition = adjustedTuition + outstandingFees;
                    //Update outstanding fee for student
                    student.OutstandingFees = fullTuition;

                    db.SaveChanges();
                }

                catch (Exception)
                {
                    returnCode = -300;
                }
            }
            return returnCode;
        }
        

        /// <summary>
        /// Updates the grade and notes for a registration, recalculates the grade point average for the student
        /// and returns the new grade point average.
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="registrationId"></param>
        /// <param name="notes"></param>
        /// <returns>new grade point average</returns>
        public double? UpdateGrade(double grade, int registrationId, string notes)
        {
            
            Registration registration = db.Registrations.Where(r => r.RegistrationId == registrationId).SingleOrDefault();
            registration.Grade = grade;
            registration.Notes = notes;
            db.SaveChanges();

            double? gradePointAverage = CalculateGradePointAverage(registration.StudentId);

            return gradePointAverage;
        }
        /// <summary>
        /// Calculates the grade point average for a student based on their completed courses 
        /// and updates the student's record in the database.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        private double? CalculateGradePointAverage(int studentId) 
        {
            double totalGradePointValue = 0;
            double totalCreditHours = 0;
            double newGradePointAverage = 0;
            Student student = db.Students.Where(r => r.StudentId == studentId).SingleOrDefault();

            IQueryable <Registration> registrations = db.Registrations.Where(r => r.StudentId == studentId).Where(r=>r.Grade != null);

            foreach(Registration record in registrations.ToList() ) 
            {
                double grade = (double)record.Grade;
                int courseId = record.CourseId;

                //Get course matching the course id
                IEnumerable<Course> course = db.Courses.Where(r=>r.CourseId == courseId);

                //Get course type
                string courseType = course.Select(r => r.CourseType).SingleOrDefault();

                //Get course enum
                CourseType courseEnum = BusinessRules.CourseTypeLookup(courseType);

                if(courseEnum != CourseType.AUDIT) 
                {

                    //Get GPA of course
                    double gradePoint = BusinessRules.GradeLookup(grade, courseEnum);
                    //Get total credit hours
                    double creditHours = record.Course.CreditHours;
                    //calculate GPA based on credit hours
                    double gradePointAverage = gradePoint * creditHours;
                    //Accumulate GPA 
                    totalGradePointValue += gradePointAverage;
                    //Accumulate Credit hours
                    totalCreditHours += creditHours;
                }
            }

            if (totalCreditHours == 0)
            {
                student.GradePointAverage = null;
                db.SaveChanges();
            }
            else
            {
               newGradePointAverage = totalGradePointValue / totalCreditHours;
                db.SaveChanges();
            }

            //Obtain student record
            student = db.Students.Where(r => r.StudentId == studentId).SingleOrDefault(); 
            //Set new GPA
            student.GradePointAverage = newGradePointAverage;
            //Make sure student is in the appropriate Grade Point State
            student.ChangeState();
            db.SaveChanges();

            return newGradePointAverage;
        }
    }
}
