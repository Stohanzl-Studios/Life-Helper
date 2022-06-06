using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Enums
{
    [Flags]
    public enum Result
    {
        Success = 1 << 0,
        Failure = 1 << 1,
        InvalidCredentials = 1 << 2,
        AccountNotFound = 1 << 3,
        AccountDisabled = 1 << 4,
        Internal = 1 << 5,
        AccountAlreadyExists = 1 << 6,
        InvalidRefreshToken = 1 << 7,
        UsernameAlreadyExists = 1 << 8,
        NotImplemented = 1 << 9,
        InvalidAccessToken = 1 << 10,
        Unauthorized = 1 << 11,
        InvalidRequest = 1 << 12,
        InvalidJWTToken = 1 << 13
    }
    public enum TokenType { Access, Refresh, UID, JWT }
    public enum Visibility { Public, Friends, Private }

    [Flags]
    public enum MathOperation { None = 0, Addition = 1 << 1, Subtraction = 1 << 2, Multiplication = 1 << 3, Division = 1 << 4, Square = 1 << 5, SquareRoot = 1 << 6, Modulus = 1 << 7 }
    public enum HistoryAction { None, Edit, Delete }

    public class EnumHelper
    {
        public static Visibility GetVisibility(string name)
        {
            switch (name.ToLower())
            {
                case "friends":
                    return Visibility.Friends;

                case "public":
                    return Visibility.Public;

                default:
                case "private":
                    return Visibility.Private;
            }
        }
        public static string GetVisibilityName(Visibility visibility)
        {
            switch (visibility)
            {
                case Visibility.Public: return "Public";
                default:
                case Visibility.Private: return "Private";
                case Visibility.Friends: return "Friends";
            }
        }
        public static IEnumerable<T> GetFlags<T>(T input) where T : Enum
        {
            foreach (T value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }

        public static string GetOperation(MathOperation operation)
        {
            switch (operation)
            {
                case MathOperation.Addition: return "+";
                case MathOperation.Subtraction: return "-";
                case MathOperation.Multiplication: return "x";
                case MathOperation.Division: return "÷";
                case MathOperation.SquareRoot: return "√";
                case MathOperation.Square: return "²";
                case MathOperation.Modulus: return "%";
                default: return "";
            }
        }
        public static MathOperation GetMathOperation(char data)
        {
            switch (data)
            {
                default: return MathOperation.None;
                case '+': return MathOperation.Addition;
                case '-': return MathOperation.Subtraction;
                case 'x': return MathOperation.Multiplication;
                case '÷': return MathOperation.Division;
                case '%': return MathOperation.Modulus;
                case '√': return MathOperation.SquareRoot;
                case '²': return MathOperation.Square;
            }
        }
    }
}
