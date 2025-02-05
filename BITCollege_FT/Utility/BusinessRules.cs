﻿/*
* Name: Francis Tan
* Program: Business Information Technology
* Course: ADEV-3008 Programming 3
* Created: January 4, 2024
* Updated: January 8, 2024
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// BusinessRules:  Provides methods that help to enforce
    /// BIT College business rules.
    /// </summary>
    public static class BusinessRules
    {
        const string UNDEFINED = "";

        /// <summary>
        /// CourseFormat:Defines the mask display format for the various course types.
        /// </summary>
        /// <param name="courseType">string course type name</param>
        /// <returns>string format</returns>
        public static string CourseFormat(string courseType)
        {
            string[] COURSE_TYPE = { "Audit", "Mastery", "Graded" };
            string[] COURSE_MASK = { ">L-00-00", ">L-00-0-00", ">L-00-00-00" };


            //initial format (empty string)
            string format = UNDEFINED;

            //compare course type to predefined types
            for (int i = 0; i < COURSE_TYPE.Length; i++)
            {
                //if a match, return the corresponding mask
                if (courseType.ToLower() == COURSE_TYPE[i].ToLower())
                {
                    format = COURSE_MASK[i];
                    break;
                }
            }
            //return the mask or empty string
            return format;

            //usage:
            //String mask = BusinessRules.CourseFormat("Mastery");
            //result:  mask = ">L-00-0-00"
        }



        /// <summary>
        /// Given:
        /// CourseTypeLookup:  Matches string description
        /// with CourseType enum
        /// </summary>
        /// <param name="courseDescription">String description of course</param>
        /// <returns>CourseType enum</returns>
        public static CourseType CourseTypeLookup(string courseDescription)
        {
            CourseType courseType = CourseType.AUDIT;

            //switch course.CourseType
            switch (courseDescription)
            {
                case "Graded":
                    courseType = CourseType.GRADED;
                    break;
                case "Mastery":
                    courseType = CourseType.MASTERY;
                    break;
                default:
                    courseType = CourseType.AUDIT;
                    break;
            }

            return courseType;

            //usage:
            //CourseType type = BusinessRules.CourseTypeLookup("Graded");
            //result:  type = CourseType.GRADED
        }


        /// <summary>
        /// GradeLookup:  Looks up letter grade based on course type and earned grade.
        /// </summary>
        /// <param name="grade">Double earned grade value.</param>
        /// <param name="courseType">CourseType course type enum value.</param>
        /// <returns></returns>
        public static double GradeLookup(double grade, CourseType courseType)
        {
            double gradePoint = GradePointValue.INCOMPLETE;

            switch (courseType)
            {
                case CourseType.GRADED:
                    {
                        if (grade >= .90)
                        {
                            gradePoint = GradePointValue.A_PLUS;
                        }
                        else if (grade >= .80)
                        {
                            gradePoint = GradePointValue.A;
                        }
                        else if (grade >= .75)
                        {
                            gradePoint = GradePointValue.B_PLUS;
                        }
                        else if (grade >= .70)
                        {
                            gradePoint = GradePointValue.B;
                        }
                        else if (grade >= .65)
                        {
                            gradePoint = GradePointValue.C_PLUS;
                        }
                        else if (grade >= .60)
                        {
                            gradePoint = GradePointValue.C;
                        }
                        else if (grade >= .50)
                        {
                            gradePoint = GradePointValue.D;
                        }
                        else
                        {
                            gradePoint = GradePointValue.F;
                        }
                        break;
                    }
                case CourseType.MASTERY:
                    {
                        gradePoint = grade >= .75 ? GradePointValue.PASS : GradePointValue.FAIL;
                        break;
                    }
                default:
                    {
                        gradePoint = GradePointValue.INCOMPLETE;
                        break;
                    }
            }

            return gradePoint;

            //usage:
            //double  gradePoint = BusinessRules.GradeLookup(0.755, CourseType.GRADED);
            //result:  gradePoint = 3.5
        }

        /// <summary>
        /// RegisterError:  To be implemented by student.  Returns an
        /// appropriate error message based on the received errorCode.
        /// </summary>
        /// <param name="errorCode">Integer code indicating error type.</param>
        /// <returns></returns>
        public static string RegisterError(int errorCode)
        {
            string errorMessage = "";
            switch(errorCode)
            {
                case -100:
                    errorMessage = "Student cannot register for a course in which there is already an ungraded registration";
                    break;
                case -200:
                    errorMessage = "Student has exceeded maximum attempts on mastery course";
                    break;
                case -300:
                    errorMessage = "an Error has occurred while updating the registration";
                    break;
                default: errorMessage = "Unknown error";
                    break;
            }
            return errorMessage;
        }

        /// <summary>
        /// Parse a string
        /// </summary>
        /// <param name="stringToBeParsed">The string that will be parsed</param>
        /// <param name="parseAtIndex">The index of the string to begin parsing</param>
        /// <returns>A parsed string</returns>
        public static string ParseString(string stringToBeParsed, string parseAtIndex)
        {
            return stringToBeParsed.Substring(0, stringToBeParsed.LastIndexOf(parseAtIndex));
        }
    }
}
