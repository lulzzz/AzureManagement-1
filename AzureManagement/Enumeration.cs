﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AzureManagement
{
    /// <summary>
    /// https://github.com/HeadspringLabs/Enumeration/blob/master/Enumeration.cs
    /// </summary>
    public abstract class Enumeration : IComparable
    {
        public int Value { get; }
        public string DisplayName { get; }
        protected Enumeration(int value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        protected Enumeration() { }

        private static IEnumerable<T> GetAll<T>() where T : Enumeration, new()
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
            {
                var instance = new T();
                var locatedValue = info.GetValue(instance) as T;

                if (locatedValue == null)
                {
                    continue;
                }

                yield return locatedValue;
            }
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Value - secondValue.Value);
            return absoluteDifference;
        }
        public static T FromValue<T>(int value) where T : Enumeration, new()
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Value == value);
            return matchingItem;
        }
        public static T FromDisplayName<T>(string displayName) where T : Enumeration, new()
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.DisplayName == displayName);
            return matchingItem;
        }
        private static T Parse<T, TK>(TK value, string description, Func<T, bool> predicate) where T : Enumeration, new()
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem != null)
            {
                return matchingItem;
            }

            var message = $"'{value}' is not a valid {description} in {typeof(T)}";
            throw new ApplicationException(message);
        }
        public int CompareTo(object other)
        {
            return Value.CompareTo(((Enumeration)other).Value);
        }
        public override string ToString()
        {
            return DisplayName;
        }
        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
            {
                return false;
            }

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
        }

    }
}
