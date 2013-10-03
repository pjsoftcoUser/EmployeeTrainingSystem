using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patterson.Domain.Entities;
using Patterson.Domain.Abstract;

namespace Patterson.Domain.Concrete
{
    public class EFTestRepository : ITestRepository
    {
        private EFDbContext context = new EFDbContext();
        public IQueryable<Test> Tests
        {
            get { return context.Tests; }
        }
        public IQueryable<Question> Questions
        {
            get { return context.Questions; }
        }
        public IQueryable<Answer> Answers
        {
            get { return context.Answers; }
        }
        public IQueryable<TestsInClass> TestsInClasses
        {
            get { return context.TestsInClasses; }
        }
        public IQueryable<Exam> Exams
        {
            get { return context.Exams; }
        }
        public IQueryable<ExamRecord> ExamRecords
        {
            get { return context.ExamRecords; }
        }
        public IQueryable<Class> Classes
        {
            get { return context.Classes; }
        }
        public Test CreateTest(string title, string ident, string username, int pass)
        {

            var newTest = new Test { Title = title, Identifier = ident, CreatedBy = username, CreatedOn = DateTime.Now, PassingScore = pass };
            context.Tests.Add(newTest);
            context.SaveChanges();
            return newTest;
        }
        public int AddTest(int tid, int cid)
        {
            var newTiC = new TestsInClass { ClassID = cid, TestID = tid ,Active = 0};
            context.TestsInClasses.Add(newTiC);
            var result = context.SaveChanges();
            return result;
        }

        public Question AddQuestion(int testID, string question)
        {
            var newQuestion = new Question { TestID = testID, QuestionText = question };
            context.Questions.Add(newQuestion);
            context.SaveChanges();
            return newQuestion;
        }
        public int AddAnswer(int QID, string Answer, string cstring)
        {
            int cint;
            if (cstring != "false")
            {
                cint = 1;
            }
            else
            {
                cint = 0;
            }

            var newAnswer = new Answer { QuestionID = QID, AnswerText = Answer, Correctness = cint };
            context.Answers.Add(newAnswer);
            var result = context.SaveChanges();
            return result;
        }
        public int EditTest(Test test)
        {
            context.Entry(test).State = System.Data.EntityState.Modified;
            var result = context.SaveChanges();
            return result;
        }
        public void EditQuestion(Question question)
        {
            context.Entry(question).State = System.Data.EntityState.Modified;
            context.SaveChanges();
        }
        public void EditAnswer(Answer answer)
        {
            context.Entry(answer).State = System.Data.EntityState.Modified;
            context.SaveChanges();
        }

        public void DeleteQuestion(Question question)
        {
            foreach (var answers in question.Answers.ToList())
            {
                DeleteAnswer(answers);
            }
            context.Questions.Remove(question);
        }
        public void DeleteAnswer(Answer answer)
        {
            context.Answers.Remove(answer);
        }
        public void SaveChanges()
        {
            context.SaveChanges();
        }
        public IQueryable<RosterEntry> RosterEntries
        {
            get { return context.Roster; }
        }

        public IQueryable<User> Users
        {
            get { return context.Users; }
        }

        public Exam AddExam(int RID, int TID, int score)
        {
            Exam newExam = new Exam { RosterID = RID, TestID = TID, score = score };
            context.Exams.Add(newExam);
            context.SaveChanges();
            return newExam;
        }
        public Exam AddExam(int RID, int TID)
        {
            Exam newExam = new Exam { RosterID = RID, TestID = TID, score = 0};
            context.Exams.Add(newExam);
            context.SaveChanges();
            return newExam;
        }
             public void SubmitExam(Exam exam)
             {
                 context.Entry(exam).State = System.Data.EntityState.Modified;
                 context.SaveChanges();
             }
             public void SubmitAnswer(int ExamID, int QuestionID, int answer)
             {
                 ExamRecord exRec = new ExamRecord { QuestionID = QuestionID, ExamID = ExamID, answer = answer };
                 context.ExamRecords.Add(exRec);
                 context.SaveChanges();
             }

             public void DropTest(TestsInClass TiC)
             {
                 context.TestsInClasses.Remove(TiC);
                 context.SaveChanges();
             }
             public void MakeActive(TestsInClass tic, int cid)
             {
                 var classID = tic.ClassID;
                 var curAct = context.TestsInClasses.FirstOrDefault(i => i.ClassID == classID && i.Active > 0);
                 curAct.Active = 0;
                 tic.Active = 1;
                 context.Entry(curAct).State = System.Data.EntityState.Modified;
                 context.Entry(tic).State = System.Data.EntityState.Modified;
                 context.SaveChanges();
            }
    }
}
