﻿namespace Moonglade.Core
{
    public readonly struct Archive
    {
        public int Year { get; }
        public int Month { get; }
        public int Count { get; }

        public Archive(int year, int month, int count)
        {
            Year = year;
            Month = month;
            Count = count;
        }
    }
}
