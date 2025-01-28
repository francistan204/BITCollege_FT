using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BITCollegeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICollegeRegistration" in both code and config file together.
    [ServiceContract]
    public interface ICollegeRegistration
    {
        [OperationContract]
        void DoWork();

        /// <summary>
        /// Drops a course for a given registration ID.
        /// </summary>
        /// <param name="registrationId">The ID of the registration to drop the course from.</param>
        /// <returns>True if the course was dropped successfully, false otherwise.</returns>
        [OperationContract]
        bool DropCourse(int registrationId);

        /// <summary>
        /// Registers a student for a course.
        /// </summary>
        /// <param name="studentId">The ID of the student to register.</param>
        /// <param name="courseId">The ID of the course to register for.</param>
        /// <param name="notes">Any notes related to the registration.</param>
        /// <returns>An int value corresponding to if a course was registered.</returns>
        [OperationContract]
        int RegisterCourse(int studentId, int courseId, string notes);

        /// <summary>
        /// Updates the grade for a registration.
        /// </summary>
        /// <param name="grade">The new grade.</param>
        /// <param name="registrationId">The ID of the registration to update.</param>
        /// <param name="notes">Any notes related to the grade update.</param>
        /// <returns>The updated grade if successful, null otherwise.</returns>
        [OperationContract]
        double? UpdateGrade(double grade, int registrationId, string notes);
    }
}
