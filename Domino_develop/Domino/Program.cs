using System;
using System.Numerics;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DominoLib;

namespace Domino
{
    internal class Program
    {
        static List<string> names = new List<string>();


//Игровые методы

        //ОСНОВНОЙ МЕТОД
        static void Main(string[] args)
        {
            string statusGame = "first game";
            while (true)
            {
                //Начало игры
                StartGame(statusGame);

                //Возвращает "restart" в случае выбора игроком рестарта, .Name победителя, ошибку
                string result = Game();

                //На осноове result возвращает: "end" - в случае, когда Game() закончилась выбором победителя, "restart" - игрок выбрал рестарт, "start" - в случае ошибок (полная переигровка)
                statusGame = CheckErrorsAndGetStatusGameEnd(result);
                if (statusGame == "restart")
                    continue;
                else if (statusGame == "start")
                    continue;

                else if (statusGame == "end")
                {
                    //Возвращает выбор пользователя (рестарт или нет?)
                    statusGame = FinishGame(result);

                    if (statusGame == "end")
                    {
                        MakeIndentation(30);
                        Console.WriteLine("Конец");
                        break;
                    }
                    else
                        continue;

                    Console.WriteLine("Нажмите любую клавишу");
                    Console.ReadKey();
                }
            }
            //Доп инфа в самом конце
            MakeIndentation(10);
            Console.WriteLine("Склипал за 2 ночи. Поначалу просто делал классы по заданию, но потом для теста и чтобы пощупать различие статических классов, методов и тд от нестатических, " +
                "\n начал создавать игроков, выводить их на консоль и как-то так получилось, что структура и идея реализации, пусть и простенького, но полноценного консольного домино " +
                "\n сами собой пришли в голову)" +
                "\n Для выхода нажмите любую клавишу");
            Console.ReadKey();
        }


//Начало игры
        static void StartGame(string statusGame)
        {
            //Рефреш игровой информации (деки, доски и костяшек)
            Bones.RefreshDeck();

            if (statusGame == "start" || statusGame == "restart")
            {
                Board.BonesOnBoard.Clear();
                for (int i = 0; i < Players.players.Count; i++)
                {
                    Players.players[i].OnHand.Clear();
                }
            }

            //Создание игроков
            if (statusGame == "start" || statusGame == "first game")
            {
                names.Clear();
                GetPlayers();
                Players.CreatePlayers(names.Count);
            }

            //Раздача костяшек
            for (int player = 0; player < Players.players.Count; player++)
            {
                Players.players[player].Name = names[player];

                for (int countStartBones = 0; countStartBones < Bones.StartCountOfBones; countStartBones++)
                {
                    Bones.TakeBone(player);
                }
            }
        }

        //Получает имена и количество игроков
        static void GetPlayers()
        {
            int count = 0;
            while (count < Bones.Deck.Count / Bones.StartCountOfBones)
            {
                Console.WriteLine("Введите имя " + ((count == 0) ? "первого" : "следующего") + " игрока" + ((count > 1) ? " (если игроков больше нет, оставьте поле пустым)" : ""));
                var name = Console.ReadLine();

                if (count < 2 && name == "")
                {
                    Console.WriteLine("Создайте хотя бы двух игроков");
                    continue;
                }

                else if (name == "")
                    break;

                else
                {
                    names.Add(name);
                    count++;
                }
            }
        }


//Методы середины игры

        //Середина игры
        static string Game()
        {
            //Придумал идею с кодом, мне понравилась, решил оставить)
            Random codeGenerator = new Random();
            int code = codeGenerator.Next();

            //Статус того, что игра идёт
            string inGameStatus = "In game stage " + code.ToString();
            string statusGame = inGameStatus;

            while (true)
            {
                for (int numberOfPlayer = 0; numberOfPlayer < Players.players.Count; numberOfPlayer++)
                {
                    string resultPlayerStep = MakePlayerStep(numberOfPlayer);

                    if (resultPlayerStep == "next")
                    {
                        //Проверки на выигрыш (номер победителя; -2 - победителя пока нет)
                        statusGame = CheckWin(numberOfPlayer, statusGame);
                        if (statusGame != inGameStatus)
                            return statusGame;

                        continue;
                    }

                    else if (resultPlayerStep == "restart")
                        return "restart";
                    else if (resultPlayerStep == "start")
                        return "start";
                }
            }
            return "error";
        }

    //Группа методов, отвеячающих за ход игрока

