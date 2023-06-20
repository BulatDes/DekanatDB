using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Dekanat_DB
{
    public partial class Form1 :Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source=DespizhekPc;Initial Catalog=Деканат;Integrated Security=True");
        SqlDataAdapter adptr;
        DataTable table;
        public Form1 ()
        {
            InitializeComponent( );
        }

        private void button1_Click (object sender, EventArgs e)
        {
            int kod, kurs, countst, hours;
            DateTime startses, endses;
            DateTime datenow = DateTime.Now;
            kod = (int)numericUpDown1.Value;
            kurs = (int) numericUpDown2.Value;
            countst = (int) numericUpDown3.Value;
            hours = (int) numericUpDown4.Value;
            startses = StartSession.Value;
            endses = EndSession.Value;
            connect.Open( );
            SqlCommand comm = new SqlCommand($"select count(*) from dbo.Группы where [Код группы] = {kod}", connect);
            if ( (int) comm.ExecuteScalar( ) == 0 )
            {
                if(startses<datenow && endses < startses )
                {
                    MessageBox.Show("Значение дат неправильно.\nИсправьте  пожалуйста", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    connect.Close( );
                    return;
                }
                string query = $"INSERT INTO dbo.Группы   ([Код группы],  [Курс],  [Количество студентов],  [Объём часов],  [Начало сессии], [Окончание сессии])     VALUES    ({kod} ,{kurs}  ,{countst},{hours},'{startses}' ,'{endses}')";
                SqlCommand command = new SqlCommand(query, connect);
                command.ExecuteNonQuery( );
                MessageBox.Show("Группа добавлена", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Такая группа уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connect.Close( );
            ReloadGroup();
            ComboboxLoad();
        }

        private void Form1_Load (object sender, EventArgs e)
        {
            ReloadGroup();
            ComboboxLoad();
            ReloadDiscip();
            ReloadPrepod();
            ReloadControl();
        }
        public void ReloadGroup ()
        {
            connect.Open( );
            adptr = new SqlDataAdapter("select * from Группы", connect);
            table = new DataTable( );
            adptr.Fill(table);
            dataGridView1.DataSource = table;
            connect.Close( );
        }
        public void ReloadDiscip () {
            connect.Open();
            adptr = new SqlDataAdapter("select * from Дисциплины", connect);
            table = new DataTable();
            adptr.Fill(table);
            dataGridView2.DataSource = table;
            connect.Close();
        }
        public void ReloadPrepod () {
            connect.Open();
            adptr = new SqlDataAdapter("select * from Преподаватель", connect);
            table = new DataTable();
            adptr.Fill(table);
            dataGridView3.DataSource = table;
            connect.Close();
        }
        public void ReloadControl () {
            connect.Open();
            adptr = new SqlDataAdapter("select * from Контроль", connect);
            table = new DataTable();
            adptr.Fill(table);
            dataGridView4.DataSource = table;
            connect.Close();
        }

        private void label7_Click (object sender, EventArgs e) {

        }
        public void ComboboxLoad () {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            connect.Open();
            adptr = new SqlDataAdapter("select distinct Дисциплины.Категория from Дисциплины", connect);
            table = new DataTable();
            adptr.Fill(table);
            foreach (DataRow r in table.Rows) {
                foreach (var cell in r.ItemArray)
                    comboBox1.Items.Add(cell);
            }

            adptr = new SqlDataAdapter("select distinct Дисциплины.Код from Дисциплины", connect);
            table = new DataTable();
            adptr.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                foreach (var cell in r.ItemArray)
                    comboBox4.Items.Add(cell);
            }

            adptr = new SqlDataAdapter("select distinct Группы.[Код группы] from Группы", connect);
            table = new DataTable();
            adptr.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                foreach (var cell in r.ItemArray)
                    comboBox2.Items.Add(cell);
            }

            adptr = new SqlDataAdapter("select distinct Преподаватель.Код from Преподаватель", connect);
            table = new DataTable();
            adptr.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                foreach (var cell in r.ItemArray)
                    comboBox3.Items.Add(cell);
            }
            connect.Close();
        }

        private void button2_Click (object sender, EventArgs e) {
            int kod, hours;
            string kategoria, name;
            kod = (int) numericUpDown8.Value;
            kategoria = comboBox1.Text;
            name = textBox1.Text;
            hours = (int) numericUpDown5.Value;
            if(kategoria == "" || name == "") {
                MessageBox.Show("Не все поля ввода данных заполнены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            connect.Open();
            SqlCommand comm = new SqlCommand($"select count(*) from dbo.Дисциплины where [Код] = {kod}", connect);
            if ((int) comm.ExecuteScalar() == 0) {
                string query = $"INSERT INTO dbo.Дисциплины   (Код,  Название,  Категория,  [Объём часов])     VALUES    ({kod} ,'{name}' ,'{kategoria}',{hours})";
                SqlCommand command = new SqlCommand(query, connect);
                command.ExecuteNonQuery();
                MessageBox.Show("Дисциплина добавлена", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("Такой код дисциплины уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connect.Close();
            ReloadDiscip();
            ComboboxLoad();
        }

        private void button3_Click (object sender, EventArgs e) {
            int kod;
            string name;
            kod = (int) numericUpDown6.Value;
            name = textBox2.Text;
            if (name == "") {
                MessageBox.Show("Не все поля ввода данных заполнены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string[] checkname = name.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
            if(checkname.Length != 3) {
                MessageBox.Show("Пожалуйста введите ФИО правильно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            connect.Open();
            SqlCommand comm = new SqlCommand($"select count(*) from Преподаватель where [Код] = {kod}", connect);
            if ((int) comm.ExecuteScalar() == 0) {
                string query = $"INSERT INTO Преподаватель   (Код,  ФИО)     VALUES    ({kod} ,'{name}')";
                SqlCommand command = new SqlCommand(query, connect);
                command.ExecuteNonQuery();
                MessageBox.Show("Преподаватель добавлен", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                MessageBox.Show("Такой код преподавателя уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connect.Close();
            ReloadPrepod();
            ComboboxLoad();
        }

        private void comboBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int kod, group, discip, prepod;
            string vid;
            DateTime date;
            DateTime datenow = DateTime.Now;
            if(comboBox2.Text=="" || comboBox3.Text==""|| comboBox4.Text=="")
            {
                MessageBox.Show("Выберите данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            kod = (int)numericUpDown11.Value;
            group = int.Parse(comboBox2.Text);
            discip = int.Parse(comboBox3.Text);
            prepod = int.Parse(comboBox4.Text);
            date = dateZapis.Value;
            vid = comboBox5.Text;
            connect.Open();
            SqlCommand comm = new SqlCommand($"select count(*) from dbo.Контроль where [Код записи] = {kod}", connect);
            if ((int)comm.ExecuteScalar() == 0)
            {
                if (date < datenow)
                {
                    MessageBox.Show("Значение дат неправильно.\nИсправьте  пожалуйста", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    connect.Close();
                    return;
                }
                string query = $"INSERT INTO dbo.Контроль   ([Код записи],  [Группа],  [Преподаватель],  [Дисциплина],  [Вид контроля], [Дата])     VALUES    ({kod} ,{group}  ,{discip},{prepod},'{date}' ,'{vid}')";
                SqlCommand command = new SqlCommand(query, connect);
                command.ExecuteNonQuery();
                MessageBox.Show("Запись добавлена", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Такая запись уже существует\nМожет вы хотели отредактировать", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connect.Close();
            ReloadControl();
            ComboboxLoad();
        }

        private void comboBox2_KeyUp(object sender, KeyEventArgs e)
        {
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox4.Text = "";
            comboBox5.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int kod, kurs, countst, hours;
            DateTime startses, endses;
            DateTime datenow = DateTime.Now;
            kod = (int)numericUpDown1.Value;
            kurs = (int)numericUpDown2.Value;
            countst = (int)numericUpDown3.Value;
            hours = (int)numericUpDown4.Value;
            startses = StartSession.Value;
            endses = EndSession.Value;
            connect.Open();
            SqlCommand comm = new SqlCommand($"select count(*) from dbo.Группы where [Код группы] = {kod}", connect);
            if ((int)comm.ExecuteScalar() == 0)
            {
                MessageBox.Show("Может вы хотели Добавить такую группу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (startses < datenow && endses < startses)
                {
                    MessageBox.Show("Значение дат неправильно.\nИсправьте  пожалуйста", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    connect.Close();
                    return;
                }
                string query = $"Update Группы set [Количество студентов]= {countst} where [Код группы]= {kod}";
                string query1 = $"Update Группы set [Объём часов]= {hours} where [Код группы]={kod}";
                string query2 = $"Update Группы set Курс= {kurs} where [Код группы]= {kod}";
                string query3 = $"Update Группы set [Начало сессии]= '{startses}' where [Код группы]= {kod}";
                string query4 = $"Update Группы set [Окончание сессии]= '{endses}' where [Код группы]= {kod}";
                SqlCommand command = new SqlCommand(query, connect);
                SqlCommand command1 = new SqlCommand(query1, connect);
                SqlCommand command2 = new SqlCommand(query2, connect);
                SqlCommand command3 = new SqlCommand(query3, connect);
                SqlCommand command4 = new SqlCommand(query4, connect);
                command.ExecuteNonQuery();
                command1.ExecuteNonQuery();
                command2.ExecuteNonQuery();
                command3.ExecuteNonQuery();
                command4.ExecuteNonQuery();
                MessageBox.Show("Группа изменена", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            connect.Close();
            ReloadGroup();
            ComboboxLoad();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int kod, group, discip, prepod;
            string vid;
            DateTime date;
            DateTime datenow = DateTime.Now;
            if (comboBox2.Text == "" || comboBox3.Text == "" || comboBox4.Text == "")
            {
                MessageBox.Show("Выберите данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            kod = (int)numericUpDown11.Value;
            group = int.Parse(comboBox2.Text);
            discip = int.Parse(comboBox3.Text);
            prepod = int.Parse(comboBox4.Text);
            date = dateZapis.Value;
            vid = comboBox5.Text;
            connect.Open();
            SqlCommand comm = new SqlCommand($"select count(*) from dbo.Контроль where [Код записи] = {kod}", connect);
            if ((int)comm.ExecuteScalar() != 0)
            {
                if (date < datenow)
                {
                    MessageBox.Show("Значение дат неправильно.\nИсправьте  пожалуйста", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    connect.Close();
                    return;
                }
                string query = $"update Контроль set Группа = {group} where [Код записи]= {kod}";
                string query1 = $"update Контроль set Преподаватель = '{prepod}' where [Код записи]= {kod}";
                string query2 = $"update Контроль set Дисциплина = {discip} where [Код записи]= {kod}";
                string query3 = $"update Контроль set [Вид контроля] = '{vid}' where [Код записи]= {kod}";
                string query4 = $"update Контроль set Дата = '{date}' where [Код записи]= {kod}";
                SqlCommand command = new SqlCommand(query, connect);
                SqlCommand command1= new SqlCommand(query1, connect);
                SqlCommand command2= new SqlCommand(query2, connect);
                SqlCommand command3 = new SqlCommand(query3, connect);
                SqlCommand command4 = new SqlCommand(query4, connect);
                command.ExecuteNonQuery();
                command1.ExecuteNonQuery();
                command2.ExecuteNonQuery();
                command3.ExecuteNonQuery();
                command4.ExecuteNonQuery();
                MessageBox.Show("Запись изменена", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Такая не существует\nМожет вы хотели отредактировать", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connect.Close();
            ReloadControl();
            ComboboxLoad();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int kod, hours;
            string kategoria, name;
            kod = (int)numericUpDown8.Value;
            kategoria = comboBox1.Text;
            name = textBox1.Text;
            hours = (int)numericUpDown5.Value;
            if (kategoria == "" || name == "")
            {
                MessageBox.Show("Не все поля ввода данных заполнены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            connect.Open();
            SqlCommand comm = new SqlCommand($"select count(*) from dbo.Дисциплины where [Код] = {kod}", connect);
            if ((int)comm.ExecuteScalar() != 0)
            {
                string query = $"update Дисциплины set Название= '{name}' where Код = {kod}";
                string query1 = $"update Дисциплины set Категория= '{kategoria}' where Код = {kod}";
                string query2 = $"update Дисциплины set [Объём часов]= {hours} where Код = {kod}";
                SqlCommand command = new SqlCommand(query, connect);
                SqlCommand command1 = new SqlCommand(query1, connect);
                SqlCommand command2 = new SqlCommand(query2, connect);
                command.ExecuteNonQuery();
                command1.ExecuteNonQuery();
                command2.ExecuteNonQuery();
                MessageBox.Show("Дисциплина изменена", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Такой код дисциплины не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connect.Close();
            ReloadDiscip();
            ComboboxLoad();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int kod;
            string name;
            kod = (int)numericUpDown6.Value;
            name = textBox2.Text;
            if (name == "")
            {
                MessageBox.Show("Не все поля ввода данных заполнены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string[] checkname = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (checkname.Length != 3)
            {
                MessageBox.Show("Пожалуйста введите ФИО правильно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            connect.Open();
            SqlCommand comm = new SqlCommand($"select count(*) from Преподаватель where [Код] = {kod}", connect);
            if ((int)comm.ExecuteScalar() != 0)
            {
                string query = $"update Преподаватель set ФИО = '{name}' where Код = {kod}";
                SqlCommand command = new SqlCommand(query, connect);
                command.ExecuteNonQuery();
                MessageBox.Show("Преподаватель изменен", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Такой код преподавателя не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            connect.Close();
            ReloadPrepod();
            ComboboxLoad();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (comboBox6.Text == "")
            {
                MessageBox.Show("Выберите тип поиска", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(comboBox6.Text == "Поиск по Преподавателю")
            {
                string query = $"select * from Контроль \r\ninner join Преподаватель on Преподаватель.Код = Контроль.Преподаватель\r\nwhere Преподаватель.ФИО = '{comboBox7.Text}'";
                ReloadSearch(query);
            }
            else if(comboBox6.Text == "Поиск по Дисциплине")
            {
                string query = $"select * from Контроль inner join Дисциплины on Дисциплины.Код = Контроль.Дисциплина where Дисциплины.Название = '{comboBox7.Text}'";
                ReloadSearch(query);
            }
        }

        private void comboBox7_KeyUp(object sender, KeyEventArgs e)
        {
            comboBox6.Text = "";
            comboBox7.Text = "";
            comboBox7.Items.Clear();

        }
        private void ComboBoxPrepodLoad()
        {
            comboBox7.Items.Clear();
            connect.Open();
            adptr = new SqlDataAdapter("select distinct Преподаватель.ФИО from Преподаватель", connect);
            table = new DataTable();
            adptr.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                foreach (var cell in r.ItemArray)
                    comboBox7.Items.Add(cell);
            }
            connect.Close();
        }
        private void ComboBoxDiscipLoad()
        {
            comboBox7.Items.Clear();
            connect.Open();
            adptr = new SqlDataAdapter("select distinct Дисциплины.Название from Дисциплины", connect);
            table = new DataTable();
            adptr.Fill(table);
            foreach (DataRow r in table.Rows)
            {
                foreach (var cell in r.ItemArray)
                    comboBox7.Items.Add(cell);
            }
            connect.Close();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox7.Text = "";
            if (comboBox6.Text == "Поиск по Преподавателю")
            {
                ComboBoxPrepodLoad();
            }
            else if (comboBox6.Text == "Поиск по Дисциплине")
            {
                ComboBoxDiscipLoad();
            }
        }
        public void ReloadSearch( string query)
        {
            connect.Open();
            adptr = new SqlDataAdapter(query, connect);
            table = new DataTable();
            adptr.Fill(table);
            dataGridView5.DataSource = table;
            connect.Close();
        }
        
    }
}
