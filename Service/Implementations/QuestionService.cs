using IdealDiscuss.Dtos;
using IdealDiscuss.Dtos.QuestionDto;
using IdealDiscuss.Entities;
using IdealDiscuss.Repository.Interfaces;
using IdealDiscuss.Service.Interface;
using System.Security.Claims;

namespace IdealDiscuss.Service.Implementations
{
    public class QuestionService : IQuestionService
    {
        private readonly IUserRepository _userRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionService(
            IQuestionRepository questionRepository, 
            IUserRepository userRepository, 
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _questionRepository = questionRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public BaseResponseModel Create(CreateQuestionDto createQuestionDto)
        {
            var response = new BaseResponseModel();
            var createdBy = _httpContextAccessor.HttpContext.User.Identity.Name;
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var userId = int.Parse(userIdClaim);
            var user = _userRepository.Get(userId);

            if (string.IsNullOrWhiteSpace(createQuestionDto.QuestionText))
            {
                response.Message = "Question text is required!";
                return response;
            }

            if (createQuestionDto.QuestionText.Length < 20 || createQuestionDto.QuestionText.Length > 150)
            {
                response.Message = "Question text can only be between 20 - 150 characters";
                return response;
            }

            var question = new Question
            {
                UserId = user.Id,
                QuestionText = createQuestionDto.QuestionText,
                ImageUrl = createQuestionDto.ImageUrl,
                CreatedBy = createdBy,
                DateCreated = DateTime.Now
            };

            try
            {
                _questionRepository.Create(question);
            }
            catch (Exception ex)
            {
                response.Message = $"Failed to create question: {ex.InnerException}";
                return response;
            }

            response.Status = true;
            response.Message = "Question created successfully!";

            return response;
        }

        public BaseResponseModel Update(int questionId, UpdateQuestionDto updateQuestionDto)
        {
            var response = new BaseResponseModel();
            var modifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
            var questionExist = _questionRepository.Exists(c => c.Id == questionId);

            if (!questionExist)
            {
                response.Message = "Question does not exist!";
                return response;
            }

            var question = _questionRepository.Get(questionId);

            question.QuestionText = updateQuestionDto.QuestionText;
            question.ModifiedBy = modifiedBy;
            question.LastModified = DateTime.Now;

            try
            {
                _questionRepository.Update(question);
            }
            catch (Exception ex)
            {
                response.Message = $"Could not update the Question: {ex.Message}";
                return response;
            }
            response.Message = "Question updated successfully!";
            return response;
        }

        public BaseResponseModel Delete(int questionId)
        {
            var response = new BaseResponseModel();

            var questionExist = _questionRepository.Exists(c => c.Id == questionId);

            if (!questionExist)
            {
                response.Message = "Question does not exist!";
                return response;
            }

            var question = _questionRepository.Get(questionId);

            question.IsDeleted = true;

            try
            {
                _questionRepository.Update(question);
            }
            catch (Exception ex)
            {
                response.Message = $"Question delete failed: {ex.Message}";
                return response;
            }

            response.Status = true;
            response.Message = "Question deleted successfully!";
            return response;
        }

        public QuestionsResponseModel GetAllQuestion()
        {
            var response = new QuestionsResponseModel();

            try
            {
                var questions = _questionRepository.GetQuestions(); 

                if(questions.Count == 0)
                {
                    response.Message = "No question found!";
                    return response;
                }

                response.questions = questions
                    .Where(q => q.IsClosed == false && q.IsDeleted == false)
                    .Select(question => new ViewQuestionDto
                    {
                        Id = question.Id,
                        QuestionText = question.QuestionText,
                        UserName = question.User.UserName,
                        ImageUrl = question.ImageUrl,
                    }).ToList();

                response.Status = true;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Message = $"An error occured: {ex.StackTrace}";
                return response;
            }

            return response;
        }

        public QuestionResponseModel GetQuestion(int questionId)
        {
            var response = new QuestionResponseModel();

            if (!_questionRepository.Exists(c => c.Id == questionId))
            {
                response.Message = $"Question with id {questionId} does not exist!";
                return response;
            }
            var question = _questionRepository.GetQuestion(questionId);

            response.Message = "Success";
            response.Status = true;
            response.question = new ViewQuestionDto
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                UserName = question.User.UserName,
                ImageUrl = question.ImageUrl
            };

            return response;
        }
    }
}