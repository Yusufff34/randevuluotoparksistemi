using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OtoparkProjesi
{
    public class AdminPanelForm : Form
    {
        private DataGridView dgv;
        private Label lblGunlukCiro, lblAylikCiro, lblGunlukMusteri, lblAylikMusteri;

        private Color clrBackground = Color.FromArgb(30, 30, 45);
        private Color clrPanel = Color.FromArgb(45, 45, 60);
        private Color clrHeader = Color.FromArgb(50, 50, 70);
        private Color clrText = Color.FromArgb(240, 240, 240);
        private Color clrAccentGreen = Color.FromArgb(39, 174, 96);
        private Color clrAccentGold = Color.FromArgb(243, 156, 18);
        private Color clrAccentBlue = Color.FromArgb(41, 128, 185);

        public AdminPanelForm()
        {
            InitializeModernAdminUI();
            VerileriYukle();
        }

        private void InitializeModernAdminUI()
        {
            this.Text = "Yönetici Kontrol Paneli";
            this.Size = new Size(1280, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = clrBackground;
            this.ForeColor = clrText;
            this.Font = new Font("Segoe UI", 10);

            Panel pnlTitle = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.FromArgb(25, 25, 40) };
            pnlTitle.Controls.Add(new Label { Text = "OTOPARK YÖNETİM SİSTEMİ | DASHBOARD", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, Padding = new Padding(30, 0, 0, 0) });
            this.Controls.Add(pnlTitle);

            Panel pnlStats = new Panel { Dock = DockStyle.Top, Height = 180, BackColor = clrBackground, Padding = new Padding(20) };
            int cardWidth = 280, gap = 30, startX = 30;
            pnlStats.Controls.Add(OlusturModernKart("GÜNLÜK CİRO", "💵", clrAccentGold, out lblGunlukCiro, startX));
            pnlStats.Controls.Add(OlusturModernKart("AYLIK TOPLAM", "📈", clrAccentGreen, out lblAylikCiro, startX + cardWidth + gap));
            pnlStats.Controls.Add(OlusturModernKart("BUGÜN GELEN", "🚗", clrAccentBlue, out lblGunlukMusteri, startX + (cardWidth + gap) * 2));
            pnlStats.Controls.Add(OlusturModernKart("BU AY GELEN", "🅿️", clrAccentBlue, out lblAylikMusteri, startX + (cardWidth + gap) * 3));
            this.Controls.Add(pnlStats);

            Panel pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 100, BackColor = clrPanel };
            Button btnSil = new Button { Text = "🗑️  SEÇİLİ KAYDI SİL", Size = new Size(220, 55), Location = new Point(this.ClientSize.Width - 260, 25), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Anchor = AnchorStyles.Right | AnchorStyles.Top };
            btnSil.Click += BtnSil_Click;
            pnlFooter.Controls.Add(btnSil);

            Button btnYenile = new Button { Text = "🔄  LİSTEYİ YENİLE", Size = new Size(220, 55), Location = new Point(this.ClientSize.Width - 500, 25), BackColor = Color.FromArgb(243, 156, 18), ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Anchor = AnchorStyles.Right | AnchorStyles.Top };
            btnYenile.Click += (s, e) => VerileriYukle();
            pnlFooter.Controls.Add(btnYenile);
            this.Controls.Add(pnlFooter);

            dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = clrBackground, BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, AllowUserToAddRows = false, ReadOnly = true, Font = new Font("Segoe UI", 11) };
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = clrHeader;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 55;
            dgv.DefaultCellStyle.BackColor = clrPanel;
            dgv.DefaultCellStyle.ForeColor = clrText;
            dgv.DefaultCellStyle.SelectionBackColor = clrAccentGreen;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Padding = new Padding(10);
            dgv.RowTemplate.Height = 45;
            this.Controls.Add(dgv);
            dgv.BringToFront();
        }

        private Panel OlusturModernKart(string baslik, string ikon, Color vurguRengi, out Label lblDeger, int x)
        {
            Panel pCard = new Panel { Size = new Size(280, 140), Location = new Point(x, 20), BackColor = clrPanel };
            pCard.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 8, BackColor = vurguRengi });
            pCard.Controls.Add(new Label { Text = ikon, Location = new Point(190, 10), AutoSize = true, Font = new Font("Segoe UI Emoji", 40), ForeColor = Color.FromArgb(20, 255, 255, 255), BackColor = Color.Transparent });
            pCard.Controls.Add(new Label { Text = baslik, Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(160, 160, 180) });
            lblDeger = new Label { Text = "0", Location = new Point(18, 55), AutoSize = true, Font = new Font("Segoe UI", 28, FontStyle.Bold), ForeColor = Color.White };
            pCard.Controls.Add(lblDeger);
            return pCard;
        }

        private void VerileriYukle()
        {
            dgv.DataSource = null;
            dgv.DataSource = VeriDeposu.Rezervasyonlar.Select(x => new { GercekId = x.Id, ID = x.Id.Substring(0, 8), Müşteri = x.AdSoyad, x.Plaka, x.Telefon, Giriş = x.GirisTarihi.ToShortDateString(), Çıkış = x.CikisTarihi.ToShortDateString(), Konum = x.ParkYeriKodu, Tutar = x.Ucret.ToString("C2") }).ToList();
            if (dgv.Columns["GercekId"] != null) dgv.Columns["GercekId"].Visible = false;

            var bugun = DateTime.Today;
            lblGunlukCiro.Text = VeriDeposu.Rezervasyonlar.Where(r => r.OlusturmaTarihi.Date == bugun).Sum(r => r.Ucret).ToString("C2");
            lblAylikCiro.Text = VeriDeposu.Rezervasyonlar.Where(r => r.OlusturmaTarihi.Month == bugun.Month).Sum(r => r.Ucret).ToString("C2");
            lblGunlukMusteri.Text = VeriDeposu.Rezervasyonlar.Count(r => r.GirisTarihi == bugun).ToString();
            lblAylikMusteri.Text = VeriDeposu.Rezervasyonlar.Count(r => r.GirisTarihi.Month == bugun.Month).ToString();
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0 && MessageBox.Show("Silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = dgv.SelectedRows[0].Cells["GercekId"].Value.ToString();
                var kayit = VeriDeposu.Rezervasyonlar.FirstOrDefault(x => x.Id == id);
                if (kayit != null) { VeriDeposu.Rezervasyonlar.Remove(kayit); VerileriYukle(); MessageBox.Show("Silindi."); }
            }
        }
    }
}