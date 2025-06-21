using DocumentFormat.OpenXml.InkML;
using InterViewV2.Models.DAL;
using InterViewV2.Models;
using InterViewV2.Services.Extension;
using InterViewV2.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterViewV2.Controllers
{
    public class VacancyController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DB_Context _context;
        private readonly IFileService fileService;
        private readonly IDocumentService ds;
        private readonly Repo repo;
       // public VacancyController(UserManager<User> userManager,
       //DB_Context context,
       // IFileService fileService,
       // Repo repo,
       // IConfiguration configuration,
       // IDocumentService documentService)
       // {
       //     _userManager = userManager;
       //     _context = context;
       //     this.fileService = fileService;
       //     this.repo = repo;
       //     ds = documentService;
       // }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult createVacancy()
        {
            return View();
        }
    }
}
