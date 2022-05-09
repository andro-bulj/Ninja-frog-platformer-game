using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {
                foreach (Sprite sprite in allSprites)
                {
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            if (sprite.spriteTile == null)
                            {
                                g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                            }
                            else
                                sprite.CrtajMe(g);
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.Transparent);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */
        Frog frog;
        Platforms ground, plat1, plat2,plat3,plat4,plat5,wallLeft, wallRight,platShort1,platShort2,platShort3;
        Fruit banana1, apple1, melon1;
        Ghost ghost1,ghost2;
        Plant plant;
        Trunk trunk1, trunk2;
        Bullet bulletPlant,bulletTrunk1,bulletTrunk2;

        Turtle turtle;
        Enemy saw,spike1,spike2;
        Sprite loading,odustani,igrajOpet;

        bool level1 = true;
        bool level2 = false;
        bool level3 = false;
        int fruitCount = 3;
        int enemyCount = 3;

        //Gumb na početku za pokrenuti igricu
        private void btnIgraj_Click(object sender, EventArgs e)
        {
            Ispis();
            label1.Visible = false;
            btnIgraj.Dispose();

            Game.AddSprite(saw);

            Game.AddSprite(plat1);
            Game.AddSprite(plat2);
            Game.AddSprite(plat3);
            Game.AddSprite(plat4);
            Game.AddSprite(plat5);

            Game.AddSprite(platShort1);
            Game.AddSprite(platShort2);
            Game.AddSprite(platShort3);

            Game.AddSprite(ground);
            Game.AddSprite(wallLeft);
            Game.AddSprite(wallRight);

            Game.AddSprite(frog);

            Game.AddSprite(turtle);
            Game.AddSprite(ghost1);
            Game.AddSprite(ghost2);
            Game.AddSprite(plant);

            Game.AddSprite(spike1);
            Game.AddSprite(spike2);

            Game.AddSprite(banana1);
            Game.AddSprite(apple1);
            Game.AddSprite(melon1);

            Game.AddSprite(bulletPlant);

            Game.AddSprite(bulletTrunk1);
            Game.AddSprite(bulletTrunk2);
            Game.AddSprite(trunk1);
            Game.AddSprite(trunk2);

            Game.AddSprite(loading);
            loading.SetVisible(false);

            //Scripts that start
            Game.StartScript(AnimMovement);
            Game.StartScript(Movement);
            Game.StartScript(Sensing);
            Game.StartScript(Jumping);
            Game.StartScript(enemyAnimation);

        }


        /* Initialization */
        private void SetupGame()
        {
            //Loading screen
            loading = new Sprite("sprites\\Loading.png",0,0);
            //1.    <--- Setup stage --->
            SetStageTitle("NinjaFrog");
            //setBackgroundColor(Color.WhiteSmoke);            
            setBackgroundPicture("backgrounds\\bgGreen.png");
            //none, tile, stretch, center, zoom
            //syncRate.Show();

            odustani = new Sprite("sprites\\Odustani.png", 0, -70);
            igrajOpet = new Sprite("sprites\\igrajOpet.png", 0, -70);

            //2.    <--- Add sprites --->
            //Glavni lik
            frog = new Frog("sprites\\frogSheet.png", 20, 170);
            frog.SetTile(5, 12);
            //Visina i širina žabe inače se postavi širina i visina na cijeli sheet(384,160)
            frog.Width = 28;
            frog.Heigth = 32;

            //Neprijatelji
            //Kornjača koja ima šiljke koji se ukjučuju nasumice
            turtle = new Turtle("sprites\\turtleSheet.png", 0, -40);
            turtle.SetTile(4,14);
            turtle.Heigth = 26;
            turtle.Width = 44;

            //Biljka koja puca zrno
            plant = new Plant("sprites\\plantSheet.png", 20, 79);
            plant.SetTile(4,11);
            plant.Heigth = 42;
            plant.Width = 44;
            //Zrno od biljke
            bulletPlant = new Bullet("sprites\\plantBullet.png", 45, 89);

            //Deblo koje trče i puca zrno
            trunk1 = new Trunk("sprites\\trunkSheet.png", 0, -44);
            trunk1.SetTile(4, 18);
            trunk1.Heigth = 32;
            trunk1.Width = 64;

            trunk2 = new Trunk("sprites\\trunkSheet.png", 0, -44);
            trunk2.SetTile(4, 18);
            trunk2.Heigth = 32;
            trunk2.Width = 64;
            //Zrno od debla
            bulletTrunk1 = new Bullet("sprites\\trunkBullet.png", 0, -44);
            bulletTrunk2 = new Bullet("sprites\\trunkBullet.png", 0, -44);

            //Duh koji se miče lijevo desno
            ghost1 = new Ghost("sprites\\ghostSheet.png", 388, 171);
            ghost1.SetTile(4, 10);
            ghost1.Heigth = 44;
            ghost1.Width = 30;

            ghost2 = new Ghost("sprites\\ghostSheet.png", 158, 117);
            ghost2.SetTile(4, 10);
            ghost2.Heigth = 44;
            ghost2.Width = 30;

            //Okrugla pila koja se vrti
            saw = new Enemy("sprites\\Saw.png", 280, 127);
            saw.SetTile(1, 8);
            saw.Heigth = 38;
            saw.Width = 34;

            //Bodlje na podu koje ubiju žabu kada skoči na njih
            spike1 = new Enemy("sprites\\Spikes.png", 60, 104);
            spike2 = new Enemy("sprites\\Spikes.png", 112, 216);

            //Pod
            ground = new Platforms("sprites\\Floor.jpg", 15, 248);
            ground.AddCostumes("sprites\\Floor2.png", "sprites\\Floor3.png");
            //Pravokutne platforme
            //                                               X<->, Yˇ^
            //Desna dolje
            plat1 = new Platforms("sprites\\GrassBlock.png", 390, 200);
            plat1.AddCostumes("sprites\\GrassBlock2.png", "sprites\\GrassBlock3.png");
            //Sredina
            plat2 = new Platforms("sprites\\GrassBlockBig.png", 158, 147);
            plat2.AddCostumes("sprites\\GrassBlockBig2.png", "sprites\\GrassBlockBig3.png");
            //Lijeva dolje
            plat3 = new Platforms("sprites\\GrassBlock.png", 96, 232);
            plat3.AddCostumes("sprites\\GrassBlock2.png", "sprites\\GrassBlock3.png");
            //Lijeva gore
            plat4 = new Platforms("sprites\\GrassBlock.png", 16, 120);
            plat4.AddCostumes("sprites\\GrassBlock2.png", "sprites\\GrassBlock3.png");
            //Desna gore
            plat5 = new Platforms("sprites\\GrassBlock.png", 410, 100);
            plat5.AddCostumes("sprites\\GrassBlock2.png", "sprites\\GrassBlock3.png");

            //Uske platforme
            platShort1 = new Platforms("sprites\\WoodPlatformShort.png", 0, -44);
            platShort1.AddCostumes("sprites\\GrassPlatformShort.png");
            platShort2 = new Platforms("sprites\\WoodPlatformShort.png", 0, -44);
            platShort2.AddCostumes("sprites\\GrassPlatformShort.png");
            platShort3 = new Platforms("sprites\\WoodPlatform.png", 0, -44); 
            platShort3.AddCostumes("sprites\\GrassPlatform.png");

            //Lijevi zid levela
            wallLeft = new Platforms("sprites\\WallLeft.png", 0, 30);
            wallLeft.AddCostumes("sprites\\WallLeft2.png", "sprites\\WallLeft3.png");
            //Desni zid levela
            wallRight = new Platforms("sprites\\WallRight.png", 522, 0);
            wallRight.AddCostumes("sprites\\WallRight2.png", "sprites\\WallRight3.png");

            //Voće koje se skuplja
            banana1 = new Fruit("sprites\\Banana.png", 85, 102);
            apple1 = new Fruit("sprites\\Apple.png", 134, 214);
            melon1 = new Fruit("sprites\\Melon.png", 446, 84);
        }

        /* Scripts */

        #region Enemy animations
        //Sve animacije neprijatelja
        private int enemyAnimation()
        {
            plant.SetHeading(90);
            turtle.SetHeading(90);
            trunk1.SetHeading(-90);
            trunk2.SetHeading(90);

            int t1 = trunk1.GetHeading();
            int t2 = trunk2.GetHeading();


            while (START)
            {
                //Animacija pile
                saw.AnimirajMe(0, 0);

                //Animacije debla

                if (frog.TouchingSprite(plat2) && level3)
                {
                    trunk1.TrunkRun(2, 3);

                    if(trunk1.X > frog.X)
                    {
                        trunk1.SetHeading(-90);
                        trunk1.X -= 5;
                    }
                    else if (trunk1.X < frog.X)
                    {
                        trunk1.SetHeading(90);
                        trunk1.X += 5;
                    }
                }
                else
                {
                    trunk1.AnimirajMe(0, 1);
                }

                //Animacija drugog debla
                if (frog.TouchingSprite(ground) && level3)
                {
                    trunk2.TrunkRun(2, 3);

                    if (trunk2.X > frog.X)
                    {
                        trunk2.SetHeading(-90);
                        trunk2.X -= 5;
                    }
                    else if (trunk2.X < frog.X)
                    {
                        trunk2.SetHeading(90);
                        trunk2.X += 5;
                    }
                }
                else
                {
                    trunk2.AnimirajMe(0, 1);
                }


                //Animacije zrna debla
                if (bulletTrunk1.Shooting)
                {
                    if (t1 == 90)
                    {
                        bulletTrunk1.X += bulletTrunk1.Speed;
                    }
                    else
                    {
                        bulletTrunk1.X -= bulletTrunk1.Speed;
                    }
                }
                else
                {
                    t1 = trunk1.GetHeading();
                    bulletTrunk1.Goto_Sprite(trunk1);
                    bulletTrunk1.Shooting = true;
                }

                if (bulletTrunk2.Shooting)
                {
                    if (t2 == 90)
                    {
                        bulletTrunk2.X += bulletTrunk2.Speed;
                    }
                    else
                    {
                        bulletTrunk2.X -= bulletTrunk2.Speed;
                    }
                }
                else
                {
                    t2 = trunk2.GetHeading();
                    bulletTrunk2.Goto_Sprite(trunk2);
                    bulletTrunk2.Shooting = true;
                }


                //Animacija kornjače
                if (turtle.Patrolling)
                    turtle.Patrol();

                if(turtle.SpikesOpen)
                {
                    turtle.AnimirajMe(0, 1);
                }
                else
                    turtle.AnimirajMe(2, 3);

                //Animacija biljke
                plant.AnimirajMe(0, 1);

                //Animacija zrna biljke
                if (!bulletPlant.Shooting)
                {
                    if (level1)
                        bulletPlant.X = 45;
                    else if (level2)
                        bulletPlant.X = 420;
                    bulletPlant.Shooting = true;
                }

                if (level1 && plant.Alive)
                {
                    bulletPlant.X += bulletPlant.Speed;
                }
                else if (level2 && plant.Alive)
                {
                    bulletPlant.X -= bulletPlant.Speed;
                }

                //Animacija duhova
                if (ghost1.Patrolling)
                {
                    //Animacija duha
                    ghost1.Idle(0, 3);
                    //Kretanje lijevo-desno
                    ghost1.Patrol();
                }
                if (ghost2.Patrolling)
                {
                    ghost2.Idle(0, 3);
                    ghost2.Patrol();
                }

                Wait(0.1);
            }
            return 0;
        }
        #endregion

        #region Movement animation
        private int AnimMovement()
        {
            frog.SetDirection(90);
            while (START)
            {
                //Ako nista nije pritisnuto onda se vrti idle animacija

                if (!sensing.KeyPressedTest)
                {
                    frog.Idle(4,1);
                    Wait(0.01);
                }
                //Animacija kretanje desno
                if (sensing.KeyPressed(Keys.D))
                {
                    frog.SetDirection(90);
                    frog.AnimirajMe(2, 0);
                }
                
                //Animacija kretanje lijevo
                if (sensing.KeyPressed(Keys.A))
                {
                    frog.SetDirection(-90);
                    frog.AnimirajMe(2, 0);
                }
                Wait(0.02);
            }
            return 0;

        }
        #endregion

        #region Movement
        private int Movement()
        {
            while (START)
            {
                //Ako je u zraku i ne skače povlači ga doli
                if (frog.IsInAir && !frog.IsJumping)
                    frog.Y += frog.Gravity;
                //Kretanje desno
                if (sensing.KeyPressed(Keys.D))
                {
                    if (frog.Speed == 0)
                        frog.Speed = 4;
                    //Ako je na platformi ima normalnu brzinu
                    else if (touchingPlatform())
                    {
                        //Slučaj kada stoji na platformi i ide prema zidu
                        if (frog.WillTouchSprite(wallRight, "d"))
                        {
                            frog.X -= 4;
                            frog.Speed = 0;
                        }
                        else
                            frog.Speed = 4;
                    }
                    //Ako će dotaknuti platformu pomakne ga nazat
                    else if (frog.WillTouchSprite(plat1, "d") || frog.WillTouchSprite(plat2, "d") || frog.WillTouchSprite(plat3, "d") || frog.WillTouchSprite(wallRight, "d") || frog.WillTouchSprite(plat5, "d") || frog.WillTouchSprite(platShort1,"d") || frog.WillTouchSprite(platShort2, "d") || frog.WillTouchSprite(platShort3, "d"))
                    {
                        frog.X -= 4;
                        frog.Speed = 0;
                    }

                    frog.X += frog.Speed;
                }
                //Kretanje lijevo
                else if (sensing.KeyPressed(Keys.A))
                {
                    if (frog.Speed == 0)
                        frog.Speed = 4;

                    else if (touchingPlatform())
                    {
                        if (frog.WillTouchSprite(wallLeft, "a"))
                        {
                            frog.X += 4;
                            frog.Speed = 0;
                        }
                        else
                            frog.Speed = 4;
                    }
                    else if (frog.WillTouchSprite(plat1, "a") || frog.WillTouchSprite(plat2, "a") || frog.WillTouchSprite(plat3, "a") || frog.WillTouchSprite(plat4, "a") || frog.WillTouchSprite(plat5, "a") || frog.WillTouchSprite(wallLeft, "a") || frog.WillTouchSprite(platShort1, "a") || frog.WillTouchSprite(platShort2, "a") || frog.WillTouchSprite(platShort3, "a"))
                    {
                        frog.X += 4;
                        frog.Speed = 0;
                    }

                    frog.X -= frog.Speed;
                }

                //Ako pritisne space i nije u zraku da skoči
                if (sensing.KeyPressed(Keys.Space) && !frog.IsInAir)
                {
                    frog.Y -= 1;
                    frog.IsJumping = true;
                    Game.StartScript(Jumping);
                }
                Wait(0.01);
            }
            return 0;
        }
        #endregion

        #region Jumping
        private int Jumping()
        {
            //Dok je true da treba skakati ide prema gore za 3, 30 puta
            while (frog.IsJumping)
            {
                frog.Jumping();

                //Ako dotakne platformu dok skače prekida se skakanje
                if (touchingPlatform())
                {
                    frog.Y += 2; 
                    frog.jumpingReset();
                }
                Wait(0.01);
            }
            return 0;
        }
        #endregion 

        #region Sensing/enemy collision
        private int Sensing()
        {
            while (START)
            {
                //Ako je na podu gravitacija je 0 da se ne miče dolje, ali i dalje se može micati lijevo desno
                if (touchingPlatform() || frog.TouchingSprite(ground))
                {
                    frog.IsInAir = false;
                    frog.Gravity = 0;
                }


                //Ako je ne dira pod ili platformu onda neka se miče doli kod ne dotakne pod
                if (!touchingPlatform() && !frog.TouchingSprite(ground))
                {
                    frog.IsInAir = true;

                    //Iako se gravity postavi na 3 neće micati dok skače zato što je postavljeno !frog.IsJumping u kodu gdje se koristi gravitacija za kretanje dolje
                    frog.Gravity = 3;
                }

                if (frog.TouchingSprite(ghost1))
                {
                    //Postaviti duha na nevidljivo stanje, pomaknuti ga u gornji lijevi kut izvan ekrana(0,-40) i ispisati
                    if (frog.IsInAir && !frog.IsJumping)
                    {
                        ghost1.SetVisible(false);
                        ghost1.Patrolling = false;
                        ghost1.GotoXY(0, -44);

                        frog.X -= 3;
                        enemyCount -= 1;
                        Ispis();
                    }
                    else
                    {
                        //Ako ga je duh ubio level se resetira
                        resetLevel();
                    }
                }

                else if (frog.TouchingSprite(ghost2))
                {
                    //Postaviti drugog duha izvan ekrana
                    if (frog.IsInAir && !frog.IsJumping)
                    {
                        ghost2.SetVisible(false);
                        ghost2.Patrolling = false;
                        ghost2.GotoXY(0, -44);

                        frog.X -= 3;
                        enemyCount -= 1;
                        Ispis();
                    }
                    else
                    {
                        resetLevel();
                    }
                }
                
                else if (frog.TouchingSprite(plant))
                {
                    //Postaviti biljku i zrno izvan ekrana 
                    if (frog.IsInAir && !frog.IsJumping)
                    {
                        plant.Alive = false;
                        plant.GotoXY(0, -44);

                        bulletPlant.GotoXY(0, -20);

                        frog.X -= 3;
                        enemyCount -= 1;
                        Ispis();
                    }
                    else
                    {
                        resetLevel();
                    }
                }

                else if (frog.TouchingSprite(turtle))
                {
                    //Postaviti kornjaču izvan ekrana
                    if (frog.IsInAir && !frog.IsJumping && !turtle.SpikesOpen)
                    {
                        turtle.SetVisible(false);
                        turtle.GotoXY(0, -44);

                        frog.X -= 3;
                        enemyCount -= 1;
                        Ispis();
                        turtle.Patrolling = false;
                    }
                    else
                    {
                        resetLevel();
                    }
                }

                else if (frog.TouchingSprite(trunk1))
                {
                    //Postaviti trunk1 izvan ekrana
                    if (frog.IsInAir && !frog.IsJumping)
                    {
                        trunk1.GotoXY(0, -44);

                        frog.X -= 3;
                        enemyCount -= 1;
                        Ispis();
                    }
                    else
                    {
                        resetLevel();
                    }
                }

                else if (frog.TouchingSprite(trunk2))
                {
                    //Postaviti trunk1 izvan ekrana
                    if (frog.IsInAir && !frog.IsJumping)
                    {
                        trunk2.GotoXY(0, -44);

                        frog.X -= 3;
                        enemyCount -= 1;
                        Ispis();
                    }
                    else
                    {
                        resetLevel();
                    }
                }

                //Slučaj da skoči na pilu ili bodlje
                else if (frog.TouchingSprite(saw) || frog.TouchingSprite(spike1) || frog.TouchingSprite(spike2))
                {
                    resetLevel();
                }

                //Slučaj da dotakne zrno
                else if (frog.TouchingSprite(bulletPlant) || frog.TouchingSprite(bulletTrunk1) || frog.TouchingSprite(bulletTrunk2))
                {
                    resetLevel();
                }

                if (sensing.MouseDown)
                {
                    if (igrajOpet.Clicked(sensing.Mouse))
                    {

                        igrajOpet.GotoXY(0, -70);
                        odustani.GotoXY(0, -70);

                        nextLevel();
                    }

                    if (odustani.Clicked(sensing.Mouse))
                    {
                        START = false;
                    }
                }
                
                if (START == false)
                {
                    Application.Exit();
                }
                //Sakupljanje voća
                if (frog.TouchingSprite(banana1))
                    Collecting(banana1);

                if (frog.TouchingSprite(melon1))
                    Collecting(melon1);

                if (frog.TouchingSprite(apple1))
                    Collecting(apple1);

                //Ako je sakupljeno sve i pobjeđeni neprijatelji prelazi se na sljedeći level
                //  2. Level
                if (fruitCount == 0 && enemyCount == 0 && level1)
                {
                    level1 = false;
                    level2 = true;

                    fruitCount = 3;
                    enemyCount = 2;

                    nextLevel();
                }
                //  3. Level
                else if (fruitCount == 0 && enemyCount == 0 && level2)
                {
                    level2 = false;
                    level3 = true;

                    fruitCount = 3;
                    enemyCount = 3;

                    nextLevel();
                }
                //  End
                else if (fruitCount == 0 && enemyCount == 0 && level3)
                {
                    level3 = false;
                    level1 = true;

                    fruitCount = 3;
                    enemyCount = 3;

                    ISPIS = "";

                    Game.AddSprite(igrajOpet);
                    Game.AddSprite(odustani);

                    igrajOpet.GotoXY(165, 130);
                    odustani.GotoXY(165, 200);

                    loading.SetVisible(true);
                }
            }
            return 0;
        }
        #endregion

        #region Next level
        //Postavljanje sljedećeg levela
        private void nextLevel()
        {
            if (level1)
            {
                //Postaviti platforme na sljedeći kostim

                setBackgroundPicture("backgrounds\\bgGreen.png");

                wallLeft.Y += 10;

                plat1.GotoXY(390, 200);
                plat1.NextCostume();
                plat2.GotoXY(158, 147);
                plat2.NextCostume();
                plat3.GotoXY(96, 232);
                plat3.NextCostume();
                plat4.GotoXY(16, 120);
                plat4.NextCostume();
                plat5.GotoXY(410, 100);
                plat5.NextCostume();

                ground.NextCostume();
                wallLeft.NextCostume();
                wallRight.NextCostume();

                saw.GotoXY(280, 127);

                spike1.GotoXY(60, 104);
                spike2.GotoXY(112, 216);

                platShort1.GotoXY(0, -44);
                platShort2.GotoXY(0, -44);
                platShort3.GotoXY(0, -44);

                plant.SetHeading(90);

                Ispis();

                loading.SetVisible(false);
                resetLevel();

            }

            if (level2)
            {
                ISPIS = "";
                //Postaviti loading screen dok se sve ne učita
                setBackgroundPicture("backgrounds\\bgPurple.png");
                loading.SetVisible(true);
                //Postaviti platforme na sljedeći kostim

                foreach (var x in allSprites)
                {
                    x.NextCostume();
                }

                //Postavljanje svih komponenta na svoje mjesto
                saw.GotoXY(268,212);

                plat1.GotoXY(240,232);
                plat2.GotoXY(325, 115);
                plat3.GotoXY(0, -44);
                plat4.GotoXY(0,-44);
                plat5.GotoXY(35,120);

                platShort1.GotoXY(0, -44);
                platShort2.GotoXY(196,150);
                platShort3.GotoXY(100,210);

                spike1.GotoXY(90, 104);
                spike2.GotoXY(358, 99);

                plant.SetHeading(-90);

                Wait(1.2);

                resetLevel();

                Ispis();

                loading.SetVisible(false);
            }
            else if (level3)
            {
                ISPIS = "";
                loading.SetVisible(true);
                setBackgroundPicture("backgrounds\\bgBrown.png");

                foreach (var x in allSprites)
                {
                    x.NextCostume();

                }

                //Postavljanje svih komponenta na svoje mjesto
                saw.GotoXY(0, -44);

                wallLeft.Y -= 10;
                plat1.GotoXY(0, -44);
                //Duga platforma
                plat2.GotoXY(350, 135);
                plat3.GotoXY(0, -44);
                plat4.GotoXY(0, -44);
                //Lijeva platforma
                plat5.GotoXY(16, 85);

                platShort1.GotoXY(170, 130);
                platShort2.GotoXY(260, 135);
                platShort3.GotoXY(50, 190);

                spike1.GotoXY(220, 232);
                spike2.GotoXY(236, 232);

                Wait(1.2);

                resetLevel();

                Ispis();

                loading.SetVisible(false);

            }
        }
        #endregion

        #region Reset level
        //Resetiranje trenutnog levela ako umre
        private void resetLevel()
        {
            if (level1)
            {
                frog.GotoXY(20, 170);

                ghost1.patrolReset();
                ghost1.GotoXY(388, 171);
                ghost1.SetVisible(true);
                ghost1.Patrolling = true;

                ghost2.patrolReset();
                ghost2.GotoXY(158, 117);
                ghost2.SetVisible(true);
                ghost2.Patrolling = true;

                plant.GotoXY(20, 79);
                plant.Alive = true;

                banana1.GotoXY(85, 102);
                apple1.GotoXY(134, 214);
                melon1.GotoXY(446, 84);

                bulletPlant.GotoXY(45,89);

                enemyCount = 3;
                fruitCount = 3;

                Ispis();
            }
            else if (level2)
            {
                
                frog.GotoXY(20, 170);

                turtle.GotoXY(360, 222);
                turtle.SetVisible(true);
                turtle.patrolReset();
                turtle.Patrolling = true;

                plant.GotoXY(430, 73);
                plant.Alive = true;

                bulletPlant.GotoXY(420, 82);
                bulletPlant.SetVisible(true);

                banana1.GotoXY(43, 102);
                apple1.GotoXY(494, 230);
                melon1.GotoXY(494, 96);

                enemyCount = 2;
                fruitCount = 3;

                Ispis();
            }
            else if (level3)
            {

                turtle.GotoXY(360, 222);
                turtle.SetVisible(true);
                turtle.patrolReset();
                turtle.Patrolling = true;

                frog.GotoXY(32, 46);

                trunk1.GotoXY(430,103);
                trunk2.GotoXY(60, 216);

                trunk1.SetHeading(-90);
                trunk2.SetHeading(90);

                bulletTrunk1.Goto_Sprite(trunk1);
                bulletTrunk2.Goto_Sprite(trunk2);

                banana1.GotoXY(25, 230);
                apple1.GotoXY(500, 230);
                melon1.GotoXY(490, 118);

                enemyCount = 3;
                fruitCount = 3;

                Ispis();
            }
        }
        #endregion

        #region Game methods
        //Ispisivanje podataka o trenutnom levelu, količini voća i neprijatelja na ekran
        private void Ispis()
        {
            if (level1)
                ISPIS = "Level 1 / Voća: " + fruitCount + " / Neprijatelja: " + enemyCount;

            else if (level2)
                ISPIS = "Level 2 / Voća: " + fruitCount + " / Neprijatelja: " + enemyCount;

            else if (level3)
                ISPIS = "Level 3 / Voća: " + fruitCount + " / Neprijatelja: " + enemyCount;
        }

        //Metoda za sakupljanje voća
        private void Collecting(Sprite s)
        {
            fruitCount -= 1;
            s.GotoXY(0, -20);
            Ispis();
        }

        //Metoda koja vraća ako žaba dira platformu
        private bool touchingPlatform()
        {
            if (frog.TouchingSprite(plat1) || frog.TouchingSprite(plat2) || frog.TouchingSprite(plat3) || frog.TouchingSprite(plat4) || frog.TouchingSprite(plat5) || frog.TouchingSprite(platShort1) || frog.TouchingSprite(platShort2) || frog.TouchingSprite(platShort3))
            {
                return true;
            }
            return false;
        }
        #endregion

        /* ------------ GAME CODE END ------------ */
    }
}
