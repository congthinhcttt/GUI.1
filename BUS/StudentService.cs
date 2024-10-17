using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class StudentService
    {
        ModelSinhVienDB ModelSinhVienDB = new ModelSinhVienDB();

        public List<Student> GetAll()
        {
            return ModelSinhVienDB.Students.ToList();
        }
        public List<Student> GetAllHasNoMajor()
        {
            return ModelSinhVienDB.Students.Where(p => p.MajorID == null).ToList();
        }
        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            return ModelSinhVienDB.Students.Where(p => p.MajorID == null && p.FacultyID == facultyID).ToList();
        }
        public Student FindById(string studentID)
        {
            return ModelSinhVienDB.Students.FirstOrDefault(p => p.StudentID == studentID);
        }
        public void InsertUpdate(Student s)
        {
            ModelSinhVienDB.Students.AddOrUpdate(s);
            ModelSinhVienDB.SaveChanges();

        }
        public void Remove(Student student)
        {
            // Tìm sinh viên trong cơ sở dữ liệu trước khi xóa
            var studentToRemove = ModelSinhVienDB.Students.FirstOrDefault(s => s.StudentID == student.StudentID);
            if (studentToRemove != null)
            {
                ModelSinhVienDB.Students.Remove(studentToRemove);
                ModelSinhVienDB.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Sinh viên không tồn tại.");
            }
        }
    }
}
