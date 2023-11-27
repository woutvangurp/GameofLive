//using Game_of_Live.Grid;
using Timer = System.Threading.Timer;

namespace Game_of_Live {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private Dictionary<Point, Cell> currentDictionary = new();
        private Dictionary<Point, Cell> ChangeDict = new();

        public int PlayX;
        public int PlayY;
        private int TimerTime;

        private Graphics g;

        private Pen AlivePen;
        private Pen DeadPen;
        private Pen UnbornPen;

        private SolidBrush AliveBrush;
        private SolidBrush DeadBrush;
        private SolidBrush UnbornBrush;
        private Timer timer;

        public bool First;
        private void Form1_Load(object sender, EventArgs e) {
            g = panel1.CreateGraphics();

            currentDictionary = new Dictionary<Point, Cell>();

            if (textBox1.Text != "") PlayX = int.Parse(textBox1.Text);
            else PlayX = 50;
            if (textBox2.Text != "") PlayY = int.Parse(textBox2.Text);
            else PlayY = 50;

            //PlayY = 50;
            //PlayX = 50;

            AlivePen = new Pen(Color.Black);
            DeadPen = new Pen(Color.White);
            UnbornPen = new Pen(Color.Blue);

            First = true;
        }

        private void button1_Click(object sender, EventArgs e) {
            button1.Enabled = false;
            button2.Visible = true;

            SetDictionary();
            //WritePanel();

            timer = new Timer(PostTimer, null, 0, 1000);
        }

        private void PostTimer(object state) {
            //deze wordt elke seconde uitgevoerd

            // -|-|-
            // -|/|-
            // -|-|-

            foreach (var changeCell in currentDictionary) {

                int check = 0;
                int X = changeCell.Key.X;
                int Y = changeCell.Key.Y;

                //punten om te checken
                Point[] checkPoints =
                {
                    new Point(X - 1, Y - 1),
                    new Point(X, Y - 1),
                    new Point(X + 1, Y - 1),
                    new Point(X - 1, Y),
                    new Point(X + 1, Y),
                    new Point(X - 1, Y + 1),
                    new Point(X, Y + 1),
                    new Point(X + 1, Y + 1)
                };



                checkPoints = FilterPoints(checkPoints, X, Y); // alles wat in de - staat gaat hier uit, alles wat boven het aantal wat ik heb opgegeven gaat er uit

                //hier is het kijken naar hoe ik iets naar change dict kan zetten, en deze kan uitlezen, en alles van current wordt bekeken als het wel verandering nodig is, daarna wordt change naar current gezet

                if (changeCell.Value.CurrentStatus == Cell.Status.Alive) {
                    int test = 0;
                    //checkt of de cellen om de cell heen leven
                    foreach (var point in checkPoints)
                    {
                        test = 0;
                        // als de cell leeft, check++
                        if (currentDictionary.ContainsKey(point)) 
                            if (currentDictionary[point].CurrentStatus == Cell.Status.Alive) 
                                check++;
                    }

                    //een levende cel met 2 of 3 levende buren blijft leven
                    //een levende cel gaat dood als deze MINDER dan 2 levende buren heeft
                    //een levende cel gaat dood als deze MEER dan 3 levende buren heeft
                    if (check == 2 || check == 3)
                        changeCell.Value.CurrentStatus = Cell.Status.Alive;
                    else
                        changeCell.Value.CurrentStatus = Cell.Status.Dead;

                    currentDictionary[changeCell.Key] = changeCell.Value;

                } 
                else if (changeCell.Value.CurrentStatus == Cell.Status.Unborn)
                {
                    changeCell.Value.CurrentStatus = Cell.Status.Dead;
                    currentDictionary[changeCell.Key] = changeCell.Value;
                }
                else {

                    foreach (var point in checkPoints) {

                        if (currentDictionary.ContainsKey(point))
                            if (currentDictionary[point].CurrentStatus == Cell.Status.Alive)
                                check++;
                    }

                    //een dode cel met precies drie levende buren wordt levend
                    if (check == 3)
                    {
                        changeCell.Value.CurrentStatus = Cell.Status.Alive; 
                        currentDictionary[changeCell.Key] = changeCell.Value;
                    }
                }
            }
            WritePanel(currentDictionary);
        }

        private void getCheckablePoints(Point[] checkPoints, int X, int Y) {
            foreach (var point in checkPoints) {
                // de X en Y van de change points aka de cellen om de levende cellen, kijken als ze levend mogen worden
                string CX = point.X.ToString();
                string CY = point.Y.ToString();

                if (CX.Contains("-"))
                    X--;
                else if (CX.Contains("+"))
                    X++;

                if (CY.Contains("-"))
                    Y--;
                else if (CY.Contains("+"))
                    Y--;

                if (X >= 0 && X <= PlayX && Y >= 0 && Y <= PlayY) //check als het wel in het speelveld zit
                    ChangeDict.Add(point, currentDictionary[point]); // zet de cell die gecheckt moet worden in de changedict
            }
        }

