using System;
using System.Drawing;
using System.Windows.Forms;

namespace OtoparkProjesi
{
    public class AdminLoginForm : Form
    {
        private TextBox txtUser, txtPass;

        public AdminLoginForm()
        {
            this.Text = "Admin Girişi";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            Label lblBaslik = new Label { Text = "YÖNETİCİ PANELİ", Location = new Point(120, 20), Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            this.Controls.Add(lblBaslik);

            this.Controls.Add(new Label { Text = "Kullanıcı Adı:", Location = new Point(50, 60) });
            txtUser = new TextBox { Location = new Point(150, 60), Width = 150 };
            this.Controls.Add(txtUser);

            this.Controls.Add(new Label { Text = "Şifre:", Location = new Point(50, 100) });
            txtPass = new TextBox { Location = new Point(150, 100), Width = 150, PasswordChar = '*' };
            this.Controls.Add(txtPass);

            Button btnLogin = new Button { Text = "Giriş Yap", Location = new Point(150, 140), Width = 100, BackColor = Color.Navy, ForeColor = Color.White };
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (txtUser.Text == "admingiris" && txtPass.Text == "adminsifre")
            {
                this.Hide();
                // Giriş başarılıysa Admin Panelini aç
                new AdminPanelForm().ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Hatalı Giriş!");
            }
        }
    }
}