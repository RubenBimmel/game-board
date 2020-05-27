using System;
using System.Linq;

namespace GameBoard.Data
{
    public static class DeckOfCards
    {
        private const string UrlPrefix = "https://deckofcardsapi.com/static/img/";
        private const string UrlSuffix = ".png";
        private static readonly string[] values = {"A", "2", "3", "4", "5", "6", "7", "8", "9", "J", "Q", "K"};
        private static readonly string[] colors = {"H", "D", "S", "C"};
        private static readonly string[] jokers = {"X1", "X2"};

        public static string GetRandomUrl()
        {
            var rnd = new Random();
            var deck = GetDeck(true);
            return GetUrl(GetDeck(true)[rnd.Next(0, deck.Length - 1)]);
        }
        
        public static string GetUrl(string name)
        {
            return UrlPrefix + name + UrlSuffix;
        }
        
        public static string[] GetDeck(bool includeJokers)
        {
            var result = from value in values from color in colors select value + color;
            return includeJokers ? result.Concat(jokers).ToArray() : result.ToArray();
        }
    }
}