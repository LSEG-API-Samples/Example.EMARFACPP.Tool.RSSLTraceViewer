namespace RSSLTraceDecoder
{
    namespace Utils
    {

        public partial class RdmDataConverter
        {
            internal enum MagnitudeType
            {
                /// Enumeration value is 0 - power of -14.
                ExponentNeg14 = 0,

                /// Enumeration value is 1 - power of -13.
                ExponentNeg13 = 1,

                /// Enumeration value is 2 - power of -12.
                ExponentNeg12 = 2,

                /// Enumeration value is 3 - power of -11.
                ExponentNeg11 = 3,

                /// Enumeration value is 4 - power of -10.
                ExponentNeg10 = 4,

                /// Enumeration value is 5 - power of -9.
                ExponentNeg9 = 5,

                /// Enumeration value is 6 - power of -8.
                ExponentNeg8 = 6,

                /// Enumeration value is 7 - power of -7.
                ExponentNeg7 = 7,

                /// Enumeration value is 8 - power of -6.
                ExponentNeg6 = 8,

                /// Enumeration value is 9 - power of -5.
                ExponentNeg5 = 9,

                /// Enumeration value is 10 - power of -4.
                ExponentNeg4 = 10,

                /// Enumeration value is 11 - power of -3.
                ExponentNeg3 = 11,

                /// Enumeration value is 12 -	power of -2.
                ExponentNeg2 = 12,

                /// Enumeration value is 13 -	power of -1.
                ExponentNeg1 = 13,

                /// Enumeration value is 14 - power of 0.
                Exponent0 = 14,

                /// Enumeration value is 15 - power of 1.
                ExponentPos1 = 15,

                /// Enumeration value is 16 - power of 2.
                ExponentPos2 = 16,

                /// Enumeration value is 17 - power of 3.
                ExponentPos3 = 17,

                /// Enumeration value is 18 - power of 4.
                ExponentPos4 = 18,

                /// Enumeration value is 19 - power of 5.
                ExponentPos5 = 19,

                /// Enumeration value is 20 - power of 6.
                ExponentPos6 = 20,

                /// Enumeration value is 21 - power of 7.
                ExponentPos7 = 21,

                /// Enumeration value is 22 - 1/1 - Dividend is 1. Divisor is	1.
                Divisor1 = 22,

                /// Enumeration value is 23 - 1/2 - Dividend is 1. Divisor is	2.
                Divisor2 = 23,

                /// Enumeration value is 24 - 1/4 - Dividend is 1. Divisor is	4.
                Divisor4 = 24,

                /// Enumeration value is 25 - 1/8 - Dividend is 1. Divisor is	8.
                Divisor8 = 25,

                /// Enumeration value is 26 - 1/16 - Dividend	is 1. Divisor is 16.
                Divisor16 = 26,

                /// Enumeration value is 27 - 1/32 - Dividend is 1. Divisor is	32.
                Divisor32 = 27,

                /// Enumeration value is 28 - 1/64 - Dividend	is 1. Divisor is 64.
                Divisor64 = 28,

                /// Enumeration value is 29 - 1/128 - Dividend	is 1. Divisor is 128.
                Divisor128 = 29,

                /// Enumeration value is 30 - 1/256 - Dividend is	1. Divisor is 256.
                Divisor256 = 30,

                /// Represents Infinity value is 33
                Infinity = 33,

                /// Represents negative infinity value is 34
                NegInfinity = 34,

                /// Represents Real is not a number (NaN) value is 35
                NotANumber = 35
            }
        }
    }
}