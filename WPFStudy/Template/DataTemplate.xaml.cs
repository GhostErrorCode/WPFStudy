using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFStudy.Template
{
    /// <summary>
    /// DataTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class DataTemplate : Window
    {
        public DataTemplate()
        {
            InitializeComponent();

            // 创建一个学生List集合（ListBox)
            List<StudentList> studentList = new List<StudentList>()
            {
            new StudentList() { Name = "张三", Age = 18, Grade = "高三" },
            new StudentList() { Name = "李四", Age = 17, Grade = "高二" },
            new StudentList() { Name = "王五", Age = 16, Grade = "高一" }
            };

            // 创建一个学生List集合(DataGrid)
            List<StudentDataGrid> studentDataGrid = new List<StudentDataGrid>()
            {
            new StudentDataGrid() { Name = "张三", EnglishName = "John", Age = 18, Grade = "高三", Score = 85 },
            new StudentDataGrid() { Name = "李四", EnglishName = "Mike", Age = 17, Grade = "高二", Score = 92 },
            new StudentDataGrid() { Name = "王五", EnglishName = "David", Age = 16, Grade = "高一", Score = 78 }
            };

            // 存储到数据上下文
            this.DataContext = new
            {
                StudentLists = studentList,
                StudentDataGrids = studentDataGrid
            };
            // new { Students = students } → 匿名类的定义 + 实例化
            // 这种写法的核心是：没有提前定义类，通过 new { 属性 = 值 } 让编译器「动态生成一个匿名类」，同时创建这个类的实例。

        }
    }

    // 学生类(ListBox源数据)
    public class StudentList
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Grade { get; set; }
    }

    // 学生类(DataGrid源数据)
    public class StudentDataGrid
    {
        public string? Name { get; set; }
        public string? EnglishName { get; set; }
        public int Age { get; set; }
        public string? Grade { get; set; }
        public int Score { get; set; } // 成绩分数
    }
}
