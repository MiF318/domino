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


        //Вспомогательные игровые методы:

        //Получает имена и количество игроков
        static List<string> GetPlayers()
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
            return names;
        }


        //Группа методов, отвеячающих за ход игрока

        //Ход заданного игрока
        static bool MakePlayerStep(int numberOfPlayer)
        {
            Console.WriteLine("Ходит " + Players.players[numberOfPlayer].Name + ", для продолжения нажмите любую клавишу");
            Console.ReadKey();
            MakeIndentation(5);
            PrintGameInformation();
            PrintPlayerInformation(numberOfPlayer);

            //Добиваемся от игрока валидного ответа
            while (true)
            {
                Console.WriteLine("Введите номер костяшки");
                Console.WriteLine("Или введите одну из следующих команд: \"беру\" - взять одну костяшку из колоды (осталось " + Bones.Deck.Count + "); "
                    + ((Bones.Deck.Count == 0) ? "\"передаю\" - передать ход" : "") + "\"рестарт\" - перезапуск игры");
                string choice = Console.ReadLine();

                int checkChoice = CheckAndMakeChoicePlayer(numberOfPlayer, choice);
                //Проверки выбора игрока (1 - всё ок, переход хода, 0 - неверный ответ, переигровка хода, -1 - рестарт)
                if (checkChoice == 0)
                {
                    return false;
                }
                else if (checkChoice == 1)
                {
                    continue;
                }
                else
                    return true;
            }
        }

        //Проверяет валидность выбора и совершает его (0 - повтор хода, 1 - передать ход, -1 - рестарт)
        static int CheckAndMakeChoicePlayer(int numberOfPlayer, string choice)
        {
            if (choice == "беру")
            {
                if (Bones.Deck.Count == 0)
                {
                    Console.WriteLine("Увы, но колода пуста");
                    return 0;
                }

                Bones.TakeBone(numberOfPlayer);

                MakeIndentation(3);
                PrintGameInformation();
                PrintPlayerInformation(numberOfPlayer);

                return 0;
            }

            else if (choice == "передаю" && Bones.Deck.Count == 0)
            {
                MakeIndentation(30);
                return 1;
            }
            else if (choice == "передать" && Bones.Deck.Count > 0)
            {
                Console.WriteLine("В колоде пока ещё имеются карты!");
                return 0;
            }
            else if (choice == "рестарт")
            {
                return -1;
            }

            // P.S. Здесь тестами ещё не покрыл)

            //Если число, то работаем с выбранной костяшкой
            else
            {
                var bone = new int[2];
                bone = Players.players[numberOfPlayer].OnHand[int.Parse(choice) - 1];
                bool result = CheckValidStep(bone, numberOfPlayer, true);

                if (result)
                {
                    PrintPassingTurn(numberOfPlayer);
                    return 1;
                }
                else
                    return 0;
            }
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


        //Методы конца партии

        //Проверяет, выиграл ли игрок. Возвращает: номер игрока, если выйгрыш засчитан; -2 - выигрыш не засчитан (ходы ещё есть)
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
            return -2;
        }

        //Проверяет возможность разных ошибок и возвращает статус игры
        static bool CheckErrorsAndGetStatusGameAnd(int result)
        {
            if (result == -3)
            {
                Console.WriteLine("Возникла непредвиденная ошибка 2, игра перезапускается");
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


        //Игровые методы

        //Начало игры
        static void StartGame(int result)
        {
            //Рефреш игровой информации (деки, доски и костяшек)
            Bones.RefreshDeck();
            Board.BonesOnBoard.Clear();
            for (int i = 0; i < Players.players.Count; i++)
            {
                Players.players[i].OnHand.Clear();
            }

            //Создание игроков
            if (result != -1)
            {
                var names = GetPlayers();
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

        static int Game()
        {
            int result = -2;

            while (true)
            {
                for (int numberOfPlayer = 0; numberOfPlayer < Players.players.Count; numberOfPlayer++)
                {
                    bool restart = MakePlayerStep(numberOfPlayer);

                    if (restart)
                        return -1;

                    //Проверки на выигрыш (номер победителя; -2 - победителя пока нет)
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

            Console.WriteLine("Перезапустить игру? (\"да\" - да \"нет \" - нет)");
            string answer = Console.ReadLine();

            if (answer == "да")
                return true;
            else if (answer == "нет")
                return false;
            else
                Console.WriteLine("Неверный ответ"); return false;
        }


        //ОСНОВНОЙ МЕТОД
        static void Main(string[] args)
        {
            int result = -2;
            while (true)
            {
                //Начало игры
                StartGame(result);

                //Возвращает порядковый номер Player в Players, команда на перезапуск (-1), либо одна из ошибок
                result = Game();
                var statusGame = CheckErrorsAndGetStatusGameAnd(result);
                if (!statusGame)
                    continue;

                //Возвращает возможность рестарта
                bool restart = FinishGame(result);

                if (!restart)
                {
                    MakeIndentation(30);
                    Console.WriteLine("Конец");
                    break;
                }
                result = -1;

                Console.WriteLine("Нажмите любую клавишу");
                Console.ReadKey();
            }
            //Доп инфа в самом конце
            MakeIndentation(10);
            Console.WriteLine("Склипал за ночь, рефакторил днём. Ну а тесты пока не сделал, мб когда-нибудь потом добавлю. \n Для выхода нажмите любую клавишу");
            Console.ReadKey();
        }
    }
}