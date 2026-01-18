using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OtoparkProjesi
{
    public class MusteriForm : Form
    {
        private readonly IUcretHesaplayici _hesaplayici;

        // --- MODERN PREMIUM RENK PALETİ ---
        private Color clrBackground = Color.FromArgb(30, 30, 45);
        private Color clrPanel = Color.FromArgb(45, 45, 60);
        private Color clrInput = Color.FromArgb(60, 60, 80);
        private Color clrText = Color.FromArgb(240, 240, 240);
        private Color clrAccentGreen = Color.FromArgb(39, 174, 96);
        private Color clrAccentRed = Color.FromArgb(231, 76, 60);
        private Color clrAccentGold = Color.FromArgb(243, 156, 18);
        private Color clrRoad = Color.FromArgb(64, 64, 64);          // Asfalt Rengi

        // UI Kontrolleri
        private TabControl tabSihirbaz;
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Label lblAdimBaslik;
        private Label lblCanliSaat;
        private Timer timerSaat;

        // Adım 1: Bilgiler
        private DateTimePicker dtpGiris, dtpCikis;
        private TextBox txtAdSoyad, txtTelefon, txtPlaka;
        private Label lblSureBilgisi;

        // Adım 2: Park Yeri
        private Button[] btnKatlar = new Button[3];
        private int seciliKatIndex = 0;
        private Panel pnlParkGrid;
        private string secilenParkYeri = "";
        private Label lblSecimOzet;

        // Adım 3: Ödeme
        private TextBox txtKartIsim, txtKartNo, txtAy, txtYil, txtCvv;
        private Label lblOdemeTutar, lblOdemeDetay;

        public MusteriForm()
        {
            _hesaplayici = new StandartUcretHesaplayici();
            InitializeModernWizardUI();

            timerSaat = new Timer { Interval = 1000 };
            timerSaat.Tick += (s, e) => lblCanliSaat.Text = DateTime.Now.ToString("dd.MM.yyyy\nHH:mm:ss");
            timerSaat.Start();
        }

        private void InitializeModernWizardUI()
        {
            this.Text = "Modern Otopark Sistemi - Müşteri Girişi";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1024, 768);
            this.BackColor = clrBackground;
            this.ForeColor = clrText;
            this.Font = new Font("Segoe UI", 11);

            // --- 1. SOL DİKEY SIDEBAR (LOGO & SAAT) ---
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(20, 20, 30) // Daha koyu ton
            };

            // Logo
            Label lblLogo = new Label
            {
                Text = "OTOPARK\nSİSTEMİ",
                Dock = DockStyle.Top,
                Height = 200,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White
            };
            pnlSidebar.Controls.Add(lblLogo);

            // Saat (Altta)
            lblCanliSaat = new Label
            {
                Text = DateTime.Now.ToString("dd.MM.yyyy\nHH:mm:ss"),
                Dock = DockStyle.Bottom,
                Height = 150,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Consolas", 16),
                ForeColor = Color.Cyan
            };
            pnlSidebar.Controls.Add(lblCanliSaat);

            this.Controls.Add(pnlSidebar);

            // --- 2. ÜST YATAY HEADER (ADIM BAŞLIĞI & ADMIN) ---
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(25, 25, 35)
            };

            // Admin Butonu (Sağa Yaslı)
            Button btnAdmin = new Button
            {
                Text = "🔐 YÖNETİCİ",
                Size = new Size(150, 45),
                BackColor = Color.Crimson,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Dock = DockStyle.Right
            };
            btnAdmin.FlatAppearance.BorderSize = 0;
            btnAdmin.Click += (s, e) => new AdminLoginForm().ShowDialog();
            pnlHeader.Controls.Add(btnAdmin);

            // Başlık (Ortada/Solda)
            lblAdimBaslik = new Label
            {
                Text = "KİŞİSEL BİLGİLER VE TARİH",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = clrAccentGold
            };
            pnlHeader.Controls.Add(lblAdimBaslik);

            this.Controls.Add(pnlHeader);

            // --- 3. WIZARD TABS (ANA İÇERİK) ---
            tabSihirbaz = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.FlatButtons,
                ItemSize = new Size(0, 1),
                SizeMode = TabSizeMode.Fixed
            };
            tabSihirbaz.TabPages.Add(OlusturAdim1_Bilgiler());
            tabSihirbaz.TabPages.Add(OlusturAdim2_ParkSecimi());
            tabSihirbaz.TabPages.Add(OlusturAdim3_Odeme());
            this.Controls.Add(tabSihirbaz);

            // TabControl'ün Header panelin altında kalmasını engellemek için
            tabSihirbaz.BringToFront();
        }

        // --- ADIM 1: BİLGİLER ---
        private TabPage OlusturAdim1_Bilgiler()
        {
            TabPage page = new TabPage { BackColor = clrBackground };

            // Dinamik ortalanan panel
            int panelW = 700, panelH = 600;
            Panel pnlCenter = new Panel
            {
                Size = new Size(panelW, panelH),
                BackColor = clrPanel,
            };

            // Ortala
            page.Resize += (s, e) =>
            {
                pnlCenter.Left = (page.Width - pnlCenter.Width) / 2;
                pnlCenter.Top = Math.Max(20, (page.Height - pnlCenter.Height) / 2);
            };

            Label lblTitle = new Label { Text = "📝 MÜŞTERİ KAYIT FORMU", Dock = DockStyle.Top, Height = 60, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.FromArgb(50, 50, 70) };
            pnlCenter.Controls.Add(lblTitle);

            int startY = 80, gapY = 80, contentW = 500, startX = (panelW - contentW) / 2;

            pnlCenter.Controls.Add(new Label { Text = "Giriş ve Çıkış Tarihi Seçiniz:", Location = new Point(startX, startY), ForeColor = Color.White, AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) });
            dtpGiris = new DateTimePicker { Location = new Point(startX, startY + 30), Width = 240, Format = DateTimePickerFormat.Short, MinDate = DateTime.Today, Font = new Font("Segoe UI", 12) };
            pnlCenter.Controls.Add(dtpGiris);
            dtpCikis = new DateTimePicker { Location = new Point(startX + 260, startY + 30), Width = 240, Format = DateTimePickerFormat.Short, MinDate = DateTime.Today, Font = new Font("Segoe UI", 12) };
            pnlCenter.Controls.Add(dtpCikis);

            EventHandler tarihHesapla = (s, e) => {
                if (dtpCikis.Value < dtpGiris.Value) dtpCikis.Value = dtpGiris.Value;
                TimeSpan fark = dtpCikis.Value.Date - dtpGiris.Value.Date;
                if (fark.TotalDays == 0) fark = TimeSpan.FromDays(1);
                lblSureBilgisi.Text = $"⏳ Planlanan Süre: {fark.TotalDays} Gün";
            };
            dtpGiris.ValueChanged += tarihHesapla; dtpCikis.ValueChanged += tarihHesapla;

            lblSureBilgisi = new Label { Text = "⏳ Planlanan Süre: 1 Gün", Location = new Point(startX, startY + 65), AutoSize = true, ForeColor = clrAccentGold, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            pnlCenter.Controls.Add(lblSureBilgisi);

            startY += gapY + 10;
            pnlCenter.Controls.Add(new Label { Text = "Ad Soyad:", Location = new Point(startX, startY), ForeColor = Color.White, AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) });
            txtAdSoyad = ModernTextBox(new Point(startX, startY + 30), contentW);
            txtAdSoyad.KeyPress += SadeceHarf_KeyPress;
            pnlCenter.Controls.Add(txtAdSoyad);

            startY += gapY;
            pnlCenter.Controls.Add(new Label { Text = "Telefon Numarası:", Location = new Point(startX, startY), ForeColor = Color.White, AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) });
            txtTelefon = ModernTextBox(new Point(startX, startY + 30), contentW); txtTelefon.MaxLength = 11; txtTelefon.KeyPress += SadeceSayi_KeyPress;
            pnlCenter.Controls.Add(txtTelefon);

            startY += gapY;
            pnlCenter.Controls.Add(new Label { Text = "Araç Plakası:", Location = new Point(startX, startY), ForeColor = Color.White, AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) });
            txtPlaka = ModernTextBox(new Point(startX, startY + 30), contentW); txtPlaka.CharacterCasing = CharacterCasing.Upper;
            pnlCenter.Controls.Add(txtPlaka);

            Button btnIleri = new Button { Text = "PARK YERİ SEÇ  🡆", Size = new Size(contentW, 55), Location = new Point(startX, 500), BackColor = clrAccentGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 14, FontStyle.Bold), Cursor = Cursors.Hand };
            btnIleri.FlatAppearance.BorderSize = 0;
            btnIleri.Click += (s, e) => { if (BosKontrol(txtAdSoyad, txtTelefon, txtPlaka)) SayfaDegistir(1); else MessageBox.Show("Eksik bilgi girdiniz."); };
            pnlCenter.Controls.Add(btnIleri);

            page.Controls.Add(pnlCenter);
            return page;
        }

        // --- ADIM 2: PARK YERİ (DÜZELTİLMİŞ LAYOUT) ---
        private TabPage OlusturAdim2_ParkSecimi()
        {
            TabPage page = new TabPage { BackColor = clrBackground };

            // Sağ Panel (Bilgi - Sabit Genişlik)
            Panel pnlRight = new Panel { Width = 350, BackColor = clrPanel, Dock = DockStyle.Right, Padding = new Padding(20) };
            page.Controls.Add(pnlRight);

            // Sol Panel (Kroki + Butonlar - Kalan Alanı Kaplar)
            Panel pnlLeft = new Panel { BackColor = clrPanel, Dock = DockStyle.Fill, Padding = new Padding(20) };
            page.Controls.Add(pnlLeft);

            // --- Sol Panel Yapısı ---
            // 1. Butonlar Paneli (En Altta Sabit Yükseklik)
            Panel pnlButtonsContainer = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = clrPanel };
            pnlLeft.Controls.Add(pnlButtonsContainer);

            // 2. Kroki Grid Konteyneri (Geriye Kalan Alanı Kaplar - Fill)
            Panel pnlGridContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 0, 10) };
            pnlLeft.Controls.Add(pnlGridContainer);

            // Kroki Başlık
            Label lblKat = new Label { Text = "OTOPARK KROKİSİ", Dock = DockStyle.Top, Height = 30, AutoSize = false, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.Silver, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            pnlGridContainer.Controls.Add(lblKat);

            // Kroki Çizim Paneli
            pnlParkGrid = new Panel
            {
                BackColor = Color.FromArgb(35, 35, 35),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill // Konteynerin tamamını kapla
            };
            pnlParkGrid.Resize += (s, e) => ParkKrokisiCiz(seciliKatIndex);
            pnlGridContainer.Controls.Add(pnlParkGrid);

            // KAT SEÇİM BUTONLARI (pnlButtonsContainer içinde)
            string[] katIsimleri = { "1. KAT (ZEMİN)", "2. KAT (ORTA)", "3. KAT (TERAS)" };
            int btnWidth = 200, btnHeight = 50, btnGap = 20;

            for (int i = 0; i < 3; i++)
            {
                Button btn = new Button
                {
                    Text = katIsimleri[i],
                    Size = new Size(btnWidth, btnHeight),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = (i == 0) ? clrAccentGold : clrInput,
                    ForeColor = (i == 0) ? Color.Black : Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Tag = i,
                    Cursor = Cursors.Hand,
                    Location = new Point(20 + (i * (btnWidth + btnGap)), 15) // Soldan sırala
                };
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.Gray;

                int index = i;
                btn.Click += (s, e) => KatDegistir(index);

                btnKatlar[i] = btn;
                pnlButtonsContainer.Controls.Add(btn);
            }

            // --- Sağ Panel İçeriği ---
            Label lblBilgiTitle = new Label { Text = "ℹ️ SEÇİM ÖZETİ", Dock = DockStyle.Top, Height = 50, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 14, FontStyle.Bold), BackColor = Color.FromArgb(50, 50, 70) };
            pnlRight.Controls.Add(lblBilgiTitle);

            lblSecimOzet = new Label { Text = "Lütfen krokiden boş bir yer seçiniz.", Location = new Point(20, 70), AutoSize = true, ForeColor = Color.White };
            pnlRight.Controls.Add(lblSecimOzet);

            // Sağ panel butonlarını sabitlemek için alt panel
            Panel pnlRightButtons = new Panel { Dock = DockStyle.Bottom, Height = 80 };
            pnlRight.Controls.Add(pnlRightButtons);

            Button btnGeri = new Button { Text = "🡄 GERİ", Size = new Size(140, 50), Location = new Point(0, 15), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            btnGeri.Click += (s, e) => SayfaDegistir(0);
            pnlRightButtons.Controls.Add(btnGeri);

            Button btnOdemeGec = new Button { Text = "ÖDEME 🡆", Size = new Size(160, 50), Location = new Point(150, 15), BackColor = clrAccentGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnOdemeGec.Click += (s, e) => { if (string.IsNullOrEmpty(secilenParkYeri)) MessageBox.Show("Lütfen bir park yeri seçiniz."); else SayfaDegistir(2); };
            pnlRightButtons.Controls.Add(btnOdemeGec);

            return page;
        }

        private void KatDegistir(int yeniKatIndex)
        {
            seciliKatIndex = yeniKatIndex;

            for (int i = 0; i < 3; i++)
            {
                if (i == yeniKatIndex)
                {
                    btnKatlar[i].BackColor = clrAccentGold;
                    btnKatlar[i].ForeColor = Color.Black;
                }
                else
                {
                    btnKatlar[i].BackColor = clrInput;
                    btnKatlar[i].ForeColor = Color.White;
                }
            }
            ParkKrokisiCiz(yeniKatIndex);
        }

        // --- GELİŞMİŞ AVM KROKİSİ ÇİZİMİ ---
        private void ParkKrokisiCiz(int katIndeks)
        {
            // Null kontrolü eklendi
            if (pnlParkGrid == null) return;

            pnlParkGrid.Controls.Clear();
            string[] blokKodlari = { "A", "B", "C" };
            string blok = blokKodlari[katIndeks];
            DateTime giris = dtpGiris.Value.Date;
            DateTime cikis = dtpCikis.Value.Date;

            // KATLARA GÖRE ARKAPLAN
            Color[] katRenkleri = { Color.FromArgb(40, 40, 45), Color.FromArgb(30, 45, 55), Color.FromArgb(50, 35, 30) };
            pnlParkGrid.BackColor = katRenkleri[katIndeks];

            // Kroki Çizim Ayarları (U DÖNÜŞLÜ YOL - Dinamik Ortala)
            int yolKalinlik = 140;
            int contentWidth = 900;
            int startX = (pnlParkGrid.Width - contentWidth) / 2;
            if (startX < 20) startX = 20;

            int solYolX = startX + 150;
            int sagYolX = solYolX + 450;
            int yolUzunluk = 800;

            // 1. Sol Dikey Yol
            Panel pnlYolSol = new Panel { Location = new Point(solYolX, 0), Size = new Size(yolKalinlik, yolUzunluk), BackColor = clrRoad };
            for (int k = 0; k < 15; k++) pnlYolSol.Controls.Add(new Label { BackColor = Color.White, Size = new Size(4, 40), Location = new Point(yolKalinlik / 2, 20 + (k * 80)) });
            pnlYolSol.Controls.Add(new Label { Text = "▼ GİRİŞ", ForeColor = Color.LightGreen, BackColor = Color.Transparent, Font = new Font("Arial", 14, FontStyle.Bold), Location = new Point(30, 60), AutoSize = true });
            pnlParkGrid.Controls.Add(pnlYolSol);

            // 2. Alt Yatay Bağlantı
            Panel pnlYolAlt = new Panel { Location = new Point(solYolX, yolUzunluk), Size = new Size(sagYolX - solYolX + yolKalinlik, yolKalinlik), BackColor = clrRoad };
            for (int k = 0; k < 8; k++) pnlYolAlt.Controls.Add(new Label { BackColor = Color.White, Size = new Size(40, 4), Location = new Point(20 + (k * 80), yolKalinlik / 2) });
            pnlYolAlt.Controls.Add(new Label { Text = ">>>", ForeColor = Color.Yellow, BackColor = Color.Transparent, Font = new Font("Arial", 20, FontStyle.Bold), Location = new Point(200, 50), AutoSize = true });
            pnlParkGrid.Controls.Add(pnlYolAlt);

            // 3. Sağ Dikey Yol
            Panel pnlYolSag = new Panel { Location = new Point(sagYolX, 0), Size = new Size(yolKalinlik, yolUzunluk), BackColor = clrRoad };
            for (int k = 0; k < 15; k++) pnlYolSag.Controls.Add(new Label { BackColor = Color.White, Size = new Size(4, 40), Location = new Point(yolKalinlik / 2, 20 + (k * 80)) });
            pnlYolSag.Controls.Add(new Label { Text = "▲ ÇIKIŞ", ForeColor = Color.Salmon, BackColor = Color.Transparent, Font = new Font("Arial", 14, FontStyle.Bold), Location = new Point(30, 60), AutoSize = true });
            pnlParkGrid.Controls.Add(pnlYolSag);

            // PARK YERLERİ
            int parkW = 100;
            int parkH = 65;
            int startY = 80;
            int gapY = 100;

            for (int i = 1; i <= 20; i++)
            {
                string yerKodu = $"{blok}-{i}";
                bool dolu = VeriDeposu.Rezervasyonlar.Any(r => r.ParkYeriKodu == yerKodu && dtpGiris.Value.Date < r.CikisTarihi && dtpCikis.Value.Date > r.GirisTarihi);

                Button btn = new Button
                {
                    Text = yerKodu,
                    Width = parkW,
                    Height = parkH,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Tag = yerKodu,
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 2;

                int xPos = 0, yPos = 0;
                int sideGap = 20;

                if (i <= 5) { xPos = solYolX - parkW - sideGap; yPos = startY + ((i - 1) * gapY); }
                else if (i <= 10) { xPos = solYolX + yolKalinlik + sideGap; yPos = startY + ((i - 6) * gapY); }
                else if (i <= 15) { xPos = sagYolX - parkW - sideGap; yPos = startY + ((i - 11) * gapY); }
                else { xPos = sagYolX + yolKalinlik + sideGap; yPos = startY + ((i - 16) * gapY); }

                btn.Location = new Point(xPos, yPos);

                if (dolu)
                {
                    btn.BackColor = clrAccentRed;
                    btn.ForeColor = Color.White;
                    btn.Text += "\nDOLU";
                    btn.Enabled = false;
                    btn.FlatAppearance.BorderColor = Color.DarkRed;
                }
                else if (yerKodu == secilenParkYeri)
                {
                    btn.BackColor = clrAccentGold;
                    btn.ForeColor = Color.Black;
                    btn.Text += "\nSEÇİLİ";
                    btn.FlatAppearance.BorderColor = Color.Yellow;
                }
                else
                {
                    btn.BackColor = clrAccentGreen;
                    btn.ForeColor = Color.White;
                    btn.Text += "\nBOŞ";
                    btn.FlatAppearance.BorderColor = Color.DarkGreen;
                    btn.Click += ParkYeriSec_Click;
                }

                pnlParkGrid.Controls.Add(btn);
                btn.BringToFront();
            }

            pnlYolSol.SendToBack();
            pnlYolSag.SendToBack();
            pnlYolAlt.SendToBack();
        }

        private void ParkYeriSec_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            secilenParkYeri = btn.Tag.ToString();
            ParkKrokisiCiz(seciliKatIndex);

            int numara = int.Parse(secilenParkYeri.Split('-')[1]);
            string yakinlik = (numara <= 10) ? "ANA GİRİŞ" : "ANA ÇIKIŞ";
            lblSecimOzet.Text = $"SEÇİLEN PARK YERİ:\n👉 {secilenParkYeri}\n\nKonum Bilgisi:\n📍 {yakinlik} kapısına yakın.\n\nTarih Aralığı:\n📅 {dtpGiris.Value.ToShortDateString()} - {dtpCikis.Value.ToShortDateString()}";
        }

        // --- ADIM 3: ÖDEME (Ortalama Düzeltildi) ---
        private TabPage OlusturAdim3_Odeme()
        {
            TabPage page = new TabPage { BackColor = clrBackground };
            Panel pnlCard = new Panel { Size = new Size(500, 600), BackColor = clrPanel, BorderStyle = BorderStyle.FixedSingle };

            // Paneli Ortala
            page.Resize += (s, e) => {
                pnlCard.Left = (page.Width - pnlCard.Width) / 2;
                pnlCard.Top = Math.Max(20, (page.Height - pnlCard.Height) / 2);
            };

            Label lblTitle = new Label { Text = "💳 GÜVENLİ ÖDEME", Dock = DockStyle.Top, Height = 60, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 16, FontStyle.Bold), BackColor = Color.SeaGreen, ForeColor = Color.White };
            pnlCard.Controls.Add(lblTitle);

            lblOdemeDetay = new Label { Text = "Yükleniyor...", Location = new Point(30, 80), AutoSize = true, ForeColor = Color.LightGray, Font = new Font("Segoe UI", 10) };
            pnlCard.Controls.Add(lblOdemeDetay);
            lblOdemeTutar = new Label { Text = "0.00 TL", Location = new Point(280, 80), AutoSize = true, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = clrAccentGold };
            pnlCard.Controls.Add(lblOdemeTutar);
            pnlCard.Controls.Add(new Panel { Height = 2, BackColor = Color.Gray, Location = new Point(30, 150), Width = 440 });

            int y = 170, x = 50, gap = 75;
            pnlCard.Controls.Add(new Label { Text = "Kart Üzerindeki İsim:", Location = new Point(x, y), ForeColor = Color.White, AutoSize = true });
            txtKartIsim = ModernTextBox(new Point(x, y + 25), 400);
            txtKartIsim.KeyPress += SadeceHarf_KeyPress;
            pnlCard.Controls.Add(txtKartIsim);

            y += gap;
            pnlCard.Controls.Add(new Label { Text = "Kart Numarası:", Location = new Point(x, y), ForeColor = Color.White, AutoSize = true });
            txtKartNo = ModernTextBox(new Point(x, y + 25), 400);
            txtKartNo.MaxLength = 16;
            txtKartNo.KeyPress += SadeceSayi_KeyPress;
            pnlCard.Controls.Add(txtKartNo);

            y += gap;
            pnlCard.Controls.Add(new Label { Text = "SKT (Ay / Yıl):", Location = new Point(x, y), ForeColor = Color.White, AutoSize = true });
            txtAy = ModernTextBox(new Point(x, y + 25), 60); txtAy.MaxLength = 2; txtAy.KeyPress += SadeceSayi_KeyPress;
            pnlCard.Controls.Add(txtAy);

            pnlCard.Controls.Add(new Label { Text = "/", Location = new Point(x + 70, y + 25), ForeColor = Color.White, AutoSize = true, Font = new Font("Segoe UI", 14) });

            txtYil = ModernTextBox(new Point(x + 90, y + 25), 60); txtYil.MaxLength = 2; txtYil.KeyPress += SadeceSayi_KeyPress;
            pnlCard.Controls.Add(txtYil);

            pnlCard.Controls.Add(new Label { Text = "CVV:", Location = new Point(x + 250, y), ForeColor = Color.White, AutoSize = true });
            txtCvv = ModernTextBox(new Point(x + 250, y + 25), 80); txtCvv.MaxLength = 3; txtCvv.PasswordChar = '*'; txtCvv.KeyPress += SadeceSayi_KeyPress;
            pnlCard.Controls.Add(txtCvv);

            y += 90;
            Button btnGeri = new Button { Text = "GERİ", Location = new Point(x, y), Size = new Size(120, 50), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnGeri.Click += (s, e) => SayfaDegistir(1);
            pnlCard.Controls.Add(btnGeri);
            Button btnTamamla = new Button { Text = "ÖDEMEYİ TAMAMLA", Location = new Point(x + 140, y), Size = new Size(260, 50), BackColor = clrAccentGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            btnTamamla.Click += (s, e) => RezervasyonKaydet();
            pnlCard.Controls.Add(btnTamamla);

            page.Controls.Add(pnlCard);
            return page;
        }

        // --- SAYFA GEÇİŞ VE KAYIT YÖNTEMLERİ ---
        private void SayfaDegistir(int sayfaIndeks)
        {
            tabSihirbaz.SelectedIndex = sayfaIndeks;
            // Başlık Güncelleme
            if (sayfaIndeks == 0) lblAdimBaslik.Text = "KİŞİSEL BİLGİLER VE TARİH";
            else if (sayfaIndeks == 1)
            {
                lblAdimBaslik.Text = "PARK YERİ SEÇİMİ";
                ParkKrokisiCiz(seciliKatIndex); // Çizimi güncelle
            }
            else if (sayfaIndeks == 2)
            {
                lblAdimBaslik.Text = "ÖDEME İŞLEMİ";
                HesapDetaylariniGuncelle();
            }
        }

        // HATA GİDERİLDİ: Metot artık burada tanımlı
        private void HesapDetaylariniGuncelle()
        {
            decimal tutar = _hesaplayici.Hesapla(dtpGiris.Value, dtpCikis.Value);
            lblOdemeTutar.Text = $"{tutar:C2}";
            lblOdemeDetay.Text = $"Müşteri: {txtAdSoyad.Text}\nPlaka: {txtPlaka.Text}\nKonum: {secilenParkYeri}\nGiriş: {dtpGiris.Value.ToShortDateString()}\nÇıkış: {dtpCikis.Value.ToShortDateString()}";
        }

        private void RezervasyonKaydet()
        {
            if (!BosKontrol(txtKartIsim, txtKartNo, txtCvv)) { MessageBox.Show("Lütfen kart bilgilerini eksiksiz giriniz."); return; }

            bool doluMu = VeriDeposu.Rezervasyonlar.Any(r => r.ParkYeriKodu == secilenParkYeri && dtpGiris.Value.Date < r.CikisTarihi && dtpCikis.Value.Date > r.GirisTarihi);
            if (doluMu)
            {
                MessageBox.Show("Seçtiğiniz yer az önce doldu! Lütfen başka yer seçiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SayfaDegistir(1);
                ParkKrokisiCiz(seciliKatIndex);
                return;
            }

            VeriDeposu.Ekle(new Rezervasyon
            {
                AdSoyad = txtAdSoyad.Text,
                Telefon = txtTelefon.Text,
                Plaka = txtPlaka.Text,
                GirisTarihi = dtpGiris.Value.Date,
                CikisTarihi = dtpCikis.Value.Date,
                ParkYeriKodu = secilenParkYeri,
                Ucret = decimal.Parse(lblOdemeTutar.Text.Replace("₺", "").Replace(" TL", "").Trim())
            });

            MessageBox.Show("İşleminiz Başarıyla Tamamlandı!", "Onay", MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtAdSoyad.Clear(); txtTelefon.Clear(); txtPlaka.Clear();
            txtKartIsim.Clear(); txtKartNo.Clear(); txtAy.Clear(); txtYil.Clear(); txtCvv.Clear();
            secilenParkYeri = "";
            SayfaDegistir(0);
        }

        private TextBox ModernTextBox(Point loc, int w) { return new TextBox { Location = loc, Width = w, BackColor = clrInput, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 14) }; }
        private bool BosKontrol(params TextBox[] txts) => txts.All(t => !string.IsNullOrWhiteSpace(t.Text));
        private void SadeceSayi_KeyPress(object sender, KeyPressEventArgs e) { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; }
        private void SadeceHarf_KeyPress(object sender, KeyPressEventArgs e) { if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; }
    }
}