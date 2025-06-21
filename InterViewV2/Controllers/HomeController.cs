using Microsoft.AspNetCore.Mvc;
using InterViewV2.Models;
using InterViewV2.Models.DAL;
using InterViewV2.Services.Extension;
using InterViewV2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ClosedXML;

namespace InterViewV2.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly DB_Context _context;
    private readonly IFileService fileService;
    private readonly IDocumentService ds;
    private readonly IEmailService _emailService;
    private readonly Repo repo;
    private static string? _result;
    private static string? _hire;
    private string filesLocation { get; set; }

    public HomeController(
        UserManager<User> userManager,
        DB_Context context,
        IFileService fileService,
        Repo repo,
        IConfiguration configuration,
        IDocumentService documentService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _context = context;
        this.fileService = fileService;
        this.repo = repo;
        ds = documentService;
        _emailService = emailService;
        filesLocation = configuration["FilesLocation"];
    }
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user.Id;


        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        var isCoordinator = await _userManager.IsInRoleAsync(user, "HR Coordinator");
        var isReviewer = await _userManager.IsInRoleAsync(user, "Reviewer");
        var isHrManager = await _userManager.IsInRoleAsync(user, "HR Manager");

        var candidates = (isAdmin || isCoordinator || isReviewer || isHrManager)
            ? await _context.Condidate
            .Include(x => x.Interview).ThenInclude(x => x.User1)
            .Include(x => x.Interview).ThenInclude(x => x.User2)
            .Include(x => x.Interview).ThenInclude(x => x.User3)
            .Include(x => x.Interview).ThenInclude(x => x.Positions).ThenInclude(x => x.Position)
            .ToListAsync()
            : await _context.Condidate
                .Where(c => _context.Interview.Any(i =>
                    i.CondidateId == c.Id && (i.User1Id == userId ||
                                              i.User2Id == userId ||
                                              i.User3Id == userId ||
                                              i.ExamUserId == userId)))
                .Include(x => x.Interview).ThenInclude(x => x.User1)
                .Include(x => x.Interview).ThenInclude(x => x.User2)
                .Include(x => x.Interview).ThenInclude(x => x.User3)
                .Include(x => x.Interview).ThenInclude(x => x.Positions).ThenInclude(x => x.Position)
                .ToListAsync();

        return View(candidates);
    }
    [Authorize(Roles = "HR Coordinator")]
    public IActionResult NewCandidate()
    {
        return View();
    }
    [Authorize(Roles = "HR Coordinator")]
    [HttpPost]
    public IActionResult NewCandidate(Condidate con)
    {
        _context.Condidate.Add(con);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    [Authorize(Roles = "HR Coordinator")]
    public IActionResult EditCandidate(Guid id)
    {
        return View(_context.Condidate.SingleOrDefault(x => x.Id == id));
    }
    [Authorize(Roles = "HR Coordinator")]
    [HttpPost]
    public async Task<IActionResult> EditCandidate(Condidate con, Guid id)
    {
        var oldcon = _context.Condidate.SingleOrDefault(x => x.Id == id);

        oldcon.Name = con.Name;
        oldcon.Surname = con.Surname;
        oldcon.CommonComment = con.CommonComment;
        oldcon.Phone = con.Phone;
        oldcon.PhoneCode = con.PhoneCode;
        oldcon.PhoneCountryCode = con.PhoneCountryCode;

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    [Authorize(Roles = "HR Coordinator")]
    public IActionResult DeleteCandidate(Guid id)
    {
        _context.Condidate.Remove(_context.Condidate.SingleOrDefault(x => x.Id == id));
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Interview(Guid id, string resultComment, string hireComment)
    {

        var query = await _context.Interview.Where(x => x.CondidateId == id).Include(x => x.User1).Include(x => x.User2).Include(x => x.User3)
            .Include(x => x.Positions).ThenInclude(x => x.Position).Include(x => x.Condidate).Include(x => x.Files).Include(x => x.DepartmentIntw).ToListAsync();

        //if (string.IsNullOrEmpty(hireComment))
        //{
        //    hireComment = "false";
        //}

        //var pos = await _context.Interview.FirstOrDefaultAsync(x => x.CondidateId == id);
        //pos.PositionIds = pos.Positions.Split(',').ToList();

        if (!string.IsNullOrEmpty(resultComment))
        {
            query = query.Where(x => x.ResultComment == resultComment).ToList();
        }

        if (!string.IsNullOrEmpty(hireComment))
        {
            query = query.Where(x => x.HireComment == hireComment).ToList();
        }

        ViewBag.Interviews = query;
        return View(_context.Condidate.SingleOrDefault(x => x.Id == id));
    }
    public async Task<IActionResult> ClearFilters(Guid id)
    {
        return RedirectToAction("Interview", new { id });
    }
    [Authorize(Roles = "HR Coordinator")]
    public async Task<IActionResult> GetUsersByDepartment(Guid departmentId)
    {
        var users = await _context.Users
            .Where(u => u.DepartmentId == departmentId)
            .Select(u => new
            {
                fullName = u.Name + " " + u.Surname,
                value = u.Id.ToString()
            })
            .ToListAsync();

        return Json(users);
    }
    [Authorize(Roles = "HR Coordinator")]
    public async Task<IActionResult> NewInterview(Guid cId)
    {
        try
        {
            var usersInRoleDepHead = (await _userManager.GetUsersInRoleAsync("Department Head")).ToList();
            var usersInRoleInt1 = (await _userManager.GetUsersInRoleAsync("Interviewer 1")).ToList();
            var usersInRoleInt2 = (await _userManager.GetUsersInRoleAsync("Interviewer 2")).ToList();

            var usersWithDepartments = await repo.DB.Users.Include(u => u.Department).ToListAsync();

            var depHeadWithDepartment = usersWithDepartments.Where(u => usersInRoleDepHead.Any(roleUser => roleUser.Id == u.Id)).ToList();
            var interviewers1 = usersWithDepartments.Where(u => usersInRoleInt1.Any(roleUser => roleUser.Id == u.Id)).ToList();
            var interviewers2 = usersWithDepartments.Where(u => usersInRoleInt2.Any(roleUser => roleUser.Id == u.Id)).ToList();

            var departmentUsers = depHeadWithDepartment.Select(u => new
            {
                FullName = u.Name + " " + u.Surname + " - " + u.Department.DepartmentEnName,
                Value = u.Id
            }).ToList();

            var interviewerusers1 = interviewers1.Select(u => new
            {
                FullName = u.Name + " " + u.Surname + " - " + u.Department.DepartmentEnName,
                Value = u.Id
            }).ToList();

            var interviewerusers2 = interviewers2.Select(u => new
            {
                FullName = u.Name + " " + u.Surname + " - " + u.Department.DepartmentEnName,
                Value = u.Id
            }).ToList();

            ViewBag.DepartmentId = await _context.SelectList<Department>("Id", "DepartmentEnName");
            ViewBag.InterviewerDep = new SelectList(departmentUsers, "Value", "FullName");
            ViewBag.Interviewer1 = new SelectList(interviewerusers1, "Value", "FullName");
            ViewBag.Interviewer2 = new SelectList(interviewerusers2, "Value", "FullName");
            ViewBag.DepartmentId = new SelectList(_context.Department, "Id", "DepartmentEnName");
            ViewBag.PositionId = new SelectList(_context.Position, "Id", "PositionEnName");
            ViewBag.CandidateId = cId;

            return View();
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("NewInterview");
        }
    }
    [Authorize(Roles = "HR Coordinator")]
    [HttpPost]
    public async Task<IActionResult> NewInterview(Interview model)
    {
        if (model != null)
        {
            var user = await _userManager.GetUserAsync(User);
            model.CreatedBy = user.Name + " " + user.Surname;
            model.CreatedDate = DateTime.UtcNow.AddHours(4).Date;
            model.ResultComment = "Waiting";

            _context.Interview.Add(model);
            await _context.SaveChangesAsync();

            foreach (var pId in model.SelectedPositions)
            {
                var intPos = new InterviewPosition()
                {
                    InterviewId = model.Id,
                    PositionId = pId
                };

                _context.InterviewPosition.Add(intPos);
            }

            await _context.SaveChangesAsync();

            var interviewerEmails = await _context.Users
                .Where(u => u.Id == model.User1Id || u.Id == model.User2Id || u.Id == model.User3Id)
                .Select(u => u.Email)
                .ToListAsync();

            var examUserEmail = await _context.Users
                .Where(u => u.Id == model.ExamUserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            var candidate = await _context.Condidate.Where(x => x.Id == model.CondidateId).FirstOrDefaultAsync();

            var subject = "You are invited to an interview";
            var interviewerMessage = $"You are assigned to an interview for candidate {candidate.Name} {candidate.Surname}.";
            var examUserMessage = $"You need to enter the exam results for candidate {candidate.Name} {candidate.Surname} into the system.";

            if (interviewerEmails.Any())
            {
                await _emailService.SendEmailAsync(interviewerEmails, subject, interviewerMessage);
            }

            if (!string.IsNullOrEmpty(examUserEmail))
            {
                await _emailService.SendEmailAsync(new List<string> { examUserEmail }, subject, examUserMessage);
            }

            return RedirectToAction("Interview", new { id = model.CondidateId });
        }
        else
        {
            ModelState.AddModelError("", "Interview already exists");
            return View(model);
        }
    }
    [Authorize(Roles = "HR Coordinator")]
    public async Task<IActionResult> EditInterview(Guid id, Guid candidateId)
    {
        var interview = await _context.Interview
            .Include(x => x.User1)
            .Include(x => x.User2)
            .Include(x => x.User3)
            .Include(x => x.Positions)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (interview == null)
        {
            return NotFound();
        }

        var examUser = await _context.Users.SingleOrDefaultAsync(x => x.Id == interview.ExamUserId);

        var usersInRoleInt1 = (await _userManager.GetUsersInRoleAsync("Interviewer 1")).ToList();
        var usersInRoleInt2 = (await _userManager.GetUsersInRoleAsync("Interviewer 2")).ToList();
        var usersInRoleDepHead = await _userManager.GetUsersInRoleAsync("Department Head");
        var usersWithDepartments = await repo.DB.Users.Include(u => u.Department).ToListAsync();

        var interviewers1WithDepartment = usersWithDepartments.Where(u => usersInRoleInt1.Any(roleUser => roleUser.Id == u.Id)).ToList();
        var interviewers2WithDepartment = usersWithDepartments.Where(u => usersInRoleInt2.Any(roleUser => roleUser.Id == u.Id)).ToList();
        var depHeadWithDepartment = usersWithDepartments.Where(u => usersInRoleDepHead.Any(roleUser => roleUser.Id == u.Id)).ToList();


        var userList1 = interviewers1WithDepartment.Select(u => new
        {
            FullName = u.Name + " " + u.Surname + " - " + u.Department.DepartmentEnName,
            Value = u.Id
        }).ToList();

        var userList2 = interviewers2WithDepartment.Select(u => new
        {
            FullName = u.Name + " " + u.Surname + " - " + u.Department.DepartmentEnName,
            Value = u.Id
        }).ToList();

        var userDepList = depHeadWithDepartment.Select(u => new
        {
            FullName = u.Name + " " + u.Surname + " - " + u.Department.DepartmentEnName,
            Value = u.Id
        }).ToList();

        ViewBag.ExamUser = examUser?.Name + " " + examUser?.Surname;
        ViewBag.InterviewerList1 = new SelectList(userList1, "Value", "FullName");
        ViewBag.InterviewerList2 = new SelectList(userList2, "Value", "FullName");
        ViewBag.InterviewerDep = new SelectList(userDepList, "Value", "FullName");
        ViewBag.DepartmentId = await _context.SelectList<Department>("Id", "DepartmentEnName");
        interview.SelectedPositions = interview.Positions.Select(x => x.PositionId).ToList();
        ViewBag.PositionId = new SelectList(_context.Position, "Id", "PositionEnName", interview.SelectedPositions);

        ViewBag.CandidateId = candidateId;

        return View(interview);
    }
    [Authorize(Roles = "HR Coordinator")]
    [HttpPost]
    public async Task<IActionResult> EditInterview(Interview model)
    {
        var existingInterview = await _context.Interview.Include(x => x.Positions)
            .SingleOrDefaultAsync(x => x.Id == model.Id);

        //if (model.PositionIds != null && model.PositionIds.Any())
        //{
        //    existingInterview.Positions = string.Join(",", model.PositionIds);
        //}

        existingInterview.User2Id = model.User2Id;
        existingInterview.User3Id = model.User3Id;

        _context.InterviewPosition.RemoveRange(existingInterview.Positions);

        foreach (var positionId in model.SelectedPositions)
        {
            var position = new InterviewPosition
            {
                InterviewId = existingInterview.Id,
                PositionId = positionId
            };
            _context.InterviewPosition.Add(position);
        }

        await _context.SaveChangesAsync();

        var interviewerEmails = await _context.Users
            .Where(u => u.Id == model.User1Id || u.Id == model.User2Id || u.Id == model.User3Id)
            .Select(u => u.Email)
            .ToListAsync();

        var candidate = await _context.Condidate
            .Where(x => x.Id == model.CondidateId)
            .FirstOrDefaultAsync();

        var subject = "You are invited to an interview";
        var interviewerMessage = $"You are assigned to an interview for candidate {candidate.Name} {candidate.Surname}.";

        if (interviewerEmails.Any())
        {
            await _emailService.SendEmailAsync(interviewerEmails, subject, interviewerMessage);
        }

        return RedirectToAction("Interview", new { id = existingInterview.CondidateId });
    }
    [Authorize(Roles = "HR Coordinator")]
    public IActionResult DeleteInterview(Guid id, Guid candidateId)
    {
        var interview = _context.Interview.SingleOrDefault(x => x.Id == id);
        if (interview != null)
        {
            _context.Interview.Remove(interview);
            _context.SaveChanges();
        }
        return RedirectToAction("Interview", new { id = candidateId });
    }
    [Authorize]
    public async Task<IActionResult> InterviewOperation(Guid id)
    {
        var interview = _context.Interview
            .Include(x => x.Condidate)
                .ThenInclude(c => c.Cv)
            .Include(x => x.Department)
            .SingleOrDefault(x => x.Id == id);

        if (interview == null)
        {
            return NotFound();
        }

        var employmentCv = await _context.EmploymentCv.SingleOrDefaultAsync(x => x.Id == interview.Condidate.CvId);
        var maritalStatus = employmentCv?.MaritalStatus;
        ViewBag.MaritalStatus = maritalStatus;

        return View(interview);
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ExamPost(Interview model)
    {
        var interview = await _context.Interview.Include(x => x.Condidate).SingleOrDefaultAsync(x => x.Id == model.Id);
        if (interview != null)
        {
            interview.ExamScore = model.ExamScore;
            interview.ExamComment = model.ExamComment;

            await _context.SaveChangesAsync();

            var interviewerEmail = await _context.Users
                .Where(u => u.Id == interview.User1Id)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            var subject = "You are invited to an interview";
            var interviewerMessage = $"Exam results have been added to the system. The system is waiting for you for {interview.Condidate.Name} {interview.Condidate.Surname}'s Interview 1 process";

            await _emailService.SendEmailAsync(interviewerEmail, subject, interviewerMessage);
        }
        return RedirectToAction("Interview", new { id = interview.CondidateId });
    }
    [Authorize(Roles = "Interviewer 1")]
    [HttpPost]
    public async Task<IActionResult> Interviewer1Post(Interview model)
    {
        var interview = await _context.Interview.Include(x => x.Condidate).SingleOrDefaultAsync(x => x.Id == model.Id);
        if (interview != null)
        {
            if (interview.Condidate?.BirthDate != null)
            {
                interview.Age = CalculateAge(interview.Condidate.BirthDate);
            }

            interview.FamilySituation = model.FamilySituation;
            interview.Children = model.Children;
            interview.ChildrenSum = model.ChildrenSum;
            interview.MilitaryStatus = model.MilitaryStatus;
            interview.Education = model.Education;
            interview.Qualification = model.Qualification;
            interview.ProfessionalLiscences = model.ProfessionalLiscences;
            interview.PreviousEmployer1 = model.PreviousEmployer1;
            interview.AssignedDuties1 = model.AssignedDuties1;
            interview.PreviousEmployer2 = model.PreviousEmployer2;
            interview.AssignedDuties2 = model.AssignedDuties2;
            interview.PreviousEmployer3 = model.PreviousEmployer3;
            interview.AssignedDuties3 = model.AssignedDuties3;
            interview.LastSalary = model.LastSalary;
            interview.CDCReference = model.CDCReference;
            //interview.RequiredPosition = model.RequiredPosition;
            interview.CDCKnow = model.CDCKnow;
            interview.ApplyingPosition = model.ApplyingPosition;
            interview.ConsideringPosition = model.ConsideringPosition;
            interview.OffshoreExperience = model.OffshoreExperience;
            interview.DescribeOther = model.DescribeOther;
            interview.SafetyExperience = model.SafetyExperience;
            interview.StopSmart = model.StopSmart;
            interview.CurrentLast = model.CurrentLast;
            interview.EmergencySituation = model.EmergencySituation;
            interview.GreatestStrength = model.GreatestStrength;
            interview.CurrentJob = model.CurrentJob;
            interview.DescribeDecision = model.DescribeDecision;
            interview.TeamPlayer = model.TeamPlayer;
            interview.MinimumEnglish = model.MinimumEnglish;
            interview.WrittenEnglish = model.WrittenEnglish;
            interview.CareerGoals = model.CareerGoals;
            interview.CDCHire = model.CDCHire;
            //interview.PolicyAcross = model.PolicyAcross;
            interview.Hobbies = model.Hobbies;
            interview.AnyQuestions = model.AnyQuestions;
            interview.Presentation = model.Presentation;
            interview.Attitude = model.Attitude;
            interview.Intelligence1 = model.Intelligence1;
            interview.Suitable1 = model.Suitable1;
            interview.Interview1Result = model.Interview1Result;
            interview.Interview1Comment = model.Interview1Comment;
            interview.Interview1Date = DateTime.UtcNow.AddHours(4).Date;
            interview.ExamScore = model.ExamScore;
            interview.ExamComment = model.ExamComment;

            if (interview.Interview1Result == false)
            {
                interview.ResultComment = "No Passed";
                interview.HireComment = "No Hire";
            }
            else
            {
                interview.ResultComment = "Waiting";

                var interviewerEmail = await _context.Users
                .Where(u => u.Id == interview.User2Id)
                .Select(u => u.Email)
                .ToListAsync();

                var subject = "Pending interview process";
                var interviewerMessage = $"Interview 1 process is over. The system is waiting for you for {interview.Condidate.Name} {interview.Condidate.Surname}'s Interview 2 process";

                await _emailService.SendEmailAsync(interviewerEmail, subject, interviewerMessage);
            }

            var files = Request.Form?.Files?.Where(x => x.Name == "files").ToList();
            if (files.Count > 0)
            {
                interview.FilesId = await UploadFile(files, interview.FilesId);
            }

            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Interview", new { id = interview.CondidateId });
    }
    private int CalculateAge(DateTime birthdate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthdate.Year;
        if (birthdate.Date > today.AddYears(-age)) age--;
        return age;
    }
    [Authorize(Roles = "Interviewer 2")]
    [HttpPost]
    public async Task<IActionResult> Interviewer2Post(Interview model)
    {
        var interview = await _context.Interview.Include(x => x.Condidate).SingleOrDefaultAsync(x => x.Id == model.Id);
        if (interview != null)
        {
            interview.Interview2Date = DateTime.UtcNow.AddHours(4).Date;
            interview.Intelligence2 = model.Intelligence2;
            interview.Suitable2 = model.Suitable2;
            interview.Interview2Comment = model.Interview2Comment;
            interview.Interview2Result = model.Interview2Result;

            if (interview.Interview2Result == false)
            {
                interview.ResultComment = "No Passed";
                interview.HireComment = "No Hire";
            }
            else
            {
                interview.ResultComment = "Waiting";

                var interviewerEmail = await _context.Users
                .Where(u => u.Id == interview.User3Id)
                .Select(u => u.Email)
                .ToListAsync();

                var subject = "Pending interview process";
                var interviewerMessage = $"Interview 2 process is over. The system is waiting for you for {interview.Condidate.Name} {interview.Condidate.Surname}'s Interview 3 process";

                await _emailService.SendEmailAsync(interviewerEmail, subject, interviewerMessage);
            }

            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Interview", new { id = interview.CondidateId });
    }
    [Authorize(Roles = "Department Head")]
    [HttpPost]
    public async Task<IActionResult> Interviewer3Post(Interview model)
    {
        var interview = await _context.Interview.Include(x => x.Condidate).SingleOrDefaultAsync(x => x.Id == model.Id);
        if (interview != null)
        {
            interview.Interview3Date = DateTime.UtcNow.AddHours(4).Date;
            interview.Intelligence3 = model.Intelligence3;
            interview.Suitable3 = model.Suitable3;
            interview.Interview3Comment = model.Interview3Comment;
            interview.Interview3Result = model.Interview3Result;
            interview.InterviewWaive = model.InterviewWaive;

            var intelligenceValues = new List<int>();
            if (interview.Intelligence1.HasValue) intelligenceValues.Add(interview.Intelligence1.Value);
            if (interview.Intelligence2.HasValue) intelligenceValues.Add(interview.Intelligence2.Value);
            if (interview.Intelligence3.HasValue) intelligenceValues.Add(interview.Intelligence3.Value);

            if (intelligenceValues.Count > 0)
            {
                interview.Intelligence = (int)Math.Round(intelligenceValues.Average());
            }

            var suitableValues = new List<int>();
            if (interview.Suitable1.HasValue) suitableValues.Add(interview.Suitable1.Value);
            if (interview.Suitable2.HasValue) suitableValues.Add(interview.Suitable2.Value);
            if (interview.Suitable3.HasValue) suitableValues.Add(interview.Suitable3.Value);

            if (suitableValues.Count > 0)
            {
                interview.Suitable = (int)Math.Round(suitableValues.Average());
            }

            if (interview.Interview3Result == true)
            {
                interview.ResultComment = "Passed";

                var interviewerEmail = await _context.Users
                .Where(u => u.Id == interview.User1Id)
                .Select(u => u.Email)
                .ToListAsync();

                var subject = "Ending Interview";
                var interviewerMessage = $"Interview 3 process for {interview.Condidate.Name} {interview.Condidate.Surname} is over";

                await _emailService.SendEmailAsync(interviewerEmail, subject, interviewerMessage);
            }
            else
            {
                interview.ResultComment = "No Passed";
                interview.HireComment = "No Hire";
            }

            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Interview", new { id = interview.CondidateId });
    }
    [Authorize]
    public async Task<IActionResult> InterviewDetails(Guid id, Guid candidateId)
    {
        ViewBag.CandidateId = candidateId;
        var interview = await _context.Interview.Include(x => x.Files).Include(x => x.Condidate).Include(x => x.Department).Include(x => x.User1).Include(x => x.User2).Include(x => x.User3).SingleOrDefaultAsync(x => x.Id == id);
        if (interview.FilesId != null)
        {
            interview.FilesList = await fileService.GetFiles((Guid)interview.FilesId);
        }
        return View(interview);
    }
    public async Task<IActionResult> GetDownolandFile(Guid id)
    {
        var file = await repo.DB.Cv_OtherFiles.Where(f => f.OtherFile_ID == id).ToListAsync();
        var file2 = await repo.DB.Cv_OtherFiles.FindAsync(id);
        if (file == null)
            return NotFound();

        // Fiziksel dosya yolu
        var filePath = Path.Combine(file2.FilePath, file2.FileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound("Dosya bulunamadı.");

        var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(bytes, file2.FileType, file2.FileName);
    }
    public async Task<IActionResult> GetFile(Guid id)
    {
        var file = await fileService.GetFile(id);
        if (file == null)
        {
            return NotFound();
        }

        if (file.Extension?.ToUpper() != ".PDF")
        {
            return File(fileService.Get(file), "application" + file.Extension?.Replace(".", "/"), file.Name);
        }

        return File(fileService.Get(file), "application" + file.Extension?.Replace(".", "/"));
    }
    private async Task<Guid> UploadFile(List<IFormFile> files, Guid? filesId = null)
    {
        filesId ??= (await repo.Add(new Files())).Id;
        await fileService.Upload(files, (Guid)filesId);
        return (Guid)filesId;
    }
    //[HttpGet]
    //public async Task<IActionResult> ExportToFile(Guid id)
    //{
    //    var interview = await _context.Interview
    //        .Include(i => i.Condidate).Include(x => x.Department).Include(x => x.User1).Include(x => x.User2).Include(x => x.User3).Include(x => x.HireUser).Include(x => x.ExamUser)
    //        .SingleOrDefaultAsync(i => i.Id == id);

    //    if (interview == null)
    //    {
    //        return NotFound();
    //    }

    //    var documentTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FRM-HR-015.docx");
    //    var documentTemplate = System.IO.File.ReadAllBytes(documentTemplatePath);

    //    var placeholders = new Dictionary<string, string>
    //    {
    //        { "Date", interview.Interview1Date.ToString().Substring(0, 10) },
    //        { "Name", interview.Condidate?.Name + " " + interview.Condidate?.Surname },
    //        { "Age", interview.Age.ToString() },
    //        { "Married", interview.FamilySituation == "Married" ? "check_true" : "check_false" },
    //        { "Single", interview.FamilySituation == "Single" ? "check_true" : "check_false" },
    //        { "ChildrenYes", interview.Children == "Yes" ? "check_true" : "check_false" },
    //        { "ChildrenNo", interview.Children == "No" ? "check_true" : "check_false" },
    //        { "ChildrenSum", interview.ChildrenSum },
    //        { "None", interview.Education == "None" ? "check_true" : "check_false" },
    //        { "HighSchool", interview.Education == "High School" ? "check_true" : "check_false" },
    //        { "Diploma", interview.Education == "Diploma" ? "check_true" : "check_false" },
    //        { "Degree", interview.Education == "Degree" ? "check_true" : "check_false" },
    //        { "Technical", interview.Education == "Technical" ? "check_true" : "check_false" },
    //        { "Qualification", interview.Qualification },
    //        { "ProfessionalLiscnences", interview.ProfessionalLiscences },
    //        { "PreviousEmployer1", interview.PreviousEmployer1 },
    //        { "AssignedDuties1", interview.AssignedDuties1 },
    //        { "PreviousEmployer2", interview.PreviousEmployer2 },
    //        { "AssignedDuties2", interview.AssignedDuties2 },
    //        { "PreviousEmployer3", interview.PreviousEmployer3 },
    //        { "AssignedDuties3", interview.AssignedDuties3 },
    //        { "CDCReference", interview.CDCReference },
    //        //{ "RequiredPosition", interview.RequiredPosition },
    //        { "CDCKnow", interview.CDCKnow },
    //        { "ApplyingPosition", interview.ApplyingPosition },
    //        { "ConsideringPosition", interview.ConsideringPosition },
    //        { "Drilling", interview.OffshoreExperience == "Drilling" ? "check_true" : "check_false" },
    //        { "Marine", interview.OffshoreExperience == "Marine" ? "check_true" : "check_false" },
    //        { "Deck", interview.OffshoreExperience == "Deck" ? "check_true" : "check_false" },
    //        { "Crane", interview.OffshoreExperience == "Crane" ? "check_true" : "check_false" },
    //        { "Other", interview.OffshoreExperience == "Other" ? "check_true" : "check_false" },
    //        { "DescribeOther", interview.DescribeOther },
    //        { "SafetyExperience", interview.SafetyExperience },
    //        { "StopSmart", interview.StopSmart },
    //        { "CurrentLast", interview.CurrentLast },
    //        { "EmergencySituation", interview.EmergencySituation },
    //        { "GreatestStrength", interview.GreatestStrength },
    //        { "CurrentJob", interview.CurrentJob },
    //        { "DescribeDecision", interview.DescribeDecision },
    //        { "TeamPlayer", interview.TeamPlayer },
    //        { "MinimumEnglish", interview.MinimumEnglish },
    //        { "WrittenEnglish", interview.WrittenEnglish },
    //        { "CareerGoals", interview.CareerGoals },
    //        { "CDCHire", interview.CDCHire },
    //        //{ "PolicyAcross", interview.PolicyAcross },
    //        { "Hobbies", interview.Hobbies ?? "" },
    //        { "AnyQuestions", interview.AnyQuestions },
    //        { "Presentation", interview.Presentation },
    //        { "Attitude", interview.Attitude },
    //        { "IOne", interview.Intelligence == 1 ? "check_true" : "check_false" },
    //        { "ITwo", interview.Intelligence == 2 ? "check_true" : "check_false" },
    //        { "IThree", interview.Intelligence == 3 ? "check_true" : "check_false" },
    //        { "IFour", interview.Intelligence == 4 ? "check_true" : "check_false" },
    //        { "IFive", interview.Intelligence == 5 ? "check_true" : "check_false" },
    //        { "SOne", interview.Suitable == 1 ? "check_true" : "check_false" },
    //        { "STwo", interview.Suitable == 2 ? "check_true" : "check_false" },
    //        { "SThree", interview.Suitable == 3 ? "check_true" : "check_false" },
    //        { "SFour", interview.Suitable == 4 ? "check_true" : "check_false" },
    //        { "SFive", interview.Suitable == 5 ? "check_true" : "check_false" },
    //        { "Interviewer1", interview.User1.Name + " " + interview.User1.Surname },
    //        { "Interviewer2", interview.User2.Name + " " + interview.User2.Surname },
    //        { "Interviewer3", interview.User3.Name + " " + interview.User3.Surname },
    //        { "Interviewer1Comment", interview.Interview1Comment },
    //        { "Interviewer2Comment", interview.Interview2Comment },
    //        { "Interviewer3Comment", interview.Interview3Comment },
    //        { "ExamScore", interview.ExamScore != null ? "Exam Score - " + interview.ExamScore.ToString() + " %" : "" },
    //        { "ExamDepartment", interview.DepartmentId != null ? "Exam Department - " + interview.Department?.DepartmentEnName : "" },
    //        { "ExamUser", interview.ExamUserId != null ? "Exam User - " + interview.ExamUser.Name + " " + interview.ExamUser.Surname : "" },
    //        { "ExamComment", interview.ExamComment != null ? "Exam Comment - " + interview.ExamComment : "" },
    //        { "HireUser", interview.HireUserId != null ? "Hire Approver - " + interview.HireUser.Name + " " + interview.HireUser.Surname : "" },
    //        { "HireUserComment", interview.HireComment != null ? "Hire Approver Comment - " + interview.HireReason : "" },
    //    };

    //    return File(ds.ReplaceInWord(documentTemplate, placeholders), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{interview.Id}_Assessment.docx");
    //}
    [HttpGet]
    public async Task<IActionResult> ExportToFile(Guid id, string fileType = "docx")
    {
        var interview = await _context.Interview
            .Include(i => i.Condidate)
            .Include(x => x.Department)
            .Include(x => x.User1)
            .Include(x => x.User2)
            .Include(x => x.User3)
            .Include(x => x.HireUser)
            .Include(x => x.ExamUser)
            .SingleOrDefaultAsync(i => i.Id == id);

        if (interview == null)
        {
            return NotFound();
        }

        var documentTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FRM-HR-015.docx");
        var documentTemplate = System.IO.File.ReadAllBytes(documentTemplatePath);

        var placeholders = new Dictionary<string, string>
            {
                { "Date", interview.Interview1Date.ToString().Substring(0, 10) },
                { "Name", interview.Condidate?.Name + " " + interview.Condidate?.Surname },
                { "Age", interview.Age.ToString() },
                { "Married", interview.FamilySituation == "Married" ? "check_true" : "check_false" },
                { "Single", interview.FamilySituation == "Single" ? "check_true" : "check_false" },
                { "ChildrenYes", interview.Children == "Yes" ? "check_true" : "check_false" },
                { "ChildrenNo", interview.Children == "No" ? "check_true" : "check_false" },
                { "ChildrenSum", interview.ChildrenSum },
                { "None", interview.Education == "None" ? "check_true" : "check_false" },
                { "HighSchool", interview.Education == "High School" ? "check_true" : "check_false" },
                { "Diploma", interview.Education == "Diploma" ? "check_true" : "check_false" },
                { "Degree", interview.Education == "Degree" ? "check_true" : "check_false" },
                { "Technical", interview.Education == "Technical" ? "check_true" : "check_false" },
                { "Qualification", interview.Qualification },
                { "ProfessionalLiscnences", interview.ProfessionalLiscences },
                { "PreviousEmployer1", interview.PreviousEmployer1 },
                { "AssignedDuties1", interview.AssignedDuties1 },
                { "PreviousEmployer2", interview.PreviousEmployer2 },
                { "AssignedDuties2", interview.AssignedDuties2 },
                { "PreviousEmployer3", interview.PreviousEmployer3 },
                { "AssignedDuties3", interview.AssignedDuties3 },
                { "CDCReference", interview.CDCReference },
                //{ "RequiredPosition", interview.RequiredPosition },
                { "CDCKnow", interview.CDCKnow },
                { "ApplyingPosition", interview.ApplyingPosition },
                { "ConsideringPosition", interview.ConsideringPosition },
                { "Drilling", interview.OffshoreExperience == "Drilling" ? "check_true" : "check_false" },
                { "Marine", interview.OffshoreExperience == "Marine" ? "check_true" : "check_false" },
                { "Deck", interview.OffshoreExperience == "Deck" ? "check_true" : "check_false" },
                { "Crane", interview.OffshoreExperience == "Crane" ? "check_true" : "check_false" },
                { "Other", interview.OffshoreExperience == "Other" ? "check_true" : "check_false" },
                { "DescribeOther", interview.DescribeOther },
                { "SafetyExperience", interview.SafetyExperience },
                { "StopSmart", interview.StopSmart },
                { "CurrentLast", interview.CurrentLast },
                { "EmergencySituation", interview.EmergencySituation },
                { "GreatestStrength", interview.GreatestStrength },
                { "CurrentJob", interview.CurrentJob },
                { "DescribeDecision", interview.DescribeDecision },
                { "TeamPlayer", interview.TeamPlayer },
                { "MinimumEnglish", interview.MinimumEnglish },
                { "WrittenEnglish", interview.WrittenEnglish },
                { "CareerGoals", interview.CareerGoals },
                { "CDCHire", interview.CDCHire },
                //{ "PolicyAcross", interview.PolicyAcross },
                { "Hobbies", interview.Hobbies ?? "" },
                { "AnyQuestions", interview.AnyQuestions },
                { "Presentation", interview.Presentation },
                { "Attitude", interview.Attitude },
                { "IOne", interview.Intelligence == 1 ? "check_true" : "check_false" },
                { "ITwo", interview.Intelligence == 2 ? "check_true" : "check_false" },
                { "IThree", interview.Intelligence == 3 ? "check_true" : "check_false" },
                { "IFour", interview.Intelligence == 4 ? "check_true" : "check_false" },
                { "IFive", interview.Intelligence == 5 ? "check_true" : "check_false" },
                { "SOne", interview.Suitable == 1 ? "check_true" : "check_false" },
                { "STwo", interview.Suitable == 2 ? "check_true" : "check_false" },
                { "SThree", interview.Suitable == 3 ? "check_true" : "check_false" },
                { "SFour", interview.Suitable == 4 ? "check_true" : "check_false" },
                { "SFive", interview.Suitable == 5 ? "check_true" : "check_false" },
                {
    "Interviewer1",
    interview.User1.Name + " " + interview.User1.Surname +
    (interview.Interview1Result == true ? " Passed" : (interview.Interview1Result == false ? " No Passed" : null))
    },
                {
    "Interviewer2",
    interview.User2.Name + " " + interview.User2.Surname +
    (interview.Interview2Result == true ? " Passed" : (interview.Interview2Result == false ? " No Passed" : null))
    },
                {
    "Interviewer3",
    interview.User3.Name + " " + interview.User3.Surname +
    (interview.Interview3Result == true ? " Passed" : (interview.Interview3Result == false ? " No Passed" : null))
    },
                { "Interviewer1Comment", interview.Interview1Comment },
                { "Interviewer2Comment", interview.Interview2Comment },
                { "Interviewer3Comment", interview.InterviewWaive != null ? interview.InterviewWaive : interview.Interview3Comment },
                { "ExamScore", interview.ExamScore != null ? "Exam Score - " + interview.ExamScore.ToString() + " %" : "" },
                { "ExamDepartment", interview.DepartmentId != null ? "Exam Department - " + interview.Department?.DepartmentEnName : "" },
                { "ExamUser", interview.ExamUserId != null ? "Exam User - " + interview.ExamUser.Name + " " + interview.ExamUser.Surname : "" },
                { "ExamComment", interview.ExamComment != null ? "Exam Comment - " + interview.ExamComment : "" },
                { "HireUser", interview.HireUserId != null ? "Hire Approver - " + interview.HireUser.Name + " " + interview.HireUser.Surname : "" },
                { "HireUserComment", interview.HireComment == "true" ? "Hire Approver Comment - " + interview.HireReason : "" },
            };

        var ds = new DocumentService();
        var wordBytes = ds.ReplaceInWord(documentTemplate, placeholders);

        if (fileType.ToLower() == "pdf")
        {
            using var ms = new MemoryStream(wordBytes);
            var document = new Spire.Doc.Document(ms);

            using var pdfStream = new MemoryStream();
            document.SaveToFile(pdfStream, Spire.Doc.FileFormat.PDF);
            return File(pdfStream.ToArray(), "application/pdf", $"{interview.Id}_Assessment.pdf");
        }

        return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{interview.Id}_Assessment.docx");
    }
    [Authorize(Roles = "Admin,Interviewer")]
    public async Task<IActionResult> InterviewerChat(Guid id)
    {
        return View(_context.Interview.Include(x => x.Condidate).SingleOrDefault(x => x.Id == id));
    }
    [Authorize(Roles = "Interviewer")]
    [HttpPost]
    public async Task<IActionResult> Interviewer1ChatPost(Interview model)
    {
        var interview = await _context.Interview.Include(x => x.Condidate).SingleOrDefaultAsync(x => x.Id == model.Id);
        if (interview != null)
        {
            interview.Interviewer1ChatAdmin = model.Interviewer1ChatAdmin;
            _context.SaveChanges();

            var interviewerEmails = await _context.Users
                .Where(u => u.UserName == "admin")
                .Select(u => u.Email)
                .ToListAsync();

            var candidate = await _context.Condidate.Where(x => x.Id == model.CondidateId).FirstOrDefaultAsync();

            var subject = "Change in Interview information";
            var interviewerMessage = $"Please make changes to {candidate.Name} {candidate.Surname}'s interview information by reading the following comment:\n {interview.Interviewer1ChatAdmin}";

            await _emailService.SendEmailAsync(interviewerEmails, subject, interviewerMessage);
        }
        return RedirectToAction("Interview", new { id = interview.CondidateId });
    }
    [Authorize(Roles = "Interviewer")]
    [HttpPost]
    public async Task<IActionResult> Interviewer2ChatPost(Interview model)
    {
        var interview = await _context.Interview.Include(x => x.Condidate).SingleOrDefaultAsync(x => x.Id == model.Id);
        if (interview != null)
        {
            interview.Interviewer2ChatAdmin = model.Interviewer2ChatAdmin;
            _context.SaveChanges();

            var interviewerEmails = await _context.Users
                .Where(u => u.UserName == "admin")
                .Select(u => u.Email)
                .ToListAsync();

            var candidate = await _context.Condidate.Where(x => x.Id == model.CondidateId).FirstOrDefaultAsync();

            var subject = "Change in Interview information";
            var interviewerMessage = $"Please make changes to {candidate.Name} {candidate.Surname}'s interview information by reading the following comment:\n {interview.Interviewer2ChatAdmin}";

            await _emailService.SendEmailAsync(interviewerEmails, subject, interviewerMessage);
        }
        return RedirectToAction("Interview", new { id = interview.CondidateId });
    }
    [Authorize(Roles = "Interviewer")]
    [HttpPost]
    public async Task<IActionResult> Interviewer3ChatPost(Interview model)
    {
        var interview = await _context.Interview.Include(x => x.Condidate).SingleOrDefaultAsync(x => x.Id == model.Id);
        if (interview != null)
        {
            interview.Interviewer3ChatAdmin = model.Interviewer3ChatAdmin;
            _context.SaveChanges();

            var interviewerEmails = await _context.Users
                .Where(u => u.UserName == "admin")
                .Select(u => u.Email)
                .ToListAsync();

            var candidate = await _context.Condidate.Where(x => x.Id == model.CondidateId).FirstOrDefaultAsync();

            var subject = "Change in Interview information";
            var interviewerMessage = $"Please make changes to {candidate.Name} {candidate.Surname}'s interview information by reading the following comment:\n {interview.Interviewer3ChatAdmin}";

            await _emailService.SendEmailAsync(interviewerEmails, subject, interviewerMessage);
        }
        return RedirectToAction("Interview", new { id = interview.CondidateId });
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> InterviewEditAdmin(Guid id)
    {
        return View(_context.Interview.Include(x => x.Condidate).Include(x => x.Department).Include(x => x.User1).Include(x => x.User2).Include(x => x.User3).SingleOrDefault(x => x.Id == id));
    }
    [HttpPost]
    public async Task<IActionResult> InterviewEditAdmin(Interview intv)
    {
        var oldintv = _context.Interview.SingleOrDefault(x => x.Id == intv.Id);

        oldintv.ExamScore = intv.ExamScore;
        oldintv.ExamComment = intv.ExamComment;
        oldintv.FamilySituation = intv.FamilySituation;
        oldintv.Children = intv.Children;
        oldintv.ChildrenSum = intv.ChildrenSum;
        oldintv.Education = intv.Education;
        oldintv.Qualification = intv.Qualification;
        oldintv.ProfessionalLiscences = intv.ProfessionalLiscences;
        oldintv.PreviousEmployer1 = intv.PreviousEmployer1;
        oldintv.AssignedDuties1 = intv.AssignedDuties1;
        oldintv.PreviousEmployer2 = intv.PreviousEmployer2;
        oldintv.AssignedDuties2 = intv.AssignedDuties2;
        oldintv.PreviousEmployer3 = intv.PreviousEmployer3;
        oldintv.AssignedDuties3 = intv.AssignedDuties3;
        oldintv.CDCReference = intv.CDCReference;
        //oldintv.RequiredPosition = intv.RequiredPosition;
        oldintv.CDCKnow = intv.CDCKnow;
        oldintv.ApplyingPosition = intv.ApplyingPosition;
        oldintv.ConsideringPosition = intv.ConsideringPosition;
        oldintv.OffshoreExperience = intv.OffshoreExperience;
        oldintv.DescribeOther = intv.DescribeOther;
        oldintv.SafetyExperience = intv.SafetyExperience;
        oldintv.StopSmart = intv.StopSmart;
        oldintv.CurrentLast = intv.CurrentLast;
        oldintv.EmergencySituation = intv.EmergencySituation;
        oldintv.GreatestStrength = intv.GreatestStrength;
        oldintv.CurrentJob = intv.CurrentJob;
        oldintv.DescribeDecision = intv.DescribeDecision;
        oldintv.TeamPlayer = intv.TeamPlayer;
        oldintv.MinimumEnglish = intv.MinimumEnglish;
        oldintv.WrittenEnglish = intv.WrittenEnglish;
        oldintv.CareerGoals = intv.CareerGoals;
        oldintv.CDCHire = intv.CDCHire;
        //oldintv.PolicyAcross = intv.PolicyAcross;
        oldintv.Hobbies = intv.Hobbies;
        oldintv.AnyQuestions = intv.AnyQuestions;
        oldintv.Presentation = intv.Presentation;
        oldintv.Attitude = intv.Attitude;
        oldintv.Intelligence = intv.Intelligence;
        oldintv.Suitable = intv.Suitable;

        await _context.SaveChangesAsync();
        return RedirectToAction("Interview", new { id = intv.CondidateId });
    }
    public async Task<IActionResult> HireApprove(Guid id)
    {
        return View(_context.Interview.Include(x => x.Condidate).SingleOrDefault(x => x.Id == id));
    }
    [HttpPost]
    public async Task<IActionResult> HireApprove(Interview model)
    {
        var interview = await _context.Interview.Include(x => x.Condidate).SingleOrDefaultAsync(x => x.Id == model.Id);
        var user = await _userManager.GetUserAsync(User);
        if (interview != null)
        {
            interview.HireComment = model.HireComment;
            interview.HireReason = model.HireReason;
            interview.HireUserId = user.Id;

            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Interview", new { id = interview.CondidateId });
    }
    public async Task<IActionResult> ExportToExcel(string? result, string? hire, Guid id)
    {
        IQueryable<Interview> interviews = _context.Interview.Where(x => x.CondidateId == id)
            .Include(x => x.Condidate)
            .Include(x => x.Positions)
            .ThenInclude(x => x.Position)
            .Include(x => x.User1)
            .Include(x => x.User2)
            .Include(x => x.User3)
            .Include(x => x.Department);

        if (!string.IsNullOrEmpty(result))
        {
            interviews = interviews.Where(x => x.ResultComment == result);
        }

        if (!string.IsNullOrEmpty(hire))
        {
            interviews = interviews.Where(x => x.HireComment == hire);
        }

        var interviewList = await interviews.ToListAsync();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Interviews");

        worksheet.Cells[1, 1].Value = "Interview Log";
        worksheet.Cells[1, 1, 1, 18].Merge = true;
        worksheet.Cells[1, 1].Style.Font.Bold = true;
        worksheet.Cells[1, 1].Style.Font.Size = 16;
        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        worksheet.Cells[2, 1].Value = "Number";
        worksheet.Cells[2, 2].Value = "Applicant (Name and Surname)";
        worksheet.Cells[2, 3].Value = "Telephone";
        worksheet.Cells[2, 4].Value = "Education";
        worksheet.Cells[2, 5].Value = "Military Status";
        worksheet.Cells[2, 6].Value = "Position Applied For";
        worksheet.Cells[2, 7].Value = "Exam Department";
        worksheet.Cells[2, 8].Value = "Score";
        worksheet.Cells[2, 9].Value = "Interview Status HR";
        worksheet.Cells[2, 10].Value = "Interview Status By Department";
        worksheet.Cells[2, 11].Value = "Interview Status By Rig Manager";
        worksheet.Cells[2, 12].Value = "Date Of Interview";
        worksheet.Cells[2, 13].Value = "Interview Score";
        worksheet.Cells[2, 14].Value = "Recommended For Hire Yes/No";
        worksheet.Cells[2, 15].Value = "General Director Approval";
        worksheet.Cells[2, 16].Value = "Comments";

        var row = 3;
        var number = 1;
        foreach (var interview in interviewList)
        {
            worksheet.Cells[row, 1].Value = number;
            worksheet.Cells[row, 2].Value = $"{interview.Condidate.Name} {interview.Condidate.Surname}";
            worksheet.Cells[row, 3].Value = interview.Condidate.FullPhone;
            worksheet.Cells[row, 4].Value = interview.Qualification;
            worksheet.Cells[row, 5].Value = interview.MilitaryStatus;
            worksheet.Cells[row, 6].Value = string.Join(", ", interview.Positions.Select(p => p.Position.PositionEnName));
            worksheet.Cells[row, 7].Value = interview.Department?.DepartmentEnName;
            worksheet.Cells[row, 8].Value = interview.ExamScore != null ? interview.ExamScore + " % " : "";
            if (interview.Interview1Result == true)
            {
                worksheet.Cells[row, 9].Value = "Passed by " + interview.User1.Name + " " + interview.User1.Surname;
            }
            else if (interview.Interview1Result == false)
            {
                worksheet.Cells[row, 9].Value = "Failed to pass " + interview.User1.Name + " " + interview.User1.Surname;
            }
            else if (interview.Interview1Result == null)
            {
                worksheet.Cells[row, 9].Value = "";
            }
            if (interview.Interview2Result == true)
            {
                worksheet.Cells[row, 10].Value = "Passed by " + interview.User2.Name + " " + interview.User2.Surname;
            }
            else if (interview.Interview2Result == false)
            {
                worksheet.Cells[row, 10].Value = "Failed to pass " + interview.User2.Name + " " + interview.User2.Surname;
            }
            else if (interview.Interview2Result == null)
            {
                worksheet.Cells[row, 10].Value = "";
            }
            if (interview.Interview3Result == true)
            {
                worksheet.Cells[row, 11].Value = "Passed by " + interview.User3.Name + " " + interview.User3.Surname;
            }
            else if (interview.Interview3Result == false)
            {
                worksheet.Cells[row, 11].Value = "Failed to pass " + interview.User3.Name + " " + interview.User3.Surname;
            }
            else if (interview.Interview3Result == null)
            {
                worksheet.Cells[row, 11].Value = "";
            }
            worksheet.Cells[row, 12].Value = interview.Interview1Date;
            worksheet.Cells[row, 12].Style.Numberformat.Format = "d-MMM-yy";
            worksheet.Cells[row, 13].Value = interview.Intelligence + " ; " + interview.Suitable;
            if (interview.HireComment == null)
            {
                worksheet.Cells[row, 14].Value = "";
            }
            else if (interview.HireComment == "true")
            {
                worksheet.Cells[row, 14].Value = "Yes";
            }
            else if (interview.HireComment == "false")
            {
                worksheet.Cells[row, 14].Value = "No";
            }
            if (interview.HireComment == null)
            {
                worksheet.Cells[row, 15].Value = "";
            }
            else if (interview.HireComment == "true")
            {
                worksheet.Cells[row, 15].Value = "Passed";
            }
            else if (interview.HireComment == "false")
            {
                worksheet.Cells[row, 15].Value = "No Passed";
            }
            worksheet.Cells[row, 16].Value = interview.HireComment == "true" ? $"{interview.User1.Name + " " + interview.User1.Surname} : {interview.Interview1Comment}, {interview.User2.Name + " " + interview.User2.Surname} : {interview.Interview2Comment}, {interview.User3.Name + " " + interview.User3.Surname} : {interview.Interview3Comment}, Approved by General Director" : $"{interview.User1.Name + " " + interview.User1.Surname} : {interview.Interview1Comment}, {interview.User2.Name + " " + interview.User2.Surname} : {interview.Interview2Comment}, {interview.User3.Name + " " + interview.User3.Surname} : {interview.Interview3Comment}";

            if (interview.HireComment == "true")
            {
                for (int col = 1; col <= 18; col++)
                {
                    worksheet.Cells[row, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                }
            }

            number++;
            row++;
        }

        var memoryStream = new MemoryStream();
        package.SaveAs(memoryStream);

        var fileName = $"Interviews_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        memoryStream.Position = 0;
        return File(memoryStream, contentType, fileName);
    }
    public async Task<IActionResult> ExportCandidatesToExcel()
    {
        var candidates = await _context.Condidate.ToListAsync();

        foreach (var candidate in candidates)
        {
            candidate.Interview = await _context.Interview.Where(x => x.CondidateId == candidate.Id).Include(x => x.Positions).ThenInclude(x => x.Position).Include(x => x.Department).Include(x => x.User1).Include(x => x.User2).Include(x => x.User3).ToListAsync();
        }

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Interviews");

        worksheet.Cells[1, 1].Value = "Interview Log";
        worksheet.Cells[1, 1, 1, 18].Merge = true;
        worksheet.Cells[1, 1].Style.Font.Bold = true;
        worksheet.Cells[1, 1].Style.Font.Size = 16;
        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        worksheet.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        worksheet.Cells[2, 1].Value = "Number";
        worksheet.Cells[2, 2].Value = "Applicant (Name and Surname)";
        worksheet.Cells[2, 3].Value = "Telephone";
        worksheet.Cells[2, 4].Value = "Education";
        worksheet.Cells[2, 5].Value = "Military Status";
        worksheet.Cells[2, 6].Value = "Position Applied For";
        worksheet.Cells[2, 7].Value = "Exam Department";
        worksheet.Cells[2, 8].Value = "Score";
        worksheet.Cells[2, 9].Value = "Interview Status HR";
        worksheet.Cells[2, 10].Value = "Interview Status By Department";
        worksheet.Cells[2, 11].Value = "Interview Status By Rig Manager";
        worksheet.Cells[2, 12].Value = "Date Of Interview";
        worksheet.Cells[2, 13].Value = "Interview Score";
        worksheet.Cells[2, 14].Value = "Recommended For Hire Yes/No";
        worksheet.Cells[2, 15].Value = "General Director Approval";
        worksheet.Cells[2, 16].Value = "Comments";

        var row = 3;
        var number = 1;
        foreach (var candidate in candidates)
        {
            foreach (var interview in candidate.Interview)
            {
                worksheet.Cells[row, 1].Value = number;
                worksheet.Cells[row, 2].Value = $"{interview.Condidate.Name} {interview.Condidate.Surname}";
                worksheet.Cells[row, 3].Value = interview.Condidate.FullPhone;
                worksheet.Cells[row, 4].Value = interview.Qualification;
                worksheet.Cells[row, 5].Value = interview.MilitaryStatus;
                worksheet.Cells[row, 6].Value = string.Join(", ", interview.Positions.Select(p => p.Position.PositionEnName));
                worksheet.Cells[row, 7].Value = interview.Department?.DepartmentEnName;
                worksheet.Cells[row, 8].Value = interview.ExamScore != null ? interview.ExamScore + " % " : "";
                if (interview.Interview1Result == true)
                {
                    worksheet.Cells[row, 9].Value = "Passed by " + interview.User1.Name + " " + interview.User1.Surname;
                }
                else if (interview.Interview1Result == false)
                {
                    worksheet.Cells[row, 9].Value = "Failed to pass " + interview.User1.Name + " " + interview.User1.Surname;
                }
                else if (interview.Interview1Result == null)
                {
                    worksheet.Cells[row, 9].Value = "";
                }
                if (interview.Interview2Result == true)
                {
                    worksheet.Cells[row, 10].Value = "Passed by " + interview.User2.Name + " " + interview.User2.Surname;
                }
                else if (interview.Interview2Result == false)
                {
                    worksheet.Cells[row, 10].Value = "Failed to pass " + interview.User2.Name + " " + interview.User2.Surname;
                }
                else if (interview.Interview2Result == null)
                {
                    worksheet.Cells[row, 10].Value = "";
                }
                if (interview.Interview3Result == true)
                {
                    worksheet.Cells[row, 11].Value = "Passed by " + interview.User3.Name + " " + interview.User3.Surname;
                }
                else if (interview.Interview3Result == false)
                {
                    worksheet.Cells[row, 11].Value = "Failed to pass " + interview.User3.Name + " " + interview.User3.Surname;
                }
                else if (interview.Interview3Result == null)
                {
                    worksheet.Cells[row, 11].Value = "";
                }
                worksheet.Cells[row, 12].Value = interview.Interview1Date;
                worksheet.Cells[row, 12].Style.Numberformat.Format = "d-MMM-yy";
                worksheet.Cells[row, 13].Value = interview.Intelligence + " ; " + interview.Suitable;
                if (interview.HireComment == null)
                {
                    worksheet.Cells[row, 14].Value = "";
                }
                else if (interview.HireComment == "true")
                {
                    worksheet.Cells[row, 14].Value = "Yes";
                }
                else if (interview.HireComment == "false")
                {
                    worksheet.Cells[row, 14].Value = "No";
                }
                if (interview.HireComment == null)
                {
                    worksheet.Cells[row, 15].Value = "";
                }
                else if (interview.HireComment == "true")
                {
                    worksheet.Cells[row, 15].Value = "Passed";
                }
                else if (interview.HireComment == "false")
                {
                    worksheet.Cells[row, 15].Value = "No Passed";
                }
                worksheet.Cells[row, 16].Value = interview.HireComment == "true" ? $"{interview.User1.Name + " " + interview.User1.Surname} : {interview.Interview1Comment}, {interview.User2.Name + " " + interview.User2.Surname} : {interview.Interview2Comment}, {interview.User3.Name + " " + interview.User3.Surname} : {interview.Interview3Comment}, Approved by General Director" : $"{interview.User1.Name + " " + interview.User1.Surname} : {interview.Interview1Comment}, {interview.User2.Name + " " + interview.User2.Surname} : {interview.Interview2Comment}, {interview.User3.Name + " " + interview.User3.Surname} : {interview.Interview3Comment}";

                if (interview.HireComment == "true")
                {
                    for (int col = 1; col <= 18; col++)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    }
                }

                row++;
                number++;
            }
        }

        var memoryStream = new MemoryStream();
        package.SaveAs(memoryStream);

        var fileName = $"Interviews_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        memoryStream.Position = 0;
        return File(memoryStream, contentType, fileName);
    }
    public async Task<IActionResult> EmploymentCv()
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user.Id;
        var userDepartmentId = user?.DepartmentId;

        if (userDepartmentId == null)
        {
            return NotFound("User's department not found.");
        }

        if (User.IsInRole("Employment Manager"))
        {
            var cvs = await _context.EmploymentCv.Include(x => x.Cv_Eductions).Include(x => x.Position)
                .OrderByDescending(x => x.CreatedDate).ToListAsync();
            // var cvs2 = await _context.EmploymentCv.ToListAsync();

            return View(cvs);
        }
        else
        {
            var cvs = await _context.EmploymentCv.Where(cv => cv.DepartmentId == userDepartmentId).ToListAsync();
            //var cvs = await _context.EmploymentCv.ToListAsync();
            return View(cvs);
        }
    }
    public async Task<IActionResult> CvDetails(Guid id)
    {
        var cvdet = await _context.EmploymentCv.Include(e => e.Cv_OtherFiles)
                                           .Include(x => x.Cv_Eductions)
                                           .Include(x => x.Cv_ReferanceFrients)
                                           .Include(x => x.Cv_WorkExperiences)
                                            .Include(x => x.Files).ThenInclude(x => x.FileList)
                                           .Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id);

        if (cvdet.FinCode != null)
        {
            return View("OldCvDetails", cvdet);
        }
        else
        {
            return View(cvdet);
        }
    }
    [HttpPost]
    public async Task<IActionResult> AddInterview(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("ID Not Empty.");
        }

        var cvCandidate = await _context.EmploymentCv.FirstOrDefaultAsync(c => c.Id == id);

        if (cvCandidate == null)
        {
            return NotFound("Candidate Not Find ");
        }

        cvCandidate.CV_Interview_Process_Status = CV_Interview_Process_Status.Ready_For_Interview.ToString();

        await _context.SaveChangesAsync();

        return Ok("Candidate Success Interview Added");
    }
    public async Task<IActionResult> GetUserImg(Guid id)
    {
        var userimg = (await _context.EmploymentCv.FirstOrDefaultAsync(x => x.Id == id))?.ImgName;
        if (userimg == null)
        {
            return NotFound();
        }

        var fileroot = Path.Combine(filesLocation, "CVFiles", userimg);
        var imgbyte = await System.IO.File.ReadAllBytesAsync(fileroot);
        var extension = Path.GetExtension(userimg)?.ToLower();
        return File(imgbyte, $"image/{extension?.Replace(".", "")}");
    }
    public async Task<IActionResult> CvNewCandidate(Guid id)
    {
        try
        {
            var employmentCv = await _context.EmploymentCv.FirstOrDefaultAsync(x => x.Id == id);


            if (employmentCv == null)
            {
                return NotFound();
            }

            var newCandidate = new Condidate
            {
                FinCode = employmentCv.FinCode ?? employmentCv.AppID,
                Name = employmentCv.FirstName,
                Surname = employmentCv.FamilyName,
                BirthDate = employmentCv.DateOfBirth,
                Phone = employmentCv.Phone,
                PhoneCode = employmentCv.PhoneCode,
                PhoneCountryCode = employmentCv.PhoneCountryCode,
                CvId = employmentCv.Id,
                Cv = null,
                CommonComment = ""
            };

            _context.Condidate.Add(newCandidate);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "New condidate successfully created." });
        }
        catch (Exception exc)
        {
            return Json(new { success = false, message = $"{exc.Message} --- {exc.InnerException?.Message}" });
        }
    }

    public async Task<IActionResult> CvFiles()
    {
        var cvFiles = await _context.CvFiles.Include(x => x.Position).Include(cf => cf.Files).ThenInclude(f => f.FileList).ToListAsync();
        return View(cvFiles);
    }
    public async Task<IActionResult> CvFileUpload()
    {
        ViewBag.PositionId = new SelectList(_context.Position, "Id", "PositionEnName");
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CvFileUpload(CvFiles cf)
    {
        var files = Request.Form?.Files?.Where(x => x.Name == "files").ToList();
        if (files.Count > 0)
        {
            cf.FilesId = await UploadFile(files, cf.FilesId);
        }

        _context.CvFiles.Add(cf);
        await _context.SaveChangesAsync();

        return RedirectToAction("CvFiles");
    }
    [HttpGet]
    public async Task<IActionResult> GetPositionsByDepartment(Guid departmentId)
    {
        var positions = await _context.Position.Where(p => p.DepartmentId == departmentId).Select(p => new { p.Id, p.PositionEnName }).ToListAsync();
        return Json(positions);
    }
}


