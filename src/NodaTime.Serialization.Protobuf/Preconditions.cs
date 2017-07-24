// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Serialization.Protobuf
{
    /// <summary>
    /// Helper static methods for argument/state validation. (Just the subset used within this library.)
    /// </summary>
    internal static class Preconditions
    {
        internal static T CheckNotNull<T>(T argument, string paramName) where T : class
            => argument ?? throw new ArgumentNullException(paramName);

        internal static void CheckArgument<T>(bool expression, string parameter, string messageFormat, T messageArg)
        {
            if (!expression)
            {
                string message = string.Format(messageFormat, messageArg);
                throw new ArgumentException(message, parameter);
            }
        }

        internal static void CheckArgument<T1, T2>(bool expression, string parameter, string messageFormat, T1 messageArg1, T2 messageArg2)
        {
            if (!expression)
            {
                string message = string.Format(messageFormat, messageArg1, messageArg2);
                throw new ArgumentException(message, parameter);
            }
        }
    }
}
