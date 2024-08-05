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
    static void Main(string[] args)
    {
        do
        {
            GameReset();
            Menu();
            SetupBroad();
            GiveDircetion();
            SpawnSnake_1st();
            DrawBroad();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"GAME START!!!");
            Console.ResetColor();
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
                if (gameover) break;
                Task.Delay(speed).Wait();

            }
            while (true);
        }
        while (true);
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
            Task.Delay(speed).Wait();
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
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Your score: {point}!!!");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"GAME OVER!!!");
            Console.ResetColor();
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
    }
    static void Menu()
    {
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
        input = Console.ReadLine();
        if (input.Contains("q") || input.Contains("Q"))
        {
            Environment.Exit(1);
        }
    }
}

