using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VIEWYTB
{
    public partial class Form1 : Form
    {
        ChromeOptions options = new ChromeOptions();
        ChromeDriverService service = null;
        public Form1()
        {
            InitializeComponent();
            addProxyToolStripMenuItem.Click += addProxyToolStripMenuItem_Click;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnStar_Click(object sender, EventArgs e)
        {
            // Tách danh sách proxy từ TextBox vào một mảng
            string[] proxies = txtProxy.Lines;

            // Lấy số lượng proxy được sử dụng từ TextBox txtSoLuong
            int soLuongProxy = int.Parse(txtSoLuong.Text);

            // Lấy số lượng tab từ TextBox txtSoGiay
            int soTab = int.Parse(txtSoGiay.Text);

            // Tạo danh sách các đối tượng ChromeDriver
            List<ChromeDriver> drivers = new List<ChromeDriver>();

            // Tạo các ChromeDriver mà không cần proxy nếu danh sách proxy rỗng hoặc không đủ số lượng
            if (proxies.Length == 0 || soLuongProxy > proxies.Length)
            {
                // Tạo số lượng ChromeDriver tương ứng với số luồng được chọn
                for (int i = 0; i < soLuongProxy; i++)
                {
                    ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                    service.HideCommandPromptWindow = true;

                    try
                    {
                        ChromeDriver driver = new ChromeDriver(service, options);

                        // Chỉnh kích thước của cửa sổ trình duyệt
                        driver.Manage().Window.Size = new Size(500, 300);
                        // Thêm ChromeDriver vào danh sách
                        drivers.Add(driver);
                    }
                    catch (Exception ex)
                    {
                        // Xử lý khi không thể tạo ChromeDriver, ví dụ: thông báo lỗi
                        MessageBox.Show($"Không thể tạo ChromeDriver: {ex.Message}");
                        return;
                    }

                    // Đợi một chút trước khi tạo ChromeDriver tiếp theo
                    Thread.Sleep(1000);
                }
            }
            else // Tạo các ChromeDriver với cấu hình proxy
            {
                // Lặp qua mỗi proxy và tạo một ChromeDriver với cấu hình proxy tương ứng
                for (int i = 0; i < soLuongProxy; i++)
                {
                    string proxy = proxies[i];

                    ChromeOptions options = new ChromeOptions();

                    // Thêm cấu hình proxy vào ChromeOptions
                    options.AddArgument("--proxy-server=" + proxy);

                    ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                    service.HideCommandPromptWindow = true;

                    try
                    {
                        ChromeDriver driver = new ChromeDriver(service, options);
                        driver.Manage().Window.Size = new Size(500, 300);
                        // Thêm ChromeDriver vào danh sách
                        drivers.Add(driver);
                    }
                    catch (Exception ex)
                    {
                        // Xử lý khi proxy không hoạt động, ví dụ: thông báo lỗi
                        MessageBox.Show($"Proxy {proxy} không hoạt động: {ex.Message}");
                    }

                    // Đợi một chút trước khi tạo ChromeDriver tiếp theo
                    Thread.Sleep(1000);
                }
            }

            int thoiGianMiliGiay = soTab * 1000;

            foreach (var driver in drivers)
            {
                // Thực hiện các hành động trên mỗi tab
                driver.Navigate().GoToUrl(txtURL.Text);

                // Thực hiện các hành động khác...
            }

            // Chờ trong khoảng thời gian đã chỉ định
            Thread.Sleep(thoiGianMiliGiay);

            // Sau khi đã chờ đủ thời gian, đóng tất cả các tab
            foreach (var driver in drivers)
            {
                // Tắt tab
                driver.Quit();
            }
        }

        private void addProxyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Mở hộp thoại chọn tệp
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Đọc nội dung của tệp đã chọn và thêm vào TextBox "txtProxy"
                string filePath = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(filePath);
                txtProxy.Lines = lines;
            }
        }
    }
}
