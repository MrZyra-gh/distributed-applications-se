using Microsoft.AspNetCore.Mvc;
using StudyBuddyMVC.Action_Filters;
using StudyBuddyMVC.DTOs;
using StudyBuddyMVC.Extensions;
using StudyBuddyMVC.Services;

namespace StudyBuddyMVC.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly IAssignmentApiService _assignmentApiService;
        private readonly ILogger<AssignmentsController> _logger;
        private readonly ICourseApiService _courseApiService;

        public AssignmentsController(IAssignmentApiService assignmentsApiService, ILogger<AssignmentsController> logger, ICourseApiService courseApiService)
        {
            _assignmentApiService = assignmentsApiService;
            _logger = logger;
            _courseApiService = courseApiService;
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

                List<AssignmentDto> assignments;
                ViewBag.UserRole = loggedUser.Role;
                ViewBag.UserId = loggedUser.Id;


                if (loggedUser.Role?.ToLower() == "professor" || loggedUser.Role?.ToLower() == "instructor")
                {
                    assignments = await _assignmentApiService.GetAssignmentsByUserIdAsync(loggedUser.Id);
                    ViewBag.PageTitle = "My assignments (Instructor)";
                    ViewBag.CanCreateAssignments = true;
                }
                else if (loggedUser.Role?.ToLower() == "student")
                {
                    assignments = await _assignmentApiService.GetAssignmentsByUserIdAsync(loggedUser.Id);
                    ViewBag.PageTitle = "My assignments";
                    ViewBag.CanCreateAssignments = false;
                }
                else
                {

                    assignments = new List<AssignmentDto>();
                    ViewBag.PageTitle = "Assignments";
                    ViewBag.CanCreateCourse = false;
                    TempData["InfoMessage"] = "Unable to determine user role. Please contact administrator.";
                }

                return View(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading assignments index");
                TempData["ErrorMessage"] = "An error occurred while loading assignments.";
                return View(new List<AssignmentDto>());
            }
        }
    
   
            public async Task<IActionResult> Details(int id)
            {
                try
                {
                    var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
                    if (loggedUser == null)
                    {
                        TempData["ErrorMessage"] = "Please log in to view assignment details.";
                        return RedirectToAction("Login", "Account");
                    }

                    var assignment = await _assignmentApiService.GetAssignmentByIdAsync(id);
                    if (assignment == null)
                    {
                        TempData["ErrorMessage"] = "Assignment not found.";
                        return RedirectToAction("Index", "Courses");
                    }

                    ViewBag.UserRole = loggedUser.Role;
                    ViewBag.UserId = loggedUser.Id;

                    return View(assignment);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading assignment details for ID: {AssignmentId}", id);
                    TempData["ErrorMessage"] = "An error occurred while loading assignment details.";
                    return RedirectToAction("Index", "Courses");
                }
            }

            [InstructorActionFilter]
            public async Task<IActionResult> Create(int courseId)
            {
                var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
                if (loggedUser == null || (loggedUser.Role?.ToLower() != "professor" && loggedUser.Role?.ToLower() != "instructor"))
                {
                    TempData["ErrorMessage"] = "Only instructors can create assignments.";
                    return RedirectToAction("Login", "Account");
                }

                var course = await _courseApiService.GetCourseByIdAsync(courseId);
                if (course == null || course.InstructorId != loggedUser.Id)
                {
                    TempData["ErrorMessage"] = "Access denied or course not found.";
                    return RedirectToAction("Index", "Courses");
                }

                var assignment = new AssignmentDto
                {
                    CourseId = courseId,
                    CourseTitle = course.Title,
                    DueDate = DateTime.Now.AddDays(7)
                };

                return View(assignment);
            }

            [InstructorActionFilter]    
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(AssignmentDto dto)
            {
                var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
                if (loggedUser == null || (loggedUser.Role?.ToLower() != "professor" && loggedUser.Role?.ToLower() != "instructor"))
                {
                    TempData["ErrorMessage"] = "Only instructors can create assignments.";
                    return RedirectToAction("Login", "Account");
                }

                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                var course = await _courseApiService.GetCourseByIdAsync(dto.CourseId);
                if (course == null || course.InstructorId != loggedUser.Id)
                {
                    TempData["ErrorMessage"] = "Access denied or course not found.";
                    return RedirectToAction("Index", "Courses");
                }

                var (isSuccess, errorMessage) = await _assignmentApiService.CreateAssignmentAsync(dto);
                if (isSuccess)
                {
                    TempData["SuccessMessage"] = "Assignment created successfully!";
                    return RedirectToAction("Details", "Courses", new { id = dto.CourseId });
                }

                ModelState.AddModelError("", errorMessage);
                return View(dto);
            }

            [InstructorActionFilter]    
            public async Task<IActionResult> Edit(int id)
            {
                var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
                if (loggedUser == null || (loggedUser.Role?.ToLower() != "professor" && loggedUser.Role?.ToLower() != "instructor"))
                {
                    TempData["ErrorMessage"] = "Only instructors can edit assignments.";
                    return RedirectToAction("Login", "Account");
                }

                var assignment = await _assignmentApiService.GetAssignmentByIdAsync(id);
                if (assignment == null)
                {
                    TempData["ErrorMessage"] = "Assignment not found.";
                    return RedirectToAction("Index", "Courses");
                }

                var course = await _courseApiService.GetCourseByIdAsync(assignment.CourseId);
                if (course == null || course.InstructorId != loggedUser.Id)
                {
                    TempData["ErrorMessage"] = "Access denied.";
                    return RedirectToAction("Index", "Courses");
                }

                return View(assignment);
            }

            [InstructorActionFilter]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, AssignmentDto dto)
            {
                var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
                if (loggedUser == null || (loggedUser.Role?.ToLower() != "professor" && loggedUser.Role?.ToLower() != "instructor"))
                {
                    TempData["ErrorMessage"] = "Only instructors can edit assignments.";
                    return RedirectToAction("Login", "Account");
                }

                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                var course = await _courseApiService.GetCourseByIdAsync(dto.CourseId);
                if (course == null || course.InstructorId != loggedUser.Id)
                {
                    TempData["ErrorMessage"] = "Access denied.";
                    return RedirectToAction("Index", "Courses");
                }

                var (isSuccess, errorMessage) = await _assignmentApiService.UpdateAssignmentAsync(id, dto);
                if (isSuccess)
                {
                    TempData["SuccessMessage"] = "Assignment updated successfully!";
                    return RedirectToAction("Details", new { id = dto.Id });
                }

                ModelState.AddModelError("", errorMessage);
                return View(dto);
            }

            [InstructorActionFilter]    
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(int id)
            {
                var loggedUser = HttpContext.Session.GetObject<UserSessionDto>("loggedUser");
                if (loggedUser == null || (loggedUser.Role?.ToLower() != "professor" && loggedUser.Role?.ToLower() != "instructor"))
                {
                    TempData["ErrorMessage"] = "Only instructors can delete assignments.";
                    return RedirectToAction("Login", "Account");
                }

                var assignment = await _assignmentApiService.GetAssignmentByIdAsync(id);
                if (assignment == null)
                {
                    TempData["ErrorMessage"] = "Assignment not found.";
                    return RedirectToAction("Index", "Courses");
                }

                var course = await _courseApiService.GetCourseByIdAsync(assignment.CourseId);
                if (course == null || course.InstructorId != loggedUser.Id)
                {
                    TempData["ErrorMessage"] = "Access denied.";
                    return RedirectToAction("Index", "Courses");
                }

                var (isSuccess, errorMessage) = await _assignmentApiService.DeleteAssignmentAsync(id);
                if (isSuccess)
                {
                    TempData["SuccessMessage"] = "Assignment deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = errorMessage;
                }

                return RedirectToAction("Details", "Courses", new { id = assignment.CourseId });
            }
        
    }

}
