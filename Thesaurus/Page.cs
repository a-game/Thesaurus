using System.Globalization;

namespace Thesaurus
{
    public struct Page
    {
        public int Size { get; set; }
        public int From { get; set; }

        public Page Previous()
        {
            if ((From - Size) <= 0)
            {
                return Zero();
            }
            return new Page { Size = Size, From = From - Size };
        }

        public Page? Next(long totalNumber)
        {
            if ((From + Size) < totalNumber)
            {
                return Next();
            }
            return null;
        }

        public Page Next()
        {
            return new Page { Size = Size, From = From + Size };
        }

        public Page PlusOne()
        {
            return new Page { Size = Size + 1, From = From };
        }

        public static Page Single()
        {
            return new Page { Size = 1, From = 0 };
        }

        public static Page Zero()
        {
            return new Page { Size = 0, From = 0 };
        }

        public override string ToString()
        {
            return (From / Size).ToString(CultureInfo.InvariantCulture);
        }
    }
}