using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StudyBuddyMVC.DTOs;
using StudyBuddyMVC.Extensions;

namespace StudyBuddyMVC.Action_Filters
{
    public class InstructorActionFilter:ActionFilterAttribute
    {
        
            public override void OnActionExecuting(ActionExecutingContext context)
            {
                if (context.HttpContext.Session.GetObject<UserDto>("loggedUser").Role != "instructor" ||
                    context.HttpContext.Session.GetObject<UserDto>("loggedUser").Role != "proffessor")
                    context.Result = new RedirectResult("/User/Index");
            }
        
    }
}
