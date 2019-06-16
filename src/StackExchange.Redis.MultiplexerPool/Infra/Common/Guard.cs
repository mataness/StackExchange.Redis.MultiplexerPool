using System;
using System.Runtime.CompilerServices;

namespace StackExchange.Redis.MultiplexerPool.Infra.Common
{
    /// <summary>
    /// Helper class for checking preconditions and throw exceptions if those preconditions aren't met
    /// </summary>
    internal static class Guard
    {
                /// <summary>
        /// Throws an exception if <paramref name="shouldThrow"/> is true
        /// </summary>
        /// <param name="shouldThrow">
        /// Indication whether to throw the exception or not, this should be the result of an invalid argument check the caller does
        /// </param>
        /// <param name="message">
        /// A messages that describes what's invalid about the argument(s)
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="shouldThrow"/> is true
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckArgument(bool shouldThrow, string message)
        {
            if (shouldThrow)
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Throws an exception if <paramref name="shouldThrow"/> is true
        /// </summary>
        /// <param name="shouldThrow">
        /// Indication whether to throw the exception or not, this should be the result of an invalid argument check the caller does
        /// </param>
        /// <param name="paramName">
        /// The name of the method parameter(s) checked.
        /// </param>
        /// <param name="message">
        /// A messages that describes what's invalid about the argument(s)
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="shouldThrow"/> is true
        /// </exception>
        public static void CheckArgument(bool shouldThrow, string paramName, string message)
        {
            if (shouldThrow)
            {
                throw new ArgumentException(message, paramName);
            }
        }
        /// <summary>
        /// Checks if <paramref name="argument"/> is null and throws exception if it is, otherwise returning it
        /// </summary>
        /// <param name="argument">
        /// The value to check if it's null
        /// </param>
        /// <param name="paramName">
        /// The name of the method parameter checked.
        /// </param>
        /// <typeparam name="T">
        /// Type of argument checked. Used instead of accepting <see cref="object"/> to ensure in compile-time
        /// the validation can be applied to ref types only, and not by accident applies to value types which will cause boxing
        /// </typeparam>
        /// <returns>
        /// If <paramref name="argument"/> isn't null it is returned, otherwise an exception is thrown
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The exception thrown in case <paramref name="argument"/> is null
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CheckNullArgument<T>(T argument, string paramName)
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return argument;
        }

        /// <summary>
        /// Checks whether an integer argument is greater/equal to a given min value
        /// </summary>
        /// <param name="argument">
        /// The argument to check.
        /// </param>
        /// <param name="minValueInclusive">
        /// Minimal allowed value for the argument, inclusive (meaning argument should be greater/equal than this value).
        /// </param>
        /// <param name="paramName">
        /// The name of the method parameter checked.
        /// </param>
        /// <returns>
        /// The argument, if it's greater/equal than the min value, otherwise an exception is thrown
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The exception thrown in case <paramref name="argument"/> is not greater/equal than min value
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CheckArgumentLowerBound(int argument, int minValueInclusive, string paramName)
        {
            if (argument >= minValueInclusive)
            {
                return argument;
            }

            throw new ArgumentOutOfRangeException(paramName, argument, $"Argument should be in greater/equal than {minValueInclusive}");
        }
    }
}
