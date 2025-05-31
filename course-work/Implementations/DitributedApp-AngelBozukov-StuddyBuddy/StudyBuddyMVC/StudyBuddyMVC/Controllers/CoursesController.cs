using Microsoft.AspNetCore.Mvc;
using StudyBuddyMVC.Action_Filters;
using StudyBuddyMVC.DTOs;
using StudyBuddyMVC.Extensions;
using StudyBuddyMVC.Services;
using StudyBuddyMVC.ViewModels;

namespace StudyBuddyMVC.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseApiService _courseApiService;
        private readonly ILogger<CoursesController> _logger;
        private readonly IAssignmentApiService _assignmentApiService;
        private readonly IUserApiService _userApiService;

        public CoursesController(ICourseApiService courseApiService, ILogger<CoursesController> logger, IAssignmentApiService assignmentApiService, IUserApiService userApiService)
        {
            _courseApiService = courseApiService;
            _logger = logger;
            _assignmentApiService = assignmentApiService;
            _userApiService = userApiService;
        }

        
        public async Task<IActionResult> Index()
        {
            try
            {
                
                var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
                if (loggedUser == null)
                {
                    TempData["ErrorMessage"] = "Please log in to view courses.";
                    return RedirectToAction("Login", "Account");
                }

                List<CourseDto> courses;
                ViewBag.UserRole = loggedUser.Role;
                ViewBag.UserId = loggedUser.Id;                

                
                if (loggedUser.Role?.ToLower() == "professor" || loggedUser.Role?.ToLower() == "instructor")
                {
                    courses = await _courseApiService.GetCoursesByInstructorIdAsync(loggedUser.Id);
                    ViewBag.PageTitle = "My Courses (Instructor)";
                    ViewBag.CanCreateCourse = true;
                }
                else if (loggedUser.Role?.ToLower() == "student")
                {
                    courses = await _courseApiService.GetCoursesByStudentIdAsync(loggedUser.Id);
                    ViewBag.PageTitle = "My Enrolled Courses";
                    ViewBag.CanCreateCourse = false;
                }
                else
                {
                    
                    courses = new List<CourseDto>();
                    ViewBag.PageTitle = "Courses";
                    ViewBag.CanCreateCourse = false;
                    TempData["InfoMessage"] = "Unable to determine user role. Please contact administrator.";
                }

                return View(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading courses index");
                TempData["ErrorMessage"] = "An error occurred while loading courses.";
                return View(new List<CourseDto>());
            }
        }

        // GET: /Courses/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Check if user is logged in
                var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
                if (loggedUser == null)
                {
                    TempData["ErrorMessage"] = "Please log in to view course details.";
                    return RedirectToAction("Login", "Account");
                }

                var course = await _courseApiService.GetCourseByIdAsync(id);
                if (course == null)
                {
                    TempData["ErrorMessage"] = "Course not found.";
                    return RedirectToAction("Index");
                }

                var assignments = await _assignmentApiService.GetAssignmentsByCourseIdAsync(course.Id);

                // Pass user info to view for role-based functionality
                ViewBag.UserRole = loggedUser.Role;
                ViewBag.UserId = loggedUser.Id;
                ViewBag.CanEdit = (loggedUser.Role?.ToLower() == "professor" || loggedUser.Role?.ToLower() == "instructor")
                                  && course.InstructorId == loggedUser.Id;

                var viewModel = new CourseDetailsViewModel
                {
                    Course = course,
                    Assignments = assignments
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading course details for ID: {CourseId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading course details.";
                return RedirectToAction("Index");
            }
        }

        [InstructorActionFilter]
        public IActionResult Create()
        {
            // Check if user is logged in and is a professor/instructor
            var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
            if (loggedUser == null)
            {
                TempData["ErrorMessage"] = "Please log in to create courses.";
                return RedirectToAction("Login", "Account");
            }

            if (loggedUser.Role?.ToLower() != "professor" && loggedUser.Role?.ToLower() != "instructor")
            {
                TempData["ErrorMessage"] = "Only instructors can create courses.";
                return RedirectToAction("Index");
            }

            // Pre-populate instructor ID
            var courseDto = new CourseDto
            {
                InstructorId = loggedUser.Id
            };

            return View(courseDto);
        }

        [InstructorActionFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseDto courseDto)
        {
            try
            {
                // Check if user is logged in and is a professor/instructor
                var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
                if (loggedUser == null)
                {
                    TempData["ErrorMessage"] = "Please log in to create courses.";
                    return RedirectToAction("Login", "Account");
                }

                if (loggedUser.Role?.ToLower() != "professor" && loggedUser.Role?.ToLower() != "instructor")
                {
                    TempData["ErrorMessage"] = "Only instructors can create courses.";
                    return RedirectToAction("Index");
                }

                // Ensure the instructor ID matches the logged user
                courseDto.InstructorId = loggedUser.Id;

                if (!ModelState.IsValid)
                {
                    return View(courseDto);
                }

                var (isSuccess, errorMessage) = await _courseApiService.CreateCourseAsync(courseDto);

                if (isSuccess)
                {
                    TempData["SuccessMessage"] = "Course created successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", errorMessage);
                    return View(courseDto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                ModelState.AddModelError("", "An error occurred while creating the course.");
                return View(courseDto);
            }
        }

        [InstructorActionFilter]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Check if user is logged in and is a professor/instructor
                var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
                if (loggedUser == null)
                {
                    TempData["ErrorMessage"] = "Please log in to edit courses.";
                    return RedirectToAction("Login", "Account");
                }

                if (loggedUser.Role?.ToLower() != "professor" && loggedUser.Role?.ToLower() != "instructor")
                {
                    TempData["ErrorMessage"] = "Only instructors can edit courses.";
                    return RedirectToAction("Index");
                }

                var course = await _courseApiService.GetCourseByIdAsync(id);
                if (course == null)
                {
                    TempData["ErrorMessage"] = "Course not found.";
                    return RedirectToAction("Index");
                }

                // Check if the logged user is the instructor of this course
                if (course.InstructorId != loggedUser.Id)
                {
                    TempData["ErrorMessage"] = "You can only edit your own courses.";
                    return RedirectToAction("Index");
                }

                return View(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading course for edit, ID: {CourseId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the course.";
                return RedirectToAction("Index");
            }
        }

        [InstructorActionFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseDto courseDto)
        {
            try
            {
                // Check if user is logged in and is a professor/instructor
                var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
                if (loggedUser == null)
                {
                    TempData["ErrorMessage"] = "Please log in to edit courses.";
                    return RedirectToAction("Login", "Account");
                }

                if (loggedUser.Role?.ToLower() != "professor" && loggedUser.Role?.ToLower() != "instructor")
                {
                    TempData["ErrorMessage"] = "Only instructors can edit courses.";
                    return RedirectToAction("Index");
                }

                // Ensure the instructor ID matches the logged user
                courseDto.InstructorId = loggedUser.Id;

                if (!ModelState.IsValid)
                {
                    return View(courseDto);
                }

                var (isSuccess, errorMessage) = await _courseApiService.UpdateCourseAsync(id, courseDto);

                if (isSuccess)
                {
                    TempData["SuccessMessage"] = "Course updated successfully!";
                    return RedirectToAction("Details", new { id = id });
                }
                else
                {
                    ModelState.AddModelError("", errorMessage);
                    return View(courseDto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course, ID: {CourseId}", id);
                ModelState.AddModelError("", "An error occurred while updating the course.");
                return View(courseDto);
            }
        }

        [InstructorActionFilter]
        public async Task<IActionResult> EnrollStudents(int courseId)
        {
            var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
            if (loggedUser == null || (loggedUser.Role?.ToLower() != "professor" && loggedUser.Role?.ToLower() != "instructor"))
            {
                TempData["ErrorMessage"] = "Only instructors can enroll students.";
                return RedirectToAction("Index");
            }

            var course = await _courseApiService.GetCourseByIdAsync(courseId);
            if (course == null || course.InstructorId != loggedUser.Id)
            {
                TempData["ErrorMessage"] = "Course not found or access denied.";
                return RedirectToAction("Index");
            }

            var allStudents = await _userApiService.GetUsersByRoleAsync("student");
            var enrolledStudents = await _courseApiService.GetEnrolledStudentsAsync(courseId);

            // Filter out already enrolled students
            var availableStudents = allStudents
                .Where(s => !enrolledStudents.Any(e => e.Id == s.Id))
                .ToList();

            var model = new EnrollStudentsViewModel
            {
                CourseId = courseId,
                
                AvailableStudents = availableStudents
            };

            return View(model);
        }

        [InstructorActionFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollStudents(int courseId, List<string> selectedStudentIds)
        {
            if (selectedStudentIds == null || !selectedStudentIds.Any())
            {
                TempData["ErrorMessage"] = "Please select at least one student.";
                return RedirectToAction("EnrollStudents", new { courseId });
            }

            foreach (var studentId in selectedStudentIds)
            {
                var (isSuccess, error) = await _courseApiService.EnrollStudentAsync(courseId, studentId);
                if (!isSuccess)
                {
                    TempData["ErrorMessage"] = $"Failed to enroll student {studentId}: {error}";
                    return RedirectToAction("EnrollStudents", new { courseId });
                }
            }

            TempData["SuccessMessage"] = "Students enrolled successfully.";
            return RedirectToAction("Details", new { id = courseId });
        }



    }
}