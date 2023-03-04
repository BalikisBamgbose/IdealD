﻿using IdealDiscuss.Context;
using IdealDiscuss.Entities;
using IdealDiscuss.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdealDiscuss.Repository.Implementations
{
    public class CommentReportRepository : BaseRepository<CommentReport> , ICommentReportRepository
    {
        public CommentReportRepository(IdealDiscussContext context) 
        { 
            _context = context;
        }

        public CommentReport GetComment(int id)
        {
            var commentReport = _context.CommentReports.Include(c => c.User).Include(c => c.Comment).Include(c => c.CommentReportFlags).SingleOrDefault();

            return commentReport;
        }

        public List<CommentReport> GetCommentReports()
        {
            var commentReports =_context.CommentReports.Include(c => c.User).Include(c => c.Comment).Include(c => c.CommentReportFlags).ToList();

            return commentReports;
        }


    }
}
