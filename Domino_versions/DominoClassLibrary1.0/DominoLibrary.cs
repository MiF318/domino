using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace DominoLib
{
    //Игровое поле
    public static class Board
    {
        public static List<int[]> BonesOnBoard = new List<int[]>();

        //Печатает игровое поле
        public static void PrintBoard()
        {
            for (int i = 0; i < BonesOnBoard.Count; i++)
            {
                Console.Write("|" + BonesOnBoard[i][0] + "; " + BonesOnBoard[i][1] + "|   ");
            }
        }
    }

    //В нём находится колода и производятся действия с костяшками
    public static class Bones
    {
        public static List<int[]> Deck = new List<int[]>();
        public static int StartCountOfBones = 7;

        //Добавляет костяшку игроку и удаляет её из колоды
        public static void TakeBone(int player)
        {
            Random rnd = new Random();
            var bone = Deck[rnd.Next(0, Deck.Count)];
            Players.players[player].OnHand.Add(bone);
            Deck.Remove(bone);
        }

        //Добавляет костяшку на поле
        public static void AddBoneOnBoard(int[] bone, bool inEnd)
        {
            if (inEnd)
                Board.BonesOnBoard.Add(bone);
            else
                Board.BonesOnBoard.Insert(0, bone);
        }

        //Пересоздаёт колоду
        public static void RefreshDeck()
        {
            for (int i = 0; i < Deck.Count; i++)
            {
                Deck.Clear();
            }

            //Создаётся колода полностью различных int[2] в количестве: (double)((StartCountOfBones^2 + StartCountOfBones) / 2)
            for (int i = 0; i < StartCountOfBones; i++)
            {
                for (int j = i; j < StartCountOfBones; j++)
                {
                    var bone = new int[] { i, j };
                    Deck.Add(bone);
                }
            }
        }
    }

    //Конструктор игрока
    public class Player
    {
        public string Name = "";
        public List<int[]> OnHand = new List<int[]>();

        //Печатает все костяшки игрока
        public void PrintPlayerBones()
        {
            for (int i = 0; i < OnHand.Count; i++)
                Console.Write((i + 1) + ") " + "|" + OnHand[i][0] + "; " + OnHand[i][1] + "|   ");
        }
    }

    //Содержит всех игроков
    public static class Players
    {
        public static List<Player> players = new List<Player>();

        //Создаёт определённое количество игроков
        public static void CreatePlayers(int countOfPlayers)
        {
            players.Clear();
            for (int i = 0; i < countOfPlayers; i++)
            {
                players.Add(new Player());
            }
        }
    }
}