        //Ход заданного игрока
        static string MakePlayerStep(int numberOfPlayer)
        {
            Console.WriteLine("Ходит " + Players.players[numberOfPlayer].Name + ", для продолжения нажмите любую клавишу");
            Console.ReadKey();
            MakeIndentation(5);
            PrintGameInformation();
            PrintPlayerInformation(numberOfPlayer);

            //Добиваемся от игрока валидного ответа
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Введите номер костяшки");
                Console.WriteLine("Или введите одну из следующих команд: \"беру\" (б) - взять одну костяшку из колоды (осталось " + Bones.Deck.Count + "); ");
                Console.WriteLine(((Bones.Deck.Count == 0) ? "\"передаю\" (п) - передать ход" : "") + "\n" 
                    + "\"рестарт\" - перезапуск игры для тех же игроков, \"новая игра\" - перезапуск игры для новых игроков");
                string choice = Console.ReadLine();

                //Проверки выбора игрока (next - всё ок, переход хода, again - неверный ответ, переигровка хода, restart - рестарт)
                string resultCheckChoice = CheckAndMakeChoicePlayer(numberOfPlayer, choice);

                if (resultCheckChoice == "again")
                    continue;

                return resultCheckChoice;
            }
        }

        //Проверяет валидность выбора и совершает его (again - повтор хода, next - передать ход, restert - рестарт)
        static string CheckAndMakeChoicePlayer(int numberOfPlayer, string choice)
        {
            if (choice == "беру" || choice == "б")
            {
                if (Bones.Deck.Count == 0)
                {
                    Console.WriteLine("Увы, но колода пуста");
                    return "again";
                }

                Bones.TakeBone(numberOfPlayer);

                MakeIndentation(3);
                PrintGameInformation();
                PrintPlayerInformation(numberOfPlayer);

                return "again";
            }

            else if ((choice == "передаю" || choice == "п") && Bones.Deck.Count == 0)
            {
                MakeIndentation(30);
                return "next";
            }
            else if ((choice == "передаю" || choice == "п") && Bones.Deck.Count > 0)
            {
                Console.WriteLine("В колоде пока ещё имеются карты!");
                return "again";
            }
            else if (choice == "рестарт")
            {
                return "restart";
            }
            else if (choice == "новая игра")
            {
                return "start";
            }

            //Если есть исключения, возвращает "again"
            if (CheckExceptions(numberOfPlayer, choice))
                return "again";

            int choiceBone = int.Parse(choice);

            //Проверить возможность и поставить на доску выбранную костяшку. Возвращает сообщение о результате
            return CheckAndPutChoiceBone(numberOfPlayer, choiceBone);
        }

        //Проверка на исключения
        static bool CheckExceptions(int numberOfPlayer, string choice)
        {
            int choiceBone;

            //Проверка на число, иначе сообщение об ошибке
            if (!int.TryParse(choice, out choiceBone))
            {
                PrintExceptionMessage(0);
                return true;
            }

            //Если число в правильном диапазоне, то работаем с выбранной костяшкой, иначе сообщение об ошибке
            choiceBone = int.Parse(choice);
            if (!(choiceBone > 0 && choiceBone < Players.players[numberOfPlayer].OnHand.Count + 1))
            {
                PrintExceptionMessage(1);
                return true;
            }

            return false;
        }

        //Сообщения об разных исключениях
        static void PrintExceptionMessage(int numberException)
        {
            Console.WriteLine();
            if (numberException == 0)
                Console.WriteLine("Вы ошиблись вводом, повторите Ваш ответ, пожалуйста");
            else if (numberException == 1)
                Console.WriteLine("Вы ошиблись, такой костяшки у Вас нет, повторите Ваш ответ, пожалуйста");
            Console.WriteLine();
        }

        //Отрабатывает на основе выбора и возвращает сообщение об успехе
        static string CheckAndPutChoiceBone(int numberOfPlayer, int choiceBone)
        {
            var bone = new int[2];
            bone = Players.players[numberOfPlayer].OnHand[choiceBone - 1];
            bool stepIsValid = CheckValidStep(bone, numberOfPlayer, true);

            if (stepIsValid)
            {
                PrintPassingTurn(numberOfPlayer);
                return "next";
            }
            else
                return "again";
        }

        //Проверяет валидность выбора костяшки и в случае успеха выкидывает её на доску
        static bool CheckValidStep(int[] bone, int numberOfPlayer, bool andPut)
        {
            if (Board.BonesOnBoard.Count == 0)
            {
                PutBone(bone, true, numberOfPlayer, bone);
                return true;
            }

            var lastBoneOnBoard = Board.BonesOnBoard[Board.BonesOnBoard.Count - 1][1];
            var startBoneOnBoard = Board.BonesOnBoard[0][0];

            var boneReverse = new int[2] { bone[1], bone[0] };

            //Проверка и добавление в конец
            if (lastBoneOnBoard == bone[0])
            {
                if (andPut)
                    PutBone(bone, true, numberOfPlayer, bone);
                return true;
            }
            else if (lastBoneOnBoard == bone[1])
            {
                if (andPut)
                    PutBone(boneReverse, true, numberOfPlayer, bone);
                return true;
            }

            //Проверка и добавление в начало
            else if (startBoneOnBoard == bone[0])
            {
                if (andPut)
                    PutBone(boneReverse, false, numberOfPlayer, bone);
                return true;
            }
            else if (startBoneOnBoard == bone[1])
            {
                if (andPut)
                    PutBone(bone, false, numberOfPlayer, bone);
                return true;
            }

            if (andPut)
            {
                Console.WriteLine();
                Console.WriteLine("Этой костяшке некуда вставать");
                Console.WriteLine();
            }
            return false;
        }

