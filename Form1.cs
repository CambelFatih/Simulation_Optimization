using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO.Ports;
using OpenTK.Graphics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Youtube3D_Simulasyon
{
    public class datas
    {
        public int id;
        public float[,] array = new float[360, 2];
    }
    public partial class Form1 : Form
    {
        int x = 0, y = 0, z = 0;
        bool cx = false, cy = false, cz = false;
        datas ds1 = new datas(); datas ds2 = new datas(); datas ds3 = new datas();
        Color renk1 = Color.Blue, renk2 = Color.Red;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] portlar = SerialPort.GetPortNames();
            foreach (string portAdi in portlar)
            {
                cmbxSerialPort.Items.Add(portAdi);
            }
            GL.ClearColor(Color.White);//Color.FromArgb(143, 212, 150)
            TimerXYZ.Interval = 1;
            hesapla(x, z, y, 4, ds1);
            hesapla(x, z, y, 1.5f, ds2);
            hesapla(x, z, y, 0.07f, ds3);
            List<datas> datalar = new List<datas>(2);
            datalar.Add(ds1);
            datalar.Add(ds2);
            datalar.Add(ds3);
        }

        void hesapla(float x, float y, float z, float radius, datas data)
        {
            for (int i = 0; i < 360; i++)
            {
                data.array[i, 0] = (float)(x + Math.Cos(i) * radius);
                data.array[i, 1] = (float)(y + Math.Sin(i) * radius);
            }

        }
        void kapak(float x, float y, float z, Color renk, datas ds)
        {
            GL.Enable(EnableCap.Blend);
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color4(renk);
            GL.Vertex3(x, y, z);
            for (int i = 0; i < 360; i++)
            {
                renk_ataması(i);
                GL.Vertex3(ds.array[i, 0], y, ds.array[i, 1]);
            }
            GL.End();
            GL.Disable(EnableCap.Blend);
        }
        void Koni(float x, float y1, float y2, float z, datas dsp1, datas dsp2)
        {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color4(Color.Red);

            for (int i = 0; i < 360; i++)
            {
                if (i < 180)
                    renk_ataması(i);
                GL.Vertex3(dsp1.array[i, 0], y1, dsp1.array[i, 1]);
                GL.Vertex3(dsp2.array[i, 0], y2, dsp2.array[i, 1]);
            }
            GL.End();
        }
        private void TimerXYZ_Tick(object sender, EventArgs e)
        {
            if (cx == true)
            {
                if (x < 360)
                    x += 5;
                else
                    x = 0;
                lblX.Text = x.ToString();
            }
            if (cy == true)
            {
                if (y < 360)
                    y += 5;
                else
                    y = 0;
                lblY.Text = y.ToString();
            }
            if (cz == true)
            {
                if (z < 360)
                    z += 5;
                else
                    z = 0;
                lblZ.Text = z.ToString();
            }
            glControl1.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            int step = 1;//Adım genişliği çözünürlük
            int topla = step;//Tanpon 
            float radius = 4.0f;//Yarıçağ Modle Uydunun
            GL.Clear(ClearBufferMask.ColorBufferBit);//Buffer temizlenmez ise görüntüler üst üste bine o yüzden temizliyoruz.
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, 4 / 3, 1, 10000);
            Matrix4 lookat = Matrix4.LookAt(25, 0, 0, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            //Asagidaki fonksiyonlar ile nesneyi hareket ettirmemizi sağlıyor.
            GL.Rotate(x, 1.0, 0.0, 0.0);
            GL.Rotate(z, 0.0, 1.0, 0.0);
            GL.Rotate(y, 0.0, 0.0, 1.0);

            Koni(0, -5, +3, 0, ds1, ds1);
            Koni(0, -5, -10, 0, ds1, ds2);
            kapak(0, -10, 0, Color.Green, ds2);
            Koni(0, +3, +5, 0, ds1, ds2);
            kapak(0, +5, 0, Color.Green, ds2);
            Koni(0, +5, +9, 0, ds3, ds3);

            Pervane(9.0f, 7.0f, 0.3f, 0.3f);
            Pervane(7.0f, 7.0f, 0.3f, 0.3f);

            //Çizim Fonksiyonları

            //Pervane(Yükseklik,Pervane Uzunluğu,Pervane Genişliği,Pervane açısı)

            //// AŞAĞIDA X, Y, Z EKSEN CİZGELERİ ÇİZDİRİLİYOR
            GL.Begin(PrimitiveType.Lines);

            GL.Color3(Color.FromArgb(250, 0, 0));
            GL.Vertex3(-1000, 0, 0);
            GL.Vertex3(1000, 0, 0);

            GL.Color3(Color.FromArgb(25, 150, 100));
            GL.Vertex3(0, 0, -1000);
            GL.Vertex3(0, 0, 1000);

            GL.Color3(Color.FromArgb(0, 0, 0));
            GL.Vertex3(0, 1000, 0);
            GL.Vertex3(0, -1000, 0);

            GL.End();
            //GraphicsContext.CurrentContext.VSync = true;
            glControl1.SwapBuffers();
        }

        private void btnX_Click(object sender, EventArgs e)
        {
            if (cx == false)
                cx = true;
            else
                cx = false;
            TimerXYZ.Start();
        }

        private void btnY_Click(object sender, EventArgs e)
        {
            if (cy == false)
                cy = true;
            else
                cy = false;
            TimerXYZ.Start();
        }

        private void btnZ_Click(object sender, EventArgs e)
        {
            if (cz == false)
                cz = true;
            else
                cz = false;
            TimerXYZ.Start();
        }
        private void btnTelemetri_Click(object sender, EventArgs e)
        {
            try
            {
                OkumaNesnesi.BaudRate = Convert.ToInt32(texBoundRate.Text);
                OkumaNesnesi.PortName = cmbxSerialPort.Text;
                if (!OkumaNesnesi.IsOpen)
                {
                    Zamanlayici.Start();
                    OkumaNesnesi.Open();
                    //MessageBox.Show("BAĞLANTI KURULDU");
                    btnDurdur.Enabled = true;
                    btnTelemetri.Enabled = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("BAĞLANTI KURULAMADI");
                btnDurdur.Enabled = true;
                //BtnBasla.Enabled = false;
            }
        }

        private void btnDurdur_Click(object sender, EventArgs e)
        {
            OkumaNesnesi.Close();
            Zamanlayici.Stop();
            btnTelemetri.Enabled = true;
            btnDurdur.Enabled = false;
            MessageBox.Show("BAĞLANTI KESİLDİ");
        }

        private void Zamanlayici_Tick(object sender, EventArgs e)
        {
            try
            {
                string[] paket;
                string sonuc = OkumaNesnesi.ReadLine();
                paket = sonuc.Split('*');
                lblxx.Text = paket[0];
                lblyy.Text = paket[1];
                lblzz.Text = paket[2];
                x = Convert.ToInt32(paket[0]);
                y = Convert.ToInt32(paket[1]);
                z = Convert.ToInt32(paket[2]);
                glControl1.Invalidate();
                OkumaNesnesi.DiscardInBuffer();

            }
            catch
            {

            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorPicker = new ColorDialog();
            if (colorPicker.ShowDialog() == DialogResult.OK)
            {
                renk1 = colorPicker.Color;
            }
            glControl1.Invalidate();
        }

        private void btnColor2_Click(object sender, EventArgs e)
        {
            ColorDialog colorPicker = new ColorDialog();
            if (colorPicker.ShowDialog() == DialogResult.OK)
            {
                renk2 = colorPicker.Color;
            }
            glControl1.Invalidate();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);//sonradan yazdık
        }

        private void renk_ataması(int step)
        {
            if (step < 45)
                GL.Color3(renk2);
            else if (step < 90)
                GL.Color3(renk1);
            else if (step < 135)
                GL.Color3(renk2);
            else if (step < 180)
                GL.Color3(renk1);
            else if (step < 225)
                GL.Color3(renk2);
            else if (step < 270)
                GL.Color3(renk1);
            else if (step < 315)
                GL.Color3(renk2);
            else if (step < 360)
                GL.Color3(renk1);
        }

        private void Pervane(float yukseklik, float uzunluk, float kalinlik, float egiklik)
        {
            float radius = 10, angle = 45.0f;
            GL.Begin(BeginMode.Quads);

            GL.Color3(renk2);
            GL.Vertex3(uzunluk, yukseklik, kalinlik);
            GL.Vertex3(uzunluk, yukseklik + egiklik, -kalinlik);
            GL.Vertex3(0, yukseklik + egiklik, -kalinlik);
            GL.Vertex3(0, yukseklik, kalinlik);

            GL.Color3(renk2);
            GL.Vertex3(-uzunluk, yukseklik + egiklik, kalinlik);
            GL.Vertex3(-uzunluk, yukseklik, -kalinlik);
            GL.Vertex3(0, yukseklik, -kalinlik);
            GL.Vertex3(0, yukseklik + egiklik, kalinlik);

            GL.Color3(renk1);
            GL.Vertex3(kalinlik, yukseklik, -uzunluk);
            GL.Vertex3(-kalinlik, yukseklik + egiklik, -uzunluk);
            GL.Vertex3(-kalinlik, yukseklik + egiklik, 0.0);//+
            GL.Vertex3(kalinlik, yukseklik, 0.0);//-

            GL.Color3(renk1);
            GL.Vertex3(kalinlik, yukseklik + egiklik, +uzunluk);
            GL.Vertex3(-kalinlik, yukseklik, +uzunluk);
            GL.Vertex3(-kalinlik, yukseklik, 0.0);
            GL.Vertex3(kalinlik, yukseklik + egiklik, 0.0);
            GL.End();

        }
    }
}











