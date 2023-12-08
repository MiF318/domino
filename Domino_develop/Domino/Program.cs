using System;
using System.Numerics;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DominoLib;

namespace Domino
{
    internal class Program
    {
        static void MakeIndentation(int indentation)
        {
            for (int i = 0; i < indentation; i++)
            {
                Console.WriteLine();
            }
        }

        static void PrintGameInformation(int numberOfPlayer)
        {
            Console.WriteLine();
            Console.WriteLine("Доска:");
            Board.PrintBoard();
            Console.WriteLine();
            Console.WriteLine();
        }

        static void PrintPlayerInformation(int numberOfPlayer)
        {
            Console.WriteLine();
            Console.WriteLine("Ваши костяшки:");
            Players.players[numberOfPlayer].PrintPlayerBones();
            Console.WriteLine();
        }

        static int CheckAndMakeChoicePlayer(int numberOfPlayer, string choice)
        {
            if (choice == "беру")
            {
                Bones.TakeBone(numberOfPlayer);

                MakeIndentation(3);
                PrintGameInformation(numberOfPlayer);
                PrintPlayerInformation(numberOfPlayer);

                return 1;
            }

            else if (choice == "передать" && Bones.Deck.Count == 0)
            {
                MakeIndentation(30);
                return 0;
            }
            else if (choice == "передать" && Bones.Deck.Count > 0)
            {
                Console.WriteLine("В колоде пока ещё имеются карты!");
                return 1;
            }
            else if (choice == "рестарт")
            {
                return -1;
            }

            // P.S. Здесь тестами ещё не покрыл)
            else
            {
                var bone = new int[2];
                bone = Players.players[numberOfPlayer].OnHand[int.Parse(choice) - 1];
                bool result = CheckValidStep(bone, numberOfPlayer, true);

                if (result)
                {
                    PrintGameInformation(numberOfPlayer);

                    //Проверка на следующего игрока (на переполнение списка Players)
                    int numberOfNextPlayer;
                    if (numberOfPlayer < Players.players.Count - 1)
                        numberOfNextPlayer = numberOfPlayer + 1;
                    else
                        numberOfNextPlayer = 0;

                    Console.WriteLine("Для передачи хода игроку " + Players.players[numberOfNextPlayer].Name + " нажмите ОДИН раз любую клавишу");

                    Console.ReadKey();
                    MakeIndentation(30);
                    return 0;
                }
                else
                    return 1;
            }
        }

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
            Console.WriteLine();
            Console.WriteLine("Этой костяшке некуда вставать");
            Console.WriteLine();
            return false;
        }

        static void PutBone(int[] boneToBoard, bool inEnd, int numberOfPlayer, int[] boneFromHand)
        {
            Board.AddBoneOnBoard(boneToBoard, inEnd);
            Players.players[numberOfPlayer].OnHand.Remove(boneFromHand);
        }

        static int CheckWin(int numberOfPlayer)
        {
            bool gameFinished = false;
            if (Players.players[numberOfPlayer].OnHand.Count == 0)
            {
                return numberOfPlayer;
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
                    return winner[0];
                }
            }
            if (!gameFinished)
                return -2;
            else
                return -3;
        }

        static bool GetStatusGame(int result)
        {
            if (result == -3)
            {
                Console.WriteLine("Какая-то супер непредвиденная ошибка 2... Игра перезапускается");
                MakeIndentation(5);
                return false;
            }
            else if (result == -1)
            {
                Console.WriteLine("Игра перезапускается");
                MakeIndentation(5);
                return false;
            }
            else if (result == -2)
            {
                Console.WriteLine("Возникла непредвиденная ошибка 1, игра перезапускается");
                MakeIndentation(5);
                return false;
            }
            else
            {
                return true;
            }
        }

        static void StartGame()
        {
            //Рефреш деки
            Bones.RefreshDeck();
            Board.BonesOnBoard.Clear();
            for (int i = 0; i < Players.players.Count; i++)
            {
                Players.players[i].OnHand.Clear();
            }

            //Создание игроков и раздача костяшек
            var names = new List<string>();
            int count = 0;
            while (count < Bones.Deck.Count / Bones.CountOfBones)
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

            Players.CreatePlayers(names.Count);

            for (int player = 0; player < names.Count; player++)
            {
                Players.players[player].Name = names[player];

                for (int countStartBones = 0; countStartBones < Bones.CountOfBones; countStartBones++)
                {
                    Bones.TakeBone(player);
                }
            }
        }

        static int Game()
        {
            int result = -2;

            while (true)
            {
                for (int numberOfPlayer = 0; numberOfPlayer < Players.players.Count; numberOfPlayer++)
                {
                    Console.WriteLine("Ходит " + Players.players[numberOfPlayer].Name + ", для продолжения нажмите любую клавишу");
                    Console.ReadKey();
                    MakeIndentation(5);

                    PrintGameInformation(numberOfPlayer);
                    PrintPlayerInformation(numberOfPlayer);

                    //Добиваемся от игрока валидного ответа
                    while (true)
                    {
                        Console.WriteLine("Введите номер костяшки");
                        Console.WriteLine("Или введите одну из следующих команд: \"беру\" - взять одну костяшку из колоды (осталось " + Bones.Deck.Count + "); " + ((Bones.Deck.Count == 0) ? "\"передать\" - передать ход" : "") + "\"рестарт\" - перезапуск игры, например, если игра зашла в тупик");
                        string choice = Console.ReadLine();

                        //Проверки выбора игрока (0 - всё ок, переход хода, 1 - неверный ответ, -1 - рестарт)
                        var checkChoice = CheckAndMakeChoicePlayer(numberOfPlayer, choice);

                        if (checkChoice == 0)
                            break;
                        else if (checkChoice == 1)
                            continue;
                        else
                            return -1;
                    }

                    //Проверки на выигрыш
                    result = CheckWin(numberOfPlayer);
                    if (result != -2)
                        return result;
                }
            }
        }

        //Вывод победителя и выбор дальнейших действий (перезапустить/нет)
        static bool FinishGame(int result)
        {
            Console.WriteLine();
            Console.WriteLine("Игра окончена!");
            Console.WriteLine("Победитель - " + Players.players[result].Name);
            Console.WriteLine();

            Console.WriteLine("Перезапустить игру для тех же игроков? (\"да\" - да \"нет \" - нет");
            string answer = Console.ReadLine();

            if (answer == "да")
                return true;
            else if (answer == "нет")
                return false;
            else
                Console.WriteLine("Неверный ответ"); return false;
        }
        static void Main(string[] args)
        {
            while (true)
            {
                StartGame();

                //Порядковый номер Player в Players, команда на перезапуск, либо одна из ошибок
                int result = Game();

                var statusGame = GetStatusGame(result);
                if (!statusGame)
                    continue;

                bool restart = FinishGame(result);

                if (!restart)
                {
                    MakeIndentation(30);
                    Console.WriteLine("Конец");
                    break;
                }

                Console.WriteLine("Нажмите любую клавишу");
                Console.ReadKey();
            }
            MakeIndentation(10);
            Console.WriteLine("Склипал за ночь, рефакторил днём. Ну а тесты пока не сделал, мб когда-нибудь потом добавлю. \n Для выхода нажмите любую клавишу");
            Console.ReadKey();
        }
    }
}