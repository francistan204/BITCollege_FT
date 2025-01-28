using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BITCollege_FT.Data;
using BITCollege_FT.Models;
using Utility;

namespace BITCOLLEGE_FT.TuitionRateAdjustmentTest
{
    public class Program
    {
        private static BITCollege_FTContext db = new BITCollege_FTContext();

        static void Main(string[] args)
        {
            Suspended_State_Newly_Registered_44();
            Suspended_State_Newly_Registered_66();
            Probation_State_Newly_Registered_115_3courses();
            Probation_State_Newly_Registered_115_7courses();
            Regular_State_Newly_Registered_250();
            Honours_State_Newly_Registered_390();
            Honours_State_Newly_Registered_427();
            Honours_State_Newly_Registered_440_7courses();
            Honours_State_Newly_Registered_410_7courses();

            Console.WriteLine("Press any key to close this window...");
            Console.ReadKey();
        }

        static void Suspended_State_Newly_Registered_44()
        {
            //set up test student
            Student student = db.Students.Find(1);
            student.GradePointAverage = 0.44;
            student.GradePointStateId = 1;
            db.SaveChanges();

            //Get an instance of students state
            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);

            //call tuition rate adjustment
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1150");
            Console.WriteLine($"Actual: {tuitionRate}\n");
        }

        static void Suspended_State_Newly_Registered_66()
        {
            //set up test student
            Student student = db.Students.Find(1);
            student.GradePointAverage = 0.66;
            student.GradePointStateId = 1;
            db.SaveChanges();

            //Get an instance of students state
            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);

            //call tuition rate adjustment
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1120");
            Console.WriteLine($"Actual: {tuitionRate}\n");
        }

        static void Suspended_State_Newly_Registered_80()
        {
            //set up test student
            Student student = db.Students.Find(1);
            student.GradePointAverage = 0.80;
            student.GradePointStateId = 1;
            db.SaveChanges();

            //Get an instance of students state
            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);

            //call tuition rate adjustment
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1100");
            Console.WriteLine($"Actual: {tuitionRate}\n");
        }

        static void Probation_State_Newly_Registered_115_3courses()
        {
            //set up test student
            Student student = db.Students.Find(1);
            student.GradePointAverage = 1.15;
            student.GradePointStateId = 3; /// Probation and Regular state id are swapped..
            db.SaveChanges();

            //Get an instance of students state
            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);

            //call tuition rate adjustment
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1075");
            Console.WriteLine($"Actual: {tuitionRate}\n");
        }

        static void Probation_State_Newly_Registered_115_7courses()
        {
            //set up test student
            Student student = db.Students.Find(2);
            student.GradePointAverage = 1.15;
            student.GradePointStateId = 3; /// Probation and Regular state id are swapped..
            db.SaveChanges();

            //Get an instance of students state
            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);

            //call tuition rate adjustment
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1035");
            Console.WriteLine($"Actual: {tuitionRate}\n");
        }

        static void Regular_State_Newly_Registered_250()
        {
            //set up test student
            Student student = db.Students.Find(2);
            student.GradePointAverage = 2.50;
            student.GradePointStateId = 2; /// Probation and Regular state id are swapped.. 2-reg 3-prob
            db.SaveChanges();

            //Get an instance of students state
            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);

            //call tuition rate adjustment
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 1000");
            Console.WriteLine($"Actual: {tuitionRate}\n");
        }

        static void Honours_State_Newly_Registered_390()
        {
            //set up test student
            Student student = db.Students.Find(1);
            student.GradePointAverage = 3.90;
            student.GradePointStateId = 4; /// Probation and Regular state id are swapped.. 2-reg 3-prob
            db.SaveChanges();

            //Get an instance of students state
            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);

            //call tuition rate adjustment
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 900");
            Console.WriteLine($"Actual: {tuitionRate}\n");
        }

        static void Honours_State_Newly_Registered_427()
        {
            //set up test student
            Student student = db.Students.Find(1);
            student.GradePointAverage = 4.27;
            student.GradePointStateId = 4; /// Probation and Regular state id are swapped.. 2-reg 3-prob
            db.SaveChanges();

            //Get an instance of students state
            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);

            //call tuition rate adjustment
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 880");
            Console.WriteLine($"Actual: {tuitionRate}\n");
        }

        static void Honours_State_Newly_Registered_440_7courses()
        {
            //set up test student
            Student student = db.Students.Find(2);
            student.GradePointAverage = 4.40;
            student.GradePointStateId = 4; /// Probation and Regular state id are swapped.. 2-reg 3-prob
            db.SaveChanges();

            //Get an instance of students state
            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);

            //call tuition rate adjustment
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 830");
            Console.WriteLine($"Actual: {tuitionRate}\n");
        }

        static void Honours_State_Newly_Registered_410_7courses()
        {
            //set up test student
            Student student = db.Students.Find(2);
            student.GradePointAverage = 4.10;
            student.GradePointStateId = 4; /// Probation and Regular state id are swapped.. 2-reg 3-prob
            db.SaveChanges();

            //Get an instance of students state
            GradePointState state = db.GradePointStates.Find(student.GradePointStateId);

            //call tuition rate adjustment
            double tuitionRate = 1000 * state.TuitionRateAdjustment(student);

            Console.WriteLine("Expected: 850");
            Console.WriteLine($"Actual: {tuitionRate}\n");
        }
    }
}
