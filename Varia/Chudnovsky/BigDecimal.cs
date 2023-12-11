using System;
using System.Linq;
using System.Numerics;

namespace Chudnovsky
{
    internal class BigDecimal : IComparable<BigDecimal>
    {
        private BigInteger myMantissa = 0;
        private int myExponent = 0;
        //Sets the maximum precision of division operations.
        private static int MyPrecision = 50;
        //Specifies whether the significant digits should be truncated 
        //to the given precision after each operation.
        //If AlwaysTruncate Is set to true all operations are affected.
        public static bool AlwaysTruncate = false;
        //Normal Notation = mantissa = any BigInteger, Exponent = an integer value.
        //Engineering Notation = mantissa between -999.999.. and 999.999..., Exponent = multiple of 3.
        //Scientific notation = mantissa between -9.9999.. and 9.9999..., Exponent = any integer value.
        public static NotationType Notation = NotationType.Normal;

        public BigDecimal(BigInteger mantissa, int exponent)
        {
            myMantissa = mantissa;
            myExponent = exponent;
            Normalize();
            if (AlwaysTruncate) Truncate();
        }

        #region "Properties"

        public BigInteger Mantissa
        {
            get { return myMantissa;  }
            set { myMantissa = value; }
        }

        public int Exponent
        {
            get { return myExponent; }
            set { myExponent = value; }
        }
        
        public static int Precision
        {
            get { return MyPrecision; }
            set
            {
                MyPrecision = value;
                if (MyPrecision < 1) MyPrecision = 1;
            }
        }

        #endregion 

        /// <summary>
        /// Removes trailing zeros on the mantissa
        /// </summary>
        public void Normalize()
        {
            BigInteger remainder = 0;
            BigInteger shortened;
            if (myMantissa.IsZero)
            {
                myExponent = 0;
            }
            else
            {
                while (remainder == 0)
                {
                    shortened = BigInteger.DivRem(myMantissa, 10, out remainder);
                    if(remainder == 0)
                    {
                        myMantissa = shortened;
                        myExponent += 1;
                    }
                }
            }
        }

        public void Truncate()
        {
            Truncate(MyPrecision);
        }

        public void Truncate(int digits)
        {
            if (digits < 1) digits = 1;
            //remove the least significant digits, as long as the number of digits Is higher than the given Precision
            while(NumberOfDigits(myMantissa) > digits)
            {
                myMantissa /= 10;
                myExponent += 1;
            }
        }

        public static int NumberOfDigits(BigInteger value)
        {
            //do Not count the sign
            return (value * value.Sign).ToString().Length;
        }

        #region "Conversions"

        public static implicit operator BigDecimal (int value)
        {
            return new BigDecimal(value, 0);
        }

        public static implicit operator BigDecimal(long value)
        {
            return new BigDecimal(value, 0);
        }

        public static implicit operator BigDecimal(decimal value)
        {
            BigInteger mantissa = new BigInteger(value);
            int exponent = 0;
            decimal scaleFactor = 1;
            while((decimal)mantissa != value * scaleFactor)
            {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = new BigInteger(value * scaleFactor);
            }
            return new BigDecimal(mantissa , exponent);
        }

        public static implicit operator BigDecimal(float value)
        {
            string mantstring;
            string expString;
            BigInteger mant;
            int exp = 0;
            string valueString = value.ToString();
            if (valueString.Contains('E'))
            {
                int Epos = valueString.IndexOf('E');
                mantstring = valueString.Substring(0, Epos);
                expString = valueString.Substring(Epos + 1, valueString.Length - Epos - 1);
                exp = int.Parse(expString);
            }
            else
            {
                mantstring = valueString;
            }
            if (mantstring.Contains('.'))
            {
                int DotPos = mantstring.IndexOf('.');
                exp -= mantstring.Length - DotPos - 1;
                mantstring = mantstring.Substring(0, DotPos) + mantstring.Substring(DotPos + 1, mantstring.Length - DotPos - 1);
            }
            mant = BigInteger.Parse(mantstring);
            return new BigDecimal(mant, exp);
        }

