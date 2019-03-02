using System.Collections.Generic;

namespace Spiritmonger
{

    public class ScryfallObject
    {
        public ScryfallObject()
        {
            Data = new List<Data>();
        }
        public string _object { get; set; }
        public int Total_cards { get; set; }
        public bool Has_more { get; set; }
        public string Next_page { get; set; }
        public IList<Data> Data { get; set; }
    }

    public class Data
    {
        public string _object { get; set; }
        public string Id { get; set; }
        public string Oracle_id { get; set; }
        public int[] Multiverse_ids { get; set; }
        public string Name { get; set; }
        public string Printed_name { get; set; }
        public string Lang { get; set; }
        public string Released_at { get; set; }
        public string Uri { get; set; }
        public string Scryfall_uri { get; set; }
        public string Layout { get; set; }
        public bool Highres_image { get; set; }
        public Image_Uris Image_uris { get; set; }
        public string Mana_cost { get; set; }
        public string Cmc { get; set; }
        public string Type_line { get; set; }
        public string Printed_type_line { get; set; }
        public string Oracle_text { get; set; }
        public string Printed_text { get; set; }
        public string[] Colors { get; set; }
        public string[] Color_identity { get; set; }
        public Legalities Legalities { get; set; }
        public string[] Games { get; set; }
        public bool Reserved { get; set; }
        public bool Foil { get; set; }
        public bool Nonfoil { get; set; }
        public bool Oversized { get; set; }
        public bool Promo { get; set; }
        public bool Reprint { get; set; }
        public string Set { get; set; }
        public string Set_name { get; set; }
        public string Set_uri { get; set; }
        public string Set_search_uri { get; set; }
        public string Scryfall_set_uri { get; set; }
        public string Rulings_uri { get; set; }
        public string Prints_search_uri { get; set; }
        public string Collector_number { get; set; }
        public bool Digital { get; set; }
        public string Rarity { get; set; }
        public string Watermark { get; set; }
        public string Flavor_text { get; set; }
        public string Illustration_id { get; set; }
        public string Artist { get; set; }
        public string Border_color { get; set; }
        public string Frame { get; set; }
        public string Frame_effect { get; set; }
        public bool Full_art { get; set; }
        public bool Story_spotlight { get; set; }
        public int Edhrec_rank { get; set; }
        public Prices Prices { get; set; }
        public Related_Uris Related_uris { get; set; }
        public Purchase_Uris Purchase_uris { get; set; }
        public string Eur { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public All_Parts[] All_parts { get; set; }
        public int tcgplayer_id { get; set; }
        public string Usd { get; set; }
    }

    public class Image_Uris
    {
        public string Small { get; set; }
        public string Normal { get; set; }
        public string Large { get; set; }
        public string Png { get; set; }
        public string Art_crop { get; set; }
        public string Border_crop { get; set; }
    }

    public class Legalities
    {
        public string Standard { get; set; }
        public string Future { get; set; }
        public string Frontier { get; set; }
        public string Modern { get; set; }
        public string Legacy { get; set; }
        public string Pauper { get; set; }
        public string Vintage { get; set; }
        public string Penny { get; set; }
        public string Commander { get; set; }
        public string Duel { get; set; }
        public string Oldschool { get; set; }
    }

    public class Prices
    {
        public object Usd { get; set; }
        public string Usd_foil { get; set; }
        public string Eur { get; set; }
        public object Tix { get; set; }
    }

    public class Related_Uris
    {
        public string Gatherer { get; set; }
        public string Tcgplayer_decks { get; set; }
        public string Edhrec { get; set; }
        public string Mtgtop8 { get; set; }
    }

    public class Purchase_Uris
    {
        public string Tcgplayer { get; set; }
        public string Cardmarket { get; set; }
        public string Cardhoarder { get; set; }
    }

    public class All_Parts
    {
        public string _object { get; set; }
        public string Id { get; set; }
        public string Component { get; set; }
        public string Name { get; set; }
        public string Type_line { get; set; }
        public string Uri { get; set; }
    }

}
