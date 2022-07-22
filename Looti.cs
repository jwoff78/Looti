using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuskOS.DuskSystem.Applications
{
    public class LootiEditor
    {
        public static bool repalcevars = false;
        public static bool IDE = false;
        public static int xint;
        public static bool click = false;
        public static bool wrap = true;
        static string tosav;
        static string cursor = " ";
        static string ver = "INDEV 1.8";

        static ConsoleColor BarFg = ConsoleColor.Black;
        static ConsoleColor BarBg = ConsoleColor.Gray;
        static ConsoleColor EnterBg = ConsoleColor.Black;
        static ConsoleColor EnterFg = ConsoleColor.White;
        static ConsoleColor EditorBg = ConsoleColor.Blue;
        static ConsoleColor EditorFg = ConsoleColor.Gray;
        static ConsoleColor MenuBg = ConsoleColor.Gray;
        static ConsoleColor MenuFg = ConsoleColor.Black;
        public static string Run(string[] argv)
        {
            try
            {
                Editor(argv);
                Console.CursorVisible = true;
                Console.CursorSize = 3;
            }
            catch(Exception e)
            {
                Console.WriteLine(" ~ ERROR ~\n"+e.Message);
            }
            return null;
        }


        public static string Editor(string[] argv)
        {
            Console.CursorVisible = false;
            string PATH = "";
            Console.Clear();

            for (int i = 0; i < argv.Length; i++)
            {
                if (argv[i] == "--help") { Console.WriteLine("Looti " + ver + ", MIT license.\nhttps://github.com/luftkatzeBASIC/\nLooti\n2021,2022 Luftkatze\n"); Console.CursorVisible = true; return null; }
                else if (argv[i] == "--ide") IDE = true;
                else if (PATH == "") { PATH = argv[i]; if (!PATH.Contains("\\"))
                    {
                        PATH = Directory.GetCurrentDirectory() + $"\\{PATH}";
                    }
                }
                else { Console.Write("\nUnknow argument - "); Console.ForegroundColor = ConsoleColor.Red; Console.Write(argv[i]); Console.ForegroundColor = ConsoleColor.White; Console.CursorVisible = true; return null; }
            }



            if (File.Exists("lticonf.bin"))
            {
                byte[] config = File.ReadAllBytes("lticonf.bin");
                if (config.Length != 8)
                {
                    File.WriteAllText("Looti.log", "> Invalid lticonf.bin");
                    File.WriteAllBytes("lticonf.bin", new byte[] { (int)ConsoleColor.Black, (int)ConsoleColor.Gray, (int)ConsoleColor.Black, (int)ConsoleColor.White, (int)ConsoleColor.Blue, (int)ConsoleColor.Gray, (int)ConsoleColor.Gray, (int)ConsoleColor.Black });
                }
                BarFg = (ConsoleColor)config[0];
                BarBg = (ConsoleColor)config[1];
                EnterBg = (ConsoleColor)config[2];
                EnterFg = (ConsoleColor)config[3];
                EditorBg = (ConsoleColor)config[4];
                EditorFg = (ConsoleColor)config[5];
                MenuBg = (ConsoleColor)config[6];
                MenuFg = (ConsoleColor)config[7];
            }
            else
            {
                File.WriteAllBytes("lticonf.bin", new byte[] { (int)ConsoleColor.Black, (int)ConsoleColor.Gray, (int)ConsoleColor.Black, (int)ConsoleColor.White, (int)ConsoleColor.Blue, (int)ConsoleColor.Gray, (int)ConsoleColor.Gray, (int)ConsoleColor.Black });
            }


            if (!File.Exists(@PATH))
            {
                File.Create(PATH);
            }

            Console.BackgroundColor = EditorBg;
            string arrow = "";
            string old = File.ReadAllText(@PATH);
            tosav = File.ReadAllText(@PATH);
            for (; ; ) {
                Console.BackgroundColor = EditorBg;
                Console.ForegroundColor = EditorFg;
                Console.Clear();
                Console.Write("\n\n");
                if (tosav.Split("\n").Length > 22)
                {
                    for(int i=tosav.Split('\n').Length - 22; i < tosav.Split('\n').Length; i++)
                    {
                        Console.Write(tosav.Split('\n')[i]);
                        if (i != tosav.Split('\n').Length - 1 || !tosav.EndsWith('\n'))
                        {
                            Console.Write('\n');
                        }
                    }
                }
                else
                {
                    Console.Write(tosav);
                }
                xint = Console.CursorLeft;
                int yint = Console.CursorTop;
                for(int kk = 0; kk < arrow.Length; kk++)
                {
                    if (Console.CursorTop == Console.WindowHeight - 1)
                    {
                        break;
                    }
                    Console.Write(arrow[kk]);
                }
                Console.SetCursorPosition(xint, yint);
                Console.BackgroundColor = EditorFg;
                Console.ForegroundColor = EditorBg;
                if (arrow != "" && arrow[0] != '\n') {
                    Console.Write(arrow[0]);
                }
                else
                {
                    Console.Write(cursor);
                }
                xint = Console.CursorLeft;
                yint = Console.CursorTop;
                Console.SetCursorPosition(0, 0);
                Drw(@PATH, tosav, arrow, old);
                Console.SetCursorPosition(xint, yint);
                Console.BackgroundColor = EditorBg;
                Console.ForegroundColor = EditorFg;

                ConsoleKeyInfo input = new ConsoleKeyInfo();
                input = Console.ReadKey(true);
                if (repalcevars == true)
                {
                    if (tosav.Contains("%DATE%"))
                    {
                        var d = DateTime.Now.ToShortDateString();
                        tosav = tosav.Replace("%DATE%", d.ToString());
                    }
                    else if (tosav.Contains("%TIME%"))
                    {
                        DateTime dt = DateTime.Now;
                        var t = dt.ToShortTimeString();
                        tosav = tosav.Replace("%TIME%", t.ToString());
                    }
                    else if (tosav.Contains("%NOW%"))
                    {
                        DateTime dt = DateTime.Now;

                        tosav = tosav.Replace("%NOW%", dt.ToString());
                    }
                }
                if (input.Key == ConsoleKey.Enter)
                {
                    tosav += "\n";

                }
                else if ((input.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if ((input.Key & ConsoleKey.S) != 0)
                    {
                        File.Delete(@PATH);
                        File.AppendAllText(@PATH, tosav + arrow);
                        old = tosav + arrow;
                    }
                }
                else if ((input.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if ((input.Key & ConsoleKey.O) != 0)
                    {
                        string open = Open();
                        if (open != null && File.Exists(open))
                        {
                            tosav = File.ReadAllText(open);
                            arrow = "";
                            PATH = open;
                        }
                    }
                }
                else if ((input.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if ((input.Key & ConsoleKey.N) != 0)
                    {
                        Console.BackgroundColor = EnterBg;
                        Console.ForegroundColor = EnterFg;
                        Console.Clear();
                        Console.SetCursorPosition(20, 13);
                        Console.WriteLine("[New file name]");
                        Console.SetCursorPosition(20, 14);
                        string newflnam = LootiTerminal();
                        if (newflnam != null)
                        {
                            tosav = File.ReadAllText(newflnam);
                            arrow = "";
                            PATH = newflnam;
                        }
                    }
                }
                else if ((input.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if ((input.Key & ConsoleKey.Q) != 0)
                    {
                        Console.BackgroundColor = EnterBg;
                        Console.ForegroundColor = EnterFg;
                        Console.Clear();
                        Console.SetCursorPosition(20, 13);
                        Console.WriteLine("[Save as...]");
                        Console.SetCursorPosition(20, 14);
                        string savas = LootiTerminal();
                        if (savas != null)
                        {
                            File.AppendAllText(@savas, old);
                            PATH = savas;
                        }
                    }

                }
                else if ((input.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if ((input.Key & ConsoleKey.H) != 0)
                    {
                        Replace(tosav, old, arrow, PATH);
                    }

                }
                else if ((input.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if ((input.Key & ConsoleKey.F) != 0)
                    {
                        Find(tosav, old, arrow, PATH);
                    }

                }
                else if ((input.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if ((input.Key & ConsoleKey.X) != 0)
                    {
                        Exit(tosav, old, arrow, PATH);
                        Console.CursorVisible = true; return null;
                    }
                }
                else if ((input.Modifiers & ConsoleModifiers.Control) != 0)
                {
                    if ((input.Key & ConsoleKey.C) != 0)
                    {
                        Console.CursorVisible = true;
                        Console.CursorSize = 3;
                        return "-1";
                    }
                }
                else if (input.Key == ConsoleKey.F1)
                {
                    Console.BackgroundColor = MenuBg;
                    Console.ForegroundColor = MenuFg;
                    Console.SetCursorPosition(0, 2);
                    Console.Write("| File            \n");
                    Console.Write("| F1  [SAVE]     S\n");
                    Console.Write("| F2  [OPEN]     O\n");
                    Console.Write("| F3  [NEW]      N\n");
                    Console.Write("| F4  [SAVE AS]  Q\n");
                    Console.Write("| F12 [EXIT]     X");
                    Console.ForegroundColor = EditorFg;

                    input = Console.ReadKey();
                    if (input.Key == ConsoleKey.F1)
                    {
                        File.Delete(@PATH);
                        File.AppendAllText(@PATH, tosav + arrow);
                        old = tosav + arrow;
                    }
                    else if (input.Key == ConsoleKey.F2)
                    {
                        string open = Open();
                        if (open != null && File.Exists(open))
                        {
                            tosav = File.ReadAllText(open);
                            arrow = "";
                            PATH = open;
                        }

                    }
                    else if (input.Key == ConsoleKey.F3)
                    {
                        Console.BackgroundColor = EnterBg;
                        Console.ForegroundColor = EnterFg;
                        Console.Clear();
                        Console.SetCursorPosition(20, 13);
                        Console.WriteLine("[New file name]");
                        Console.SetCursorPosition(20, 14);
                        string newflnam = LootiTerminal();
                        if (newflnam != null)
                        {
                            tosav = File.ReadAllText(newflnam);
                            arrow = "";
                            PATH = newflnam;
                        }
                    }
                    else if (input.Key == ConsoleKey.F4)
                    {
                        Console.BackgroundColor = EnterBg;
                        Console.ForegroundColor = EnterFg;
                        Console.Clear();
                        Console.SetCursorPosition(20, 13);
                        Console.WriteLine("[Save as...]");
                        Console.SetCursorPosition(20, 14);
                        string savas = LootiTerminal();
                        if (savas != null)
                        {
                            File.AppendAllText(@savas, old);
                            PATH = savas;
                        }
                    }

                    else if (input.Key == ConsoleKey.F12)
                    {
                        Exit(tosav, old, arrow, PATH);
                        Console.CursorVisible = true; return null;
                    }
                    else
                    {
                        Console.BackgroundColor = EditorBg;
                        DrawBar(@PATH, tosav, arrow, old);
                    }


                }
                else if (input.Key == ConsoleKey.F2)

                {
                    Console.BackgroundColor = MenuBg;
                    Console.ForegroundColor = MenuFg;
                    Console.SetCursorPosition(8, 2);
                    Console.Write("| Edit           \n");
                    Console.SetCursorPosition(8, 3);
                    Console.Write("| F1  [REPLACE] H\n");
                    Console.SetCursorPosition(8, 4);
                    Console.Write("| F2  [FIND]    K\n");
                    Console.ForegroundColor = EditorFg;
                    input = Console.ReadKey();

                    if (input.Key == ConsoleKey.F1)
                    {
                        Replace(tosav, old, arrow, PATH);
                    }
                    else if (input.Key == ConsoleKey.F2)
                    {
                        Find(tosav, old, arrow, PATH);

                    }
                    else
                    {
                        Console.BackgroundColor = EditorBg;
                        DrawBar(@PATH, tosav, arrow, old);
                    }
                }
                else if (input.Key == ConsoleKey.F3)
                {
                    Console.BackgroundColor = MenuBg;
                    Console.ForegroundColor = MenuFg;
                    Console.SetCursorPosition(19, 2);
                    Console.Write("| Other          \n");
                    Console.SetCursorPosition(19, 3);
                    Console.Write("| F1   [CTRL-C]  \n");
                    Console.SetCursorPosition(19, 4);
                    Console.Write("| F2   [ABOUT]   \n");
                    Console.SetCursorPosition(19, 5);
                    Console.Write("| F3   [PATH]    \n");
                    Console.ForegroundColor = EditorFg;
                    input = Console.ReadKey();
                    if (input.Key == ConsoleKey.F1)
                    {
                        return null;
                    }
                    else if (input.Key == ConsoleKey.F2)
                    {
                        Console.BackgroundColor = EditorBg;
                        Console.ForegroundColor = EditorFg;
                        Console.Clear();
                        Console.WriteLine(
                            "    Looti text editor\n" +
                            "    Version" + ver + "\n" +
                            "    Created by:\n" +
                            "    Programming: Luftkatze\n" +
                            "    Thx to: BaseMax, Ilobilo, Profesor DJ\n" +
                            "    2021,2022, Luftkatze\n\n\n\n" +
                            "    Default Looti file extension: *.LTI\n"+
                            $"    Change Log ({ver}):\n" +
                            "     * Added ability to change colors\n" +
                            "     * Better text drawing code\n" +
                            "     * Faster code\n" +
                            "     Todo:\n" +
                            "     * Config file"
                            ) ;
                        Console.ReadKey();
                    }
                    else if (input.Key == ConsoleKey.F3)
                    {
                        Console.BackgroundColor = EnterBg;
                        Console.ForegroundColor = EnterFg;
                        Console.Clear();
                        Console.SetCursorPosition(20, 14);
                        Console.WriteLine("[New path...]");
                        string np = LootiTerminal();
                        if (np != null)
                        {
                            File.AppendAllText(np, old);
                            PATH = np;
                        }
                    }
                    else
                    {
                        Console.BackgroundColor = EditorBg;
                        DrawBar(@PATH, tosav, arrow, old);
                    }
                }
                else if (input.Key == ConsoleKey.F4)
                {
                    Console.BackgroundColor = MenuBg;
                    Console.ForegroundColor = MenuFg;

                    Console.SetCursorPosition(29, 2);
                    Console.Write("| Insert          \n");
                    Console.SetCursorPosition(29, 3);
                    Console.Write("| F1  [NOW]       \n");
                    Console.SetCursorPosition(29, 4);
                    Console.Write("| F2  [TIME]      \n");
                    Console.SetCursorPosition(29, 5);
                    Console.Write("| F3  [DATE]      \n");
                    Console.SetCursorPosition(29, 6);
                    Console.Write("| F4  [FILE]      \n");
                    Console.SetCursorPosition(29, 7);
                    Console.Write("| F5  [ASCII]     \n");
                    Console.ForegroundColor = EditorFg;
                    input = Console.ReadKey();
                    if (input.Key == ConsoleKey.F1)
                    {
                        DateTime dt = DateTime.Now;

                        tosav += dt.ToString();
                    }
                    else if (input.Key == ConsoleKey.F2)
                    {
                        DateTime dt = DateTime.Now;
                        var t = dt.ToShortTimeString();
                        tosav += t.ToString();
                    }
                    else if (input.Key == ConsoleKey.F3)
                    {
                        var dt = DateTime.Now;
                        var d = dt.ToShortDateString();
                        tosav += d.ToString();
                    }
                    else if (input.Key == ConsoleKey.F4)
                    {

                        string open = Open();
                        if (open != null)
                        {
                            if (File.Exists(open))
                            {
                                string toconcat = File.ReadAllText(open);
                                tosav += toconcat;
                            }
                            else
                            {
                                DrawBar(@PATH, tosav, arrow, old);
                                Console.SetCursorPosition(20, 13);
                                Console.ForegroundColor = EnterFg;
                                Console.Write("CANNOT FIND FILE!"); Console.SetCursorPosition(27, 13);
                                Console.ForegroundColor = EditorFg;
                                Console.ReadKey();
                                DrawBar(@PATH, tosav, arrow, old);
                            }
                        }
                    }
                    else if (input.Key == ConsoleKey.F5)
                    {
                        Console.BackgroundColor = EditorBg;
                        DrawBar(@PATH, tosav, arrow, old);
                        Console.BackgroundColor = EditorBg;
                        Console.Write(@tosav);
                        Console.Write(@arrow);
                        Console.ForegroundColor = EnterBg;
                        Console.BackgroundColor = EnterFg;
                        Console.SetCursorPosition(20, 13);
                        Console.Write("CHAR [                              ]"); Console.SetCursorPosition(26, 13);
                        string chr = LootiTerminal();
                        if (!Int32.TryParse(chr, out int ascii))
                        {
                            Console.Beep(300, 300);
                        }
                        else
                        {
                            tosav += (char)ascii;
                        }
                        DrawBar(@PATH, tosav, arrow, old);
                    }
                    else
                    {
                        Console.BackgroundColor = EditorBg;
                        Console.Clear();
                        DrawBar(@PATH, tosav, arrow, old);
                    }
                }
                else if (input.Key == ConsoleKey.F5)
                {
                    Console.BackgroundColor = MenuBg;
                    Console.ForegroundColor = MenuFg;

                    Console.SetCursorPosition(29, 2);
                    Console.Write("| CONFIG          \n");
                    Console.SetCursorPosition(29, 3);
                    Console.Write("| F1  [>]         \n");
                    Console.SetCursorPosition(29, 4);
                    Console.Write("| F2  [<]         \n");
                    Console.SetCursorPosition(29, 5);
                    Console.Write("| F3  [+]         \n");
                    Console.SetCursorPosition(29, 6);
                    Console.Write("| F4  [-]         \n");
                    Console.SetCursorPosition(29, 7);
                    Console.Write("| F5  [DEF]       \n");
                    byte[] config = File.ReadAllBytes("lticonf.bin");

                    int index = 0;
                    for (; ; )
                    {
                        Console.ForegroundColor = EnterFg;
                        Console.BackgroundColor = EnterBg;
                        Console.SetCursorPosition(20, 13);
                        Console.Write($"{index} [                             ]");
                        Console.SetCursorPosition(23, 13);
                        Console.Write($"{config[index]} (");

                        Console.ForegroundColor = (ConsoleColor)config[index];
                        Console.Write(".");
                        Console.ForegroundColor = EnterFg;
                        Console.BackgroundColor = EnterBg;
                        Console.Write(")");
                        input = Console.ReadKey();
                        if (input.Key == ConsoleKey.F3)
                        {
                            config[index]++;
                        }
                        else if (input.Key == ConsoleKey.F4)
                        {
                            if (config[index] == 0)
                            {
                                continue;
                            }
                            config[index]--;
                        }
                        else if (input.Key == ConsoleKey.F1)
                        {
                            if (index == 7)
                            {
                                index=0;
                            }
                            index++;
                        }
                        else if (input.Key == ConsoleKey.F2)
                        {
                            if (index == 0)
                            {
                                index = 7;
                            }
                            index--;
                        }
                        else if (input.Key == ConsoleKey.F5)
                        {
                            config = new byte[] { (int)ConsoleColor.Black, (int)ConsoleColor.Gray, (int)ConsoleColor.Black, (int)ConsoleColor.White, (int)ConsoleColor.Blue, (int)ConsoleColor.Gray, (int)ConsoleColor.Gray, (int)ConsoleColor.Black };

                        }
                        else
                        {
                            File.WriteAllBytes("lticonf.bin", config);
                            Console.BackgroundColor = EditorBg;
                            Console.Clear();
                            DrawBar(@PATH, tosav, arrow, old);
                            BarFg = (ConsoleColor)config[0];
                            BarBg = (ConsoleColor)config[1];
                            EnterBg = (ConsoleColor)config[2];
                            EnterFg = (ConsoleColor)config[3];
                            EditorBg = (ConsoleColor)config[4];
                            EditorFg = (ConsoleColor)config[5];
                            MenuBg = (ConsoleColor)config[6];
                            MenuFg = (ConsoleColor)config[7];
                            break;
                        }
                    }
                }
                else if (input.Key == ConsoleKey.Home)
                {
                    Console.ResetColor();
                    Console.Clear();
                    Console.CursorVisible = true; return null;
                }

                else if (input.Key == ConsoleKey.Escape)
                {

                    Console.CursorLeft--;
                }

                else if (input.Key == ConsoleKey.Tab)
                {
                    tosav += "\t";

                }
                else if (input.Key == ConsoleKey.Spacebar)
                {
                    tosav += " ";
                }
                else if (input.KeyChar == '{')
                {

                    tosav += "{";
                    if (IDE == true) tosav += "\n"; arrow = "\n}" + arrow;

                }
                else if (input.KeyChar == '(')
                {


                    tosav += "(";
                    if (IDE == true) arrow = ")" + arrow;

                }
                else if (input.KeyChar == '<')
                {

                    tosav += "<";
                    if (IDE == true) arrow = ">" + arrow;

                }
                else if (input.KeyChar == '"')
                {


                    tosav += "\"";
                    if (IDE == true) arrow = "\"" + arrow;

                }
                else if (input.KeyChar == '\'')
                {

                    tosav += "'";
                    if (IDE == true) arrow = "'" + arrow;

                }
                else if (input.KeyChar == '[')
                {

                    tosav += "[";
                    if (IDE == true) arrow = "]" + arrow;

                }
                else if (input.Key == ConsoleKey.Backspace)
                {
                    if (tosav.Length!=0)
                    {
                        tosav = tosav.Remove(tosav.Length - 1, 1);
                    }
                }
                else if (input.Key == ConsoleKey.Delete)
                {
                    if (arrow.Length != 0)
                    {
                        arrow = arrow.Remove(0, 1);
                    }
                }
                else if (input.Key == ConsoleKey.Insert)
                {
                    Console.SetCursorPosition(0, 24);
                    Console.Write("[Document locked]");
                    ConsoleKey lockv;
                    for (; ; )
                    {
                        lockv = Console.ReadKey().Key;
                        if (lockv == ConsoleKey.Insert) break;
                        else Console.CursorLeft--;
                    }
                }
                else if (input.Key == ConsoleKey.LeftArrow)
                {

                    if (tosav.Length > 0)
                    {
                        arrow = tosav[tosav.Length - 1] + arrow;
                        tosav = tosav.Remove(tosav.Length - 1, 1);
                    }
                    else if (tosav.EndsWith("\n"))
                    {
                        arrow = "\n" + arrow;
                        tosav = tosav.Remove(tosav.Length - 1, 1);
                    }
                }
                else if (input.Key == ConsoleKey.RightArrow && arrow.Length > 0)
                {
                    tosav += arrow[0];
                    arrow = arrow.Remove(0, 1);
                }
                else if (input.Key == ConsoleKey.RightArrow && arrow.Length == 0)
                {
                    Console.CursorLeft--;
                }

                else if (input.Key == ConsoleKey.UpArrow)
                {
                    if (tosav.Length != 0 && Console.CursorTop != 2)
                    {
                        if (tosav.Contains("\n"))
                        {
                            for (; ; )
                            {
                                arrow = tosav[tosav.Length - 1] + arrow;
                                tosav = tosav.Remove(tosav.Length - 1, 1);
                                if (arrow.StartsWith("\n")) break;
                            }
                        }
                    }
                }
                else if (input.Key == ConsoleKey.DownArrow)
                {
                    if (arrow.Contains("\n"))
                    {
                        for (; ; )
                        {
                            tosav += arrow[0];
                            arrow = arrow.Remove(0, 1);
                            if (tosav.EndsWith("\n")) break;
                        }

                    }
                }
                else if (input.Key == ConsoleKey.PageUp)
                {
                    if (tosav.Length != 0)
                    {
                        arrow = tosav + arrow;
                        tosav = "";
                    }
                }

                else if (input.Key == ConsoleKey.PageDown)
                {
                    if (arrow.Length != 0)
                    {
                        tosav += arrow;
                        arrow = "";
                    }
                }

                else if (input.Key == ConsoleKey.Escape && Console.CursorLeft != 0)
                {
                    Console.CursorLeft--;
                }
                else if (input.Key == ConsoleKey.F12)
                {
                    File.Delete(@PATH);
                    File.AppendAllText(@PATH, tosav + arrow);
                    old = tosav + arrow;
                }
                else
                {
                    tosav += input.KeyChar.ToString();
                    if (wrap == true)
                    {
                        if (Console.CursorLeft == 79 && tosav.Contains(" "))
                        {
                            int LastIndex = 0;
                            string lio = tosav;
                            for (int i = 0; i < lio.Length; i++) if (lio[i] == ' ') LastIndex = i;
                            tosav = tosav.Remove(LastIndex, " ".Length).Insert(LastIndex, "\n");
                        }
                    }
                }

            }
        }
        public static void DrawBar(string PATH, string tosav, string arrow, string old)
        {
            Console.Clear();
            Console.ForegroundColor = EditorFg;
            Console.BackgroundColor = EditorBg;
            Console.Clear();
            Drw(PATH, tosav, arrow, old);
        }

        public static void Drw(string PATH, string tosav, string arrow, string old)
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = BarFg;
            Console.BackgroundColor = BarBg;
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < 80; i++) Console.Write(" ");
            Console.SetCursorPosition(0, 0);
            Console.Write($"Looti {ver} - " + @PATH);
            if (old != tosav + arrow) Console.Write("*");
            Console.Write("\n");
            Console.SetCursorPosition(35, 0);
            string tolenght = tosav.Replace("\n", "") + arrow.Replace("\n", "");
            int lines = tosav.Split('\n').Length + arrow.Split('\n').Length;
            lines--;
            string[] tarr = tosav.Split('\n');
            string t = tarr[tarr.Length - 1];
            string pos = (tosav.Split('\n').Length - 1).ToString() + "," + t.Length;
            Console.Write("Pos: [" + pos + "]  |  Lines: [" + lines + "] Chars: [" + tolenght.Length + "]");
            Console.SetCursorPosition(0, 1);
            for (int i = 0; i < 80; i++) Console.Write(" ");
            Console.SetCursorPosition(0, 1);
            Console.Write("F1 FILE | F2 EDIT | F3 OTHER | F4 INSERT | F5 CONFIG");
            Console.SetCursorPosition(0, 2);
            Console.ForegroundColor = EditorFg;
        }

        static void Replace(string tosav, string old, string arrow, string PATH)
        {
            Console.BackgroundColor = EditorBg;
            DrawBar(@PATH, tosav, arrow, old);
            Console.BackgroundColor = EditorBg;
            Console.Write(@tosav);
            Console.Write(@arrow);
            Console.ForegroundColor = EnterFg;
            Console.BackgroundColor = EnterBg;
            Console.SetCursorPosition(20, 13);
            Console.Write("Old [                          ]"); Console.SetCursorPosition(20, 14);
            Console.Write("New [                          ]");
            Console.SetCursorPosition(25, 13);
            string oldrep = LootiTerminal();
            if (oldrep != null)
            {
                Console.SetCursorPosition(25, 14);
                string newrep = LootiTerminal();
                if (newrep != null)
                {
                    if (tosav.Contains(oldrep))
                    {
                        tosav = tosav.Replace(oldrep, newrep);
                    }
                    if (arrow.Contains(oldrep))
                    {
                        arrow = arrow.Replace(oldrep, newrep);
                    }
                    else
                    {
                        DrawBar(@PATH, tosav, arrow, old);
                        Console.SetCursorPosition(25, 8);
                        Console.WriteLine("CANNOT FIND WORD TO REPLACE!");
                        Console.ReadKey();
                    }
                }

            }
            DrawBar(@PATH, tosav, arrow, old);
        }

        static string Exit(string tosav, string old, string arrow, string PATH)
        {
            if (tosav != old)
            {
                Console.BackgroundColor = EnterBg;
                Console.ForegroundColor = EnterFg;
                Console.Clear();
                bool shouldSave;
                Console.SetCursorPosition(20, 13);
                Console.WriteLine("[Do you want to save changes? Y/N ]");
                Console.SetCursorPosition(20, 14);
                string answer = Console.ReadKey().KeyChar.ToString();
                if (answer.ToLower() == "y") shouldSave = true;
                else shouldSave = false;
                if (shouldSave == false)
                {
                    File.Delete(@PATH);
                    File.AppendAllText(@PATH, old);
                    Console.ResetColor();
                    Console.Clear();
                    Console.CursorVisible = true; return null;
                }
                else if (shouldSave == true)
                {
                    File.Delete(@PATH);
                    File.AppendAllText(@PATH, tosav + arrow);
                    Console.ResetColor();
                    Console.Clear();
                    Console.CursorVisible = true; return null;
                }
            }
            else
            {
                Console.ResetColor();
                Console.Clear();
                Console.CursorVisible = true; return null;
            }
            Console.CursorVisible = true; return null;
        }


        static void Find(string tosav, string old, string arrow, string PATH)
        {
            Console.BackgroundColor = EditorBg;
            DrawBar(@PATH, tosav, arrow, old);
            Console.BackgroundColor = EditorBg;
            Console.Write(@tosav);
            Console.Write(@arrow);
            Console.ForegroundColor = EnterFg;
            Console.BackgroundColor = EnterBg;
            Console.SetCursorPosition(20, 13);
            Console.Write("FIND [                              ]"); Console.SetCursorPosition(26, 13);
            string find = LootiTerminal();
            if (find != null)
            {
                if (tosav.Contains(find) || arrow.Contains(find))
                {
                    string tofind = "";
                    if (tosav.Contains(find)) for (int i = 0; i < find.Length; i++) tofind += tosav.Replace(find, "!");
                    if (arrow.Contains(find)) for (int i = 0; i < find.Length; i++) tofind += arrow.Replace(find, "!");
                    Console.BackgroundColor = EditorBg;
                    DrawBar(@PATH, tofind, arrow, old);
                    Console.BackgroundColor = EditorBg;
                    Console.ForegroundColor = EditorFg;
                    Console.Write(tofind);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.ReadKey();

                    DrawBar(@PATH, tofind, arrow, old);

                }
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                DrawBar(@PATH, tosav, arrow, old);
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write(@tosav);
                Console.Write(@arrow);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(20, 13);
                Console.Write($"Text does not contain '{find}'!"); Console.SetCursorPosition(27, 13);
                Console.ReadKey();
                DrawBar(@PATH, tosav, arrow, old);
            }
        }

        static string Open()
        {
            Console.BackgroundColor = EnterBg;
            Console.ForegroundColor = EnterFg;
            Console.Clear();
            Console.SetCursorPosition(20, 0);
            Console.Write("Files:");
            string[] Fils = Directory.GetFiles(Directory.GetCurrentDirectory());
            int numoffiles = Fils.Length;
            for (int i = 0; i < numoffiles; i++)
            {
                Console.SetCursorPosition(20, i + 1);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(Fils[i]);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("]\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("[File to open]");
            Console.SetCursorPosition(0, 1);
            string open = LootiTerminal();
            return open;
        }

        static public string LootiTerminal()
        {
            string toreturn = "";
            for (; ; )
            {
                string arrow = "";
                ConsoleKeyInfo input = Console.ReadKey();
                if (input.Key == ConsoleKey.Enter)
                {
                    return toreturn + arrow;
                }
                else if (input.Key == ConsoleKey.Backspace)
                {
                    if (toreturn.Length != 0)
                    {

                        Console.CursorLeft--;
                        toreturn = toreturn.Remove(toreturn.Length - 1, 1);
                        Console.Write(" ");
                        Console.CursorLeft--;
                    }
                    else
                    {
                        Console.CursorLeft++;
                    }
                }
                else if (input.Key == ConsoleKey.LeftArrow)
                {

                    if (toreturn.Length > 0)
                    {
                        arrow = toreturn[toreturn.Length - 1] + arrow;
                        toreturn = toreturn.Remove(toreturn.Length - 1, 1);
                    }
                }
                else if (input.Key == ConsoleKey.RightArrow && arrow.Length > 0)
                {
                    toreturn += arrow[0];
                    arrow = arrow.Remove(0, 1);
                }
                else if (input.Key == ConsoleKey.RightArrow && arrow.Length == 0)
                {
                    Console.CursorLeft--;
                }
                else if (input.Key == ConsoleKey.Escape)
                {
                    return null;
                }
                else
                {
                    toreturn += input.KeyChar.ToString();
                }
            }
        }
    }
}
