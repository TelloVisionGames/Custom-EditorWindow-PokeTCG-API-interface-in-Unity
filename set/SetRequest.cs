
using System;

namespace set
{

    
    
    public class SetRequest
    {
        public SetData data { get; set; }

        public override string ToString()
        {
            return data.ToString();
        }
    }

    public class MultiSetRequest
    {
        public SetData[] data { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int count { get; set; }
        public int totalCount { get; set; }

        public override string ToString()
        {
            return "\n" + data.ToString() +
                   "\npage: " + page +
                   "\npageSize: " + pageSize +
                   "\ncount: " + count +
                   "\ntotalCount: " + totalCount;
        }
    }

    public class SetData : IDisposable
    {
        public string id { get; set; }
        public string name { get; set; }
        public string series { get; set; }
        public int printedTotal { get; set; }
        public int total { get; set; }
        public Legalities legalities { get; set; }
        public string ptcgoCode { get; set; }
        public string releaseDate { get; set; }
        public string updatedAt { get; set; }
        public Images images { get; set; }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "\nId: " + id +
                   "\nName: " + name +
                   "\nSeries: " + series +
                   "\nPrinted Total: " + printedTotal +
                   "\nTotal: " + total +
                   "\nLegalities: " + legalities.ToString() +
                   "\nptcGoCode: " + ptcgoCode +
                   "\nRelease Date: " + releaseDate +
                   "\nUpdated At: " + updatedAt +
                   "\nImages: " + images.ToString();
        }
    }

    public class Legalities
    {
        public string unlimited { get; set; }
        public string standard { get; set; }
        public string expanded { get; set; }

        public override string ToString()
        {
            return "\nUnlimited: " + unlimited +
                   "\nStandard: " + standard +
                   "\nExpanded: " + expanded;
        }
    }

    public class Images
    {
        public string symbol { get; set; }
        public string logo { get; set; }

        public override string ToString()
        {
            return "\nSymbol: " + symbol +
                   "\nLogo: " + logo;
        }
    }

}