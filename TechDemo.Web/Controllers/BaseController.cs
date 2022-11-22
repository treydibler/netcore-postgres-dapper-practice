using TechDemo.Core;
using Microsoft.AspNetCore.Mvc;

namespace TechDemo.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly ILogger<BaseController> _log;
        protected BaseController(ILogger<BaseController> log)
        {
            _log = log;
        }
    }   
}