using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class FacultyService
    {
        public List<Faculty> GetAll()
        {
            ModelSinhVienDB modelSinhVienDB = new ModelSinhVienDB();
            return modelSinhVienDB.Faculties.ToList();
        }
    }
}
