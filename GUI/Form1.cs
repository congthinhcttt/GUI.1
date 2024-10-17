using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class Form1 : Form
    {
        public readonly StudentService studentService = new StudentService();
        public readonly FacultyService facultyService = new FacultyService();
        public Form1()
        {
            InitializeComponent();
            this.btnAddPicture.Click += new System.EventHandler(this.btnAddPicture_Click);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvTableSV);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            listFacultys.Insert(0, new Faculty());
            this.cbbChuyenNganh.DataSource = listFacultys;
            this.cbbChuyenNganh.DisplayMember = "FacultyName";
            this.cbbChuyenNganh.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvTableSV.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvTableSV.Rows.Add();
                dgvTableSV.Rows[index].Cells[0].Value = item.StudentID;
                dgvTableSV.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                    dgvTableSV.Rows[index].Cells[2].Value =
                    item.Faculty.FacultyName;
                dgvTableSV.Rows[index].Cells[3].Value = item.AverageScore + "";
                if (item.MajorID != null)
                    dgvTableSV.Rows[index].Cells[4].Value = item.Major.Name + "";
                //ShowAvatar(item.Avatar);
            }
        }
        /*private void ShowAvatar(string ImageName)
        {
            if (string.IsNullOrEmpty(ImageName))
            {
                pictureAvatar.Image = null;
            }
            else
            {
                string parentDirectory =
                Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(parentDirectory, "Images",
                ImageName);
                pictureAvatar.Image = Image.FromFile(imagePath);
                pictureAvatar.Refresh();
            }
        }*/
        public void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle =
            DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void chkUnregisterMajor_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.chkUnregisterMajor.Checked)
                listStudents = studentService.GetAllHasNoMajor();
            else
                listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy thông tin từ các trường nhập liệu
                string studentID = txtMSSV.Text;
                string fullName = txtHoten.Text;
                double averageScore = double.Parse(txtDTB.Text);
                int facultyID = (int)cbbChuyenNganh.SelectedValue;
                string imagePath = pictureAvatar.Tag?.ToString(); // Lấy đường dẫn ảnh từ Tag

                // Tạo một đối tượng sinh viên mới
                var student = new Student
                {
                    StudentID = studentID,
                    FullName = fullName,
                    AverageScore = averageScore,
                    FacultyID = facultyID,
                    MajorID = null, // Nếu có ngành học, bạn có thể lấy giá trị từ combobox khác nếu có
                    AvatarPath = imagePath // Lưu đường dẫn ảnh
                };

                // Gọi service để thêm sinh viên
                studentService.InsertUpdate(student);

                // Cập nhật lại danh sách sinh viên trên DataGridView
                BindGrid(studentService.GetAll());

                // Thông báo thành công
                MessageBox.Show("Thêm sinh viên thành công!");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu có dòng được chọn trong DataGridView
                if (dgvTableSV.SelectedRows.Count > 0)
                {
                    // Lấy sinh viên hiện tại từ DataGridView
                    var selectedRow = dgvTableSV.SelectedRows[0];
                    var studentID = selectedRow.Cells[0].Value.ToString();

                    // Tìm sinh viên theo StudentID
                    var student = studentService.FindById(studentID);

                    if (student != null)
                    {
                        // Cập nhật các thông tin sinh viên từ giao diện
                        student.FullName = txtHoten.Text;             // Cập nhật tên
                        student.AverageScore = double.Parse(txtDTB.Text);  // Cập nhật điểm trung bình
                        student.FacultyID = (int)cbbChuyenNganh.SelectedValue;  // Cập nhật khoa từ ComboBox
                        student.MajorID = null; // Bạn có thể lấy MajorID từ một ComboBox khác nếu có

                        // Gọi phương thức InsertUpdate() để cập nhật thông tin sinh viên
                        studentService.InsertUpdate(student);

                        // Cập nhật lại danh sách sinh viên trên DataGridView
                        BindGrid(studentService.GetAll());

                        // Thông báo thành công
                        MessageBox.Show("Cập nhật sinh viên thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên để sửa.");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để sửa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvTableSV.SelectedRows.Count > 0)
                {
                    var selectedRow = dgvTableSV.SelectedRows[0];
                    var studentID = selectedRow.Cells[0].Value.ToString();

                    // Tìm sinh viên theo StudentID
                    var student = studentService.FindById(studentID);

                    if (student != null)
                    {
                        var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            // Gọi phương thức Remove từ đối tượng studentService
                            studentService.Remove(student); // Đảm bảo gọi từ đối tượng đã khởi tạo
                            BindGrid(studentService.GetAll());
                            MessageBox.Show("Xóa sinh viên thành công!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên để xóa.");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để xóa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnAddPicture_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureAvatar.Image = Image.FromFile(openFileDialog.FileName);
                pictureAvatar.Tag = openFileDialog.FileName; // Lưu đường dẫn ảnh trong Tag
                pictureAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
        private void SaveImagePathToDatabase(string imagePath)
        {
            string connectionString = "DESKTOP-BR9PJB9\\SQLEXPRESS";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Students (AvatarPath) VALUES (@AvatarPath)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AvatarPath", imagePath);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
