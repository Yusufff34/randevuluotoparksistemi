using System;
using System.Windows.Forms;

namespace OtoparkProjesi
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Modern arayüzlü ana formu başlatıyoruz
            Application.Run(new MusteriForm());
        }
    }
}