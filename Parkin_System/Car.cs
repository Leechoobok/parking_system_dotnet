using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Xml.Schema;



namespace Parking_System
{
    public class Car
    {
        string spot;
        string license_plate;
        int into_ptimeH, into_ptimeM;
        int fare;
        List<string> default_spot;
        List<string> default_plate;
        public Car(string spot, string license_plate, int into_ptimeH, int into_ptimeM)
        {
            this.spot = spot;
            this.license_plate = license_plate;
            this.into_ptimeH = into_ptimeH;
            this.into_ptimeM = into_ptimeM;
            this.default_spot = new List<string>();
            this.default_plate = new List<string>();

        }
        public int Rate_check()
        {
            int chk_timeA, chk_timeB;
            DateTime time = DateTime.Now;
            chk_timeA = (time.Hour * 60) + time.Minute;
            chk_timeB = (this.into_ptimeH * 60) + this.into_ptimeM;
            this.fare = (chk_timeA - chk_timeB) / 10 * 800;
            return fare;

        }
        public void Set_car(string license_plate)
        {
            DateTime time = DateTime.Now;
            this.license_plate = license_plate;
            this.into_ptimeH = time.Hour;
            this.into_ptimeM = time.Minute;
        }
        public void Set_spot(string spot)
        {
            this.spot = spot;
        }
        public string Get_spot()
        {
            return this.spot;
        }
        public string GetLicense_plate()
        {
            string str;
            str = this.license_plate;
            return str;
        }
        public int Getinto_timeH()
        {
            int into_t_h;
            into_t_h = this.into_ptimeH;
            return into_t_h;
        }
        public int Getinto_timeM()
        {
            int into_t_m;
            into_t_m = this.into_ptimeM;
            return into_t_m;
        }
        public int Getparking_time()
        {
            int chk_time, chk_timeA, chk_timeB;
            DateTime time = DateTime.Now;
            chk_timeA = (time.Hour * 60) + time.Minute;
            chk_timeB = (this.into_ptimeH * 60) + this.into_ptimeM;
            chk_time = chk_timeA - chk_timeB;
            return chk_time;
        }
        public void WriteData()
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream("parking_car.dat", FileMode.Append)))
            {
                bw.Write(this.spot);
                bw.Write(this.license_plate);
                bw.Write(this.into_ptimeH);
                bw.Write(this.into_ptimeM);
                bw.Write("\n");
            }
            this.default_spot.Add(this.spot);
            this.default_plate.Add(this.license_plate);

        }
        public int ReadData(string str)
        {
            FileStream fs = File.Open("parking_car.dat", FileMode.Open, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fs))
            {
                while (fs.Length != fs.Position)
                {
                    string spot = br.ReadString();
                    string plate = br.ReadString();
                    int into_ptimeH = br.ReadInt32();
                    int into_ptimeM = br.ReadInt32();
                    string n_line = br.ReadString();
                    this.spot = spot;
                    this.license_plate = plate;
                    this.into_ptimeH = into_ptimeH;
                    this.into_ptimeM = into_ptimeM;
                    if (this.license_plate == str) break;
                    //Console.Write(s1 + " " + " " + s2 + " " + " " + i1 + " " + i2 + " " + " " + s3);
                }
            }
            fs.Close();
            if (this.license_plate == str) return 1;
            else return 0;
        }
        public void ReadData()
        {
            FileStream fs = File.Open("parking_car.dat", FileMode.OpenOrCreate, FileAccess.Read);
            using (BinaryReader br = new BinaryReader(fs))
            {
                while (fs.Length != fs.Position)
                {
                    string spot = br.ReadString();
                    string plate = br.ReadString();
                    int into_ptimeH = br.ReadInt32();
                    int into_ptimeM = br.ReadInt32();
                    string n_line = br.ReadString();
                    this.spot = spot;
                    this.license_plate = plate;
                    this.into_ptimeH = into_ptimeH;
                    this.into_ptimeM = into_ptimeM;
                    this.default_plate.Add(this.license_plate);
                    this.default_spot.Add(this.spot);
                    //Console.Write(s1 + " " + " " + s2 + " " + " " + i1 + " " + i2 + " " + " " + s3);
                }
            }
            fs.Close();
            if(this.default_spot.ElementAt(0) == "")
            {
                this.license_plate = "";
                this.default_plate.RemoveAt(default_plate.Count - 1);
                this.default_spot.RemoveAt(default_spot.Count - 1);
                return;
            }
        }
        public int Get_Count()
        {
            return this.default_plate.Count();
        }
        public string Get_default_Sport(int i)
        {
            return this.default_spot.ElementAt(i);
        }
        public string Get_default_License_plate(int i)
        {
            return this.default_plate.ElementAt(i);
        }
        public void DeleteData(string str)
        {
            FileStream fs1 = File.Open("parking_car.dat", FileMode.Open, FileAccess.Read);
            FileStream fs2 = File.Open("tmp.dat", FileMode.Create, FileAccess.Write);
            using (BinaryReader br = new BinaryReader(fs1))
            {
                using (BinaryWriter bw = new BinaryWriter(fs2))
                {
                    while (fs1.Length != fs1.Position)
                    {
                        string spot = br.ReadString();
                        string plate = br.ReadString();
                        int into_ptimeH = br.ReadInt32();
                        int into_ptimeM = br.ReadInt32();
                        string n_line = br.ReadString();
                        this.spot = spot;
                        this.license_plate = plate;
                        this.into_ptimeH = into_ptimeH;
                        this.into_ptimeM = into_ptimeM;

                        if (this.license_plate == str)
                        {
                            continue;
                        }
                        else
                        {
                            bw.Write(this.spot);
                            bw.Write(this.license_plate);
                            bw.Write(this.into_ptimeH);
                            bw.Write(this.into_ptimeM);
                            bw.Write("\n");
                        }
                    }
                }
            }
            fs2.Close();
            fs1.Close();
            File.Delete("parking_car.dat");
            File.Move("tmp.dat", "parking_car.dat");
            for(int i = 0;i<default_plate.Count;i++)
            {
                if(str == this.default_plate.ElementAt(i))
                {
                    this.default_spot.RemoveAt(i);
                    this.default_plate.RemoveAt(i);
                }
            }
        }
    }
}
