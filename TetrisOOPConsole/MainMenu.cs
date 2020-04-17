using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisOOPConsole
{
    class MainMenu
    {
        const int MENU_LEFT = 10;
        const int MENU_TOP = 10;


        /// <summary>
        /// Показать заставку
        /// </summary>
        /// <param name="delaySeconds">Время показа заставки в секундах. Если равно ноль, то показывать до тех пор, пока пользователь не нажмёт любую клавишу</param>
        public static void ShowSplashScreen(int delaySeconds = 0)
        {
            Console.Clear();

            DateTime d = DateTime.Now;
            TimeSpan ts = new TimeSpan(0, 0, 1);
            Console.ForegroundColor = ConsoleColor.White;

            // Показываем стартовый экран на протяжении заданного интервала либо до нажатия клавиши
            // Если передали нулевой интервал, то выходим только по нажатию клавиши
            do
            {

                Console.WriteLine("TetrisOOP Game SplashScreen");
                if(Console.ForegroundColor == ConsoleColor.DarkBlue)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, 0);
                    Console.Clear();
                }
                else
                {
                    Console.ForegroundColor--;
                }
                System.Threading.Thread.Sleep(ts);
            } while((delaySeconds == 0 || d.AddSeconds(delaySeconds) > DateTime.Now)
                    && !Console.KeyAvailable);

            if(Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Вывести на экран справку игры
        /// </summary>
        public static void ShowHelp()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("ПРАВИЛА");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Случайные фигурки тетрамино падают сверху в прямоугольный стакан шириной 10");
            Console.WriteLine("и высотой 20 клеток. В полёте игрок может поворачивать фигурку на 90°");
            Console.WriteLine("и двигать её по горизонтали. Также можно «сбрасывать» фигурку, то есть");
            Console.WriteLine("ускорять её падение, когда уже решено, куда фигурка должна упасть.");
            Console.WriteLine("Фигурка летит до тех пор, пока не наткнётся на другую фигурку либо");
            Console.WriteLine("на дно стакана. Если при этом заполнился горизонтальный ряд из 10 клеток,");
            Console.WriteLine("он пропадает и всё, что выше него, опускается на одну клетку.");
            Console.WriteLine("Дополнительно показывается фигурка, которая будет следовать после текущей —");
            Console.WriteLine("это подсказка, которая позволяет игроку планировать действия.");
            Console.WriteLine("Темп игры постепенно увеличивается. Игра заканчивается, когда новая фигурка");
            Console.WriteLine("не может поместиться в стакан. Игрок получает очки за каждый заполненный ряд,");
            Console.WriteLine("поэтому его задача — заполнять ряды, не заполняя сам стакан (по вертикали)");
            Console.WriteLine("как можно дольше, чтобы таким образом получить как можно больше очков.");

            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\nУПРАВЛЕНИЕ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("Стрелки влево и вправо - двигать фигуру влево/вправо");
            Console.WriteLine("Стрелка вверх - повернуть фигуру на 90 градусов по часовой стрелке");
            Console.WriteLine("Стрелка вниз - ускорение падения фигуры");
            Console.WriteLine("Пробел - опустить фигуру до предела");
            Console.WriteLine("Esc - поставить игру на паузу и выйти в основное меню");

            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n\nНажмите любую клавишу, чтобы вернуться в основное меню");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ReadKey(true);
        }

        /// <summary>
        /// Отобразить окно "О программе"
        /// </summary>
        public static void ShowCredits()
        {
            ShowSplashScreen();
        }

        /// <summary>
        /// Вывести на экран меню и обработать пользовательский выбор пункта меню
        /// </summary>
        /// <param name="menu">Массив пунктов меню</param>
        /// <returns>Выбранный пункт меню</returns>
        public static GameMenu GetMenuSelection(MenuItem[] menu, ref int currentItem)
        {
            bool itemConfirm = false;

            Console.Clear();

            do
            {
                MainMenu.ShowMenu(menu, currentItem);

                // Ожидаем нажатия клавиши
                do
                {
                    System.Threading.Thread.Sleep(1);
                } while(!Console.KeyAvailable);

                // Обрабатываем пользовательский ввод
                ConsoleKey key = Console.ReadKey(true).Key;
                switch(key)
                {
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Enter:
                        itemConfirm = true;
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.Tab:
                        SelectNextMenuItem(menu, ref currentItem);
                        break;
                    case ConsoleKey.UpArrow:
                        SelectPreviousMenuItem(menu, ref currentItem);
                        break;
                    default:
                        break;
                }

            } while(!itemConfirm);

            return menu[currentItem].Value;
        }

        /// <summary>
        /// Вывести меню на экран
        /// </summary>
        /// <param name="menu">Массив пунктов меню</param>
        /// <param name="currentItem">Выбранный пункт меню</param>
        private static void ShowMenu(MenuItem[] menu, int currentItem)
        {

            Console.SetCursorPosition(MENU_LEFT, MENU_TOP);

            for(int i = 0; i < menu.Length; i++)
            {
                if(i == currentItem)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    if(menu[i].Enabled)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                }

                Console.CursorLeft = MENU_LEFT;
                Console.WriteLine(menu[i].Text);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Сделать текущим следующий не запрещённый пункт меню. Меню цикличное
        /// </summary>
        /// <param name="menu">Массив пунктов меню</param>
        /// <param name="currentItem">Выбранный пункт меню</param>
        private static void SelectPreviousMenuItem(MenuItem[] menu, ref int currentItem)
        {
            int newCurrent = currentItem;

            do
            {
                if(newCurrent == 0)
                {
                    newCurrent = menu.Length - 1;
                }
                else
                {
                    newCurrent--;
                }
            } while(!menu[newCurrent].Enabled && newCurrent != currentItem); // если нашли разрешённый пункт меню или дошли по кругу до исходного пункта, то выходим из цикла

            currentItem = newCurrent;
        }

        /// <summary>
        /// Сделать текущим предыдующий не запрещённый пункт меню. Меню цикличное
        /// </summary>
        /// <param name="menu">Массив пунктов меню</param>
        /// <param name="currentItem">Выбранный пункт меню</param>
        private static void SelectNextMenuItem(MenuItem[] menu, ref int currentItem)
        {
            int newCurrent = currentItem;

            do
            {
                if(newCurrent == menu.Length - 1)
                {
                    newCurrent = 0;
                }
                else
                {
                    newCurrent++;
                }
            } while(!menu[newCurrent].Enabled && newCurrent != currentItem); // если нашли разрешённый пункт меню или дошли по кругу до исходного пункта, то выходим из цикла

            currentItem = newCurrent;
        }

    }
}
