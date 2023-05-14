using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;
using System.Data;
using System.Security.Claims;

namespace OnlineExaminationSystem.Controllers
{
    
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateExam()
        {
        

            var viewModel = new ExamViewModel();
            viewModel.Questions = await GetQuestionsAsync();
            viewModel.Courses = await GetCoursesAsync();
            return View("CreateExamViewModel", viewModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam(ExamViewModel viewModel, string SelectedQuestionIdsJson)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Questions = await GetQuestionsAsync();
                viewModel.Courses = await GetCoursesAsync();
                return View("CreateExamViewModel",viewModel);
            }

            var exam = new Exam
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                ExamDuration = viewModel.ExamDuration,
                ApplicationUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                CourseId = viewModel.CourseId
            };

            if (!string.IsNullOrEmpty(SelectedQuestionIdsJson))
            {
                var selectedQuestionIds = JsonConvert.DeserializeObject<int[]>(SelectedQuestionIdsJson);
                foreach (var questionId in selectedQuestionIds)
                {
                    var question = _context.Questions.Find(questionId);
                    _context.ExamQuestions.Add(new ExamQuestion { Exam = exam, Question = question });
                }
            }

            _context.Exams.Add(exam);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public async Task<List<SelectListItem>> GetQuestionsAsync(int? courseId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = _context.Questions
                .Where(q => q.ApplicationUserId == userId);

            if (courseId.HasValue)
            {
                query = query.Where(q => q.CourseId == courseId.Value);
            }

            var questions = await query
                .Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Text })
                .ToListAsync();

            return questions;
        }

        public async Task<List<SelectListItem>> GetCoursesAsync()
        {
            var courses = await _context.Courses
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

            return courses;
        }


        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> Detail()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IQueryable<Exam> examsQuery = _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Answers);

            if (User.IsInRole("teacher"))
            {
                examsQuery = examsQuery.Where(e => e.ApplicationUserId == userId);
            }

            var exams = await examsQuery.ToListAsync();

            return View("SeeAllExams", exams);

        }

         
        public async Task<IActionResult> Edit(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null)
            {
                return NotFound();
            }

            var viewModel = new ExamViewModel
            {
                Name = exam.Name,
                Description = exam.Description,
                ExamDuration = exam.ExamDuration,
                CourseId = exam.CourseId,
                Courses = await GetCoursesAsync(),
                SelectedQuestionIds = exam.Questions.Select(q => q.Id).ToList(),
                Questions = await GetQuestionsAsync()
            };

            return View("EditExam",viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamViewModel viewModel, string SelectedQuestionIdsJson)
        {
            if (ModelState.IsValid)
            {
                var exam = await _context.Exams
                    .Include(e => e.Course)
                    .Include(e => e.Questions)
                    .FirstOrDefaultAsync(e => e.Id == id);

                exam.Name = viewModel.Name;
                exam.Description = viewModel.Description;
                exam.ExamDuration = viewModel.ExamDuration;
                exam.CourseId = viewModel.CourseId;

                var selectedQuestionIds = JsonConvert.DeserializeObject<int[]>(SelectedQuestionIdsJson);
  
                var currentQuestionIds = exam.Questions.Select(q => q.Id).ToList();
                var addedQuestionIds = selectedQuestionIds.Except(currentQuestionIds).ToList();
                var removedQuestionIds = currentQuestionIds.Except(selectedQuestionIds).ToList();

                // Add new selected questions to ExamQuestion
                foreach (var questionId in addedQuestionIds)
                {
                    var examQuestion = new ExamQuestion { ExamId = exam.Id, QuestionId = questionId };
                    _context.ExamQuestions.Add(examQuestion);
                }

                // Remove unselected questions from ExamQuestion
                foreach (var questionId in removedQuestionIds)
                {
                    var examQuestion = await _context.ExamQuestions
                        .FirstOrDefaultAsync(eq => eq.ExamId == exam.Id && eq.QuestionId == questionId);
                    if (examQuestion != null)
                    {
                        _context.ExamQuestions.Remove(examQuestion);
                    }
                }

                try
                {
                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(exam.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return View("EditExam", viewModel);
            }

            viewModel.Courses = await GetCoursesAsync();
            viewModel.Questions = await GetQuestionsAsync();
            return View("EditExam", viewModel);
        }



        private bool ExamExists(int id)
        {
            return _context.Exams.Any(e => e.Id == id);
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }

            // Delete related records in ExamQuestion table
            var relatedExamQuestions = await _context.ExamQuestions.Where(eq => eq.ExamId == id).ToListAsync();
            _context.ExamQuestions.RemoveRange(relatedExamQuestions);

            // Remove exam and save changes
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> TakeExam(int examId, int assignmentId)
        {
            var exam = await _context.Exams
                .Include(e => e.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(e => e.Id == examId);

            if (exam == null)
            {
                return NotFound();
            }

            var viewModel = new TakeExamViewModel
            {
                ExamId = exam.Id,
                ExamName = exam.Name,
                AssignmentId= assignmentId,
                ExamDuration = exam.ExamDuration,
                Questions = exam.Questions.Select(q => new TakeExamQuestionViewModel
                {
                    Id = q.Id,
                    Text = q.Text,
                    QuestionType = q.Type, // set the QuestionType property
                    Answers = q.Answers.Select(o => new TakeExamAnswerViewModel
                    {
                        Id = o.Id,
                        Text = o.Text
                    }).ToList()
                }).ToList()


            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> TakeExam(TakeExamViewModel model, string answersJson, string textAnswersJson)
        {
            // Retrieve the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Create a new Submission object
            var submission = new Submission
            {
                ApplicationUserId = userId,
                AssignmentId = model.AssignmentId,
                SubmissionTime = DateTime.Now
            };

            // Loop through the selected answers and add them to the Submission object
           
            Dictionary<int,List<int>> answers = new Dictionary<int, List<int>>();
            Dictionary<int, string> textAnswers =  new Dictionary<int, string>();
            if (answersJson != null) {
                answers=JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(answersJson);

            }
            if (textAnswersJson != null) {
                textAnswers = JsonConvert.DeserializeObject<Dictionary<int, string>>(textAnswersJson);
            }
            foreach (var answerId in answers)
            {
                var quest = answerId.Key;
                var anss = answerId.Value;
          
                foreach (var ans in anss) { 
                        if(ans == -1)
                    {
                        var studentAnswer = new StudentAnswer
                        {
                            QuestionId = quest,
                            SubmissionId = submission.Id
                        };
                        submission.StudentAnswers.Add(studentAnswer);
                    }
                    else { 
                        var studentAnswer = new StudentAnswer
                        {

                            AnswerId = ans,
                            QuestionId = quest,
                            SubmissionId = submission.Id
                        };
                        submission.StudentAnswers.Add(studentAnswer);
                    }             

                }
            }
            foreach (var textAnswer in textAnswers)
            {
                var questionId = textAnswer.Key;
                var answerText = textAnswer.Value;

                var studentAnswer = new StudentAnswer
                {
                    Text = answerText,
                    SubmissionId = submission.Id,
                    QuestionId = questionId
                };
                submission.StudentAnswers.Add(studentAnswer);
            }
         
            try
            {
                // Save the Submission object to the database
                _context.Submissions.Add(submission);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the error or display it to the user
                Console.WriteLine(ex.Message);
                throw;
            }
            var examResult = await CreateExamResultAsync(model.ExamId, userId, answers, textAnswers, submission);
            submission.ExamResult = examResult;
            return RedirectToAction("Detail", "ExamResult", new { id = examResult.Id });
        }

        private async Task<ExamResult> CreateExamResultAsync(int examId, string userId, Dictionary<int,List<int>> answerIds, Dictionary<int, string> textAnswers, Submission sub)
        {
            // Retrieve the Exam object
            var exam = await _context.Exams
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(e => e.Id == examId);
            if (exam == null)
            {
                throw new ArgumentException($"No exam found with ID {examId}");
            }

            // Calculate the score
            var score = 0.0;
            foreach (var question in exam.Questions)
            {
                if (question.Type == QuestionType.ShortAnswer ||question.Type == QuestionType.Essay)
                {
                    continue;
                }

                var questionId = question.Id;
                var questionAnswerIds = answerIds.ContainsKey(questionId) ? answerIds[questionId] : new List<int>();

                var correctAnswerIds = question.Answers.Where(a => a.IsCorrect).Select(a => a.Id).ToList();
                var selectedCorrectAnswers = questionAnswerIds.Intersect(correctAnswerIds).ToList();

                var incorrectAnswerIds = question.Answers.Where(a => !a.IsCorrect).Select(a => a.Id).ToList();
                var selectedIncorrectAnswers = questionAnswerIds.Intersect(incorrectAnswerIds).ToList();

                if (question.Type == QuestionType.MultipleChoice)
                {
                    if (selectedIncorrectAnswers.Any())
                    {
                       continue;
                    }
                    else
                    {
                        score += question.Points;
                    }
                }
                else if (question.Type == QuestionType.TrueFalse || question.Type == QuestionType.SingleChoice)
                {
                    if (selectedCorrectAnswers.Any())
                    {
                        score += question.Points;
                    }
                }
            }

            string comment = "Final result";
            if (textAnswers != null && textAnswers.Count > 0)
            {
                comment = "Wait for teacher's points!";
            }

            // Create a new ExamResult object
            var result = new ExamResult
            {
                ExamId = examId,
                Score = score,
                Comment = comment,
                ApplicationUserId = userId,
                StudentAnswers = sub.StudentAnswers,
                SubmissionId = sub.Id
            };
   
            // Save the ExamResult object to the database
            _context.ExamResults.Add(result);
            await _context.SaveChangesAsync();

            return result;
        }







    }

}
