using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //Grid Select
            BindGrid();
            //Row Changed Event
            dataGrid1.SelectedCellsChanged += dataGrid1_SelectedCellsChanged;
            //Init Bind Controls
            InitGridControls();
        }

        //Bind the Grid  
        XDocument xmldoc;
        //Path
        //C:\Users\J\Documents\GitHub\XmlCRUD\WpfApplication1\WpfApplication1\Xml\Emp12.xml
        string _path = string.Format("{0}{1}", Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, @"\Xml\Emp12.xml");
        //WPF Parttern
        //string path = AppDomain.CurrentDomain.BaseDirectory;

        public void BindGrid()
        {
            xmldoc = XDocument.Load(_path);   //add xml document  
            var bind = xmldoc.Descendants("Employee").Select(p => new
            {
                Id = p.Element("id").Value,
                Name = p.Element("name").Value,
                Salary = p.Element("salary").Value,
                Email = p.Element("email").Value,
                Address = p.Element("address").Value
            }).OrderBy(p => p.Id);
            dataGrid1.ItemsSource = bind;
        }

        void dataGrid1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (UIElement element in stackPanel1.Children)
            {
                if (element.GetType().Equals(typeof(TextBox)))
                {
                    foreach (var item in e.AddedCells)
                    {
                        var col = item.Column as DataGridColumn;

                        if ((element as TextBox).Name.Equals(col.Header.ToString().ToLower()))
                        {
                            var fc = col.GetCellContent(item.Item);

                            if (fc is CheckBox)
                            {
                                Console.WriteLine((fc as CheckBox).IsChecked);
                            }
                            else if (fc is TextBlock)
                            {
                                (element as TextBox).Text = (fc as TextBlock).Text;
                                //Console.WriteLine((fc as TextBlock).Text);
                            }
                            //// Like this for all available types of cells
                        }
                    }
                }
            }

        }

        private void InitGridControls()
        {
            foreach(DataGridColumn col in dataGrid1.Columns)
            {
                Label lbl = new Label();
                lbl.Content = col.Header.ToString();
                TextBox txb = new TextBox();
                txb.Name = col.Header.ToString().ToLower();
                txb.TextChanged += txb_TextChanged;
                stackPanel1.Children.Add(lbl);
                stackPanel1.Children.Add(txb);

                stackPanel1.Height += lbl.Height += txb.Height;
            }

            Thickness thick = new Thickness(10, 10, 0, 0);

            StackPanel btnPanel = new StackPanel();
            btnPanel.Orientation = Orientation.Horizontal;
            btnPanel.Margin = thick;
            btnPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            
            Button btn = new Button();
            btn.Content = "추가";
            btn.Margin = thick;
            btn.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            btn.Click += btn_Click;
            btnPanel.Children.Add(btn);

            Button btn2 = new Button();
            btn2.Content = "수정";
            btn2.Margin = thick;
            btn2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            btn2.Click += btn_Click;
            btnPanel.Children.Add(btn2);

            Button btn3 = new Button();
            btn3.Content = "삭제";
            btn3.Margin = thick;
            btn3.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            btn3.Click += btn_Click;
            btnPanel.Children.Add(btn3);

            stackPanel1.Height += btnPanel.Height;
            stackPanel1.Children.Add(btnPanel);
        }

        void txb_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MessageBox.Show(string.Format("{0}:{1}", (sender as TextBox).Name, (sender as TextBox).Text));
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Content.Equals("추가"))
            {
                Insert();
                Reset();
            }
            if ((sender as Button).Content.Equals("수정"))
            {
                Update("id");
            }
            if ((sender as Button).Content.Equals("삭제"))
            {
                Delete("id");
                Reset();
            }
        }

        void Reset()
        {
            foreach (UIElement element in stackPanel1.Children)
            {
                if (element.GetType().Equals(typeof(TextBox)))
                {
                    (element as TextBox).Text = string.Empty;
                }
            }
        }

        void Update(string key)
        {
            string keyValue = string.Empty;
            foreach (UIElement element in stackPanel1.Children)
            {
                if (element.GetType().Equals(typeof(TextBox)))
                {
                    if (key.Equals((element as TextBox).Name))
                    {
                        keyValue = (element as TextBox).Text;
                        break;
                    }
                }
            }

            XElement emp = xmldoc.Descendants("Employee").FirstOrDefault(p => p.Element(key).Value == keyValue);

            foreach (UIElement element in stackPanel1.Children)
            {
                if (element.GetType().Equals(typeof(TextBox)))
                {
                    if (emp != null)
                    {
                        if (key.Equals((element as TextBox).Name))
                            continue;

                        emp.Element((element as TextBox).Name).Value = (element as TextBox).Text;
                    }
                }
            }

            if (emp != null)
            {
                xmldoc.Save(_path);
                BindGrid();
            }
        }

        void Insert()
        {
            XElement emp = new XElement("Employee");

            foreach (UIElement element in stackPanel1.Children)
            {
                if (element.GetType().Equals(typeof(TextBox)))
                {
                    emp.Add(new XElement((element as TextBox).Name, (element as TextBox).Text));
                }
            }

            xmldoc.Root.Add(emp);
            xmldoc.Save(_path);
            BindGrid();
        }

        void Delete(string key)
        {
            string keyValue = string.Empty;
            foreach (UIElement element in stackPanel1.Children)
            {
                if (element.GetType().Equals(typeof(TextBox)))
                {
                    if ((element as TextBox).Name.Equals(key))
                    {
                        keyValue = (element as TextBox).Text;
                        break;
                    }
                }
            }

            XElement emp = xmldoc.Descendants("Employee").FirstOrDefault(p => p.Element(key).Value == keyValue);
            if (emp != null)
            {
                emp.Remove();
                xmldoc.Save(_path);
                BindGrid();
            }
        }

        protected void Insert_Click(object sender, EventArgs e)
        {
            //XElement emp = new XElement("Employee",
            //    new XElement("id", txtid.Text),
            //    new XElement("name", txtname.Text),
            //    new XElement("salary", txtsalary.Text),
            //    new XElement("email", txtemail.Text),
            //    new XElement("address", txtaddress.Text));
            //xmldoc.Root.Add(emp);
            //xmldoc.Save("D:/Emp12.xml");
            //BindGrid();
            //Reset(); // For clear textbox  

        }

        protected void Find(object sender, EventArgs e)
        {
            //XElement emp = xmldoc.Descendants("Employee").FirstOrDefault(p => p.Element("id").Value == txtid.Text);
            //if (emp != null)
            //{
            //    txtname.Text = emp.Element("name").Value;
            //    txtsalary.Text = emp.Element("salary").Value;
            //    txtemail.Text = emp.Element("email").Value;
            //    txtaddress.Text = emp.Element("address").Value;

            //}
        }

        protected void Update_click(object sender, EventArgs e)
        {

            //XElement emp = xmldoc.Descendants("Employee").FirstOrDefault(p => p.Element("id").Value == txtid.Text);
            //if (emp != null)
            //{
            //    emp.Element("name").Value = txtname.Text;
            //    emp.Element("salary").Value = txtsalary.Text;
            //    emp.Element("email").Value = txtemail.Text;
            //    emp.Element("address").Value = txtaddress.Text;
            //    xmldoc.Root.Add(emp);
            //    xmldoc.Save(_path);
            //    BindGrid();
            //    Reset();

            //}
        }

        protected void delete_click(object sender, EventArgs e)
        {
            //XElement emp = xmldoc.Descendants("Employee").FirstOrDefault(p => p.Element("id").Value == txtid.Text);
            //if (emp != null)
            //{
            //    emp.Remove();
            //    xmldoc.Save(_path);
            //    BindGrid();
            //    Reset();
            //}
        }



    }
}
