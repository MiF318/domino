using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace DominoLib
{
    public static class Board
    {
        public static List<int[]> BonesOnBoard = new List<int[]>();

        public static void PrintBoard()
        {
            for (int i = 0; i < BonesOnBoard.Count; i++)
            {
                Console.Write("|" + BonesOnBoard[i][0] + "; " + BonesOnBoard[i][1] + "|   ");
            }
        }

        public static void AddBoneOnBoard(int[] bone, bool inEnd)
        {
            if (inEnd)
                BonesOnBoard.Add(bone);
            else
                BonesOnBoard.Insert(0, bone);
        }
    }

    public static class Bones
    {
        public static List<int[]> Deck = new List<int[]>();
        public static int CountOfBones = 7;

        public static void TakeBone(int player)
        {
            Random rnd = new Random();
            var bone = Deck[rnd.Next(0, Deck.Count)];
            Players.players[player].AddBone(bone);
            Deck.Remove(bone);
        }

        public static void RefreshDeck()
        {

            for (int i = 0; i < Deck.Count; i++)
            {
                Deck.Clear();
            }

            //Создаётся колода разных интов[2] в количестве 28
            for (int i = 0; i < 7; i++)
            {
                for (int j = i; j < 7; j++)
                {
                    var bone = new int[] { i, j };
                    Deck.Add(bone);
                }
            }
        }
    }

    public class Player
    {
        public string Name = "";
        public List<int[]> OnHand = new List<int[]>();

        public void AddBone(int[] bone)
        {
            OnHand.Add(bone);
        }

        public void PrintPlayerBones()
        {
            for (int i = 0; i < OnHand.Count; i++)
                Console.Write((i + 1) + ") " + "|" + OnHand[i][0] + "; " + OnHand[i][1] + "|   ");
        }
    }

    public static class Players
    {
        public static List<Player> players = new List<Player>();

        public static void CreatePlayers(int countOfPlayers)
        {
            for (int i = 0; i < countOfPlayers; i++)
            {
                players.Add(new Player());
            }
        }
    }
}
