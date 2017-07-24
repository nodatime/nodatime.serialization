// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;
using System;

namespace NodaTime.Serialization.JsonNet
{
    /// <summary>
    /// Helper static methods for argument/state validation. (Just the subset used within this library.)
    /// </summary>
    internal static class Preconditions
    {
        internal static T CheckNotNull<T>(T argument, string paramName) where T : class
            => argument ?? throw new ArgumentNullException(paramName);
        
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
