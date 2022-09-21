﻿using Cosmos.HAL;
using Cosmos.System.Graphics;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.IPv4.UDP.DNS;
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;
using System.IO;


//Goplex Studios - GoOS
//Copyright (C) 2022  Owen2k6
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <https://www.gnu.org/licenses/>.

namespace GoOS
{
    public class Kernel : Sys.Kernel
    {
        private Boolean adminconsoledisk = false;
        private static Sys.FileSystem.CosmosVFS FS;
        public static string file;

        private static string request = string.Empty;
        private static TcpClient tcpc = new TcpClient(80);
        private static Address dns = new Address(8, 8, 8, 8);
        private static EndPoint endPoint = new EndPoint(dns, 80);
        public static bool ParseHeader()
        {
            return false;
        }
        bool isenabled = true;
        public static VGAScreen VScreen = new VGAScreen();
        /* SNAKE VARS */
        public int[] matrix;
        public List<int[]> commands;
        public List<int[]> snake;
        public List<int> food;
        public int randomNumber;
        public int snakeCount;
        Random rnd = new Random();

        /* SNAKE FUNCS */
        public string getSnakeScore()
        {
            if (snakeCount < 10)
            {
                return snakeCount + "   ";
            }
            else if (snakeCount < 100)
            {
                return snakeCount + "  ";
            }
            else if (snakeCount < 1000)
            {
                return snakeCount + " ";
            }
            else
            {
                return snakeCount + "";
            }
        }

        public void updatePosotion()
        {
            List<int[]> tmp = new List<int[]>();

            foreach (int[] point in snake)
            {
                switch (point[1])
                {
                    case 1:
                        point[0] = point[0] - 1;
                        break;
                    case 2:
                        point[0] = point[0] + 80;
                        break;
                    case 3:
                        point[0] = point[0] + 1;
                        break;
                    case 4:
                        point[0] = point[0] - 80;
                        break;
                    default:
                        break;
                }
                tmp.Add(point);
            }
            snake = tmp;
        }

        public void changeArray()
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = 0;
            }

            foreach (int[] point in snake)
            {
                matrix[point[0]] = 3;
            }

            foreach (int point in food)
            {
                matrix[point] = 2;
            }

