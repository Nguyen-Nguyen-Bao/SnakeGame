using System.Diagnostics;
using System.Text;
namespace SnakeGame;
class Point
{
    public int row { get; set; }
    public int col { set; get; }
    public Point(int _row, int _col)
    {
        row = _row;
        col = _col;
    }
    public Point()
    {

    }
}
class Program
{
    static string input;
    static int speed = 500;
    static int point = 0;
    static bool gameover = false;
    static string path = @"C:\NguyenBao\C#\Test\SnakeGame";
    static string accountsfile = "UserName.txt";
    static string passwordsfile = "PassWord.txt";
    static string scoreshistoryfile = "HighScoreHistory.txt";
    static string scoresfile = "HighScore.txt";
    static string scorebroadfile = "HighScoreBroad.txt";
    static string nameAccount;
    static string passwordAccount;
    static string nameCheck;
    static string passwordCheck;
    static string pass;
    static bool doNameExist = false;
    static bool doPasswordExist = false;
    static Stopwatch stopwatch = new Stopwatch();
    static bool foodRight;
    static Random rand = new Random();
    static int rows = 7, cols = 7;
    static int count = 0;

    static string direction;
    static bool doFoodExisted = false;

    static string[,] broad = new string[rows, cols];
    static Point snake_head = new Point();
    static Point food = new Point();
    static int[] snake_body_row = new int[2];
    static int[] snake_body_col = new int[2];
    static DateTime timeBegin;
    static DateTime timeFinish;
    static TimeSpan timespan;
    static void Main(string[] args)
    {
        Console.InputEncoding = Encoding.Unicode;
        Console.OutputEncoding = Encoding.Unicode;
        do
        {
            GameReset();
            GameMenu();
            if (string.IsNullOrEmpty(pass))
            {
                Console.Clear();
                Console.WriteLine("Your Name Is Hidden Away!!!");
            }
            SetupBroad();
            GiveDircetion();
            SpawnSnake_1st();
            DrawBroad();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine();
            Console.WriteLine($"GAME START!!!");
            Console.ResetColor();
            SetupTimer(out DateTime timeStart);
            Task.Delay(speed).Wait();
            Thread thrd = new Thread(Listenkey);
            thrd.Start();
            do
            {
                Console.Clear();
                SetupBroad();
                SpawnSnake();
                SpawnFood();
                SnakeMove();
                DoesEatFood();
                DrawBroad();
                Console.WriteLine();
                GameOverCheck();
                if (gameover)
                {
                    EndTimer(out DateTime timeEnd);
                    timeBegin = timeStart;
                    timeFinish = timeEnd;
                    timespan = timeFinish - timeBegin;
                    Record();
                    do
                    {
                        EndMenu();
                        if (string.IsNullOrEmpty(input))
                        {
                            break;
                        }
                    }
                    while (true);
                    break;
                }
                Task.Delay(speed).Wait();

            }
            while (true);
        }
        while (true);
    }
    static void SetupTimer(out DateTime timeSTART)
    {
        DateTime _timeSTART = DateTime.Now;
        timeSTART = _timeSTART;
    }
    static void SetupBroad()
    {
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                if (i == 0 || j == 0 || i == rows - 1 || j == cols - 1)
                {
                    broad[i, j] = " # ";
                }
                else
                {
                    broad[i, j] = "   ";
                }
            }
        }
    }
    static void DrawBroad()
    {
        stopwatch.Start();
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < cols; ++j)
            {
                if (broad[i, j] == " Q ")
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write(broad[i, j]);
                    Console.ResetColor();
                }
                else if (broad[i, j] == " # ")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(broad[i, j]);
                    Console.ResetColor();
                }
                else if (broad[i, j] == " < " || broad[i, j] == " > " || broad[i, j] == " ^ " || broad[i, j] == " v ")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(broad[i, j]);
                    Console.ResetColor();
                }
                else if (broad[i, j] == " 0 ")
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(broad[i, j]);
                    Console.ResetColor();
                }
                else
                    Console.Write(broad[i, j]);
            } 
            Console.WriteLine();
        }
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write("Time: ");
        Console.ResetColor();
        Console.Write($"{stopwatch}");
    }
    static void GiveDircetion()
    {
        int r = rand.Next(1, 4);
        if (r == 1)
        {
            direction = "right";
        }
        else if (r == 2)
        {
            direction = "left";
        }
        else if (r == 3)
        {
            direction = "up";
        }
        else
        {
            direction = "down";
        }
    }
    static void SpawnSnake_1st()
    {
        snake_head.row = rand.Next(3, rows - 4);
        snake_head.col = rand.Next(3, cols - 4);
        int r;
        if (direction == "right")
        {
            broad[snake_head.row, snake_head.col] = " < ";
        }
        else if (direction == "left")
        {
            broad[snake_head.row, snake_head.col] = " > ";
        }
        else if (direction == "up")
        {
            broad[snake_head.row, snake_head.col] = " v ";
        }
        else
        {
            broad[snake_head.row, snake_head.col] = " ^ ";
        }
        do
        {
            r = rand.Next(1, 4);
            if (r == 1 && direction != "right")
            {
                for (int i = snake_head.col + 1; i < rows * rows; ++i)
                {
                    broad[snake_head.row, i] = " 0 ";
                    snake_body_row[count] = snake_head.row;
                    snake_body_col[count] = i;
                    ++count;
                    if (count == 2) break;
                }
            }
            else if (r == 2 && direction != "left")
            {
                for (int i = snake_head.col - 1; i < rows * rows; --i)
                {
                    broad[snake_head.row, i] = " 0 ";
                    snake_body_row[count] = snake_head.row;
                    snake_body_col[count] = i;
                    ++count;
                    if (count == 2) break;
                }
            }
            else if (r == 3 && direction != "up")
            {
                for (int i = snake_head.row - 1; i < rows * rows; --i)
                {
                    broad[i, snake_head.col] = " 0 ";
                    snake_body_row[count] = i;
                    snake_body_col[count] = snake_head.col;
                    ++count;
                    if (count == 2) break;
                }
            }
            else if (r == 4 && direction != "down")
            {
                for (int i = snake_head.row + 1; i < rows * rows; ++i)
                {
                    broad[i, snake_head.col] = " 0 ";
                    snake_body_row[count] = i;
                    snake_body_col[count] = snake_head.col;
                    ++count;
                    if (count == 2) break;
                }
            }
        }
        while (r == 1 && direction == "right" || r == 2 && direction == "left" || r == 3 && direction == "up" || r == 4 && direction == "down");
    }
    static void SnakeMove()
    {
        switch (direction)
        {
            case "right":
                if (snake_head.col == cols - 2
                )
                {
                    snake_head.col = 0;
                }
                ++snake_head.col;
                break;
            case "left":
                if (snake_head.col == 1)
                {
                    snake_head.col = cols - 1;
                }
                --snake_head.col;
                break;
            case "up":
                if (snake_head.row == 1)
                {
                    snake_head.row = rows - 1;
                }
                --snake_head.row;
                break;
            case "down":
                if (snake_head.row == rows - 2)
                {
                    snake_head.row = 0;
                }
                ++snake_head.row;
                break;
        }
        if (direction == "right")
        {
            broad[snake_head.row, snake_head.col] = " < ";
        }
        else if (direction == "left")
        {
            broad[snake_head.row, snake_head.col] = " > ";
        }
        else if (direction == "up")
        {
            broad[snake_head.row, snake_head.col] = " v ";
        }
        else
        {
            broad[snake_head.row, snake_head.col] = " ^ ";
        }
    }
    static void SpawnSnake()
    {
        for (int i = snake_body_col.Length - 1; i > 0; --i)
        {
            snake_body_row[i] = snake_body_row[i - 1];
            snake_body_col[i] = snake_body_col[i - 1];
        }
        snake_body_row[0] = snake_head.row;
        snake_body_col[0] = snake_head.col;
        for (int i = 0; i < snake_body_row.Length; i++)
        {
            broad[snake_body_row[i], snake_body_col[i]] = " 0 ";
        }
    }
    static void Listenkey()
    {
        do
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            switch (keyInfo.Key)
            {
                case ConsoleKey.RightArrow:
                    if (direction != "left")
                    {
                        direction = "right";
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    if (direction != "right")
                    {
                        direction = "left";
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (direction != "down")
                    {
                        direction = "up";
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (direction != "up")
                    {
                        direction = "down";
                    }
                    break;
            }
        }
        while (true);
    }
    static public void SpawnFood()
    {
        if (!doFoodExisted)
        {
            do
            {
                food.row = rand.Next(1, rows - 1);
                food.col = rand.Next(1, cols - 1);
                foodRight = true;
                for (int i = 0; i < snake_body_row.Length; ++i)
                {
                    if (food.row == snake_body_row[i] && food.col == snake_body_col[i])
                    {
                        foodRight = false;
                        break;
                    }
                }
            }
            while (food.row == snake_head.row && food.col == snake_head.col || !foodRight);
        }
        doFoodExisted = true;
        broad[food.row, food.col] = " Q ";
    }
    static public void DoesEatFood()
    {
        if (snake_head.row == food.row && snake_head.col == food.col)
        {
            doFoodExisted = false;
            Array.Resize(ref snake_body_row, snake_body_row.Length + 1);
            Array.Resize(ref snake_body_col, snake_body_col.Length + 1);
            ++point;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("nice!");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine();
        }

    }
    static public void GameOverCheck()
    {
        for (int i = 0; i < snake_body_col.Length; ++i)
        {
            if (snake_body_row[i] == snake_head.row && snake_body_col[i] == snake_head.col)
            {
                gameover = true;
            }
        }
        if (!gameover)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Score: {point}");
            Console.ResetColor();
        }
        else if (gameover)
        {
            stopwatch.Stop();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Your score: {point}!!!");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"GAME OVER!!!");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
    static void GameReset()
    {
        point = 0;
        gameover = false;
        count = 0;
        doFoodExisted = false;
        snake_body_row = new int[2];
        snake_body_col = new int[2];
        nameAccount = "NameIsHidden!!!";
        stopwatch.Reset();
    }
    static string GameMenu()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("MENU");
        Console.WriteLine("---------------------------");
        Console.ResetColor();
        Console.Write($"Press any buttom to ");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write($"START");
        Console.ResetColor();
        Console.WriteLine("!!!");
        Console.Write($"Contains q or Q is ");
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write($"Quit");
        Console.ResetColor();
        Console.WriteLine($"!!!");
        Console.Write("AND ALSO Press u or U for ");
        Console.Write("Sign Up ");
        Console.Write("and Press i or I for ");
        Console.Write("Sign In");
        Console.WriteLine("!!!");
        input = Console.ReadLine();
        if (input.Contains("q") || input.Contains("Q"))
        {
            Environment.Exit(0);
        }
        else if (input.Contains("u") || input.Contains("U"))
        {
            do
            {
                SignUp();
                if (string.IsNullOrEmpty(input))
                    break;
            }
            while (true);
            do
            {
                SignIn(out string pass);
                if (string.IsNullOrEmpty(input))
                {
                    return pass;
                }
            }
            while (true);
        }
        else if (input.Contains("i") || input.Contains("I"))
        {
            do
            {
                SignIn(out string pass);
                if (string.IsNullOrEmpty(input))
                {
                    return pass;
                }
            }
            while (true);
        }
        return "";
    }
    static string SignUp()
    {
        Console.Clear();
        FileStream frN = new FileStream(Path.Combine(path, accountsfile), FileMode.Open);
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("BẠN ĐANG ĐĂNG KÝ TÀI KHOẢN:");
        Console.WriteLine("---------------------------");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Viết tên của bạn: ");
        Console.ResetColor();
        nameAccount = Console.ReadLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Viết mật khẩu của bạn: ");
        Console.ResetColor();
        passwordAccount = Console.ReadLine();
        using (StreamReader readN = new StreamReader(frN))
        {
            do
            {
                if ((nameCheck = readN.ReadLine()) == nameAccount)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("This player already Exist");
                    Console.ResetColor();
                    doNameExist = true;
                    break;
                }
            }
            while (nameCheck != null);
        }
        if (!doNameExist)
        {
            FileStream fwN = new FileStream(Path.Combine(path, accountsfile), FileMode.Append);
            using (StreamWriter writeN = new StreamWriter(fwN))
            {
                writeN.WriteLine($"{nameAccount}");
            }
            FileStream fwP = new FileStream(Path.Combine(path, passwordsfile), FileMode.Append);
            using (StreamWriter writeP = new StreamWriter(fwP))
            {
                writeP.WriteLine($"{passwordAccount}");
            }
            Console.Write("Sign Up Successfully!");
            Console.WriteLine("!!!");
            doNameExist = false;
        }
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Try SiGN UP again? Press any buttom!");
        Console.WriteLine("Dont press anything and Sign In");
        Console.ResetColor();
        input = Console.ReadLine();
        return input;
    }
    static string SignIn(out string pass)
    {
        FileStream frN = new FileStream(Path.Combine(path, accountsfile), FileMode.Open);
        FileStream frP = new FileStream(Path.Combine(path, passwordsfile), FileMode.Open);
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("BẠN ĐANG ĐĂNG NHẬP TÀI KHOẢN:");
        Console.WriteLine("-----------------------------");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write("Viết tên của bạn: ");
        Console.ResetColor();
        nameAccount = Console.ReadLine();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write("Viết mật khẩu của bạn: ");
        Console.ResetColor();
        passwordAccount = Console.ReadLine();
        using (StreamReader readN = new StreamReader(frN))
        {
            do
            {
                if ((nameCheck = readN.ReadLine()) == nameAccount)
                {
                    doNameExist = true;
                    break;
                }
            }
            while (nameCheck != null);
        }
        using (StreamReader readP = new StreamReader(frP))
        {
            do
            {
                if ((passwordCheck = readP.ReadLine()) == passwordAccount)
                {
                    doPasswordExist = true;
                    break;
                }
            }
            while (passwordCheck != null);
        }
        if (doNameExist && doPasswordExist)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Sign In Successfully!");
            Console.ResetColor();
            doNameExist = false;
            doPasswordExist = false;
            input = null;
            pass = passwordAccount;
            Task.Delay(speed).Wait();
            return input;
        }
        else
        {
            nameAccount = "NameIsHidden!!!";
            passwordAccount = null;
        }
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Sign In Fails!");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Try SiGN IN again? Press any buttom!");
        Console.WriteLine("Dont press anything and hide YOUR NAME AWAY!");
        Console.ResetColor();
        input = Console.ReadLine();
        Console.Clear();
        pass = passwordAccount;
        return input;
    }
    static void Record()
    {
        FileStream fwh1 = new FileStream(Path.Combine(path, scoreshistoryfile), FileMode.Append);
        using (StreamWriter writeS1 = new StreamWriter(fwh1))
        {
            writeS1.Write($"Player:{nameAccount}");
            for (int i = nameAccount.Length; i <= 25; ++i)
            {
                writeS1.Write(" ");
            }
            writeS1.WriteLine($"Score:{point}\tTime:{timespan.Minutes}(m)\t{timespan.Seconds}(s) {timespan.Milliseconds}(ms)\tDateTime:{timeBegin}");
        }
        FileStream fwh2 = new FileStream(Path.Combine(path, scoresfile), FileMode.Append);
        using (StreamWriter writeS2 = new StreamWriter(fwh2))
        {
            writeS2.WriteLine($"{nameAccount} {point} {timespan.TotalMilliseconds} {timeBegin}");
        }
    }
    static void EndTimer(out DateTime t)
    {
        DateTime _timeEND = DateTime.Now;
        t = _timeEND;
    }
    static string EndMenu()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("MENU");
        Console.WriteLine("--------------------------------");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Press h or H to see Play History");
        Console.WriteLine("Press n or N to see Player list");
        Console.WriteLine("Press p or P to see Player's Records");
        Console.WriteLine("Press r or R to see All Records");
        Console.WriteLine("Dont press anything and back to MENU");
        Console.ResetColor();
        input = Console.ReadLine();
        Console.WriteLine($"Bạn đã nhập: {input}");
        if (input.Contains("h") || input.Contains("H"))
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("LICH SỬ CHƠI GAME");
            Console.WriteLine("-----------------");
            Console.WriteLine();
            Console.ResetColor();
            FileStream frh = new FileStream(Path.Combine(path, scoreshistoryfile), FileMode.Open);
            using (StreamReader readH = new StreamReader(frh))
            {
                string line;
                while ((line = readH.ReadLine()) != null)
                {

                    Console.WriteLine($"{line}");
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
        else if (input.Contains("n") || input.Contains("N"))
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("DANH SÁCH NGƯỜI CHƠI");
            Console.WriteLine("--------------------");
            Console.ResetColor();
            int u = 1;
            FileStream frN = new FileStream(Path.Combine(path, accountsfile), FileMode.Open);
            using (StreamReader readN = new StreamReader(frN))
            {
                string line;
                while ((line = readN.ReadLine()) != null)
                {
                    Console.WriteLine($"{line}");
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

        }
        else if (input.Contains("p") || input.Contains("P"))
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Viết tên người chơi");
            Console.ResetColor();
            string playername = Console.ReadLine();
            string[] playerpoints = new string[0];
            string[] playertimespans = new string[0];
            string[] playerdatetimesd = new string[0];
            string[] playerdatetimess = new string[0];
            string[] playerInfo = new string[6];
            int u = 0;
            FileStream frh = new FileStream(Path.Combine(path, scoresfile), FileMode.Open);
            using (StreamReader readH = new StreamReader(frh))
            {
                string line;
                while ((line = readH.ReadLine()) != null)
                {
                    playerInfo = line.Split(" ");
                    if (playerInfo[0] == playername)
                    {
                        Array.Resize(ref playerpoints, playerpoints.Length + 1);
                        Array.Resize(ref playertimespans, playertimespans.Length + 1);
                        Array.Resize(ref playerdatetimesd, playerdatetimesd.Length + 1);
                        Array.Resize(ref playerdatetimess, playerdatetimess.Length + 1);
                        playerpoints[u] = playerInfo[1];
                        playertimespans[u] = playerInfo[2];
                        playerdatetimesd[u] = playerInfo[3];
                        playerdatetimess[u] = playerInfo[4];
                        ++u;
                    }
                }
            }
            int[] playerpoints2 = Array.ConvertAll(playerpoints, int.Parse);
            double[] playertimespans2 = Array.ConvertAll(playertimespans, double.Parse);
            DateTime[] playerdatetimesd2 = Array.ConvertAll(playerdatetimesd, DateTime.Parse);
            DateTime[] playerdatetimess2 = Array.ConvertAll(playerdatetimess, DateTime.Parse);
            int[] playerpointsort = new int[playerpoints.Length];
            double[] playertimespansort = new double[playertimespans.Length];
            DateTime[] playerdatetimesdsort = new DateTime[playerdatetimesd.Length];
            DateTime[] playerdatetimesssort = new DateTime[playerdatetimess.Length];
            int rank = 0;
            for (int i = 0; i < playerpoints.Length; ++i)
            {
                for (int j = 0; j < playerpoints.Length; ++j)
                {
                    if (playerpoints2[i] < playerpoints2[j])
                    {
                        ++rank;
                    }
                    else if (playerpoints2[i] == playerpoints2[j])
                    {
                        if (playertimespans2[i] > playertimespans2[j])
                        {
                            ++rank;
                        }
                    }
                }
                playerpointsort[rank] = playerpoints2[i];
                playertimespansort[rank] = playertimespans2[i];
                playerdatetimesdsort[rank] = playerdatetimesd2[i];
                playerdatetimesssort[rank] = playerdatetimess2[i];
                rank = 0;
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("BẢNG XẾP HẠNG THÀNH TÍCH CÁ NHÂN");
            Console.WriteLine("--------------------------------");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine($"CỦA NGƯỜI CHƠI {playername}");
            for (int i = 0; i < playerpoints.Length; ++i)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"RANK ");
                Console.Write($"{i + 1}: \t");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"score: ");
                Console.ResetColor();
                Console.Write($"{playerpointsort[i]}\t");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"timespan: ");
                Console.ResetColor();
                Console.Write($"{playertimespansort[i]}\t");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"datetime:");
                Console.ResetColor();
                Console.Write($"{playerdatetimesdsort[i]} {playerdatetimesssort[i]}");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

        }
        else if (input.Contains("r") || input.Contains("R"))
        {
            Console.Clear();
            Console.WriteLine("Viết tên người chơi");
            string playername = "NameIsHidden!!!";
            string[] playernames = new string[0];
            string[] playerpoints = new string[0];
            string[] playertimespans = new string[0];
            string[] playerdatetimesd = new string[0];
            string[] playerdatetimess = new string[0];
            string[] playerInfo = new string[6];
            int u = 0;
            FileStream frh = new FileStream(Path.Combine(path, scoresfile), FileMode.Open);
            using (StreamReader readH = new StreamReader(frh))
            {
                string line;
                while ((line = readH.ReadLine()) != null)
                {
                    playerInfo = line.Split(" ");
                    Array.Resize(ref playernames, playernames.Length + 1);
                    Array.Resize(ref playerpoints, playerpoints.Length + 1);
                    Array.Resize(ref playertimespans, playertimespans.Length + 1);
                    Array.Resize(ref playerdatetimesd, playerdatetimesd.Length + 1);
                    Array.Resize(ref playerdatetimess, playerdatetimess.Length + 1);
                    playernames[u] = playerInfo[0];
                    playerpoints[u] = playerInfo[1];
                    playertimespans[u] = playerInfo[2];
                    playerdatetimesd[u] = playerInfo[3];
                    playerdatetimess[u] = playerInfo[4];
                    ++u;
                }
            }
            int[] playerpoints2 = Array.ConvertAll(playerpoints, int.Parse);
            double[] playertimespans2 = Array.ConvertAll(playertimespans, double.Parse);
            DateTime[] playerdatetimesd2 = Array.ConvertAll(playerdatetimesd, DateTime.Parse);
            DateTime[] playerdatetimess2 = Array.ConvertAll(playerdatetimess, DateTime.Parse);
            string[] playernamesort = new string[playernames.Length];
            int[] playerpointsort = new int[playerpoints.Length];
            double[] playertimespansort = new double[playertimespans.Length];
            DateTime[] playerdatetimesdsort = new DateTime[playerdatetimesd.Length];
            DateTime[] playerdatetimesssort = new DateTime[playerdatetimess.Length];
            int rank = 0;
            for (int i = 0; i < playerpoints.Length; ++i)
            {
                for (int j = 0; j < playerpoints.Length; ++j)
                {
                    if (playerpoints2[i] < playerpoints2[j])
                    {
                        ++rank;
                    }
                    else if (playerpoints2[i] == playerpoints2[j])
                    {
                        if (playertimespans2[i] > playertimespans2[j]) ++rank;
                    }
                }
                playernamesort[rank] = playernames[i];
                playerpointsort[rank] = playerpoints2[i];
                playertimespansort[rank] = playertimespans2[i];
                playerdatetimesdsort[rank] = playerdatetimesd2[i];
                playerdatetimesssort[rank] = playerdatetimess2[i];
                rank = 0;
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("BẢNG XẾP HẠNG THÀNH TÍCH");
            Console.WriteLine("------------------------");
            Console.ResetColor();
            Console.WriteLine();
            for (int i = 0; i < playerpoints.Length; ++i)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"RANK {i + 1}\t");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"playername:");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"{playernamesort[i]}");
                for (int j = playernamesort[i].Length; j <= 25; ++j)
                {
                    Console.Write(" ");
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"score: ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"{playerpointsort[i]}\t");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"timespan: ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"{playertimespansort[i]}\t");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"datetime: ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"{playerdatetimesdsort[i]} ");
                Console.Write($"{playerdatetimesssort[i]}");
                Console.ResetColor();
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }
        return input;
    }
}
