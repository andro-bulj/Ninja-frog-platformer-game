using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    class Frog : Sprite
    {
        private int speed;
        private bool isInAir;
        private int gravity;
        private bool isJumping;

        private int jumpUpTimes = 33;
        public int Speed
        {
            set
            {
                speed = value;
            }
            get
            {
                return speed;
            }
        }

        public bool IsInAir
        {
            set
            {
                isInAir = value;
            }
            get
            {
                return isInAir;
            }
        }

        public int Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }

        public bool IsJumping
        {
            get { return isJumping; }
            set { isJumping = value; }
        }
        
        public Frog(string imgPath, int posX, int posY) : base(imgPath, posX, posY)
        {
            this.speed = 4;
            this.gravity = 2;
            this.isInAir = false;
            this.isJumping = false;
        }

        //Skakanje žabe
        public void Jumping()
        {
            jumpUpTimes -= 1;

            this.Y -= 2;

            if (jumpUpTimes==0)
            {
                jumpUpTimes = 33;
                isJumping = false;
            }
        }

        //Resetiranje skakanja žabe
        public void jumpingReset()
        {
            this.jumpUpTimes = 33;
            this.IsJumping = false;
        }

    }

    //Platforme
    class Platforms : Sprite
    {

        public Platforms(string imgPath, int posX, int posY) : base(imgPath, posX, posY)
        {

        }

    }

    //Voće
    class Fruit : Sprite
    {
        public Fruit(string imgPath, int posX, int posY) : base(imgPath, posX, posY)
        {

        }
    }
}