        public static implicit operator BigDecimal(double value)
        {
            string mantstring;
            string expString;
            BigInteger mant;
            int exp = 0;
            string valueString = value.ToString();
            if (valueString.Contains('E'))
            {
                int Epos = valueString.IndexOf('E');
                mantstring = valueString.Substring(0, Epos);
                expString = valueString.Substring(Epos + 1, valueString.Length - Epos - 1);
                exp = int.Parse(expString);
            }
            else
            {
                mantstring = valueString;
            }
            if (mantstring.Contains('.'))
            {
                int DotPos = mantstring.IndexOf('.');
                exp -= mantstring.Length - DotPos - 1;
                mantstring = mantstring.Substring(0, DotPos) + mantstring.Substring(DotPos + 1, mantstring.Length - DotPos - 1);
            }
            mant = BigInteger.Parse(mantstring);
            return new BigDecimal(mant, exp);
        }

        public static explicit operator double(BigDecimal value)
        {
            BigDecimal Max = double.MaxValue;
            BigDecimal Min = double.MinValue;
            if (value > Max | value < Min) throw new OverflowException();
            return (double)value.Mantissa * Math.Pow(10, value.Exponent);
        }

        public static explicit operator float(BigDecimal value)
        {
            BigDecimal Max = float.MaxValue;
            BigDecimal Min = float.MinValue;
            if (value > Max | value < Min) throw new OverflowException();
            return (float)((double)value.Mantissa * Math.Pow(10, value.Exponent));
        }

        public static explicit operator decimal(BigDecimal value)
        {
            BigDecimal Max = decimal.MaxValue;
            BigDecimal Min = decimal.MinValue;
            if (value > Max | value < Min) throw new OverflowException();
            return (decimal)value.Mantissa * (decimal)Math.Pow(10, value.Exponent);
        }

        public static explicit operator long(BigDecimal value)
        {
            BigDecimal Max = long.MaxValue;
            BigDecimal Min = long.MinValue;
            if (value > Max | value < Min) throw new OverflowException();
            return (long)((double)value.Mantissa * Math.Pow(10, value.Exponent));
        }

        public static explicit operator int(BigDecimal value)
        {
            BigDecimal Max = int.MaxValue;
            BigDecimal Min = int.MinValue;
            if (value > Max | value < Min) throw new OverflowException();
            return (int)((double)value.Mantissa * Math.Pow(10, value.Exponent));
        }

        #endregion


        #region "Operators"

        public static BigDecimal operator +(BigDecimal value)
        {
            return value;
        }

        public static BigDecimal operator -(BigDecimal value)
        {
            value.Mantissa *= -1;
            return value;
        }

