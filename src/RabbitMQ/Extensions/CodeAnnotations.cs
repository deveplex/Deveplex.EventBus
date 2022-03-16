/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;

namespace System.Diagnostics.CodeAnalysis
{
#if NET45 || NETSTANDARD2_0
    [AttributeUsage(
        AttributeTargets.Method
        | AttributeTargets.Parameter
        | AttributeTargets.Property
        | AttributeTargets.Delegate
        | AttributeTargets.Field)]
    internal sealed class NotNullAttribute : Attribute
    {
    }
#endif
}
