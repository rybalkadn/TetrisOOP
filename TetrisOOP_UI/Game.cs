using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TetrisCore;

namespace TetrisOOP_UI
{
    public partial class Game : Form
    {
        private GameField _field;
        //private NextShapeField _nextShapefield;


        public Game()
        {
            InitializeComponent();
        }

        private void Game_Activated(object sender, EventArgs e)
        {

        }
    }
}
