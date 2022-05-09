using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    class Enemy : Sprite
    {
        protected bool shooting;
        private int speed;

        public bool Shooting { get => shooting; set => shooting = value; }

        public int Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        public Enemy(string imgPath, int posX, int posY) : base(imgPath, posX, posY)
        {
            this.speed = 3;
        }
    }

    class Ghost : Enemy
    {
        private int ghostMoveLeft = 16;
        private int ghostMoveRight = 16;
        private bool patrolling;

        public bool Patrolling
        {
            get
            {
                return patrolling;
            }
            set
            {
                patrolling = value;
            }
        }

        public Ghost(string imgPath, int posX, int posY) : base(imgPath, posX, posY)
        {
            this.patrolling = true;
        }

        public void Patrol()
        {
            //Kretanje lijevo desno
            if (ghostMoveLeft > 0)
            {
                this.SetDirection(90);
                ghostMoveLeft -= 1;
                this.X += this.Speed;
            }
            else if (ghostMoveLeft == 0 && ghostMoveRight > 0)
            {
                this.SetDirection(-90);
                ghostMoveRight -= 1;
                this.X -= this.Speed;
            }
            else
            {
                patrolReset();
            }
        }

        //Resetiranje kretanja duha
        public void patrolReset()
        {
            this.ghostMoveLeft = 16;
            this.ghostMoveRight = 16;
        }
    }

    class Plant : Enemy
    {
        private bool alive;
        public bool Alive { get => alive; set => alive = value; }


        public Plant(string imgPath, int posX, int posY) : base(imgPath, posX, posY)
        {
            this.alive = true;
        }

    }

    class Trunk : Enemy
    {

        public Trunk(string imgPath, int posX, int posY) : base(imgPath, posX, posY)
        {

        }

    }

    class Bullet : Enemy
    {

        public Bullet(string imgPath, int posX, int posY) : base(imgPath, posX, posY)
        {
            this.shooting = true;
            this.Speed = 8;
        }

        public override int X
        {
            get
            {
                return base.X;
            }
            set
            {
                //Postavljanje zrna kada ode izvan ekrana
                if (value > GameOptions.RightEdge - 120)
                {
                    this.shooting = false;
                    return;
                }
                else if (value < GameOptions.LeftEdge - 20)
                {
                    this.shooting = false;
                    return;
                }
                base.X = value;
            }
        }
    }

    class Turtle : Enemy
    {
        readonly Random rand = new Random();

        private int turtleMoveRight = 30;
        private int turtleMoveLeft = 30;

        private bool patrolling;
        private bool spikesOpen;
        public bool Patrolling
        {
            get
            {
                return patrolling;
            }
            set
            {
                patrolling = value;
            }
        }

        public bool SpikesOpen
        {
            get
            {
                return spikesOpen;
            }
            set
            {
                spikesOpen = value;
            }
        }

        public Turtle(string imgPath, int posX, int posY) : base(imgPath, posX, posY)
        {
            this.patrolling = false;
        }

        //Kretanje kornjače lijevo desno
        public void Patrol()
        {
            //  1/3 šanse da ugasi bodlje
            if (turtleMoveLeft == 27 || turtleMoveRight == 27)
            {
                if (rand.Next(0, 3) == 1)
                {
                    SpikesOpen = false;
                }
                else
                {
                    SpikesOpen = true;
                }
            }

            if (turtleMoveLeft > 0)
            {
                this.SetDirection(90);
                turtleMoveLeft -= 1;
                this.X += this.Speed;
            }
            else if (turtleMoveLeft == 0 && turtleMoveRight > 0)
            {
                this.SetDirection(-90);
                turtleMoveRight -= 1;
                this.X -= this.Speed;
            }
            else
            {
                patrolReset();
            }
        }

        //Resetiranje kretanja kornjače
        public void patrolReset()
        {
            this.turtleMoveLeft = 30;
            this.turtleMoveRight = 30;
        }
    }
}