        private Point[] FilterPoints(Point[] checkPoints, int X, int Y) {
            foreach (var coordination in checkPoints) {

                string CX = coordination.X.ToString();
                string CY = coordination.Y.ToString();

                if (X == 0 && Y == 0)
                    checkPoints = checkPoints.Where(C => C.X != -1 && C.Y != -1).ToArray();
                else if (X == PlayX && Y == PlayY)
                    checkPoints = checkPoints.Where(C => C.X != +1 && C.Y != +1).ToArray();
                else
                {
                    if (Y == 0)
                    {
                        if (CY.Contains("-"))
                            checkPoints = checkPoints.Where(C => C.Y != -1).ToArray();
                    }
                    else if (Y == PlayY)
                        if (CY.Contains("+"))
                            checkPoints = checkPoints.Where(C => C.Y != PlayY + 1).ToArray();
                    
                    if (X == 0)
                    {
                        if (CX.Contains("-"))
                            checkPoints = checkPoints.Where(C => C.X != -1).ToArray();
                    }
                    else if (X == PlayX)
                        if (CX.Contains("+"))
                            checkPoints = checkPoints.Where(C => C.X != PlayX + 1).ToArray();
                }
            }
            return checkPoints;
        }

        protected void WritePanel(Dictionary<Point, Cell> CellDictionary) {
            lock (g)
            {
                int f = 10;
                AliveBrush = new SolidBrush(Color.Black);
                DeadBrush = new SolidBrush(Color.White);
                UnbornBrush = new SolidBrush(Color.Blue);

                if (First)
                {
                    foreach (var Cells in CellDictionary)
                    {
                        //Coördinaten van de cell
                        int x = Cells.Key.X;
                        int Y = Cells.Key.Y;

                        //state van de cell
                        string state = Cells.Value.CurrentStatus.ToString();
                        //voeg de cell toe aan de changes, om te kijken wat er nog mee moet gebeuren
                        //ChangeDict.Add(Cells.Key, Cells.Value);

                        if (state == "Alive")
                            g.FillRectangle(AliveBrush, new Rectangle(x * f, Y * f, f, f));
                        else if (state == "Dead")
                            g.FillRectangle(DeadBrush, new Rectangle(x * f, Y * f, f, f));
                        else
                            g.FillRectangle(UnbornBrush, new Rectangle(x * f, Y * f, f, f));
                    }

                    First = false;
                    //currentDictionary.Clear();
                }
                else
                {
                    foreach (var Cells in CellDictionary)
                    {
                        int X = Cells.Key.X;
                        int Y = Cells.Key.Y;

                        string state = Cells.Value.CurrentStatus.ToString();

                        if (state == "Alive")
                            g.FillRectangle(AliveBrush, X * f, Y * f, f, f);
                        else if (state == "Dead")
                            g.FillRectangle(DeadBrush, X * f, Y * f, f, f);
                        else
                            g.FillRectangle(UnbornBrush, X * f, Y * f, f, f);

                    }
                }
            }
        }

        protected void DisposeValues() {
            //dispose en verwijder alles
            //g.Dispose();
            DeadPen.Dispose();
            AlivePen.Dispose();
            UnbornPen.Dispose();
            AliveBrush.Dispose();
            DeadBrush.Dispose();
            UnbornBrush.Dispose();
            timer.Dispose();

            currentDictionary.Clear();
            ChangeDict.Clear();
        }

        private void SetDictionary() {
            //maak een cell aan, zet deze in de dictionary en randomize de cell state

            Random random = new Random();
            int randomInt;
            for (int j = 0; j < PlayX; j++) {
                for (int i = 0; i < PlayY; i++) {

                    randomInt = random.Next(0, 5);
                    Cell newCell = new Cell();

                    if (randomInt == 0)
                        newCell.CurrentStatus = Cell.Status.Alive;
                    else
                        newCell.CurrentStatus = Cell.Status.Unborn;

                    //newCell.CurrentStatus = randomInt == 0 ? Cell.Status.Alive : Cell.Status.Unborn;

                    currentDictionary.Add(new Point(j, i), newCell);
                    //ChangeDict.Add(new Point(j, i), newCell);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            DisposeValues();
            button1.Enabled = true;
            button2.Enabled = false;
            button2.Visible = false;
        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

        }
    }
}