        //Выкладывает костяшку на доску в зависимости от выбора игрока
        static void PutBone(int[] boneToBoard, bool inEnd, int numberOfPlayer, int[] boneFromHand)
        {
            Bones.AddBoneOnBoard(boneToBoard, inEnd);
            Players.players[numberOfPlayer].OnHand.Remove(boneFromHand);
        }

        //Печатает передачу хода
        static void PrintPassingTurn(int numberOfPlayer)
        {
            PrintGameInformation();

            //Проверка на следующего игрока (на переполнение списка Players)
            int numberOfNextPlayer;
            if (numberOfPlayer < Players.players.Count - 1)
                numberOfNextPlayer = numberOfPlayer + 1;
            else
                numberOfNextPlayer = 0;

            Console.WriteLine("Для передачи хода игроку " + Players.players[numberOfNextPlayer].Name + " нажмите ОДИН раз любую клавишу");

            Console.ReadKey();
            MakeIndentation(30);
        }

        //Проверяет, выиграл ли игрок. Возвращает: номер игрока, если выйгрыш засчитан; -2 - выигрыш не засчитан (ходы ещё есть)
        static string CheckWin(int numberOfPlayer, string statusGame)
        {
            bool gameFinished = false;
            if (Players.players[numberOfPlayer].OnHand.Count == 0)
            {
                return Players.players[numberOfPlayer].Name;
            }

            else if (Bones.Deck.Count == 0)
            {
                var winner = new int[] { 0, int.MaxValue };
                gameFinished = true;
                for (int i = 0; i < Players.players.Count; i++)
                {
                    int count = 0;
                    for (int numberOfBone = 0; numberOfBone < Players.players[i].OnHand.Count; numberOfBone++)
                    {
                        if (CheckValidStep(Players.players[i].OnHand[numberOfBone], numberOfPlayer, false))
                            gameFinished = false;
                        count += (Players.players[i].OnHand[numberOfBone][0] + Players.players[i].OnHand[numberOfBone][1]);
                    }
                    if (count <= winner[1])
                    {
                        winner[0] = i;
                        winner[1] = count;
                    }
                }
                if (gameFinished)
                {
                    return Players.players[winner[0]].Name;
                }
            }
            return statusGame;
        }


//Методы конца игры

        //Вывод победителя и выбор дальнейших действий (перезапустить/нет)
        static string FinishGame(string result)
        {
            Console.WriteLine();
            Console.WriteLine("Игра окончена!");
            Console.WriteLine("Победитель - " + result);
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Перезапустить игру для тех же игроков? (\"да\" - да, \"нет \" - нет (выйти из игры), \"заново\" - начать новую игру и создать новых игроков)");
                string answer = Console.ReadLine();

                if (answer == "да")
                    return "restart";
                else if (answer == "нет")
                    return "end";
                else if (answer == "заново")
                    return "start";
                else
                    Console.WriteLine("Возможно, вы ошиблись. Пожалуйста, ответте заново \n"); continue;
            }
        }

        //Проверяет возможность разных ошибок и возвращает статус игры (обрабатывает резульат Game())
        static string CheckErrorsAndGetStatusGameEnd(string result)
        {
            if (result == "restart")
            {
                Console.WriteLine("Игра перезапускается");
                MakeIndentation(5);
                return "restart";
            }
            else if (result == "start")
            {
                Console.WriteLine("Игра перезапускается");
                MakeIndentation(5);
                return "start";
            }
            else if (result == "error")
            {
                Console.WriteLine("Возникла непредвиденная ошибка. Игра перезапускается");
                MakeIndentation(5);
                return "start";
            }
            else
            {
                return "end";
            }
        }



//Вспомогательные методы:

        //Вспомогательный метод, делающий отступ
        static void MakeIndentation(int indentation)
        {
            for (int i = 0; i < indentation; i++)
            {
                Console.WriteLine();
            }
        }

        //Вспомогательный метод, который выводит на экран стилизованную текущую информацию об игре
        static void PrintGameInformation()
        {
            Console.WriteLine();
            Console.WriteLine("Доска:");
            Board.PrintBoard();
            Console.WriteLine();
            Console.WriteLine();
        }

        //Вспомогательный метод, который выводит на экран информацию о заданном игроке
        static void PrintPlayerInformation(int numberOfPlayer)
        {
            Console.WriteLine();
            Console.WriteLine("Ваши костяшки:");
            Players.players[numberOfPlayer].PrintPlayerBones();
            Console.WriteLine();
        }
    }
}