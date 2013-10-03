using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patterson.Domain.Entities;

namespace Patterson.Domain.Abstract
{
    public interface ITestRepository
    {
        IQueryable<User> Users { get; }
        IQueryable<RosterEntry> RosterEntries { get; }
        IQueryable<Test> Tests { get; }
        IQueryable<Question> Questions { get; }
        IQueryable<Answer> Answers { get; }
        IQueryable<TestsInClass> TestsInClasses { get; }
        IQueryable<Exam> Exams { get; }
        IQueryable<ExamRecord> ExamRecords { get; }
        IQueryable<Class> Classes { get; }
        Test CreateTest(string title, string ident, string username, int pass);
        int AddTest(int tid, int cid);
        Question AddQuestion(int testID, string question);
        int AddAnswer(int QID, string Answer, string cstring);
        int EditTest(Test test);
        void EditQuestion(Question question);
        void EditAnswer(Answer answer);
        void DeleteQuestion(Question question);
        void DeleteAnswer(Answer answer);
        Exam AddExam(int RID, int TID, int score);
        Exam AddExam(int RID, int TID);
        void SubmitExam(Exam exam);
        void SubmitAnswer(int ExamID, int QuestionID, int answer);
        void DropTest(TestsInClass TiC);
        void MakeActive(TestsInClass tic, int cid);
    }
}