        public static BigDecimal operator +(BigDecimal left, BigDecimal right)
        {
            if (left.Exponent > right.Exponent)
            {
                return new BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent);
            }
            else
            {
                return new BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent);
            }
        }

        public static BigDecimal operator -(BigDecimal left, BigDecimal right)
        {
            if (left.Exponent > right.Exponent)
            {
                return new BigDecimal(AlignExponent(left, right) - right.Mantissa, right.Exponent);
            }
            else
            {
                return new BigDecimal(left.Mantissa - AlignExponent(right, left), left.Exponent);
            }
        }

        public static BigDecimal operator *(BigDecimal left, BigDecimal right)
        {
            return new BigDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);
        }

        public static BigDecimal operator /(BigDecimal dividend, BigDecimal divisor)
        {
            int exponentChange = MyPrecision - (NumberOfDigits(dividend.Mantissa) - NumberOfDigits(divisor.Mantissa));
            if (exponentChange < 0) exponentChange = 0;
            BigDecimal result = new BigDecimal(dividend.Mantissa * BigInteger.Pow(10, exponentChange) / divisor.Mantissa, dividend.Exponent - divisor.Exponent - exponentChange);
            //Dim result As New BigDecimal(dividend.Mantissa * BigInteger.Pow(10, exponentChange + 1) / divisor.Mantissa, dividend.Exponent - divisor.Exponent - exponentChange - 1)
            result.Truncate();
            return result;
        }

        public static BigDecimal operator %(BigDecimal left, BigDecimal right)
        {
            return left - right * new BigDecimal((left / right).Floor(),0);
        }

        public static bool operator ==(BigDecimal left, BigDecimal right)
        {
            return (left.Mantissa == right.Mantissa & left.Exponent == right.Exponent);
        }

        public static bool operator !=(BigDecimal left, BigDecimal right)
        {
            return (left.Mantissa != right.Mantissa | left.Exponent != right.Exponent);
        }

        public override bool Equals(object obj)
        {
            if (obj is int) return (BigDecimal)obj == this;
            if (obj is long) return (BigDecimal)obj == this;
            if (obj is float) return (BigDecimal)obj == this;
            if (obj is double) return (BigDecimal)obj == this;
            if (obj is decimal) return (BigDecimal)obj == this;
            if (obj is BigDecimal) return (BigDecimal)obj == this;
            return false;
        }

        public static bool operator <(BigDecimal left, BigDecimal right)
        {
            if (left.Exponent > right.Exponent)
            {
                return AlignExponent(left, right) < right.Mantissa;
            }  
        else
            {
                return left.Mantissa < AlignExponent(right, left);
            }
        }

        public static bool operator >(BigDecimal left, BigDecimal right)
        {
            if (left.Exponent > right.Exponent)
            {
                return AlignExponent(left, right) > right.Mantissa;
            }
            else
            {
                return left.Mantissa > AlignExponent(right, left);
            }
        }

        public static bool operator <=(BigDecimal left, BigDecimal right)
        {
            if (left.Exponent > right.Exponent)
            {
                return AlignExponent(left, right) <= right.Mantissa;
            }
            else
            {
                return left.Mantissa <= AlignExponent(right, left);
            }
        }

        public static bool operator >=(BigDecimal left, BigDecimal right)
        {
            if (left.Exponent > right.Exponent)
            {
                return AlignExponent(left, right) >= right.Mantissa;
            }
            else
            {
                return left.Mantissa >= AlignExponent(right, left);
            }
        }

            //Returns the mantissa of value, aligned to the exponent of reference.
            //Assumes the exponent of value Is larger than of reference.
            private static BigInteger AlignExponent(BigDecimal value, BigDecimal reference)
            {
                return value.Mantissa * BigInteger.Pow(10, value.Exponent - reference.Exponent);
            }

        #endregion

        #region "Additional Mathematical Functions

        public static BigDecimal Exp(BigDecimal exponent )
        { 
            //Newton's Series Expansion for e^(x) converges fast for small x
            BigInteger Mant;
            int Expo;
            BigDecimal A;
            BigDecimal APow;
            BigDecimal eA;
            BigDecimal Iter;
            BigDecimal result;
            BigDecimal Accur = new BigDecimal(1, -MyPrecision);
            int I = 0;
            if (exponent > 10 | exponent< -10 ) 
            {
                //Convert exponent to scientific notation A x 10^(B) where A < 10.
                Mant = exponent.Mantissa * exponent.Mantissa.Sign;
                Expo = exponent.Exponent;
                while (Mant >= 10)
                {
                    Mant /= 10;
                    Expo += 1;
                }
                //Step 1: calculate e^(A)
                A = exponent / Math.Pow(10, Expo);
                APow = new BigDecimal(1, 0);
                Iter = new BigDecimal(1, 0);
                eA = new BigDecimal(0, 0);
                while (BigDecimal.Abs(Iter) > Accur)
                {
                    Iter = APow / BigDecimal.Fact(I);
                    eA += Iter;
                    APow *= A;
                    I += 1;
                }
                eA = eA.Round(MyPrecision);
                //Step 2: Multiply e^(A) * e^(A) 10^(B) times
                result = new BigDecimal(1, 0);
                for (double J = 1; J <= Math.Pow(10, Expo); J += 1.0)
                {
                    result *= eA;
                }
            } 
            else 
            {
                //Calculate e^(exponent) directly
                APow = new BigDecimal(1, 0);
                Iter = new BigDecimal(1, 0);
                result = new BigDecimal(0, 0);
                while (BigDecimal.Abs(Iter) > Accur)
                { 
                    Iter = APow / BigDecimal.Fact(I);
                    result += Iter;
                    APow *= exponent;
                    I += 1;
                }
            }
            return result.Round(MyPrecision);
        }

        public static BigDecimal Abs(BigDecimal number)
        {
            return new BigDecimal(number.Mantissa * number.Mantissa.Sign, number.Exponent);
        }

        public static BigDecimal Logn(BigDecimal number)
        {
            //Use a Taylor series to calculate logn(Z) where Z < 2
            //Convert number to scientific notation A x 10^(B) where 1 <= A < 10.
            //logn(A x 10^(B)) = logn(A) + B*logn(10);
            //Sqrt(Sqrt(A)) <= 2 -> logn(A) = 4*logn(Sqrt(Sqrt(A)));
            //Sqrt(Sqrt(10)) < 2 -> logn(10) = 4*logn(Sqrt(Sqrt(10)));
            //--------------------
            //Logn(number) = 4*logn(Sqrt(Sqrt(A))) + 4*B*logn(Sqrt(Sqrt(10)));
            BigInteger Mant;
            int Expo;
            BigDecimal A;
            BigDecimal B;
            BigDecimal Z;
            BigDecimal ZFactor;
            BigDecimal ZPow;
            BigDecimal LnA;
            BigDecimal Ln10;
            BigDecimal Iter;
            BigDecimal result;
            BigDecimal Accur = new BigDecimal(1, -MyPrecision);
            BigDecimal I;
            if (number.Mantissa < 0)
            {
                throw new ArithmeticException("Logarithm of negative numbers do not exist.");
            }
            else if (number.Mantissa == 0)
            {
                throw new NotFiniteNumberException("Logarithm of 0 is infinite.");
            }
            else
            {
                //Convert number to scientific notation A x 10^(B) where A < 10.
                Mant = number.Mantissa * number.Mantissa.Sign;
                Expo = number.Exponent;
                while (Mant >= 10)
                {
                    Mant /= 10;
                    Expo += 1;
                }
                A = number / Math.Pow(10, Expo);
                B = Expo;
                //Calculate logn(A)
                Z = BigDecimal.SQRT(BigDecimal.SQRT(A));
                ZFactor = (Z - 1) / (Z + 1);
                I = new BigDecimal(0, 0);
                ZPow = ZFactor;
                Iter = new BigDecimal(1, 0);
                LnA = new BigDecimal(0, 0);
                while (BigDecimal.Abs(Iter) > Accur)
                {
                    Iter = ZPow / (2 * I + 1);
                    LnA += Iter;
                    ZPow = ZPow * ZFactor * ZFactor;
                    I += 1;
                }
                if (B != 0)
                {
                    //Calculate logn(10)
                    Z = BigDecimal.SQRT(BigDecimal.SQRT(10));
                    ZFactor = (Z - 1) / (Z + 1);
                    I = new BigDecimal(0, 0);
                    ZPow = ZFactor;
                    Iter = new BigDecimal(1, 0);
                    Ln10 = new BigDecimal(0, 0);
                    while (BigDecimal.Abs(Iter) > Accur)
                    {
                        Iter = ZPow / (2 * I + 1);
                        Ln10 += Iter;
                        ZPow = ZPow * ZFactor * ZFactor;
                        I += 1;
                    }
                }
                else
                {
                    Ln10 = new BigDecimal(0, 0);
                }

                //Calculate logn(number)
                result = 8 * LnA + 8 * B * Ln10;
            }
            return result.Round(MyPrecision);
        }

        public static BigDecimal Pow(BigDecimal basis, BigDecimal exponent)
        {
            //A^(B) = Exp(logn(A^(B))) = Exp(B*logn(A));
            try
            {
                return Exp(exponent * Logn(basis));
            }
            catch (Exception)
            {
                throw new ArithmeticException(basis.ToString() + " to the power of " + exponent.ToString() + " can not be calculated.");
            }
        }

        public static BigDecimal Fact(int number)
        {
            BigDecimal temp = new BigDecimal(1, 0);
            for (int I = 1; I <= number; I++)
            {
                temp *= (BigDecimal)I;
            }
            return temp;
        }

        public static BigDecimal SQRT(BigDecimal number)
        {
            BigInteger Mant;
            int Expo;
            BigDecimal result;
            if (number < 0)
            {
                throw new ArithmeticException("Square Root of negative numbers can not be calculated.");
            }
            else if (number.Mantissa == 0)
            {
                return new BigDecimal(0, 0);
            }
            //Get a start value
            Mant = number.Mantissa;
            Expo = number.Exponent;
            while (Mant >= 10)
            {
                Mant /= 10;
                Expo += 1;
            }
            result = Math.Sqrt((double)(number / Math.Pow(10, Expo))) * Math.Pow(10, Expo / 2);
            //Use Newton method for iterative calculation of square root
            for (int I = 0; I <= MyPrecision ; I++)
            {
                result = 0.5 * (result + number / result);
            }
            result.Truncate(MyPrecision);
            return result;
        }

        //Rounds the number to the given precision.
        public BigDecimal Round(int precision)
        {
            BigDecimal result = new BigDecimal(myMantissa, myExponent);
            BigInteger remainder = 0;
            if (precision < 1) precision = 1;
            //remove the least significant digits, as long as the number of digits Is higher than the given Precision
            while (NumberOfDigits(result.Mantissa) > precision)
            {
                result.Mantissa = BigInteger.DivRem(result.Mantissa, 10, out remainder);
                result.Exponent += 1;
            }
            if (remainder >= 5) result.Mantissa += 1;
            return result;
        }

        //Returns the largest BigInteger less than or equal to the current BigDecimal
        public BigInteger Floor()
        {
            BigInteger result = myMantissa;
            int exp = myExponent;
            if (exp >= 0)
            {
                while (exp > 0)
                {
                    result *= 10;
                    exp -= 1;
                }
            }
            else
            {
                while (exp < 0)
                {
                    result /= 10;
                    exp += 1;
                }
                if (result < 0) result -= 1;
            }
            return result;
        }

        #endregion

        //Displays the BigDecimal in the selected notation
        public override string ToString()
        {
            BigInteger Mant = myMantissa * myMantissa.Sign;
            string MantString = myMantissa.ToString();
            int Exp = myExponent;
            string ExpString = "";
            int DecimalPosition = 0;
            string returnString = "";
            if (Notation == NotationType.Engineering)
            {
                while (Mant >= 10)
                {
                    Mant /= 10;
                    Exp += 1;
                }
                DecimalPosition = 1;
                while (Exp % 3 > 0)
                {
                    Mant *= 10;
                    Exp -= 1;
                    DecimalPosition += 1;
                }
                if (myMantissa.Sign == -1) DecimalPosition += 1;
            }
            else if (Notation == NotationType.Scientific)
            {
                while (Mant >= 10)
                {
                    Mant /= 10;
                    Exp += 1;
                }
                DecimalPosition = 1;
                if (myMantissa.Sign == -1) DecimalPosition += 1;
            }
            else
            {
                DecimalPosition = 0;
            }
            if (Exp > 0)
            {
                ExpString = "E+" + Exp.ToString();
            }
            else if (Exp < 0)
            {
                ExpString = "E" + Exp.ToString();
            }
            else
            {
                ExpString = "";
            }
            while (DecimalPosition + 1 > MantString.Length)
            {
                MantString += "0";
            }
            if (DecimalPosition > 0)
            {
                returnString = string.Concat(MantString.Insert(DecimalPosition, "."), ExpString);
            }
            else
            {
                returnString = string.Concat(MantString, ExpString);
            }
            return returnString;
        }

        public bool Equals(BigDecimal other)
        {
            return (other.Mantissa.Equals(myMantissa) & other.Exponent == myExponent);
        }


        public override int GetHashCode()
        {
            return (int)((myMantissa.GetHashCode() * 397) ^ myExponent);
        }

        public int CompareTo(BigDecimal other)
        {
            if (this < other)
            {
                return -1;
            }
            else if (this > other)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }


    public enum NotationType
    {
        Normal = 1,
        Engineering = 2,
        Scientific = 3
    }

}
