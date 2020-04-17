using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisCore;

namespace TetrisOOPConsole
{
    class Program
    {
        const int SCREEN_WIDTH = 80;
        const int SCREEN_HEIGHT = 50;

        static void Main(string[] args)
        {
            RandomShapeGenerator generator = new RandomShapeGenerator(new ClassicShapeSet());
            GameLevel[] levels = {new GameLevel(1, 2000, new TimeSpan(0, 0, 0, 1, 100)),
                                     new GameLevel(2, 4000, new TimeSpan(0, 0, 0, 0, 900)),
                                     new GameLevel(3, 7000, new TimeSpan(0, 0, 0, 0, 700)),
                                     new GameLevel(4, 11000, new TimeSpan(0, 0, 0, 0, 500)),
                                     new GameLevel(5, 15000, new TimeSpan(0, 0, 0, 0, 300)),
                                     new GameLevel(6, 1000000, new TimeSpan(0, 0, 0, 0, 200))
                                 };

            TetrisUI gameUI = new TetrisUI();
            Game game = null;

            Console.CursorVisible = false;
            Console.SetWindowSize(SCREEN_WIDTH, SCREEN_HEIGHT);

            MenuItem[] menu;
            InitializeMenu(out menu);

            GameMenu menuItemSelected;
            int currentItem = 0;

            do
            {
                menuItemSelected = MainMenu.GetMenuSelection(menu, ref currentItem);

                switch(menuItemSelected)
                {
                    case GameMenu.StartGame:
                        game = new Game(width: 10, height: 20, generator: generator, levels: levels, ui: gameUI, syncObj: null);
                        gameUI.Initialize(game);
                        game.Start();
                        menu[(int)GameMenu.ResumeGame].Enabled = !game.GameOver;
                        if (!game.GameOver)
                        {
                            currentItem = (int)GameMenu.ResumeGame; // если игра не завершена, то переключаемся на пункт GameMenu.ResumeGame
                        }
                        break;

                    case GameMenu.ResumeGame:
                        game.Resume();
                        menu[(int)GameMenu.ResumeGame].Enabled = !game.GameOver;
                        if(game.GameOver)
                        {
                            currentItem = (int)GameMenu.StartGame; // в завершённую игру запрещено возвращаться. переключаемся на пункт GameMenu.StartGame
                        }
                        break;

                    case GameMenu.ShowHelp:
                        MainMenu.ShowHelp();
                        break;

                    case GameMenu.ShowCredits:
                        MainMenu.ShowCredits();
                        break;

                    default:
                        break;
                }
            } while(menuItemSelected != GameMenu.QuitApplication);
        } //  static void Main

        static void InitializeMenu(out MenuItem[] menu)
        {
            menu = new MenuItem[5];

            menu[0] = new MenuItem() { Value = GameMenu.StartGame, Enabled = true, Text = "Новая игра" };
            menu[1] = new MenuItem() { Value = GameMenu.ResumeGame, Enabled = false, Text = "Возобновить игру" };
            menu[2] = new MenuItem() { Value = GameMenu.ShowHelp, Enabled = true, Text = "Помощь" };
            menu[3] = new MenuItem() { Value = GameMenu.ShowCredits, Enabled = true, Text = "О программе" };
            menu[4] = new MenuItem() { Value = GameMenu.QuitApplication, Enabled = true, Text = "Выход" };
        }

    }
}
