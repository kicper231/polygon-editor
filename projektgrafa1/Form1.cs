using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Configuration;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static projektgrafa1.Form1.Geometry;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace projektgrafa1
{






    public partial class Form1 : Form
    {
        public static class Geometry
        {
            public class figure
            {
                public List<Segment> segments { get; set; }
                public List<PointR> points { get; set; } 
                public List<PointR> offsetpoints { get; set; }
                public int id;

                public figure(List<PointR> points, List<Segment> segments, int id)  
                {
                    this.segments = segments;
                    this.points = points;
                    this.id = id;
                }
            }

            public static double Epsilon = 5;

            public class PointR
            {
                public int X { get; set; }
                public int Y { get; set; }
                public int relation { get; set; }



                public PointR clone()
                {
                    return new PointR(this.X, this.Y);
                }



                public PointR(int x, int y, int relation = 0)
                {
                    X = x;
                    Y = y;
                    this.relation = relation;

                }
                public static bool operator ==(PointR a, PointR b)
                {
                    if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                        return true;

                    if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                        return false;

                    return a.X == b.X && a.Y == b.Y;
                }

                public static bool operator !=(PointR a, PointR b)
                {
                    return !(a == b);
                }

                public override bool Equals(object obj)
                {
                    if (obj == null || !(obj is PointR))
                        return false;

                    PointR point = (PointR)obj;
                    return X == point.X && Y == point.Y;
                }

                public override int GetHashCode()
                {
                    return X.GetHashCode() ^ Y.GetHashCode();
                }


                public static PointR FromPoint(System.Drawing.Point point, int id = 0)
                {
                    return new PointR(point.X, point.Y);
                }
                public static Point ToPoint(PointR point)
                {
                    return new Point(point.X, point.Y);
                }
                public static int CrossProduct(PointR p1, PointR p2) { return p1.X * p2.Y - p2.X * p1.Y; }

                public static int DotProduct(PointR p1, PointR p2) { return p1.X * p2.X + p1.Y * p2.Y; }

            }

            public class Segment
            {



                public PointR ps;

                public int grubosc;


                public PointR pe;


                public int relation;

                public Segment(PointR pps, PointR ppe, int relation = 0,int grubosc=2)
                {
                    ps = pps;
                    pe = ppe;
                    this.relation = relation;
                    this.grubosc=grubosc;
                }

             
                public PointR FindNon(PointR point)
                {

                    if (ps == point)
                    {
                        return pe;
                    }

                    else if (pe == point)
                    {
                        return ps;
                    }

                    else
                    {
                        return null;
                    }
                }

                public bool CzyRowne(Segment other)
                {

                    if (other == null) return false;


                    bool startsAreEqual = (this.ps.X == other.ps.X) && (this.ps.Y == other.ps.Y);
                    bool endsAreEqual = (this.pe.X == other.pe.X) && (this.pe.Y == other.pe.Y);

                    return startsAreEqual && endsAreEqual;
                }


                public bool PBS(PointR point)
                {

                    bool isSameAsStart = (point.X == ps.X) && (point.Y == ps.Y);
                    bool isSameAsEnd = (point.X == pe.X) && (point.Y == pe.Y);

                    return isSameAsStart || isSameAsEnd;
                }
                public PointR getrowne(PointR targetPoint)
                {
                    if (ps.X == targetPoint.X && ps.Y == targetPoint.Y)
                    {
                        return ps;
                    }
                    else if (pe.X == targetPoint.X && pe.Y == targetPoint.Y)
                    {
                        return pe;
                    }
                    else
                    {
                        return null;
                    }
                }
                public void changetoProsto()
                {
                    relation = 1;
                    pe.X = ps.Y;
                }
            }

        }

        public List<PointR> Mapowanie(List<Segment> segments)
        {
            HashSet<PointR> points = new HashSet<PointR>();

            foreach (var segment in segments)
            {
             
                points.Add(segment.ps);
                points.Add(segment.pe);
            }

           
            return new List<PointR>(points);
        }
        
        private Graphics g;
        private bool paint = false;
        private Pen p = new Pen(Color.Black, 2);
        private Pen p1 = new Pen(Color.Red, 2);
        private int index;
        private PointR px, py;
        private SolidBrush brush = new SolidBrush(Color.Black);

        // listy 
        private List<PointR> points = new List<PointR>();
        public List<Geometry.Segment> segments = new List<Geometry.Segment>();
        public List<figure> figures = new List<figure>();


        //zmienne 
        public int numberoffigures = 0;
        public int pointid = 0;
        public int algorytm = 0;

        //przesuwanie punktu
        public PointR catched;
        public bool zlapane;
        public Segment a, b;
        public PointR a1, b1;
        public PointR a11, b11;


        //logika rysowania wylaczenia i uzupelnienia do figury
        public bool skonczone = true;

        //przesuwanie figury
        figure tomove;
        public PointR catchedfigure;
        public bool zlapanefigure;

        //segmenty dodawanie punktu 
        Segment dodaniepunktu;
        int szerotoczki = 5;
        //przesuwanie lini
        Segment przesun;
        Segment adjacentSegment1;
        Segment adjacentSegment2;
        bool zlapanesegment;
        PointR punkcik;
        PointR punkcik1, punkcik2;
        figure przesunfigure;
        Segment adjact11;
        Segment adjact22;

        PointR adjacentpoint1przy = null;
        PointR adjacentpoint2przy = null;
        PointR adjacentpoint1nprzy = null;
        PointR adjacentpoint2nprzy = null;
        PointR adjacentpoint2nprzyzpoints = null;
        PointR adjacentpoint1nprzyzpoints = null;
        // prostopadla linia
        Segment prostokrawedz;

        //prostopadla dodawanie pointu 
        PointR catchedcopy;
        figure zapisogolny;
        bool temporarySegmentCreated = false;
        Segment nowysegment = null; // przechowuje referencję do tymczasowego segmentu
        PointR nowypunkcior = null;
        PointR zdrowie = null;

        // rownoleglosc
        Segment rownoleglo;

        Segment zmienszeroko;

        // otoczka

        int otoczka = 0;

        public Form1()
        {
            index = 1;
            InitializeComponent();
        }

        public (Segment aa, Segment bb) sasiedzi(Segment c, figure zapis)
        {
            //Segment aa; Segment bb;
            if (zapis != null)
            {
                foreach (var seg in zapis.segments)
                {
                    if (seg.PBS(c.ps) && !seg.PBS(c.pe))
                    {
                        a = seg;
                    }
                    if (seg.PBS(c.pe) && !seg.PBS(c.ps))
                    {
                        b = seg;
                    }
                }
            }
            return (a, b);
        }
        private void pict_MouseDown(object sender, MouseEventArgs e)
        {
           
            px = PointR.FromPoint(e.Location, pointid);
            switch (index)
            {
                case 1:
                    {
                        paint = true;


                        if (points.Count == 0)
                        {
                            points.Add(px);
                            skonczone = false;

                        }
                        else if (isClose(px, points[0]))
                        {
                            paint = false;

                            segments.Add(new Segment(new PointR(points[points.Count - 1].X, points[points.Count - 1].Y), new PointR(points[0].X, points[0].Y)));// Dodajemy linię do pierwszego punktu
                            pict.Invalidate();


                            figures.Add(new figure(points, segments, numberoffigures));
                            numberoffigures++;
                            // resetowanie list oraz dodanie do figury
                            points = new List<PointR>();
                            segments = new List<Segment>();
                            pointid = 0;
                            skonczone = true;

                        }
                        else
                        {

                            points.Add(px); // wazna kolejnosc bo indeksuje od 0;

                            segments.Add(new Segment(new PointR(points[points.Count - 2].X, points[points.Count - 2].Y), new PointR(px.X, px.Y)));
                            // pointid++;
                            pict.Invalidate();
                        }
                        break;
                    } // tworzenie figury

                case 2:
                    {


                        bool trafione = false;
                        PointR todelete = new PointR(0, 0);

                        figure zapis = null;

                        foreach (var figur in figures)
                        {
                            foreach (var points in figur.points)
                            {
                                if (isClose(points, px))
                                {
                                    todelete = points; trafione = true;
                                    zapis = figur;

                                }
                            }


                        }

                        if (trafione)
                        {


                            zapis.points.Remove(todelete);
                            if (zapis.points.Count() >= 3)
                            {
                                Segment a = zapis.segments.Find(x => (x.ps.X == todelete.X && x.ps.Y == todelete.Y) || (x.pe.X == todelete.X && x.pe.Y == todelete.Y));

                                
                                zapis.segments.Remove(a);


                                Segment b = zapis.segments.Find(x => (x.ps.X == todelete.X && x.ps.Y == todelete.Y) || (x.pe.X == todelete.X && x.pe.Y == todelete.Y));

                                zapis.segments.Remove(b);

                                PointR pointX = null;
                                PointR pointY = null;

                                if (b != null)
                                {
                                    pointX = (b.ps.X == todelete.X && b.ps.Y == todelete.Y) ? b.pe : b.ps;
                                }

                                if (a != null)
                                {
                                    pointY = (a.ps.X == todelete.X && a.ps.Y == todelete.Y) ? a.pe : a.ps;
                                }

                                Segment newSegment = new Segment(pointX, pointY);
                                if (zapis.segments.Count >= 2) zapis.segments.Add(newSegment);
                            }
                            else
                            {
                                figures.Remove(zapis);
                                numberoffigures--;
                            }
                            pict.Refresh();
                        }

                        break;
                    } //usuwanie


                case 3:// przesun punkt 
                    {
                        if (figures.Count != 0)
                        {
                            bool trafione = false;
                            PointR todelete = new PointR(0, 0);
                            figure zapis = null;


                            foreach (var figur in figures)
                            {
                                foreach (var points in figur.points)
                                {
                                    if (isClose(points, px))
                                    {
                                        todelete = points; trafione = true; zapis = figur;
                                        zapisogolny = figur;
                                    }
                                }


                            }

                            if (trafione)
                            {
                                zlapane = true;
                                catched = todelete;

                                catchedcopy = new PointR(catched.X, catched.Y, catched.relation);

                                a = zapis.segments.Find(x =>
                                (x.ps.X == todelete.X && x.ps.Y == todelete.Y) ||
                                (x.pe.X == todelete.X && x.pe.Y == todelete.Y));

                                int firstIndex = zapis.segments.FindIndex(x =>
                                    (x.ps.X == todelete.X && x.ps.Y == todelete.Y) ||
                                    (x.pe.X == todelete.X && x.pe.Y == todelete.Y));


                                b = null;
                                if (firstIndex != -1 && firstIndex < zapis.segments.Count - 1)
                                {
                                    b = zapis.segments.Skip(firstIndex + 1).ToList().Find(x =>
                                        (x.ps.X == todelete.X && x.ps.Y == todelete.Y) ||
                                        (x.pe.X == todelete.X && x.pe.Y == todelete.Y));
                                }


                                a1 = a.FindNon(catched);
                                b1 = b.FindNon(catched);
                                a1 = zapisogolny.points.Find(x => x == a1);
                                b1 = zapisogolny.points.Find(x => x == b1);
                                a11 = zapisogolny.segments.Find(x => x.PBS(a1) && !x.PBS(catched)).getrowne(a1);
                                b11 = zapisogolny.segments.Find(x => x.PBS(b1) && !x.PBS(catched)).getrowne(b1);

                            }
                        }
                        pict.Refresh();
                        break;
                    }

                case 4: // figura
                    {


                        foreach (var figur in figures)
                        {
                            int centroidX = figur.points.Sum(p => p.X) / figur.points.Count;
                            int centroidY = figur.points.Sum(p => p.Y) / figur.points.Count;
                            PointR centroid = new PointR(centroidX, centroidY);
                            var sortedPoints = figur.points.OrderBy(p => Math.Atan2(p.Y - centroid.Y, p.X - centroid.X)).ToList();


                            if (IsPointInPolygon4(sortedPoints.ToArray(), px))
                            {
                                tomove = figur;
                                catchedfigure = px;
                                zlapanefigure = true;
                                break;
                            }

                        }




                        break;
                    }


                case 5: //dodanie punktu na krawedzi 
                    {
                        bool c = false;
                        figure zapis = null;
                        foreach (var figur in figures)
                        {
                            foreach (var seg in figur.segments)
                            {
                                if (czyblisko(px, seg, 10))
                                {
                                    dodaniepunktu = seg;
                                    c = true;
                                    zapis = figur;
                                    break;

                                }
                            }
                        }


                        if (c)
                        {
                            PointR a = new PointR(dodaniepunktu.ps.X, dodaniepunktu.ps.Y);
                            PointR b = new PointR(dodaniepunktu.pe.X, dodaniepunktu.pe.Y);

                            zapis.segments.Remove(dodaniepunktu);

                            zapis.points.Add(px);
                            zapis.segments.Add(new Segment(a.clone(), px.clone()));
                            zapis.segments.Add(new Segment(b.clone(), px.clone()));


                            zapis = null;
                            dodaniepunktu = null;
                            c = false;
                            px = null;
                        }
                        pict.Refresh();
                        break;
                    }


                case 6:  // zlapanie krawedzi przsuwanie
                    {
                        zlapanesegment = false;
                        figure zapis = null;
                        foreach (var figur in figures)
                        {
                            foreach (var seg in figur.segments)
                            {
                                if (czyblisko(px, seg, 10))
                                {
                                    punkcik = px;
                                    przesun = seg;
                                    zlapanesegment = true;
                                    przesunfigure = figur;
                                    zapis = figur;

                                    break;

                                }
                            }



                        }


                        if (zlapanesegment)
                        {
                            PointR point1 = przesun.ps;
                            PointR point2 = przesun.pe;
                            //pointy w pointsach segmetu zlapanego
                            punkcik1 = zapis.points.Find(x => x.X == point2.X && x.Y == point2.Y);
                            punkcik2 = zapis.points.Find(x => x.X == point1.X && x.Y == point1.Y);

                            adjacentSegment1 = null;
                            adjacentSegment2 = null;


                            // znalezienie dwóc segmentów sasiadów a i b
                            foreach (var segment in zapis.segments)
                            {


                                if (segment.PBS(point2) && !segment.PBS(point1))
                                {


                                    adjacentSegment2 = segment;

                                }
                                if (segment.PBS(point1) && !segment.PBS(point2))
                                {
                                    adjacentSegment1 = segment;
                                }



                            }

                            adjacentpoint1przy = adjacentSegment1.getrowne(point1);
                            adjacentpoint2przy = adjacentSegment2.getrowne(point2);
                            adjacentpoint1nprzy = adjacentSegment1.FindNon(adjacentpoint1przy);
                            adjacentpoint2nprzy = adjacentSegment2.FindNon(adjacentpoint2przy);
                            adjacentpoint2nprzyzpoints = zapis.points.Find(x => x == adjacentpoint2nprzy);
                            adjacentpoint1nprzyzpoints = zapis.points.Find(x => x == adjacentpoint1nprzy);

                            if (adjacentSegment1.relation == 1 || adjacentSegment1.relation == 2)
                            {
                                adjact11 = zapis.segments.Find(x => x.PBS(adjacentpoint1nprzy) && !x.PBS(adjacentpoint1przy));
                            }
                            if (adjacentSegment2.relation == 1 || adjacentSegment2.relation == 2)
                            {
                                adjact22 = zapis.segments.Find(x => x.PBS(adjacentpoint2nprzy) && !x.PBS(adjacentpoint2przy));
                            }



                        }
                        pict.Refresh();

                        break;
                    }

                case 7:  // ustaw prostokatna
                    {
                        if (figures.Count != 0)
                        {
                            bool c = false;
                            figure zapis = null;
                            foreach (var figur in figures)
                            {
                                foreach (var seg in figur.segments)
                                {
                                    if (czyblisko(px, seg, 10))
                                    {
                                        prostokrawedz = seg;
                                        c = true;
                                        zapis = figur;
                                        break;

                                    }
                                }

                            }
                            if (prostokrawedz != null)
                            {
                                Segment sasiad1; Segment sasiad2;
                                (sasiad1, sasiad2) = sasiedzi(prostokrawedz, zapis);
                                if (zapis.segments.Count >= 3)
                                {
                                    if (sasiad1.relation == 1 || sasiad2.relation == 1)
                                    {
                                        c = false;
                                    }
                                }


                                if (c) 
                                {

                                    PointR obokpunkt;
                                   
                                    Segment krawedzobok = zapis.segments.Find(x => x.PBS(prostokrawedz.ps) && !x.PBS(prostokrawedz.pe));
                                    if (krawedzobok.ps.X == prostokrawedz.ps.X && krawedzobok.ps.Y == prostokrawedz.ps.Y)
                                    {
                                        obokpunkt = krawedzobok.ps;

                                    }
                                    else
                                    {
                                        obokpunkt = krawedzobok.pe;
                                    }
                                    

                                    PointR pointus = zapis.points.Find(x => x.X == prostokrawedz.ps.X && x.Y == prostokrawedz.ps.Y);
                                    pointus.X = prostokrawedz.pe.X;
                                    pointus.relation = 1;
                                    PointR pointus1 = zapis.points.Find(x => x.X == prostokrawedz.pe.X && x.Y == prostokrawedz.pe.Y);
                                    pointus1.relation = 1;
                                    // z wszystkich punktow wez tego który jest startowy i zmien jego x na x drugiego punktu krawędzi 
                                    obokpunkt.X = prostokrawedz.pe.X;

                                    // zmien x krawędzi krawędzi wybranej 

                                    prostokrawedz.ps.X = prostokrawedz.pe.X;

                                    prostokrawedz.relation = 1;
                                    prostokrawedz.ps.relation = 1;
                                    prostokrawedz.pe.relation = 1;


                                    //znajdz punkt ktory przesunelem


                                }
                            }
                        }
                        prostokrawedz = null;
                        break;



                    }
                case 8:  // ustaw prostokatna uusun
                    {
                        if (figures.Count != 0)
                        {
                            bool c = false;
                            figure zapis = null;
                            foreach (var figur in figures)
                            {
                                foreach (var seg in figur.segments)
                                {
                                    if (czyblisko(px, seg, 10))
                                    {
                                        prostokrawedz = seg;
                                        c = true;
                                        zapis = figur;
                                        break;

                                    }
                                }

                            }


                            if (c && prostokrawedz.relation == 1) // jesli krawedz kliknieta
                            {










                                prostokrawedz.relation = 0;
                                prostokrawedz.ps.relation = 0;
                                prostokrawedz.pe.relation = 0;





                            }
                        }
                        pict.Refresh();
                        prostokrawedz = null;
                        break;



                    }

                case 9: // ustaw rownolegle
                    {
                        if (figures.Count != 0)
                        {
                            bool c = false;
                            figure zapis = null;
                            foreach (var figur in figures)
                            {
                                foreach (var seg in figur.segments)
                                {
                                    if (czyblisko(px, seg, 10))
                                    {
                                        rownoleglo = seg;
                                        c = true;
                                        zapis = figur;
                                        break;

                                    }
                                }

                            }
                            if (rownoleglo != null)
                            {
                                Segment sasiad1; Segment sasiad2;
                                (sasiad1, sasiad2) = sasiedzi(rownoleglo, zapis);
                                if (zapis.segments.Count >= 3)
                                {
                                    if (sasiad1.relation == 2 || sasiad2.relation == 2)
                                    {
                                        c = false;
                                    }
                                }


                                if (c) // jesli krawedz kliknieta
                                {

                                    PointR obokpunkt;
                                    
                                    Segment krawedzobok = zapis.segments.Find(x => x.PBS(rownoleglo.ps) && !x.PBS(rownoleglo.pe));
                                    if (krawedzobok.ps.X == rownoleglo.ps.X && krawedzobok.ps.Y == rownoleglo.ps.Y)
                                    {
                                        obokpunkt = krawedzobok.ps;

                                    }
                                    else
                                    {
                                        obokpunkt = krawedzobok.pe;
                                    }
                                    // wyznacz ten punkt oc jest taki samy jak ten startowy

                                    PointR pointus = zapis.points.Find(x => x.X == rownoleglo.ps.X && x.Y == rownoleglo.ps.Y);
                                    pointus.Y = rownoleglo.pe.Y;
                                    pointus.relation = 2;
                                    PointR pointus1 = zapis.points.Find(x => x.X == rownoleglo.pe.X && x.Y == rownoleglo.pe.Y);
                                    pointus1.relation = 2;
                                    // z wszystkich punktow wez tego który jest startowy i zmien jego x na x drugiego punktu krawędzi 
                                    obokpunkt.Y = rownoleglo.pe.Y;

                                    // zmien x krawędzi krawędzi wybranej 

                                    rownoleglo.ps.Y = rownoleglo.pe.Y;

                                    rownoleglo.relation = 2;
                                    rownoleglo.ps.relation = 2;
                                    rownoleglo.pe.relation = 2;


                                    //znajdz punkt ktory przesunelem


                                }
                            }
                        }
                        //rownoleglo = null;
                        pict.Refresh();
                        break;



                    }

                case 10:
                    {
                        Segment rowno = null;
                        if (figures.Count != 0)
                        {
                            bool c = false;
                            figure zapis = null;
                            foreach (var figur in figures)
                            {
                                foreach (var seg in figur.segments)
                                {
                                    if (czyblisko(px, seg, 10))
                                    {
                                        rowno = seg;
                                        c = true;
                                        zapis = figur;
                                        break;

                                    }
                                }

                            }


                            if (c && rowno.relation == 2) // jesli krawedz kliknieta
                            {



                                rowno.relation = 0;
                                rowno.ps.relation = 0;
                                rowno.pe.relation = 0;





                            }
                        }

                        rowno = null;

                        break;
                    }

                case 11:
                    {

                        if (figures.Count != 0)
                        {
                            bool c = false;
                            figure zapis = null;
                            foreach (var figur in figures)
                            {
                                foreach (var seg in figur.segments)
                                {
                                    if (czyblisko(px, seg, 10))
                                    {
                                        zmienszeroko = seg;
                                        c = true;
                                        zapis = figur;
                                        break;

                                    }
                                }

                               

                            }


                            if(c)
                            {
                                zmienszeroko.grubosc = grubosc.Value;
                            }
                        }

                        pict.Refresh();
                            break;
                    }
            }
        }

        private void pict_MouseMove(object sender, MouseEventArgs e)
        {

            py = PointR.FromPoint(e.Location);
            label1.Text = string.Format("{0,0:F3}, {1,0:F3}", py.X, py.Y);
            pict.Refresh();


            switch (index)
            {

                case 3:
                    {
                        if (zlapane)
                        {

                            if (b != null)
                            {
                                if (b.relation == 0)
                                {
                                    if (b.ps.X == catched.X && b.ps.Y == catched.Y)
                                    {
                                        b.ps = new PointR(py.X, py.Y);

                                    }
                                    else if (b.pe.X == catched.X && b.pe.Y == catched.Y)
                                    {
                                        b.pe = new PointR(py.X, py.Y);
                                    }
                                }

                                if (b.relation == 1)
                                {
                                    if (b.ps.Y == catched.Y)
                                    {



                                        b.ps = new PointR(py.X, py.Y);
                                        b.pe = new PointR(py.X, b.pe.Y);
                                        // b1 = new PointR(py.X, b.pe.Y);
                                        b1.X = py.X;
                                        b1.Y = b.pe.Y;
                                        b11.X = py.X;
                                        b11.Y = b.pe.Y;
                                    }
                                    else if (b.pe.Y == catched.Y)
                                    {
                                        // nowypunkcior.Y = py.Y;
                                        b.pe = new PointR(py.X, py.Y);
                                        b.ps = new PointR(py.X, b.ps.Y);
                                        b1.X = py.X;
                                        b1.Y = b.ps.Y;
                                        b11.X = py.X;
                                        b11.Y = b.ps.Y;

                                    }
                                }
                                if (b.relation == 2)
                                {

                                    if (b.ps.X == catched.X)
                                    {
                                        b.ps = new PointR(py.X, py.Y);
                                        b.pe = new PointR(b.pe.X, py.Y); 
                                        b1.X = b.pe.X;
                                        b1.Y = py.Y;
                                        b11.X = b.pe.X;
                                        b11.Y = py.Y;
                                    }
                                    else if (b.pe.X == catched.X)
                                    {
                                        b.pe = new PointR(py.X, py.Y);
                                        b.ps = new PointR(b.ps.X, py.Y); 
                                        b1.X = b.ps.X;
                                        b1.Y = py.Y;
                                        b11.X = b.ps.X;
                                        b11.Y = py.Y;
                                    }

                                }

                            }



                            if (a != null)
                            {
                                if (a.relation == 0)
                                {
                                    if (a.ps.X == catched.X && a.ps.Y == catched.Y)
                                    {


                                        a.ps = new PointR(py.X, py.Y);

                                    }
                                    else if (a.pe.X == catched.X && a.pe.Y == catched.Y)
                                    {
                                        a.pe = new PointR(py.X, py.Y);


                                    }
                                }

                                if (a.relation == 1)
                                {
                                    if (a.ps.Y == catched.Y)
                                    {


                                        a.ps = new PointR(py.X, py.Y);

                                        a.pe = new PointR(py.X, a.pe.Y);
                                        a1.X = py.X;
                                        a1.Y = a.pe.Y;
                                        a11.X = py.X;
                                        a11.Y = a.pe.Y;


                                    }
                                    else if (a.pe.Y == catched.Y)
                                    {

                                        a.pe = new PointR(py.X, py.Y);
                                        a.ps = new PointR(py.X, a.ps.Y);
                                        a1.X = py.X;
                                        a1.Y = a.ps.Y;
                                        a11.X = py.X;
                                        a11.Y = a.ps.Y;

                                    }
                                }

                                if (a.relation == 2)
                                {
                                    if (a.ps.X == catched.X)
                                    {
                                        a.ps = new PointR(py.X, py.Y);
                                        a.pe = new PointR(a.pe.X, py.Y);
                                        a1.X = a.pe.X;
                                        a1.Y = py.Y;
                                        a11.X = a.pe.X;
                                        a11.Y = py.Y;
                                    }
                                    else if (a.pe.X == catched.X)
                                    {
                                        a.pe = new PointR(py.X, py.Y);
                                        a.ps = new PointR(a.ps.X, py.Y);
                                        a1.X = a.ps.X;
                                        a1.Y = py.Y;
                                        a11.X = a.ps.X;
                                        a11.Y = py.Y;
                                    }
                                }

                            }




                          
                            catched.X = py.X;
                            catched.Y = py.Y;

                           
                            pict.Refresh();

                        }

                        break;
                    }


                case 4:
                    {
                        if (zlapanefigure)
                        {
                           
                            int deltaX = py.X - catchedfigure.X;
                            int deltaY = py.Y - catchedfigure.Y;


                            foreach (var point in tomove.points)
                            {
                                point.X += deltaX;
                                point.Y += deltaY;
                            }


                            foreach (var segment in tomove.segments)
                            {
                                segment.ps.X += deltaX;
                                segment.ps.Y += deltaY;
                                segment.pe.X += deltaX;
                                segment.pe.Y += deltaY;
                            }


                            catchedfigure.X = py.X;
                            catchedfigure.Y = py.Y;


                        }
                        pict.Invalidate();
                        break;
                    }



                case 6:
                    {

                        if (zlapanesegment)
                        {
                            //roznica pomiedzy kliknietym a gdzie teraz jest 
                            int deltaX = py.X - punkcik.X;
                            int deltaY = py.Y - punkcik.Y;

                            adjacentpoint1przy.X += deltaX;
                            adjacentpoint1przy.Y += deltaY;
                            adjacentpoint2przy.X += deltaX;
                            adjacentpoint2przy.Y += deltaY;


                            if (adjacentSegment1.relation == 1)
                            {
                                adjact11.getrowne(adjacentpoint1nprzy).X = adjacentpoint1przy.X;
                                adjacentpoint1nprzy.X = adjacentpoint1przy.X;

                                adjacentpoint1nprzyzpoints.X = adjacentpoint1przy.X;

                            }

                            if (adjacentSegment2.relation == 1)
                            {
                                adjact22.getrowne(adjacentpoint2nprzy).X = adjacentpoint2przy.X;
                                adjacentpoint2nprzy.X = adjacentpoint2przy.X;

                                adjacentpoint2nprzyzpoints.X = adjacentpoint2przy.X;

                            }

                            if (adjacentSegment1.relation == 2)
                            {
                                adjact11.getrowne(adjacentpoint1nprzy).Y = adjacentpoint1przy.Y;
                                adjacentpoint1nprzy.Y = adjacentpoint1przy.Y;
                                adjacentpoint1nprzyzpoints.Y = adjacentpoint1przy.Y;
                            }

                            if (adjacentSegment2.relation == 2)
                            {
                                adjact22.getrowne(adjacentpoint2nprzy).Y = adjacentpoint2przy.Y;
                                adjacentpoint2nprzy.Y = adjacentpoint2przy.Y;
                                adjacentpoint2nprzyzpoints.Y = adjacentpoint2przy.Y;
                            }


                            przesun.ps.X += deltaX;
                            przesun.pe.Y += deltaY;
                            przesun.ps.Y += deltaY;
                            przesun.pe.X += deltaX;
                            //punkcik1 i punkcik2 to sa chyba punkty 
                            if (punkcik2 != null && punkcik1 != null)
                            {
                                punkcik2.X += deltaX;
                                punkcik2.Y += deltaY;
                                punkcik1.Y += deltaY;
                                punkcik1.X += deltaX;
                            }






                            // zeby nie przesuwac o kwadrat itd coraz wiecej
                            punkcik.X = py.X;
                            punkcik.Y = py.Y;

                        }
                        pict.Invalidate();
                        break;
                    }
            }

        }

        private void Uprade(PointR point, PointR targetPoint, int deltaX, int deltaY)
        {
            if (point.X == targetPoint.X && point.Y == targetPoint.Y)
            {
                point.X += deltaX;
                point.Y += deltaY;
            }
        }
        private void pict_MouseUp(object sender, MouseEventArgs e)
        {
            switch (index)
            {
                case 3:
                    {
                        if (catched != null)
                        {
                            // zdobycie jednego z punktów tego przeuswanego zebhy on spowrotem skoczyl na miejsce nablizszej krawędzi 
                            PointR point1 = a.getrowne(catched);
                            PointR point2 = b.getrowne(catched);

                            if (a != null && a.relation == 1)
                            {

                                double distanceToAStart = CalculateDistance(PointR.FromPoint(e.Location), a.ps);
                                double distanceToAEnd = CalculateDistance(PointR.FromPoint(e.Location), a.pe);




                                if (distanceToAStart < 30)
                                {

                                    catched.X = a.ps.X;
                                    catched.Y = a.ps.Y;
                                    point2.X = a.ps.X;
                                    point2.Y = a.ps.Y;
                                    zapisogolny.points.Remove(nowypunkcior);
                                    zapisogolny.segments.Remove(nowysegment);
                                }
                                else if (distanceToAEnd < 30)
                                {

                                    catched.X = a.pe.X;
                                    catched.Y = a.pe.Y;
                                    point2.X = a.pe.X;
                                    point2.Y = a.pe.Y;
                                    zapisogolny.points.Remove(nowypunkcior);
                                    zapisogolny.segments.Remove(nowysegment);
                                }

                            }

                            if (b != null && b.relation == 1)
                            {

                                double distanceToBStart = CalculateDistance(PointR.FromPoint(e.Location), b.ps);
                                double distanceToBEnd = CalculateDistance(PointR.FromPoint(e.Location), b.pe);


                                if (distanceToBStart < 30)
                                {

                                    catched.X = b.ps.X;
                                    catched.Y = b.ps.Y;
                                    point1.X = b.ps.X;
                                    point1.Y = b.ps.Y;
                                    zapisogolny.points.Remove(nowypunkcior);
                                    zapisogolny.segments.Remove(nowysegment);
                                    //
                                }
                                else if (distanceToBEnd < 30)
                                {

                                    catched.X = b.pe.X;
                                    catched.Y = b.pe.Y;
                                    point1.X = b.pe.X;
                                    point1.Y = b.pe.Y;
                                    zapisogolny.points.Remove(nowypunkcior);
                                    zapisogolny.segments.Remove(nowysegment);
                                }

                            }
                        }
                        zlapane = false;
                        catched = null;
                        a = null;
                        b = null;
                        zapisogolny = null;
                        catchedcopy = null;



                        break;
                    }
                case 4:
                    {
                        if (zlapanefigure)
                        {
                            zlapanefigure = false;
                            catchedfigure = null;
                            tomove = null;
                        }
                        break;
                    }
                case 6:
                    {
                        if (zlapanesegment)
                        {
                            zlapanesegment = false;
                        }
                        break;
                    }
            }
        }


      


        public double CalculateDistance(PointR point1, PointR point2)
        {
            return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            index = 1;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

            index = 2;
            przerwaniepracy1();
        }

        private void przesunp_CheckedChanged(object sender, EventArgs e)
        {
            index = 3;
            przerwaniepracy1();
        }

        private void figuraprzesun_CheckedChanged(object sender, EventArgs e)
        {
            index = 4;
            przerwaniepracy1();

        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            index = 5;
            przerwaniepracy1();
        }

        private void przesunkrawedz_CheckedChanged(object sender, EventArgs e)
        {
            index = 6;
            przerwaniepracy1();
        }

        private void prosto_CheckedChanged(object sender, EventArgs e)
        {
            index = 7;
            przerwaniepracy1();
        }


        public void przerwaniepracy1()
        {
            if (index != 1 && !skonczone)
            {
                if (segments.Count <= 1)
                {
                    points = new List<PointR>();
                    segments = new List<Segment>();
                    pointid = 0;
                    skonczone = true;
                    pict.Invalidate();
                    return;
                }
                segments.Add(new Segment(new PointR(points[points.Count - 1].X, points[points.Count - 1].Y), new PointR(points[0].X, points[0].Y)));
                pict.Invalidate();

                figures.Add(new figure(points, segments, numberoffigures));
                numberoffigures++;


                points = new List<PointR>();
                segments = new List<Segment>();
                pointid = 0;
                skonczone = true;
            }
        }
        private void DrawPixel(Graphics g, Pen pen, int x, int y)
        {

            g.FillRectangle(pen.Brush, x, y, 1, 1);
        }
        private void plotLine(Graphics g, Segment line)
        {
            if (Math.Abs(line.pe.Y - line.ps.Y) < Math.Abs(line.pe.X - line.ps.X))
            {
                if (line.ps.X > line.pe.X)
                    MidpointLine(g, new PointR(line.pe.X, line.pe.Y), new PointR(line.ps.X, line.ps.Y), line.grubosc);
                else
                    MidpointLine(g, new PointR(line.ps.X, line.ps.Y), new PointR(line.pe.X, line.pe.Y), line.grubosc);
            }
            else
            {
                if (line.ps.Y > line.pe.Y)
                    MidpointLineUP(g, new PointR(line.pe.X, line.pe.Y), new PointR(line.ps.X, line.ps.Y), line.grubosc);
                else
                    MidpointLineUP(g, new PointR(line.ps.X, line.ps.Y), new PointR(line.pe.X, line.pe.Y), line.grubosc);
            }
        }
        private void MidpointLine(Graphics g, PointR a, PointR b, int  szeroko)
        {

            int x1 = a.X;
            int x2 = b.X;
            int y1 = a.Y;
            int y2 = b.Y;


            

            int dx = x2 - x1;
            int dy = y2 - y1;
            int d = 2 * dy - dx;
            int y0 = 1;
            if (dy < 0)
            {
                y0 = -1;
                dy = -dy;
            }
            int incrE = 2 * dy; // 
            int incrNE = 2 * (dy - dx);
            int x = x1;
            int y = y1;

            DrawPixel(g, p, x, y);

            while (x < x2)
            {
                if (d < 0)
                {
                    d += incrE;
                    x++;
                }
                else
                {
                    d += incrNE;
                    x++;
                    y = y0 + y;
                }
                int n = (szeroko - 1) / 2;
               for(int i=-n;i<=n;i++) // 3 3-1=2   i=-1 i=0 i=1
                {
                    DrawPixel(g, p, x, y+i);
                }
                
            }
        }
       //wikipedia
       private void MidpointLineUP(Graphics g, PointR a, PointR b, int szeroko)
        {

            int x1 = a.X;
            int x2 = b.X;
            int y1 = a.Y;
            int y2 = b.Y;


            ;

            int dx = x2 - x1;
            int dy = y2 - y1;
            int d = 2 * dx - dy;
            int x0 = 1;
            if (dx < 0)
            {
                x0 = -1;
                dx = -dx;
            }
            int incrE = 2 * dx;
            int incrNE = 2 * (dx - dy);
            int x = x1;
            int y = y1;

            DrawPixel(g, p, x, y);

            while (y < y2)
            {
                if (d < 0)
                {
                    d += incrE;
                    y++;
                }
                else
                {
                    d += incrNE;
                    y++;
                    x = x0 + x;
                }


                int n = (szeroko - 1) / 2;
                for (int i = -n; i <= n; i++) // 3 3-1=2   i=-1 i=0 i=1
                {
                    DrawPixel(g, p, x+i , y);
                }
            }
        }

        private void plotLinesy(Graphics g, Segment line)
        {
            if (Math.Abs(line.pe.Y - line.ps.Y) < Math.Abs(line.pe.X - line.ps.X))
            {
                if (line.ps.X > line.pe.X)
                    MidpointLinesy(g, new PointR(line.pe.X, line.pe.Y), new PointR(line.ps.X, line.ps.Y), line.grubosc);
                else
                    MidpointLinesy(g, new PointR(line.ps.X, line.ps.Y), new PointR(line.pe.X, line.pe.Y), line.grubosc);
            }
            else
            {
                if (line.ps.Y > line.pe.Y)
                    MidpointLineUPsy(g, new PointR(line.pe.X, line.pe.Y), new PointR(line.ps.X, line.ps.Y), line.grubosc);
                else
                    MidpointLineUPsy(g, new PointR(line.ps.X, line.ps.Y), new PointR(line.pe.X, line.pe.Y), line.grubosc);
            }
        }
        private void MidpointLinesy(Graphics g, PointR a, PointR b, int szeroko)
        {
            int x1 = a.X;
            int x2 = b.X;
            int y1 = a.Y;
            int y0 = 1;
            int y2 = b.Y;
            int dx = x2 - x1;
            int dy = y2 - y1;
            if (dy < 0)
            {
                y0 = -1;
                dy = -dy;
            }
            int incrE = 2 * dy;
            int incrNE = 2 * (dy - dx);
            int d = 2 * dy - dx;
            int xf = x1; int yf = y1;
            int xb = x2; int yb = y2;
            DrawPixel(g, p, xf, yf);
            DrawPixel(g, p, xb, yb);
           
            

            while (xf < xb)
            {
                xf++; xb--;
                if (d < 0) //Choose E and W
                    d += incrE;
                else //Choose NE and SW
                {
                    d += incrNE;
                    yf +=y0 ;
                    yb -= y0;
                }

                int n = (szeroko - 1) / 2;
                for (int i = -n; i <= n; i++) // 3 3-1=2   i=-1 i=0 i=1
                {
                    DrawPixel(g, p, xf, yf+i );
                    DrawPixel(g, p, xb, yb+i );
                }
            }

           
        }
        //wikipedia
        private void MidpointLineUPsy(Graphics g, PointR a, PointR b, int szeroko)
        {
            int x1 = a.X;
            int x2 = b.X;
            int y1 = a.Y;
            int y2 = b.Y;
            int x0 = 1;
            int dx = x2 - x1;
            int dy = y2 - y1;
            if (dx < 0)
            {
                x0 = -1;
                dx = -dx;
            }
            int d = 2 * dx - dy;
            int incrE = 2 * dx;
            int incrNE = 2 * (dx - dy);
            int yf = y1;
            int yb = y2;
            int xf = x1;
            int xb = x2;
           

            

            DrawPixel(g, p, xf, yf);
            DrawPixel(g, p, xb, yb);

            while (yf < yb)
            {
                if (d < 0)
                {
                    d += incrE;
                    yf++;
                    yb--;
                }
                else
                {
                    d += incrNE;
                    yf++;
                    yb--;
                    xf += x0;
                    xb -= x0;
                }
                int n = (szeroko - 1) / 2;
                for (int i = -n; i <= n; i++) // 3 3-1=2   i=-1 i=0 i=1
                {
                    DrawPixel(g, p, xf+i, yf);
                    DrawPixel(g, p, xb+i, yb);
                }
               
               
            }
        }





        private void pict_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (var line in segments)
            {
                p.Width = line.grubosc;
                // double stala = (line.ps.Y - line.pe.Y) / (line.ps.X - line.pe.X);
                // label2.Text = string.Format("{}", stala);
                if (algorytm == 0)
                    e.Graphics.DrawLine(p, PointR.ToPoint(line.ps), PointR.ToPoint(line.pe));
                else if (algorytm == 1) plotLine(e.Graphics, line);
                else if (algorytm == 2) plotLinesy(e.Graphics, line);


            }

            if (paint && points.Count > 0)
            {
                if (index == 1)
                {
                    if (algorytm == 0)
                        e.Graphics.DrawLine(p, PointR.ToPoint(points[points.Count - 1]), PointR.ToPoint(py));

                    plotLine(e.Graphics, new Segment(new PointR(points[points.Count - 1].X, points[points.Count - 1].Y), new PointR(py.X, py.Y)));
                }
            }

            foreach (var point in points)
            {

                e.Graphics.DrawEllipse(p, point.X - 3, point.Y - 3, 6, 6);
            }

            foreach (var figur in figures)
            {
                if (otoczka == 2&&!duplikaty(figur.points))
                {

                    DrawOffsetPolygon(e.Graphics, PunktyZKrawedzi(figur.segments, figur.points), szerotoczki, e.Graphics);
                }
            }

            foreach (var figur in figures)
            {

                if (otoczka == 1 && !duplikaty(figur.points))
                {
                    List<PointR> points4 = new List<PointR>();
                    GraphicsPath path = new GraphicsPath();



                    figur.offsetpoints = OffsetPolygon(PunktyZKrawedzi(figur.segments, figur.points), szerotoczki);

                    if (figur.offsetpoints != null && figur.offsetpoints.Count > 1)
                    {



                        foreach (var point in figur.offsetpoints)
                        {
                            e.Graphics.DrawEllipse(p1, point.X - 3, point.Y - 3, 6, 6);
                        }
                        for (int i = 0; i < figur.offsetpoints.Count; i++)
                        {
                            PointR currentPoint = figur.offsetpoints[i];
                            PointR nextPoint = (i == figur.offsetpoints.Count - 1) ? figur.offsetpoints[0] : figur.offsetpoints[i + 1];
                            e.Graphics.DrawLine(p1, PointR.ToPoint(currentPoint), PointR.ToPoint(nextPoint));
                        }



                    }

                }

                if (otoczka == 2 && !duplikaty(figur.points))
                {
                    e.Graphics.FillPolygon(Brushes.Red, PunktyZKrawedzi(figur.segments, figur.points).Select(p => new PointF(p.X, p.Y)).ToArray());
                }




                foreach (var line in figur.segments)
                {
                    if (algorytm == 0)
                    {
                        p.Width = line.grubosc;
                        e.Graphics.DrawLine(p, PointR.ToPoint(line.ps), PointR.ToPoint(line.pe));
                        if (line.relation == 1)
                        {

                            float midX = (float)((line.ps.X + line.pe.X) / 2);
                            float midY = (float)((line.ps.Y + line.pe.Y) / 2);


                            float offsetX = 10;
                            float offsetY = 5;


                            PointF point1 = new PointF(midX + offsetX - 5, midY + offsetY);
                            PointF point2 = new PointF(midX + offsetX + 5, midY + offsetY);
                            PointF point3 = new PointF(midX + offsetX, midY + offsetY);
                            PointF point4 = new PointF(midX + offsetX, midY + offsetY - 10);




                            e.Graphics.DrawLine(p, point1, point2);
                            e.Graphics.DrawLine(p, point3, point4);

                        }

                        if (line.relation == 2)
                        {
                            float midX = (float)((line.ps.X + line.pe.X) / 2);
                            float midY = (float)((line.ps.Y + line.pe.Y) / 2);


                            float offsetX = 10;
                            float offsetY = 5;


                            PointF point1 = new PointF(midX + offsetX + 5, midY - offsetY + 10);
                            PointF point2 = new PointF(midX + offsetX + 5, midY + offsetY + 10);
                            PointF point3 = new PointF(midX + 2 * offsetX, midY - offsetY + 10);
                            PointF point4 = new PointF(midX + 2 * offsetX, midY + offsetY + 10);




                            e.Graphics.DrawLine(p, point1, point2);
                            e.Graphics.DrawLine(p, point3, point4);

                        }
                    }
                    else if(algorytm==1)
                        plotLine(e.Graphics, line);
                     else if(algorytm==2)
                        plotLinesy(e.Graphics, line);

                }
                foreach (var point in figur.points)
                {
                    p.Width = 2;
                    e.Graphics.DrawEllipse(p, point.X - 3, point.Y - 3, 6, 6);
                }
            }


        }


        private bool isClose(PointR a, PointR b)
        {
            return Math.Abs(a.X - b.X) < 5 && Math.Abs(a.Y - b.Y) < 10;
        }
        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            algorytm = 1;
            pict.Refresh();
        }



        private void resetbutton_Click(object sender, EventArgs e)
        {
            ResetState();
            pict.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Resetowanie stanu aplikacji
            ResetState();


            List<PointR> lista = new List<PointR>();

            points.Add(new PointR(200, 100));
            skonczone = false;
            points.Add(new PointR(500, 150)); // wazna kolejnosc bo indeksuje od 0;

            segments.Add(new Segment(new PointR(200, 100), new PointR(500, 150)));
            points.Add(new PointR(500, 700));
            segments.Add(new Segment(new PointR(500, 150), new PointR(500, 700), 1));
            points.Add(new PointR(150, 700));
            segments.Add(new Segment(new PointR(500, 700), new PointR(150, 700)));
            segments.Add(new Segment(new PointR(150, 700), new PointR(200, 100)));
            figures.Add(new figure(points, segments, numberoffigures));
            pict.Invalidate();
            numberoffigures++;

            // figures.Add(new figure(points, segments, numberoffigures));
            points = new List<PointR>();
            segments = new List<Segment>();
            pointid = 0;
            skonczone = true;
            paint = false;
            //    new PointR(600, 200), // górny punkt trójkąta
            //new PointR(650, 400), // prawy dolny punkt trójkąta
            //new PointR(550, 400)

            skonczone = false;
            lista = new List<PointR>();
            points.Add(new PointR(800, 200));
            points.Add(new PointR(1000, 400));
            // wazna kolejnosc bo indeksuje od 0;

            segments.Add(new Segment(new PointR(800, 200), new PointR(1000, 400)));
            points.Add(new PointR(650, 400));
            segments.Add(new Segment(new PointR(1000, 400), new PointR(650, 400), 2));

            segments.Add(new Segment(new PointR(650, 400), new PointR(800, 200)));

            figures.Add(new figure(points, segments, numberoffigures));
            pict.Invalidate();
            numberoffigures++;

            // figures.Add(new figure(points, segments, numberoffigures));
            points = new List<PointR>();
            segments = new List<Segment>();
            pointid = 0;
            skonczone = true;
            paint = false;


            pict.Refresh();
        }

        private void usunprostopadla_CheckedChanged(object sender, EventArgs e)
        {
            index = 8;
            przerwaniepracy1();
        }

        private void radioButton3_CheckedChanged_1(object sender, EventArgs e)
        {
            index = 9;
            przerwaniepracy1();
        }




        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

           
        }

        private void usunrownolegle_CheckedChanged(object sender, EventArgs e)
        {
            index = 10;
            przerwaniepracy1();
        }
        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            index = 11;
            przerwaniepracy1();
        }

        private void ResetState()
        {
            points = new List<PointR>();
            segments = new List<Geometry.Segment>();
            figures = new List<figure>();
            skonczone = true;
            index = 1;
            rysowanie.Checked = true;
            numberoffigures = 0;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            otoczka = 1;
            pict.Refresh();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            szerotoczki = trackBar1.Value * 5;
            pict.Refresh();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            algorytm = 0;
            pict.Refresh();
        }



        public static bool IsPointInPolygon4(PointR[] polygon, PointR testPoint)
        {
            PointR p1, p2;
            bool inside = false; 

          
            if (polygon.Length < 3)
            {
                return inside; 
            }

          
            var oldPoint = new PointR(
                polygon[polygon.Length - 1].X, polygon[polygon.Length - 1].Y);

           
            for (int i = 0; i < polygon.Length; i++)
            {
               
                var newPoint = new PointR(polygon[i].X, polygon[i].Y);

               
                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                
                if ((newPoint.X < testPoint.X) == (testPoint.X <= oldPoint.X)
                    && (testPoint.Y - p1.Y) * (p2.X - p1.X)
                    < (p2.Y - p1.Y) * (testPoint.X - p1.X))
                {
                    inside = !inside; 
                }

                oldPoint = newPoint; 
            }

            return inside; 
        }




        private bool czyblisko(PointR clickPoint, Segment segment, double thickness)
        {
            PointR pointA = segment.ps.clone();
            PointR pointB = segment.pe.clone();

            double dx = pointB.X - pointA.X;
            double dy = pointB.Y - pointA.Y;


            if (dx == 0)
            {

                return (clickPoint.X >= pointA.X - thickness / 2) && (clickPoint.X <= pointA.X + thickness / 2) &&
                       (clickPoint.Y >= Math.Min(pointA.Y, pointB.Y)) && (clickPoint.Y <= Math.Max(pointA.Y, pointB.Y));
            }

            double m = dy / dx;
            double c1 = pointA.Y - m * pointA.X;


            double c2 = c1 + thickness / 2 * Math.Sqrt(m * m + 1);
            double c3 = c1 - thickness / 2 * Math.Sqrt(m * m + 1);


            bool be = (clickPoint.Y >= m * clickPoint.X + c3) && (clickPoint.Y <= m * clickPoint.X + c2);


            bool b = (clickPoint.X >= Math.Min(pointA.X, pointB.X)) && (clickPoint.X <= Math.Max(pointA.X, pointB.X));

            return b && be;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            otoczka = 2;
            pict.Refresh();
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            otoczka = 0;
            pict.Refresh();
        }

        public static List<PointR> OffsetPolygon(List<PointR> polyCoords, double offset)
        {
            var newPolyCoords = new List<PointR>();

            if (offset == 0.0 || polyCoords.Count < 3)
                return newPolyCoords;

            int nVerts = polyCoords.Count;

            for (int curr = 0; curr < polyCoords.Count; curr++)
            {
                // ustal indeksy poprzedniego itd
                int prev = curr != 0 ? curr - 1 : nVerts - 1;
                int next = curr != nVerts - 1 ? curr + 1 : 0;

                if (swps(polyCoords[next], polyCoords[curr], polyCoords[curr], polyCoords[prev]))
                { continue; }
                // oblicz wektor next-curr
                double vnX = polyCoords[next].X - polyCoords[curr].X;
                double vnY = polyCoords[next].Y - polyCoords[curr].Y;
                //jego dlugsc
                double lengthVn = Math.Sqrt(vnX * vnX + vnY * vnY);

                //  normalizacja
                double vnnX = vnX / lengthVn;
                double vnnY = vnY / lengthVn;
                //prostopadlosc
                double nnnX = vnnY;
                double nnnY = -vnnX;

                //tak samo z tym drugim
                double vpX = polyCoords[curr].X - polyCoords[prev].X;
                double vpY = polyCoords[curr].Y - polyCoords[prev].Y;
                double lengthVp = Math.Sqrt(vpX * vpX + vpY * vpY);
                double vpnX = vpX / lengthVp;
                double vpnY = vpY / lengthVp;

                double npnX = vpnY;
                double npnY = -vpnX;
                // mamy dwa znormalizowane prostopadle robimy z nich bisektre mocne

                double bisX = (nnnX + npnX);
                double bisY = (nnnY + npnY);
                double lengthBis = Math.Sqrt(bisX * bisX + bisY * bisY);

                //dlugosc biistektry
                double bisnX = bisX / lengthBis;
                double bisnY = bisY / lengthBis;
                //normalizacja go znowu

                //                l = d / Sqrt((1 + dotproduct(na, nb)) / 2)
                //  /(derived from l= d / cos(fi / 2) and half - angle cosine formula)

                double bisLen = offset / Math.Sqrt((1 + nnnX * npnX + nnnY * npnY) / 2);
                if (bisLen > 1000)
                {
                    bisLen = 1000;
                }
                //przesuniecie wlasnie problem bo moze byc dalekie jesli kat jest maly
                newPolyCoords.Add(new PointR((int)(polyCoords[curr].X + bisLen * bisnX), (int)(polyCoords[curr].Y + bisLen * bisnY)));
            }

            return newPolyCoords;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            algorytm = 2;
            pict.Refresh();
        }

        public static List<PointR> PunktyZKrawedzi(List<Segment> segmenty, List<PointR> punkty)
        {
            List<PointR> nowepunkty = new List<PointR>();


            punkty.Sort((a, b) => a.Y.CompareTo(b.Y));

            PointR naj = punkty
            .OrderByDescending(p => p.Y)  
            .ThenBy(p => p.X)             
            .First();

            List<Segment> sasiednieSegmenty = segmenty.FindAll(x => x.PBS(naj));
            Segment pierwszy = sasiednieSegmenty[0];
            Segment drugi = sasiednieSegmenty[1];

            PointR pierwszyp = pierwszy.FindNon(naj);
            PointR drugip = drugi.FindNon(naj);

            double tangensPierwszego = Math.Atan2(pierwszyp.Y - naj.Y, pierwszyp.X - naj.X);
            double tangensDrugiego = Math.Atan2(drugip.Y - naj.Y, drugip.X - naj.X);


            PointR wybranyPunkt;
            Segment wybrany;

            if (tangensPierwszego < tangensDrugiego)
            {
                wybranyPunkt = pierwszyp;
                wybrany = pierwszy;
            }
            else
            {
                wybranyPunkt = drugip;
                wybrany = drugi;
            }


            PointR petla1 = naj.clone();
            PointR petla2 = wybranyPunkt.clone();
            Segment petla;
            nowepunkty.Add(naj);
            while (petla2 != naj)
            {
                petla = segmenty.Find(x => x.PBS(petla2) && !x.PBS(petla1));
                if (petla == null) return punkty;
                petla1 = petla.getrowne(petla2).clone();
                nowepunkty.Add(petla2);
                petla2 = petla.FindNon(petla2).clone();


            }


            return nowepunkty;
        }

        public static bool swps(PointR a1, PointR a2, PointR b1, PointR b2)
        {
            double ax = a2.X - a1.X;
            double ay = a2.Y - a1.Y;
            double bx = b2.X - b1.X;
            double by = b2.Y - b1.Y;


            double lengthA = Math.Sqrt(ax * ax + ay * ay);
            double lengthB = Math.Sqrt(bx * bx + by * by);
            ax /= lengthA;
            ay /= lengthA;
            bx /= lengthB;
            by /= lengthB;


            double dotProduct = ax * bx + ay * by;


            double tolerance = 1e-9;
            return Math.Abs(dotProduct - 1) < tolerance || Math.Abs(dotProduct + 1) < tolerance;
        }

        public static Segment GetOffsetSegment(Segment segment, float offset, Graphics g)
        {
            double dx = segment.ps.X - segment.pe.X;
            double dy = segment.ps.Y - segment.pe.Y;

            double length = Math.Sqrt(dx * dx + dy * dy);
            double offsetX = -offset * dy / length;
            double offsetY = offset * dx / length;

            Segment offsetSegment = new Segment
            (
                new PointR((int)(segment.ps.X + offsetX), (int)(segment.ps.Y + offsetY)),
                new PointR((int)(segment.pe.X + offsetX), (int)(segment.pe.Y + offsetY))
            );


            Pen pen = new Pen(Color.Red);
            //  g.DrawLine(pen, offsetSegment.ps.X, offsetSegment.ps.Y, offsetSegment.pe.X, offsetSegment.pe.Y);

            return offsetSegment;
        }

        public void DrawOffsetPolygon(Graphics graphics, List<PointR> polyCoords, float offset, Graphics g)
        {
            for (int i = 0; i < polyCoords.Count; i++)
            {
                PointR start = polyCoords[i];
                PointR end = polyCoords[(i + 1) % polyCoords.Count];

                Segment originalSegment = new Segment(start, end);
                Segment offsetSegment = GetOffsetSegment(originalSegment, offset, g);

                //DrawOffset(graphics, originalSegment, offsetSegment, radius);
                PointR[] rectanglePoints = { start, end, offsetSegment.pe, offsetSegment.ps };
                graphics.FillPolygon(Brushes.Yellow, rectanglePoints.Select(p => new PointF(p.X, p.Y)).ToArray());



                graphics.FillEllipse(Brushes.Yellow, start.X - offset, start.Y - offset, offset * 2, offset * 2);


            }
        }


        public static bool duplikaty(List<PointR> punkty)
        {
            for (int i = 0; i < punkty.Count; i++)
            {
                for (int j = i + 1; j < punkty.Count; j++)
                {
                    if (punkty[i].X == punkty[j].X && punkty[i].Y == punkty[j].Y)
                    {
                        return true;
                    }
                }
            }

            return false;
        }






    }

}