            for (int i = 0; i < matrix.Length; i++)
            {
                if (i <= 79 && i >= 0)
                {
                    matrix[i] = 1;
                }
                else if (i <= 1760 && i >= 1679)
                {
                    matrix[i] = 1;
                }
                else if (i % 80 == 0)
                {
                    matrix[i] = 1;
                }

                else if (i % 80 == 79)
                {
                    matrix[i] = 1;
                }
            }
        }

        public Boolean gameover()
        {
            int head = snake[0][0];
            for (int i = 1; i < snakeCount; i++)
            {
                if (head == snake[i][0])
                {
                    return true;
                }
            }
            if (head % 80 == 79 || head % 80 == 0 || head <= 1760 && head >= 1679 || head <= 79 && head >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void printGame()
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                if (gameover() && i == 585)
                {
                    Console.Write("###############################");
                    i = i + 30;
                }
                else if (gameover() && i == 665)
                {
                    Console.Write("###############################");
                    i = i + 30;
                }
                else if (gameover() && i == 745)
                {
                    Console.Write("####                       ####");
                    i = i + 30;
                }
                else if (gameover() && i == 825)
                {
                    Console.Write("####       GAMEOVER        ####");
                    i = i + 30;
                }
                else if (gameover() && i == 905)
                {
                    Console.Write("####      Score: " + getSnakeScore() + "      ####");
                    i = i + 30;
                }
                else if (gameover() && i == 985)
                {
                    Console.Write("####                       ####");
                    i = i + 30;
                }
                else if (gameover() && i == 1065)
                {
                    Console.Write("###############################");
                    i = i + 30;
                }
                else if (gameover() && i == 1145)
                {
                    Console.Write("###############################");
                    i = i + 30;
                }
                else if (matrix[i] == 1)
                {
                    Console.Write("#");
                }
                else if (matrix[i] == 2)
                {
                    Console.Write("$");
                }
                else if (matrix[i] == 3)
                {
                    Console.Write("*");
                }
                else
                {
                    Console.Write(" ");
                }
            }

            Console.Write("#  Current score: " + getSnakeScore() + "      Exit game: ESC button      Restart game: R button  #");
            Console.Write("################################################################################");
        }

        public void delay(int time)
        {
            for (int i = 3; i < time; i++) ;
        }

        public int xy2p(int x, int y)
        {
            return y * 80 + x;
        }

        public int randomFood()
        {
            int rand = rnd.Next(81, 1700);
            if (rand != randomNumber)
            {
                randomNumber = rand;
                return rand;
            }
            else
            {
                return 1400;
            }
        }

        public void configSnake()
        {
            matrix = new int[1760];
            commands = new List<int[]>();
            snake = new List<int[]>();
            food = new List<int>();
            randomNumber = 0;
            snake.Add(new int[2] { xy2p(10, 10), 3 });
            changeArray();
            food.Add(randomFood());
        }

        public void updateDirections()
        {
            List<int[]> tmp = new List<int[]>();
            foreach (int[] com in commands)
            {
                if (com[1] < snakeCount)
                {
                    snake[com[1]][1] = com[0];
                    com[1] = com[1] + 1;
                    tmp.Add(com);
                }
            }
            commands = tmp;
        }

        public void checkIfTouchFood()
        {
            List<int> foodtmp = new List<int>();
            foreach (int pos in food)
            {
                if (snake[0][0] == pos)
                {
                    foodtmp.Add(randomFood());
                    int tmp1 = snake[snakeCount - 1][0];
                    int tmp2 = snake[snakeCount - 1][1];
                    if (tmp2 == 1)
                    {
                        tmp1 = tmp1 + 1;
                    }
                    else if (tmp2 == 2)
                    {
                        tmp1 = tmp1 - 80;
                    }
                    else if (tmp2 == 3)
                    {
                        tmp1 = tmp1 - 1;
                    }
                    else if (tmp2 == 4)
                    {
                        tmp1 = tmp1 + 80;
                    }
                    snake.Add(new int[] { tmp1, tmp2 });
                }
                else
                {
                    foodtmp.Add(pos);
                }
            }
            food = foodtmp;
        }

        public void printLogo()
        {
            Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine();
            Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine(); Console.WriteLine();
            Console.WriteLine("  #####                               ");
            Console.WriteLine(" #     # #    #   ##   #    # ######  ");
            Console.WriteLine(" #       ##   #  #  #  #   #  #       ");
            Console.WriteLine("  #####  # #  # #    # ####   #####   ");
            Console.WriteLine("       # #  # # ###### #  #   #       ");
            Console.WriteLine(" #     # #   ## #    # #   #  #       ");
            Console.WriteLine("  #####  #    # #    # #    # ######  ");
            Console.WriteLine("         Created by Denis Bartashevich");
            Console.WriteLine("    Imported into GoOS Core by Owen2k6");
            Console.WriteLine("");

        }
        protected override void BeforeRun()
        {
            try
            {
                NetworkDevice nic = NetworkDevice.GetDeviceByName("eth0"); //get network device by name
                IPConfig.Enable(nic, new Address(192, 168, 1, 69), new Address(255, 255, 255, 0), new Address(192, 168, 1, 254)); //enable IPv4 configuration
                using (var xClient = new DHCPClient())
                {
                    /** Send a DHCP Discover packet **/
                    //This will automatically set the IP config after DHCP response
                    xClient.SendDiscoverPacket();
                }
                using (var xClient = new DnsClient())
                {
                    xClient.Connect(new Address(192, 168, 1, 254)); //DNS Server address

                    /** Send DNS ask for a single domain name **/
                    xClient.SendAsk("github.com");

                    /** Receive DNS Response **/
                    Address destination = xClient.Receive(); //can set a timeout value


                }
            }
            catch
            {
                Console.WriteLine("Error starting Goplex Web Interface.");
                Console.WriteLine("The system will proceed to boot without networking.");
                Console.WriteLine("Press ENTER to continue (and yes it has to be ENTER)");
                Console.ReadLine();
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("                    GGGGGGGGGGGG                   ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("               GGGGGGGGGGGGGGGGGGGGGG              ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("  GGGGGGGGGG GGGGGGGGGGGGGGGGGGGGGGGGGG            ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("  GGGGGGGG   GGGGGGGGG        GGGGGG               ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  GGGGGGG    GGGGG                                 ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("  GGGGGG     GGG                                   ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("  GGGGG      GG                                    ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Goplex Studios GoOS.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  GGGGG      G            GGGGGGGGGGGGGGGGGGGG     ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Copyright 2022 (c) Owen2k6.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("  GGGGG      GG           GGGGGGGGGGGGGGGGGGG      ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Version 1.4");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("  GGGGG      GG           GGGGGGGGGGGGGGGGGGG      ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Private Development Build");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("  GGGGGG     GGGG         GGGGGGGGGGGGGGGGGG       ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  GGGGGGG    GGGGGG              GGGGGGGGGG        ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("  GGGGGGGGG  GGGGGGGGGGGGGGGGGGGGGGGGGGGG          ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("  GGGGGGGGGGG                                      ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("  GGGGGGGGGGGGGGG                  GGGG            ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG            ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("  GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG            ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("");
            Console.WriteLine("Welcome to GoOS. Would you like a basics tutorial?");
            String input = Console.ReadLine();
            if (input == "yes")
            {
                Console.WriteLine("Start Tutorial before running GoOS Kernel.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Continue to GoOS with no tutorial.");
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }

        protected override void Run()
        {
            Console.Write("0:\\");
            String input = Console.ReadLine();
            //And so it begins...
            //Commands Section
            if (input == "cinfo")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Goplex Operating System");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("GoOS is owned by Goplex Studios.");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("SYSTEM INFOMATION:");
                Console.WriteLine("GoOS Version 1.4");
                Console.WriteLine("Owen2k6 Api version: 0.15");
                Console.WriteLine("Branch: Development");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Copyright 2022 (c) Owen2k6");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (input == "help")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Goplex Operating System");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("HELP - Shows system commands");
                Console.WriteLine("CINFO - Shows system infomation");
                Console.WriteLine("SUPPORT - Shows how to get support");
                Console.WriteLine("GAMES - Shows the list of GoOS Games");
                Console.WriteLine("CORE - Displays GoOS Core infomation");
                Console.WriteLine("CALC - Shows a list of possible calculation commands");
                Console.WriteLine("CREDITS - Shows the GoOS Developers");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (input == "credits")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Goplex Studios - GoOS");
                Console.WriteLine("Discord Link: https://discord.owen2k6.com/");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Contributors:");
                Console.WriteLine("Owen2k6 - Main Developer and creator");
                Console.WriteLine("Zulo - Helped create the command system");
                Console.WriteLine("moderator_man - Helped with my .gitignore issue and knows code fr");
                Console.WriteLine("");
            }
            else if (input == "support")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Goplex Studios Support");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("== For OS Support");
                Console.WriteLine("To get support, you must be in the Goplex Studios Discord Server.");
                Console.WriteLine("Discord Link: https://discord.owen2k6.com/");
                Console.WriteLine("Open support tickets in #get-staff-help");
                Console.WriteLine("== To report a bug");
                Console.WriteLine("Go to the issues tab on the Owen2k6/GoOS Github page");
                Console.WriteLine("and submit an issue with the bug tag.");
            }
            else if (input == "games")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Goplex Games List");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("TEXTADVENTURES - Text based adventure game because why not");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (input == "core")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("GoOS Core Ver 0.3");
                Console.Write("The Main backend to GoOS.");
                Console.Write("==========================");
                Console.Write("==Developed using Cosmos==");
                Console.Write("==========================");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("GoOS Core Is still in early development.");
                Console.Write("there are a lot of issues known and we are working on it! ");
                Console.ForegroundColor = ConsoleColor.Green;
            }

            //Games Section

            else if (input == "textadventures")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Goplex Studios - Text Adventures");
                Console.WriteLine("Developed using GoOS Core");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("????: Hello there, what's your name?");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Enter a name: ");
                String name = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("????: Ah. Hello there, " + name);
                Console.WriteLine("????: When there are Convos, press ENTER to move on to the next message :)");
                Console.ReadKey();
                Console.WriteLine("????: You probably dont know me, but its better that way...");
                Console.ReadKey();
                Console.WriteLine("????: Anyways, There are 1 stories we can enter.");
                Console.WriteLine("????: Yes i know wrong plural, but there will be more written in the future!");
                Console.ReadKey();
                Console.WriteLine("????: The first one i'll say is \"Temple Run\" ");
                Console.WriteLine("????: - You are a criminal planning the heist of a lifetime");
                Console.WriteLine("????: This heist is set on robbing the great temple.");
                Console.ReadKey();
                Console.WriteLine("????: For now, Temple Run is the only available story.");
                Console.ReadKey();
                Console.WriteLine("????: So what will it be?");
                Console.WriteLine("????: Selection Options: TEMPLERUN");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Choose One of the Options: ");
                String selection = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (selection == "templerun")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\"Temple Run\" Selected.");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("You wake up... it's 2:45AM and you can't get to sleep...");
                    Console.ReadKey();
                    Console.WriteLine("You look at your calendar...");
                    Console.ReadKey();
                    Console.WriteLine("It's August 4th 2023. 3 days before the heist.");
                    Console.ReadKey();
                    Console.WriteLine(name + ": Damn we need to get planning if we're gonna pull this off... ");
                    Console.ReadKey();
                    Console.WriteLine("You pick up your phone and call Joe the Fixer...");
                    Console.ReadKey();
                    Console.WriteLine(name + ": Joe! How have you been man...");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Joe: Hello... things are not so good...");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(name + ": What? Why?");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Joe: Because our plans aren't really in the best ways. How would we survive a 100+ Meter fall into Stone?");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(name + "That was Bob's idea... Not mine");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Joe: God. Bob really... Right im adding him to the call.");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bob has been added to the call.");
                    Console.ReadKey();
                    Console.WriteLine("Bob: What do you want Joe?");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Joe: You know what i want... This plan was pulled out of your-");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bob: OK OK. Fine. but jumping in is the best option");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Joe: Ok. well we gotta head down to the planning table before we \n can really think of anything else to do.");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bob: Alright. i'll meet you down there.");
                    Console.WriteLine("Bob Left the call");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Joe: Got that " + name + "? We'll meet you down there.");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(name + ": Got some things i want to do before heading down. see you there.");
                    Console.WriteLine(name + " Left the call");
                    Console.ReadKey();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("This Game is still under development. You have reached the end of this game so far!");
                    Console.WriteLine("Keep your OS Up to date to recieve updates for this game!");

                }
            }
            else if (input == "snake")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This game is known to crash GoOS. Press ENTER to continue");
                Console.WriteLine("CPU IS KNOWN TO OVERLOAD ON RUN. Press ENTER to continue");
                Console.ReadKey();
                configSnake();
                ConsoleKey x;
                while (true)
                {
                    while (gameover())
                    {
                        printGame();
                        Boolean endGame = false;
                        switch (Console.ReadKey(true).Key)
                        {
                            case ConsoleKey.R:
                                configSnake();
                                break;
                            case ConsoleKey.Escape:
                                endGame = true;
                                break;
                        }

                        if (endGame)
                        {
                            break;
                        }
                    }
                    while (!Console.KeyAvailable && !gameover())
                    {

                        updateDirections();

                        updatePosotion();

                        checkIfTouchFood();

                        Console.Clear();
                        changeArray();
                        printGame();
                        delay(10000000);
                    }

                    x = Console.ReadKey(true).Key;

                    if (x == ConsoleKey.LeftArrow)
                    {
                        if (snake[0][1] != 3)
                        {
                            commands.Add(new int[2] { 1, 0 });
                        }
                    }
                    else if (x == ConsoleKey.UpArrow)
                    {
                        if (snake[0][1] != 2)
                        {
                            commands.Add(new int[2] { 4, 0 });
                        }
                    }
                    else if (x == ConsoleKey.RightArrow)
                    {
                        if (snake[0][1] != 1)
                        {
                            commands.Add(new int[2] { 3, 0 });
                        }
                    }
                    else if (x == ConsoleKey.DownArrow)
                    {
                        if (snake[0][1] != 4)
                        {
                            commands.Add(new int[2] { 2, 0 });
                        }
                    }
                    else if (x == ConsoleKey.Escape)
                    {
                        break;
                    }
                    else if (x == ConsoleKey.R)
                    {
                        configSnake();
                    }
                }
            }

            //Calculator Area

            else if (input == "calc")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("GoCalc Commands");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("ADD - Add 2 numbers");
                Console.WriteLine("SUBTRACT - Subtract 2 numbers");
                Console.WriteLine("DIVIDE - Divide 2 numbers");
                Console.WriteLine("MULTIPLY - Multiply 2 numbers");
                Console.WriteLine("SQUARE - Square a number");
                Console.WriteLine("CUBE - Cube a number");
                Console.WriteLine("POWER10 - Make a number to the power of 10");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (input == "add")
            {
                Console.WriteLine("GoCalc - Addition");
                Console.WriteLine("Whole numbers only !!");
                Console.WriteLine("Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Adding up to");
                int ans = no1 + no2;
            }
            else if (input == "subtract")
            {
                Console.WriteLine("GoCalc - Subtraction");
                Console.WriteLine("Whole numbers only !!");
                Console.WriteLine("Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Adding up to");
                int ans = no1 - no2;
            }
            else if (input == "divide")
            {
                Console.WriteLine("GoCalc - Division");
                Console.WriteLine("Whole numbers only !!");
                Console.WriteLine("Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Adding up to");
                int ans = no1 / no2;
            }
            else if (input == "multiply")
            {
                Console.WriteLine("GoCalc - Multiplication");
                Console.WriteLine("Whole numbers only !!");
                Console.WriteLine("Enter the first number: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter the second number: ");
                int no2 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Adding up to");
                int ans = no1 * no2;
            }
            else if (input == "square")
            {
                Console.WriteLine("GoCalc - Squaring");
                Console.WriteLine("Whole numbers only !!");
                Console.WriteLine("Enter number to square: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                int ans = no1 * no1;
            }
            else if (input == "cube")
            {
                Console.WriteLine("GoCalc - Cubing");
                Console.WriteLine("Whole numbers only !!");
                Console.WriteLine("Enter number to cube: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                int ans = no1 * no1 * no1;
            }
            else if (input == "power10")
            {
                Console.WriteLine("GoCalc - To the power of 10");
                Console.WriteLine("Whole numbers only !!");
                Console.WriteLine("Enter number to p10: ");
                int no1 = Convert.ToInt32(Console.ReadLine());
                int ans = no1 * no1 * no1 * no1 * no1 * no1 * no1 * no1 * no1 * no1;
            }

            // GoOS Admin

            //Disk Only stuff
            //FS = new Sys.FileSystem.CosmosVFS(); Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS); FS.Initialize();
            else if (input == "loaddisk")
            {
                if (!adminconsoledisk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("GoOS Admin: Ensure you are using the iso file provided by Owen2k6 on release.");
                    Console.WriteLine("GoOS Admin: The system will crash if a disk can not be located. ");
                    Console.WriteLine("GoOS Admin: Press any key to continue");
                    Console.ReadKey();
                    FS = new Sys.FileSystem.CosmosVFS(); Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS); FS.Initialize(true);
                    Console.WriteLine("GoOS Admin: HardDisk enabled for session");
                    adminconsoledisk = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (adminconsoledisk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("GoOS Admin: System Already has hard disk loaded");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
            }
            else if (input == "diskcheck")
            {
                if (!adminconsoledisk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("GoOS Admin: There is currently no disk loaded to the system. type \"LOADDISK\" to activate the disk");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (adminconsoledisk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("GoOS Admin: Showing Disk Information for 0:\\");
                    var available_space = FS.GetAvailableFreeSpace(@"0:\");
                    var total_space = FS.GetTotalSize(@"0:\");
                    var label = FS.GetFileSystemLabel(@"0:\");
                    var fs_type = FS.GetFileSystemType(@"0:\");
                    Console.WriteLine("Available Free Space: " + available_space + "(" + (available_space/ 1e+9) + "GiB)");
                    Console.WriteLine("Total Space on disk: " + total_space + "(" + (total_space / 1e+9) + "GiB)");
                    Console.WriteLine("Disk Label: " + label);
                    Console.WriteLine("File System Type: " + fs_type);
                    Console.ForegroundColor = ConsoleColor.Green;
                }
            }
            else if (input == "ls")
            {
                if (!adminconsoledisk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("GoOS Admin: There is currently no disk loaded to the system. type \"LOADDISK\" to activate the disk");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (adminconsoledisk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    var directory_list = Directory.GetFiles(@"0:\");
                    foreach (var file in directory_list)
                    {
                        Console.WriteLine(file);
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                }
            }
            else if (input == "notepad") {
                if (!adminconsoledisk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("GoOS Admin: There is currently no disk loaded to the system. type \"LOADDISK\" to activate the disk");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (adminconsoledisk)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    MIV.StartMIV();
                    Console.ForegroundColor = ConsoleColor.Green;
                }
            }



            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("sorry, but ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("`" + input + "` ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("is not a command");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("");
                Console.WriteLine("Type HELP for a list of commands");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}
