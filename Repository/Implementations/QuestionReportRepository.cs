﻿using IdealDiscuss.Context;
using IdealDiscuss.Entities;
using IdealDiscuss.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdealDiscuss.Repository.Implementations
{
    public class QuestionReportRepository : BaseRepository<QuestionReport> , IQuestionReportRepository
    {
        public QuestionReportRepository(IdealDiscussContext context) : base(context)
        {
        }

        public QuestionReport GetQuestionReport(string id)
        {
            var questionReport = _context.QuestionReports
                .Include(u => u.User)
                .Include(c => c.Question)
                .Include(crf => crf.QuestionReportFlags)
                .ThenInclude(f => f.Flag)
                .Where(cr => cr.Id.Equals(id))
                .FirstOrDefault();

            return questionReport;
        }

        public List<QuestionReport> GetQuestionReports(string questionId)
        {
            var questionWithReports = _context.QuestionReports
                        .Where(qr => qr.QuestionId.Equals(questionId))
                        .Include(qr => qr.User)
                        .Include(qr => qr.QuestionReportFlags)
                            .ThenInclude(f => f.Flag)
                        .ToList();

            return questionWithReports;
        }
    }
}
