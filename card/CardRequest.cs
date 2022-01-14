namespace card
{
    [System.Serializable]
    public class CardRequest
    {
        public CardData data { get; set; }
    }

    [System.Serializable]
    public class MultiCardRequest
    {
        public CardData[] data { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int count { get; set; }
        public int totalCount { get; set; }

        public override string ToString()
        {
            var retStr =
                data.ToString() +
                "\n" +
                page.ToString() +
                "\n" +
                pageSize.ToString() +
                "\n" +
                count.ToString() +
                "\n" +
                totalCount.ToString();
            return retStr;
        }
    }

    [System.Serializable]
    public class CardData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string supertype { get; set; }
        public string[] subtypes { get; set; }
        public string   level { get; set; }
        public string hp { get; set; }
        public string[] types { get; set; }
        public string evolvesFrom { get; set; }
        public string[] evolvesTo { get; set; }
        
        public string[] rules { get; set; }
        public AncientTrait ancientTrait { get; set; }
        public Ability[] abilities { get; set; }
        public Attack[] attacks { get; set; }
        public Weakness[] weaknesses { get; set; }
        public Resistance[] resistances { get; set; }
        public string[] retreatCost { get; set; }
        public int convertedRetreatCost { get; set; }
        public set.SetData set { get; set; }
        public string number { get; set; }
        public string artist { get; set; }
        public string rarity { get; set; }
        public int[] nationalPokedexNumbers { get; set; }
        public set.Legalities legalities { get; set; }
        public CardImages images { get; set; }
        public Tcgplayer tcgplayer { get; set; }
        public Cardmarket cardmarket { get; set; }

        public override string ToString()
        {
            string retStr = "";
            /*    "ID: " + id +
                "\nName: " + name +
                "\nSuperType: " + supertype +
                "\nSubtypes: ";

            foreach (var subt in subtypes)
            {
                retStr += "\n\t" + subt;
            }
            
            retStr += "\nLevel: " +level + "\n" +
                "HP: " + hp + "\nTypes: ";

            if (types != null)
            {

                foreach (var typ in types)
                {
                    retStr += "\t" + typ + "\n";
                }
            }

            retStr += "\nEvolvesFrom: " + evolvesFrom + "\nEvolvesTo: ";

            if (evolvesTo != null)
            {
                foreach (var evTo in evolvesTo)
                {
                    retStr += "\t" + evTo + "\n";
                }
            }

            retStr += "\nRules: \n";
            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    retStr += ":: " + rule + "\n";
                }
            }

            if (ancientTrait != null)
            {
                retStr +="\n" +
                         ancientTrait.ToString();
            }

            if (abilities != null)
            {
                retStr += "\nAbilities: ";
                foreach (var abl in abilities)
                {
                    retStr += "\n" +
                        abl.ToString();
                }
                
            }

            if (attacks != null)
            {
                retStr += "\n\nAttacks:  ";
                foreach (var attack in attacks)
                {
                    retStr += "\t" + attack.ToString();
                }

            }
            
            if (weaknesses != null)
            {
                retStr += "\nWeaknesses:  ";
                foreach (var weak in weaknesses)
                {
                    retStr += weak.ToString();
                }

            }
            
            if (resistances != null)
            {
                retStr += "\n\nResistances:  ";
                foreach (var resist in resistances)
                {
                    retStr += resist.ToString();
                }

            }
            
            retStr += "\n\nRetreat Cost: ";
            if (retreatCost != null)
            {
                foreach (var retc in retreatCost)
                {
                    retStr += "\n\t" + retc;
                }
            }

            retStr += "\nConverted Retreat Cost: " +
                      convertedRetreatCost;*/
            

            return retStr;
        }
    }

    [System.Serializable]
    public class Resistance
    {
        public string type;
        public string value;

        public override string ToString()
        {
            string retStr = "";
            if (type != null) retStr += "\n\tType:\t" + type;
            if (value != null) retStr += "\n" + value;
            return retStr;
        }
    }

    [System.Serializable]
    public class Ability
    {
        public string name { get; set; }
        public string text { get; set; }
        public string type { get; set; }

        public override string ToString()
        {
            string retStr = "";
            if (type != null) retStr += "\tType:\t" + type;
            if (name != null) retStr += "\n\tName\t" + name;
            if (text != null) retStr += "\n" + text;
            return retStr;
        }
    }

    [System.Serializable]
    public class AncientTrait
    {
        public string name { get; set; }
        public string text { get; set; }

        public override string ToString()
        {
            string retStr = "";
            if (name != null) retStr += "\n\tName\t" + name;
            if (text != null) retStr += "\n" + text;
            return retStr;
        }
    }


    [System.Serializable]
    public class CardImages
    {
        public string small { get; set; }
        public string large { get; set; }
        
        public override string ToString()
        {
            string retStr = "Images:";
            
            if (small != null) retStr += "\n\tSmall: " + small;
            if (large != null) retStr += "\n\tLarge: " + large;
            return retStr;
        }
    }

    [System.Serializable]
    public class Tcgplayer
    {
        public string url { get; set; }
        public string updatedAt { get; set; }
        public Prices prices { get; set; }
        public override string ToString()
        {
            return "\nUrl: " + url + 
                   "\nUpdated At: " + updatedAt + 
                   "\n" + prices.ToString();
        }
    }

    [System.Serializable]
    public class Prices
    {
        public Holofoil holofoil { get; set; }

        public override string ToString()
        {
            return holofoil.ToString();
        }
    }

    [System.Serializable]
    public class Holofoil
    {
        public float low { get; set; }
        public float mid { get; set; }
        public float high { get; set; }
        public float market { get; set; }

        public override string ToString()
        {
            return "\nLow: " + low +
                   "\nMid: " + mid +
                   "\nHi: " + high +
                   "\nMkt: " + market;
        }
    }

    [System.Serializable]
    public class Cardmarket
    {
        public string url { get; set; }
        public string updatedAt { get; set; }
        public Prices1 prices { get; set; }

        public override string ToString()
        {
            return "\nUrl: " + url + 
                   "\nUpdated At: " + updatedAt + 
                   "\nPrices: " + prices.ToString();
        }
    }

    [System.Serializable]
    public class Prices1
    {
        public float averageSellPrice { get; set; }
        public float lowPrice { get; set; }
        public float trendPrice { get; set; }
        public float reverseHoloTrend { get; set; }
        public float lowPriceExPlus { get; set; }
        public float avg1 { get; set; }
        public float avg7 { get; set; }
        public float avg30 { get; set; }
        public float reverseHoloAvg1 { get; set; }
        public float reverseHoloAvg7 { get; set; }
        public float reverseHoloAvg30 { get; set; }

        public override string ToString()
        {
            return "\naverageSellPrice: " + averageSellPrice + "\n" +
                   "lowPrice: " + lowPrice + "\n" +
                   "trendPrice: " + trendPrice + "\n" +
                   "reverseHoloTrend: " + reverseHoloTrend + "\n" +
                   "lowPriceExPlus: " + lowPriceExPlus + "\n" +
                   "avg1: " + avg1 + "\n" +
                   "avg7: " + avg7 + "\n" +
                   "avg30: " + avg30 + "\n" +
                   "reverseHoloAvg1: " + reverseHoloAvg1 + "\n" +
                   "reverseHoloAvg7: " + reverseHoloAvg7 + "\n" +
                   "reverseHoloAvg30: " + reverseHoloAvg30 + "\n";
        }
    }

    [System.Serializable]
    public class Attack
    {
        public string name { get; set; }
        public string[] cost { get; set; }
        public int convertedEnergyCost { get; set; }
        public string damage { get; set; }
        public string text { get; set; }

        public override string ToString()
        {
            string retStr = "";
            
            if (name != null) retStr += "\nName: " + name;

            if (cost != null)
            {
                retStr += "\n\tCost: ";
                foreach (var xCost in cost)
                {
                    retStr += "\n\t\t" + xCost;
                }
            }

            if (damage != null)
            {
                retStr += "\n\tDMG: " + damage;
            }
            
            if (text != null)
            {
                retStr += "\n\n" + text;
            }
            
            
            
            return retStr;


        }
    }

    [System.Serializable]
    public class Weakness
    {
        public string type { get; set; }
        public string value { get; set; }

        public override string ToString()
        {
            return
                "\tType: " + type +
                "\n\tValue: " + value;
        }
    }
}



