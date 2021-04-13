using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace File_Manager
{
    class Program
    {
        /// <summary>
        /// Метод реализует перемещение по дискам.
        /// </summary>
        /// <returns></returns>
        static DriveInfo Disks()
        {
            Console.Clear();
            int j = 0;
            List<char> option = new List<char>();
            ConsoleKeyInfo button = new ConsoleKeyInfo();
            for (int i = 0; i < DriveInfo.GetDrives().Length; i++)
            {
                option.Add(' ');
            }
            do
            {
                Console.Clear();
                if (button.Key == ConsoleKey.DownArrow && j + 1 < DriveInfo.GetDrives().Length)
                {
                    option[j++] = ' ';
                }

                if (button.Key == ConsoleKey.UpArrow && j > 0)
                {
                    option[j--] = ' ';
                }
                option[j] = '>';
                for (int i = 0; i < DriveInfo.GetDrives().Length; i++)
                {
                    Console.WriteLine($"{option[i]} {DriveInfo.GetDrives()[i].Name}");
                }


                button = Console.ReadKey();
            } while (button.Key != ConsoleKey.RightArrow);
            return DriveInfo.GetDrives()[j];
        }

        /// <summary>
        /// Метод для "красивого" (через '>') выбора  папки.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        static string Folders(string folder)
        {
            int j = 0;
            ConsoleKeyInfo button = new ConsoleKeyInfo();
            try
            {
                string[] dirs = Directory.GetDirectories(folder);
                string[] fls = Directory.GetFiles(folder);
                List<char> option = new List<char>();
                option.Clear();
                for (int i = 0; i < dirs.Length + fls.Length; i++)
                {
                    option.Add(' ');
                }

                do
                {
                    Console.Clear();
                    if (button.Key == ConsoleKey.DownArrow && j + 1 < dirs.Length + fls.Length)
                    {
                        option[j++] = ' ';
                    }
                    if (button.Key == ConsoleKey.UpArrow && j > 0)
                    {
                        option[j--] = ' ';
                    }
                    Console.Clear();
                    if (button.Key == ConsoleKey.Spacebar)
                    {
                        if (j < dirs.Length && dirs[j].Contains('.'))
                        {
                            Files(dirs[j]);
                            return folder;
                        }
                        else if (fls[j - dirs.Length].Contains('.'))
                        {
                            Files(fls[j - dirs.Length]);
                            return folder;
                        }

                    }
                    button = Menu(j, dirs, fls, option);

                } while (button.Key != ConsoleKey.RightArrow && button.Key != ConsoleKey.LeftArrow && button.Key != ConsoleKey.Enter);
                if (button.Key == ConsoleKey.Enter)
                {
                    return "!";
                }
                if (button.Key == ConsoleKey.LeftArrow)
                {
                    try
                    {
                        return Directory.GetParent(folder).FullName;
                    }
                    catch (NullReferenceException)
                    {
                        return FolderExplorer();
                    }
                }
                if (j < dirs.Length)
                {
                    return dirs[j];
                }
                else
                {
                    return fls[j - dirs.Length];
                }
            }
            catch (IOException)
            {
                Console.WriteLine($"{Environment.NewLine}Диск не готов.");
                Console.ReadKey();
                return Folders(Disks().Name);
            }

        }

        /// <summary>
        /// Метод печатает названия папок и устанавливает курсор (является вспомогательным к предыдущему).
        /// </summary>
        /// <param name="j"></param>
        /// <param name="dirs"></param>
        /// <param name="fls"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private static ConsoleKeyInfo Menu(int j, string[] dirs, string[] fls, List<char> option)
        {
            ConsoleKeyInfo button;
            if (option.Count > 0)
            {
                option[j] = '>';
            }
            for (int i = 0; i < dirs.Length + fls.Length; i++)
            {
                if (i < dirs.Length)
                {
                    Console.WriteLine($"{option[i]} {dirs[i]}");
                }
                else
                {
                    Console.WriteLine($"{option[i]} {fls[i - dirs.Length]}");
                }
            }

            button = Console.ReadKey();
            return button;
        }

        /// <summary>
        /// Метод для открытия файлов с возможностью выбора кодировки.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="encod"></param>
        static void Opening(string fileName, int encod)
        {
            Console.Clear();
            int[] letters = new int[] {65001, 1200, 12000, 20127};
            using (StreamReader content = new StreamReader(fileName, Encoding.GetEncoding(letters[encod])))
            {
                string theLine;
                while ((theLine = content.ReadLine()) != null)
                {
                    Console.WriteLine(theLine);
                }
            }
            Console.ReadKey();
        }
        /// <summary>
        /// Метод, выводящий на экран информацию о выбранном файле.
        /// </summary>
        /// <param name="fileName"></param>
        static void Information(FileInfo fileName)
        {
            Console.Clear();
            Console.WriteLine($"Файл {fileName.Name}{Environment.NewLine}" +
                              $"Создан: {fileName.CreationTime}{Environment.NewLine}" +
                              $"Изменён: {fileName.LastWriteTime}{Environment.NewLine}" +
                              $"Последнее открытие: {fileName.LastAccessTime}{Environment.NewLine}" +
                              $"Путь: {fileName.FullName}");
            Console.ReadKey();

        }

        /// <summary>
        /// Метод, создающий копию выбранного файла.
        /// </summary>
        /// <param name="fileName"></param>
        static void Copying(string fileName)
        {
            Console.Clear();
            FileInfo theFile = new FileInfo(fileName);
            Console.Write("Дайте название копии файла: ");
            theFile.CopyTo($"{Console.ReadLine()}TheCopyOf{fileName}", true);
            Console.WriteLine("Копия создана, для перемещения её в другой каталог используйте 'Функции -> Переместить'");
            Console.ReadKey();
        }

        static void Deleting(FileInfo fileName)
        {
            fileName.Delete();
        }

        // Метод, меняющий дату и время различных взаимодействий с файлом.
        //static void TimeChanging(FileInfo fileName)
        //{
        //    Console.Write($"Введите новое время создания файла {Environment.NewLine}");
        //    DateTime date = DateTimeInput();
        //    File.SetCreationTimeUtc(fileName.Name, date);
        //    Console.Write($"Введите новое время последнего изменения файла: {Environment.NewLine}");
        //    date = DateTimeInput();
        //    File.SetLastWriteTimeUtc(fileName.Name, date);
        //    Console.Write($"Введите новое время последнего открытия файла: {Environment.NewLine}");
        //    date = DateTimeInput();
        //    File.SetLastAccessTimeUtc(fileName.Name, date);
        //}

        //private static DateTime DateTimeInput()
        //{
        //    Console.Clear();
        //    string afternoon = TimeEntering();
        //    DateTime date;
        //    while (!DateTime.TryParse(afternoon, out date))
        //    {
        //        Console.WriteLine(
        //            "Введённые данные не удалось перевести в формат даты и времени. Попробуйте ещё раз.");
        //        Console.ReadKey();
        //        Console.Clear();
        //        afternoon = TimeEntering();
        //    }

        //    return date;
        //}

        //private static string TimeEntering()
        //{
        //    string afternoon;
        //    {
        //        Console.WriteLine("Введено некорректное значение. Попробуйте снова: ");
        //        Console.Write($"{Environment.NewLine}Введите новую дату и время в формате мес/день/год час:минута:секунда AM/PM (например, 03/01/2009 05:42:00 AM)   ");
        //    afternoon = Console.ReadLine();
        //    }

            

        //    return afternoon;
        //}

        //static void Concatenation()
        //{
        //    Console.WriteLine("Выберите первый файл");
        //}

        /// <summary>
        /// Метод, вызывающий методы для взаимодействия с файлом.
        /// </summary>
        /// <param name="fileName"></param>
        static void Files(string fileName)
        {
            string[] option = new string[4] { " ", " ", " ", " "};
            int j = 0;
            ConsoleKeyInfo button = new ConsoleKeyInfo();
            do
            {
                if (button.Key == ConsoleKey.DownArrow && j + 1 < 4)
                {
                    option[j++] = " ";
                }

                if (button.Key == ConsoleKey.UpArrow && j > 0)
                {
                    option[j--] = " ";
                }
                option[j] = ">";
                Console.Clear();
                Console.WriteLine($"Выберите действие с файлом:{Environment.NewLine}" +
                                  $"{option[0]} Открыть{Environment.NewLine}" +
                                  $"{option[1]} Информация{Environment.NewLine}" +
                                  $"{option[2]} Скопировать{Environment.NewLine}" +
                                  $"{option[3]} Удалить{Environment.NewLine}");
                button = Console.ReadKey();
            } while (button.Key != ConsoleKey.RightArrow && button.Key != ConsoleKey.LeftArrow);
            if (button.Key == ConsoleKey.RightArrow)
            {
                switch (j)
                {
                    case 0:
                        {
                            int cursor;
                            ConsoleKeyInfo butt;
                            Option(j, out cursor, out butt);
                            if (butt.Key == ConsoleKey.RightArrow)
                            {
                                Opening(fileName, cursor);

                            }
                            if (butt.Key == ConsoleKey.LeftArrow)
                            {
                                Opening(fileName, cursor);
                            }
                            break;
                        }
                    case 1:
                        {
                            Information(new FileInfo(fileName));
                            break;
                        }
                    case 2:
                        {
                            Copying(fileName);
                            break;
                        }
                    case 3:
                        {
                            Deleting(new FileInfo(fileName));
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Метод, реализующий выбор кодировки.
        /// </summary>
        /// <param name="j"></param>
        /// <param name="cursor"></param>
        /// <param name="butt"></param>
        private static void Option(int j, out int cursor, out ConsoleKeyInfo butt)
        {
            string[] opt = new string[] { " ", " ", " ", " " };
            cursor = 0;
            butt = new ConsoleKeyInfo();
            do
            {
                if (butt.Key == ConsoleKey.DownArrow && j + 1 < 4)
                {
                    opt[cursor++] = " ";
                }

                if (butt.Key == ConsoleKey.UpArrow && j > 0)
                {
                    opt[cursor--] = " ";
                }
                opt[cursor] = ">";
                Console.Clear();
                Console.WriteLine($"Выберие кодировку:{Environment.NewLine}" +
                  $"{opt[0]} UTF-8{Environment.NewLine}" +
                  $"{opt[1]} UTF-16{Environment.NewLine}" +
                  $"{opt[2]} UTF-32{Environment.NewLine}" +
                  $"{opt[3]} ASCII (США){Environment.NewLine}");
                butt = Console.ReadKey();
            } while (butt.Key != ConsoleKey.RightArrow && butt.Key != ConsoleKey.LeftArrow);
        }

        /// <summary>
        /// Метод для взаимодействия с файлами и папками.
        /// </summary>
        /// <returns></returns>
        private static string FolderExplorer()
        {
            string whereWeGo = Folders(Disks().Name);
            while (whereWeGo != "!")
            {
                try
                {
                    if (whereWeGo.Contains(".txt") || whereWeGo.Contains(".html") || whereWeGo.Contains(".cs"))
                    {
                        Opening(whereWeGo, 0); 
                        Console.ReadKey();
                        whereWeGo = Folders(Directory.GetParent(whereWeGo).FullName);
                    }
                    else if (!whereWeGo.Contains('.'))
                    {
                        Console.WriteLine(Directory.GetDirectories(whereWeGo));
                        whereWeGo = Folders(whereWeGo);
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Просмотр файлов данного формата не поддерживается.");
                        Console.ReadKey();
                        whereWeGo = Folders(Directory.GetParent(whereWeGo).FullName);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("К данной папке (файлу) нет доступа");
                    Console.ReadKey();
                    whereWeGo = Folders(Directory.GetParent(whereWeGo).FullName);
                }
            }
            return whereWeGo;
        }


        static void Main(string[] args)
        {
            Console.WriteLine($"Инструкция:{Environment.NewLine}" +
                $"1. Переходы по папкам и меню, а ткаже открытие текстовых файлов в стандартной кодировке производитя посредством стрелок.{Environment.NewLine}" +
                $"2. Для открытия меню с действиями для файла нажмите пробел.{Environment.NewLine}" +
                $"3. После вывода информации программа будет ожидать нажатия какой-нибудь клавиши.{Environment.NewLine}" +
                $"4. Повтор решения производися автоматически.{Environment.NewLine}{Environment.NewLine}" +
                $"Поехали!");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
            FolderExplorer();
        }



    }
}