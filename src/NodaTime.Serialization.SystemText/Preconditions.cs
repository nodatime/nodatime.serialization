// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.Serialization.SystemText
{
    /// <summary>
    /// Helper static methods for argument/state validation. (Just the subset used within this library.)
    /// </summary>
    internal static class Preconditions
    {
        internal static T CheckNotNull<T>(T argument, string paramName)
            => argument == null ? throw new ArgumentNullException(paramName) : argument;

        internal static void CheckArgument(bool expression, string parameter, string message)
        {
            if (!expression)
            {
                throw new ArgumentException(message, parameter);
            }
        }

        internal static void CheckData<T>(bool expression, string messageFormat, T messageArg)
        {
            if (!expression)
            {
                string message = string.Format(messageFormat, messageArg);
                throw new InvalidNodaDataException(message);
            }
        }
    }
}
