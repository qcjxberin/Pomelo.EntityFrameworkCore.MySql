﻿// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    public class MySqlRegexIsMatchTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo IsMatch;
        private static readonly MethodInfo IsMatchWithRegexOptions;

        private const RegexOptions UnsupportedRegexOptions = RegexOptions.RightToLeft | RegexOptions.ECMAScript;

        static MySqlRegexIsMatchTranslator()
        {
            IsMatch = typeof (Regex).GetTypeInfo().GetDeclaredMethods("IsMatch").Single(m =>
                m.GetParameters().Count() == 2 &&
                m.GetParameters().All(p => p.ParameterType == typeof(string))
            );
            IsMatchWithRegexOptions = typeof(Regex).GetTypeInfo().GetDeclaredMethods("IsMatch").Single(m =>
               m.GetParameters().Count() == 3 &&
               m.GetParameters().Take(2).All(p => p.ParameterType == typeof(string)) &&
               m.GetParameters()[2].ParameterType == typeof(RegexOptions)
            );
        }

        public Expression Translate([NotNull] MethodCallExpression methodCallExpression)
        {
            // Regex.IsMatch(string, string)
            if (methodCallExpression.Method == IsMatch)
            {
                return new RegexMatchExpression(
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1],
                    RegexOptions.None
                );
            }

            // Regex.IsMatch(string, string, RegexOptions)
            if (methodCallExpression.Method == IsMatchWithRegexOptions)
            {
                var constantExpr = methodCallExpression.Arguments[2] as ConstantExpression;

                if (constantExpr == null)
                {
                    return null;
                }

                var options = (RegexOptions) constantExpr.Value;

                if ((options & UnsupportedRegexOptions) != 0)
                {
                    return null;
                }

                return new RegexMatchExpression(
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1],
                    options
                );
            }

            return null;
        }
    }
}
